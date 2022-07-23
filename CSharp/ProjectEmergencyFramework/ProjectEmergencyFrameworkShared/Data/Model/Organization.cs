using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Data.Model
{
    public class Organization
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string CallableId { get; set; }
        public string Abbrev { get; set; }
        public string OrganizationName { get; set; }
        public List<OrganizationMember> Members { get; set; } 
        public List<Vehicle> Vehicles { get; set; }
        public string OrgType { get; set; }
        public bool AllowEmails { get; set; }
        public string EmailTemplate { get; set; }
    }
    public class OrganizationMember
    {
        public string ServiceNum { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string CharacterId { get; set; }
    }
}
