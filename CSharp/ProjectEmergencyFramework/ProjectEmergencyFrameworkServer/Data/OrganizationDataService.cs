using CitizenFX.Core;
using MongoDB.Driver;
using ProjectEmergencyFrameworkShared.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkServer.Data
{
    public static class OrganizationDataService
    {
        internal static IMongoCollection<Organization> _org;
        internal static IMongoCollection<Character> _char;
        internal static IMongoCollection<ProjectEmergencyFrameworkShared.Data.Model.Vehicle> _vehicles;

        public static Organization GetOrganization(string id)
        {
            return _org.Find(e => e.CallableId == id).FirstOrDefault();
        }
        public static bool DoesOrganizationExist(string name)
        {
            return _org.Find(e => e.CallableId == name).CountDocuments() > 0;
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
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
                CharacterId=charId,
                ServiceNum=RandomString(4)
            });
            UpdateOrganization(orga);
        }
        public static void CreateOrganization(Organization org, bool keepBaseId = false)
        {
            var randoId = RandomString(16);
            if (!keepBaseId)
            {
                org.CallableId = randoId;
            }
            _org.InsertOne(org);

            if (keepBaseId)
                return;

            var newOrg = GetOrganization(randoId);
            newOrg.CallableId = newOrg.Id;

            _org.ReplaceOne(e => e.CallableId == randoId, newOrg);
        }
        public static void UpdateOrganization(Organization org)
        {
            _org.ReplaceOne(e => e.CallableId == org.CallableId, org);
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
    }
}
