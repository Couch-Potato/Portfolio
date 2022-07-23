using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Data.Model
{
    public class PersistentItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string PropName { get; set; }
        public string Universe { get; set; }
        public string OwnerId { get; set; }
        public string transportString { get; set; }
        public MVector3 Position { get; set; }
        public MVector3 Rotation { get; set; }
    }
    public class MVector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }
}
