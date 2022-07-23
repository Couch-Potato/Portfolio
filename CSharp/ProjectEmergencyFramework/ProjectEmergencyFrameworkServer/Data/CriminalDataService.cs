using CitizenFX.Core;
using MongoDB.Driver;
using Newtonsoft.Json;
using ProjectEmergencyFrameworkShared.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkServer.Data
{
    public static class CriminalDataService
    {
        internal static IMongoCollection<Warrant> _warrants;
        internal static IMongoCollection<NCIC> _ncic;
        internal static IMongoCollection<Arrests> _arrests;
        internal static IMongoCollection<IncarcerationRecord> _incarcer;
        internal static IMongoCollection<CitationRecord> _citation;
        public static IncarcerationRecord GetIncarcerationRecordFromArrest(string arrest)
        {
            var arrestIt = _arrests.Find(a => a.Id == arrest).FirstOrDefault();
            if (arrestIt != null)
            {
                return _incarcer.Find(i => i.Id == arrestIt.LinkedIncarceration).FirstOrDefault();
            }
            return null;
        }
        public static Arrests GetArrest(string arrest)
        {
            var arrestIt = _arrests.Find(a => a.Id == arrest).FirstOrDefault();
            return arrestIt;
        }
        public static string FormatAndFileArrest(dynamic d)
        {
            Arrests arrest = new Arrests();
            arrest.OfficerId = d.officer;
            arrest.AgencyAbbrev = d.agency;
            arrest.Arrestee = new Arrestee
            {
                ProvidedFirstName = d.arrestee.firstName.ToUpper(),
                ProvidedLastName = d.arrestee.lastName.ToUpper(),
                ProvidedDlid = d.arrestee.dlid.ToUpper(),
                ProvidedDob = d.arrestee.dob,
                Fingerprint = d.arrestee.fingerPrint,
                Mugshot = d.arrestee.mugshot
            };
            arrest.Charges = new List<Charge>();
            foreach (var charge in d.charges)
            {
                arrest.Charges.Add(new Charge
                {
                   Category=charge.category,
                   Offense = charge.offense,
                   Location = charge.location,
                   Venue = charge.venue,
                   Narrative=charge.narrative
                });
            }
            arrest.Evidence = new CaseEvidence();
            arrest.Evidence.RelatedPhysicalEvidence = new List<string>();
            foreach (var ev in d.evidence.physicalEvidence)
            {
                if (ev != "")
                {
                    arrest.Evidence.RelatedPhysicalEvidence.Add(((string)ev).Trim(','));
                }
            }
            arrest.Evidence.ArresteeProperty = ((string)d.evidence.property).Trim(',');
            arrest.Evidence.Witnesses = new List<CaseWitness>();
            foreach (var witness in d.evidence.witnesses)
            {
                arrest.Evidence.Witnesses.Add(new CaseWitness
                {
                    FirstName=((string)witness.firstName).ToUpper(),
                    LastName= ((string)witness.lastName).ToUpper(),
                    DLID = ((string)witness.dlid).ToUpper(),
                    DOB = ((string)witness.dob)
                });
            }
            arrest.Evidence.NonPhysicalCaseEvidence = new List<NonPhysicalCaseEvidence>();
            foreach (var ev in d.evidence.nonPhysicalEvidence)
            {
                arrest.Evidence.NonPhysicalCaseEvidence.Add(new NonPhysicalCaseEvidence
                {
                    Narrative = ev.narrative,
                    Source = ev.source,
                    Type = ev.type
                });
            }

            _arrests.InsertOne(arrest);


            var incarceration = new IncarcerationRecord()
            {
                Arrest = arrest.Id,
                Bail = 1000f,
                BailPaid = 0f,
                TimeOfRelease = BankService.Timestamp() + (60 * 60 * 24 * 4),
                Type="FELONY",
                Fingerprint = arrest.Arrestee.Fingerprint,
                ArrestTimestap = BankService.Timestamp()
            };
            _incarcer.InsertOne(incarceration);

            arrest.LinkedIncarceration = incarceration.Id;
            _arrests.ReplaceOne(arr => arr.Id == arrest.Id, arrest);
            var crimData = PullNCIC(arrest.Arrestee.Fingerprint);
            if (!crimData.Aliases.Contains($"{arrest.Arrestee.ProvidedFirstName.ToUpper()} ,{arrest.Arrestee.ProvidedFirstName.ToUpper()}"))
            {
                crimData.Aliases.Add($"{arrest.Arrestee.ProvidedFirstName.ToUpper()} ,{arrest.Arrestee.ProvidedFirstName.ToUpper()}");
            }
            foreach (var witness in arrest.Evidence.Witnesses)
            {
                bool found = false;
                foreach (var ap in crimData.AssociatedPersons)
                {
                    if (ap.FirstName == witness.FirstName && ap.LastName == witness.LastName)
                    {
                        ap.Reason += $"ASSOCIATED WITH ARREST - {arrest.Id} (WITNESS) /";
                        found = true;
                    }
                }
                if (!found)
                {
                    crimData.AssociatedPersons.Add(new AssociatedPerson
                    {
                        FirstName = witness.FirstName,
                        LastName = witness.LastName,
                        Reason = $"ASSOCIATED WITH ARREST - {arrest.Id} (WITNESS) /"
                    });
                }
            }
            crimData.Arrests.Add(arrest.Id);
            crimData.Mugshots.Add(arrest.Arrestee.Mugshot);
            foreach (var evidenceIds in arrest.Evidence.RelatedPhysicalEvidence)
            {
                crimData.EvidenceFound.Add(evidenceIds);
                EvidenceDataService.LockEvidence(evidenceIds, "RELATED TO ARREST: " + arrest.Id, "CRIME-EVIDENCE");
            }
            EvidenceDataService.LockEvidence(arrest.Evidence.ArresteeProperty, "ARRESTEE PROPERTY", "ARRESTEE-PROPERTY");
            crimData.Incarcerations.Add(incarceration.Id);

            var character = PlayerDataService.GetCharacterFromId(crimData.Fingerprint);
            character.IsIncarcerated = true;
            character.IncarcerationRecord = incarceration.Id;

            PlayerDataService.UpdateCharacter(character);
            UpdateNCIC(crimData);

            return arrest.Id;
        }

        public static NCIC PullNCIC(string fingerPrint)
        {
            var ncic = _ncic.Find(n => n.Fingerprint == fingerPrint);
            if (ncic.CountDocuments() == 0)
            {
                NCIC c = new NCIC()
                {
                    Aliases = new List<string>(),
                    Arrests = new List<string>(),
                    AssociatedPersons = new List<AssociatedPerson>(),
                    EvidenceFound = new List<string>(),
                    Incarcerations = new List<string>(),
                    Mugshots = new List<string>(),
                    Fingerprint = fingerPrint,
                    Warrants = new List<string>(),
                };
                _ncic.InsertOne(c);
                return c;
            }
            return ncic.FirstOrDefault();
        }
        public static bool GetIfIsOnBail(string charId)
        {
            foreach (var fine in _incarcer.Find(n => n.Fingerprint == charId).ToList())
            {
                if (fine.BailPaid > 0)
                {
                    if (fine.ArrestTimestap + (60*60*25*3) < BankService.Timestamp())
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static bool HasBackdatedFines(string charId)
        {
            foreach (var fine in _incarcer.Find(n => n.Fingerprint == charId).ToList())
            {
                if (fine.Fine > 0 && fine.Fine > fine.FinePaid)
                {
                    if (fine.DateFineDue < BankService.Timestamp())
                    {
                        return true;
                    }
                }
            }
            foreach (var fine in _citation.Find(n => n.Person == charId).ToList())
            {
                if (fine.Fine > 0 && fine.Fine > fine.FinePaid)
                {
                    if (fine.DateFineDue < BankService.Timestamp())
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static void UpdateNCIC(NCIC c)
        {
            _ncic.ReplaceOne(n => n.Id == c.Id, c);
        }
        public static void UpdateArrest(Arrests a)
        {
            _arrests.ReplaceOne(n => a.Id == n.Id, a);
        }
        public static IncarcerationRecord GetIncarcerationRecord(string a)
        {
            return _incarcer.Find(n => a == n.Id).FirstOrDefault();
        }

        public static void UpdateIncracerationRecord(IncarcerationRecord a)
        {
            _incarcer.ReplaceOne(n => a.Id == n.Id, a);
        }
        public static void PostFines(string charId)
        {
            foreach (var fine in _incarcer.Find(n=>n.Fingerprint == charId).ToList())
            {
                if (fine.Fine > 0 && fine.Fine > fine.FinePaid)
                {
                    fine.FinePaid = fine.Fine;
                    UpdateIncracerationRecord(fine);
                }
            }
            foreach (var fine in _citation.Find(n=>n.Person == charId).ToList())
            {
                if (fine.Fine > 0 && fine.Fine > fine.FinePaid)
                {
                    fine.FinePaid = fine.Fine;
                    _citation.ReplaceOne(n => n.Id == fine.Id, fine);
                }
            }
        }

        public static void PostBail(string charId)
        {
            foreach (var fine in _incarcer.Find(n => n.Fingerprint == charId).ToList())
            {
                if (fine.Bail > 0 && fine.Bail > fine.BailPaid)
                {
                    fine.BailPaid = fine.Bail;
                    UpdateIncracerationRecord(fine);
                }
            }
        }

        public static float GetFines(string charId)
        {
            float result = 0f;
            foreach (var fine in _incarcer.Find(n => n.Fingerprint == charId).ToList())
            {
                if (fine.Fine > 0 && fine.Fine > fine.FinePaid)
                {
                    result += fine.Fine - fine.FinePaid;
                }
            }
            foreach (var fine in _citation.Find(n => n.Person == charId).ToList())
            {
                if (fine.Fine > 0 && fine.Fine > fine.FinePaid)
                {
                    result += fine.Fine - fine.FinePaid;
                }
            }

            return result;
        }

        public static float MakeCitation(dynamic d)
        {
            CitationRecord citation = new CitationRecord()
            {
                Fine = (float)d.total,
                GivenFirstName = d.firstName,
                GivenLastName = d.lastName,
                GivenDLID = d.dlid,
                GivenDOB = d.dob,
                VehiclePlate = d.plate,
                VehicleName = d.makeModel,
                Person = PlayerDataService.FindCharacterBattery(d).Id,
                FinePaid = 0,
                DateFineDue = BankService.Timestamp() + (60 * 60 * 24 * 7),
                DateFineIssued = BankService.Timestamp(),
                CitationCharges = new List<CitationCharge>()
            };
            foreach (var charge in d.charges)
            {

                if (Utility.CrappyWorkarounds.HasProperty(charge, "charge"))
                {
                    citation.CitationCharges.Add(new CitationCharge
                    {
                        CitationChargeAmount = (float)charge.fine,
                        CitationChargeName = charge.charge,
                        CitationType = charge.type
                    });
                }
                
            }
            _citation.InsertOne(citation);
            return d.total;
        }
    }
}
