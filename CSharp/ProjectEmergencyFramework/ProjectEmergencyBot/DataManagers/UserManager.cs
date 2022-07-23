using MongoDB.Driver;
using ProjectEmergencyFrameworkShared.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyBot.DataManagers
{
   public class UserManager
    {
        public static uint Timestamp()
        {
            return (uint)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
        public static IMongoCollection<User> _users;
        public static string AddWhitelist(string firstname, string lastname, string discord)
        {
            User user = new User()
            {
                IsBanned = false,
                FirstName=firstname,
                LastName = lastname,
                Discord =discord,
                HasActiveHolds=false,
                Holds = new List<Hold>(),
                WhitelistDate = Timestamp(),
                UserType = UserType.Member
            };
            _users.InsertOne(user);
            return user.Id;
        }
        public static User RemoveWhitelist(string id)
        {
            var beforeUser = _users.Find(e => e.Id == id).FirstOrDefault();
            _users.DeleteOne(e=>e.Id == id);
            return beforeUser;
        }
        public static User Lookup(string first, string last)
        {
            return _users.Find(e => e.FirstName.ToUpper() == first.ToUpper() && e.LastName.ToUpper() == last.ToUpper()).FirstOrDefault();
        }
        public static User Lookup(string discord)
        {
            return _users.Find(e => e.Discord == discord).FirstOrDefault();
        }
        public static void SetAccessLevel(string id, int l)
        {
            var user = GetUser(id);
            user.UserType = (UserType)l;
            UpdateUser(user);
        }
        public static User GetUser(string id)
        {
            return _users.Find(ux => ux.Id == id).FirstOrDefault();
        }
        public static void UpdateUser(User u)
        {
            _users.ReplaceOne(ux => u.Id == ux.Id, u);
        }
        public static void AddHold(string id, string reason, string from, int hours, int type)
        {
            var user = GetUser(id);
            user.HasActiveHolds = true;
            user.Holds.Add(new Hold()
            {
                IsIndef = hours == 0,
                Creator = from,
                HoldType = (HoldType)type,
                Hours = hours,
                Reason = reason,
                TimeStart = Timestamp()
            });
            UpdateUser(user);
        }
        public static Hold[] GetHolds(string id)
        {
            var user = GetUser(id);
            return user.Holds.ToArray();
        }
        public static void RemoveHold(string id, int holdId)
        {
            var user = GetUser(id);
            user.Holds.RemoveAt(holdId);
            UpdateUser(user);
        }
    }
}
