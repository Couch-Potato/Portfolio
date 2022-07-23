using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Data.Model
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Discord { get; set; }
        public bool HasActiveHolds { get; set; }
        public bool IsBanned { get; set; }
        public string BanReason { get; set; }
        public string LinkedPlayerId { get; set; }
        public List<Hold> Holds { get; set; }
        public UInt32 WhitelistDate { get; set; }
        public UserType UserType { get; set; }
    }
    public enum UserType
    {
        [BsonRepresentation(BsonType.String)]
        Administrator,
        [BsonRepresentation(BsonType.String)]
        Staff,
        [BsonRepresentation(BsonType.String)]
        Member
    }
    public enum HoldType
    {
        [BsonRepresentation(BsonType.String)]
        ServerJoin,
        [BsonRepresentation(BsonType.String)]
        CharacterCreate,
        [BsonRepresentation(BsonType.String)]
        NoLawEnforcement,
        [BsonRepresentation(BsonType.String)]
        NoFire,
        [BsonRepresentation(BsonType.String)]
        NoJob
    }
    public class Hold
    {
        public string Creator { get; set; }
        public string Reason { get; set; }
        public bool IsIndef { get; set; }
        public int Hours { get; set; }
        public UInt32 TimeStart { get; set; }

        public HoldType HoldType { get; set; }
    }
}
