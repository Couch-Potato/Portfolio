using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Data.Model
{
    public class Player
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Steam { get; set; }
        public string Discord { get; set; }
        public bool IsBanned { get; set; }
        public long LastJoinDate { get; set; }
        public long FirstJoinDate { get; set; }
        public BsonDocument ModerationHistory { get; set; }
        public string BanReason { get; set; }
        public int MaxCharacterSlots { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string CurrenctCharacter { get; set; }
        public string NetworkId { get; set; }
        public bool IsOnline { get; set; }
        public string WhitelistId { get; set; }
    }
}
