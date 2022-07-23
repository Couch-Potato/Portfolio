using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Data.Model
{
    public class MessageBus
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Route { get; set; }
        public string Data { get; set; }
        public bool IsQuery { get; set; }
        public string QueryId { get; set; }
        public MessageHost Author { get; set; }
        public MessageHost Destination { get; set; }
    }
    public enum MessageHost
    {
        [BsonRepresentation(BsonType.String)]
        FiveM,
        [BsonRepresentation(BsonType.String)]
        Bot
    };

}
