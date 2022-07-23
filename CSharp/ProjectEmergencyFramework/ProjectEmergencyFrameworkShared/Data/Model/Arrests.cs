using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Data.Model
{
    public class Arrests
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string OfficerId { get; set; }
        public string AgencyAbbrev { get; set; }
        public Arrestee Arrestee { get; set; }
        public CaseEvidence Evidence { get; set; }
        public List<Charge> Charges { get; set; }
        public string Plea { get; set; }
        public string LinkedIncarceration { get; set; }
    }
    public class NonPhysicalCaseEvidence
    {
        public string Type { get; set; }
        public string Source { get; set; }
        public string Narrative { get; set; }
    }
    public class CaseEvidence
    {
        public List<string> RelatedPhysicalEvidence { get; set; }
        public string ArresteeProperty { get; set; }
        public List<CaseWitness> Witnesses { get; set; }

        public List<NonPhysicalCaseEvidence> NonPhysicalCaseEvidence { get; set; }
    }
    public class CaseWitness
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DLID { get; set; }
        public string DOB { get; set; }
    }
    public class Charge
    {
        public string Category { get; set; }
        public string Offense { get; set; }
        public string Location { get; set; }
        public string Venue { get; set; }
        public string Narrative { get; set; }
    }
    public class Arrestee
    {
        public string ProvidedFirstName { get; set; }
        public string ProvidedLastName { get; set; }
        public string Fingerprint { get; set; }
        public string Mugshot { get; set; }
        public string ProvidedDlid { get; set; }
        public string ProvidedDob { get; set; }
    }
}
