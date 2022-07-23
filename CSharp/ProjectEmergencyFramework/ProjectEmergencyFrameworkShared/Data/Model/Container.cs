using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Data.Model
{
    public class Container
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public List<InventoryItem> Inventory { get; set; }
        public string Type { get; set; }
        public int MaxItems { get; set; }
        public string Name { get; set; }
    }
    public class ContainerTransport
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string ContainerId { get; set; }
        public dynamic ItemSet { get; set; }
    }
}
