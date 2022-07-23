using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Data.Model
{
    public class NCIC
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Fingerprint { get; set; }
        public List<string> Aliases { get; set; }
        public List<string> Mugshots { get; set; }
        public List<string> Arrests { get; set; }
        public List<string> EvidenceFound { get; set; }
        public List<string> Warrants { get; set; }
        public List<string> Incarcerations { get; set; }
        public List<AssociatedPerson> AssociatedPersons { get; set; }
    }
    public class Warrant
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string IssuingJudgeId { get; set; }
        public string IssuingJudgeName { get; set; }
        public string Reason { get; set; }
        public string SubjectId { get; set; }
        public bool IsArrestWarrant { get; set; }
        public bool IsExpired { get; set; }
    }
    public class AssociatedPerson
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Reason { get; set; }
    }
    public class IncarcerationRecord
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public float Bail { get; set; }
        public float BailPaid { get; set; }
        public float Fine { get; set; }
        public float FinePaid { get; set; }
        public string Arrest { get; set; }
        public string Type { get; set; } // INFRACTION / MISDEMEANOR / FELONY / PRETRIAL_HOLDING
        public uint TimeOfRelease { get; set; }
        public uint DateFineDue { get; set; }
        public string Fingerprint { get; set; }
        public string Plea { get; set; } = "NONE";
        public uint ArrestTimestap { get; set; }
    }

    public class CitationRecord
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public float Fine { get; set; }
        public float FinePaid { get; set; }
        public uint DateFineDue { get; set; }
        public uint DateFineIssued { get; set; }
        public string Person { get; set; }
        public List<CitationCharge> CitationCharges { get; set; }
        public string VehiclePlate { get; set; }
        public string GivenFirstName { get; set; }
        public string GivenLastName { get; set; }
        public string GivenDOB { get; set; }
        public string GivenDLID { get; set; }
        public string VehicleName { get; set; }
    }
    public class CitationCharge
    {
        public string CitationType { get; set; }
        public string CitationChargeName { get; set; }
        public float CitationChargeAmount { get; set; }
    }
}
