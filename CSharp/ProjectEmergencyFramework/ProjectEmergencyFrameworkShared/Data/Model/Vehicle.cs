using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Data.Model
{
    public class Vehicle
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string RegisteredOwnerId { get; set; }
        public string LicensePlate { get; set; }
        public int ColorId { get; set; }
        public string SpawnName { get; set; }
        public string Make { get; set; }
        public int Health { get; set; }
        public bool Destroyed { get; set; }
        public bool Imponded { get; set; }
        public string Model { get; set; }
        public bool BelongsToOrganization { get; set; }
        public bool IsInsured { get; set; }
        public bool IsGovernmentInsured { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string Container { get; set; }

        public bool IsStolen { get; set; } = false;
    }
}
