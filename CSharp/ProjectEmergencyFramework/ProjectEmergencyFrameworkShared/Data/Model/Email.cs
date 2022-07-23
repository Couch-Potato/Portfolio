using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Data.Model
{
    public class Email
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string EmailAddress { get; set; }
        public string ConnectedCharacter { get; set; }
        public string ConnectedOrganization { get; set; }
        public string Name { get; set; }
        public bool Frozen { get; set; }
        public List<EmailItem> Incoming { get; set; }
        public List<EmailItem> Outgoing { get; set; }
    }
    public class EmailItem
    {
        public string ToAddress { get; set; }
        public string FromAddress { get; set; }
        public string Subject { get; set; }
        public UInt32 SendTime { get; set; }
        public string Body { get; set; }
    }
}
