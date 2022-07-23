using MongoDB.Driver;
using ProjectEmergencyFrameworkShared.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkServer.Data
{
    public class HealthDataService
    {
        internal static IMongoCollection<HealthRecord> _health;
        internal static IMongoCollection<DeathCertificates> _deathCertificates;
        public static string FileHealthReport(dynamic d)
        {
            HealthRecord healthRecord = new HealthRecord()
            {
                Examiner = d.examiner,
                AssociatedDLID = d.patient.dlid,
                AssociatedFirstName = d.patient.firstName,
                AssociatedLastName = d.patient.lastName,
                AssociatedDOB = d.patient.dob,
                InsuranceUsed = d.patient.insurance,
                InpatientMethod = d.patient.method,
                Condition = d.injuries.condition
            };

            foreach (var injury in d.injuries.injuries)
            {
                healthRecord.Injuries.Add(new InjuryRecord()
                {
                    Type = injury.type,
                    Location = injury.location,
                    Severity = injury.severity,
                    Narrative = injury.narrative
                });
            }
            var character = PlayerDataService.FindCharacterBattery(d.patient);
            // FIND CHARACTER REF
            BankService.CreateDebt("MEDICAL", d.total, 3, character?.Id);

            _health.InsertOne(healthRecord);
            return healthRecord.Id;
        }

        public static string FileDeathCertificate(dynamic d)
        {

            DeathCertificates dc = new DeathCertificates()
            {
                AssociatedDLID = d.deceased.dlid,
                AssociatedDOB = d.deceased.dob,
                AssociatedFirstName = d.deceased.firstName,
                AssociatedLastName = d.deceased.lastName,
                Examiner = d.examiner,
                Reason = d.deceased.reason
            };

            _deathCertificates.InsertOne(dc);

            Character character = PlayerDataService.FindCharacterBattery(d.deceased);

            if (character != null)
            {
                character.IsMarkedDeceased = true;
                character.DeathCertificate = dc.Id;
                PlayerDataService.UpdateCharacter(character);
            }

            return dc.Id;
        }

        
    }
}
