using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Data.Model
{
    public class Evidence
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Container { get; set; }
        public bool IsLocked { get; set; }
        public string LockReason { get; set; }
        public string EvidenceType { get; set; }
        public uint Created { get; set; }
        public string Creator { get; set; }
        public string CustodialAgency { get; set; }
        public uint DestructionTime { get; set; }
        public bool IsEarmarkedForDestruction { get; set; }
    }
    
}
