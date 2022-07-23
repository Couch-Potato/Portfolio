using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Data.Model
{
    public class Apartment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string OwnerId { get; set; }
        public bool IsLocked { get; set; }
        public string UniverseId { get; set; }
        public string ApartmentConfigName { get; set; }
    }
    public class UniverseInstance
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string UniverseType { get; set; }
        public string AttachedInstanceType { get; set; }
        public string AttachedInstance { get; set; }
    }
}
