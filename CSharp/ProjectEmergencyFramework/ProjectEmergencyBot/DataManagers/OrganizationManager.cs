using MongoDB.Driver;
using ProjectEmergencyBot.CommandHandlers;
using ProjectEmergencyFrameworkShared.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyBot.DataManagers
{
    public class OrganizationManager
    {
        internal static IMongoCollection<Organization> _org;
        public static Organization GetOrganization(string id)
        {
            return _org.Find(e => e.CallableId == id).FirstOrDefault();
        }
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static void UpdateOrganization(Organization org)
        {
            _org.ReplaceOne(e => e.CallableId == org.CallableId, org);
        }
        public static string RandomStringAN(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static void AddCharacterToOrganization(string orgId, string charId)
        {
            var orga = GetOrganization(orgId);
            orga.Members.Add(new OrganizationMember()
            {
                CharacterId = charId,
                ServiceNum = RandomString(4)
            });
            UpdateOrganization(orga);
        }
        public static void RemoveCharacterFromOrganization(string orgId, string charId)
        {
            var orga = GetOrganization(orgId);
            OrganizationMember toDelete = null;
            foreach (var member in orga.Members)
            {
                if (member.CharacterId == charId)
                {
                    toDelete = member;
                }
            }
            if (toDelete is not null)
                orga.Members.Remove(toDelete);
            UpdateOrganization(orga);
        }
        public static bool IsCharacterMemberOf(string charId, string org)
        {
            var orga = GetOrganization(org);
            foreach (var member in orga.Members)
            {
                if (member.CharacterId == charId)
                    return true;
            }
            return false;
        }
        public static string GetCharacterServiceNumber(string charId, string org)
        {
            var orga = GetOrganization(org);
            foreach (var member in orga.Members)
            {
                if (member.CharacterId == charId)
                    return member.ServiceNum;
            }
            return null;
        }
    }
}
