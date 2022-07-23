using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Data.Model
{
    public class HealthRecord
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Examiner { get; set; }
        public string AssociatedFirstName { get; set; }
        public string AssociatedLastName { get; set; }
        public string AssociatedDOB { get; set; }
        public string AssociatedDLID { get; set; }
        public string FingerprintId { get; set; }
        public string InpatientMethod { get; set; }
        public string InsuranceUsed { get; set; }
        public string Condition { get; set; }
        public List<InjuryRecord> Injuries { get; set; }
    }
    public class InjuryRecord
    {
        public string Type { get; set; }
        public string Location { get; set; }
        public string Severity { get; set; }
        public string Narrative { get; set; }
    }

    public class DeathCertificates
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Examiner { get; set; }
        public string AssociatedFirstName { get; set; }
        public string AssociatedLastName { get; set; }
        public string AssociatedDOB { get; set; }
        public string AssociatedDLID { get; set; }
        public string Reason { get; set; }
        public string FingerprintId { get; set; }
    }
}
