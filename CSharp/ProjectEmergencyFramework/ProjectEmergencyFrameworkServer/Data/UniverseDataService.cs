using MongoDB.Driver;
using ProjectEmergencyFrameworkShared.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkServer.Data
{
    public class UniverseDataService
    {
        internal static IMongoCollection<UniverseInstance> _universe;
        internal static IMongoCollection<Apartment> _apartment;

        public static Apartment GetPrimaryApartment(string charId)
        {
            return _apartment.Find(apx => apx.OwnerId == charId).FirstOrDefault();
        }
        public static Apartment GetApartment(string apxId)
        {
            return _apartment.Find(apx => apx.Id == apxId).FirstOrDefault();
        }
        public static void CreateApartment(string cfgName, string universeType, string owner)
        {
            UniverseInstance unv = new UniverseInstance()
            {
                UniverseType = universeType
            };

            _universe.InsertOne(unv);

            Apartment a = new Apartment()
            {
                ApartmentConfigName = cfgName,
                OwnerId=owner,
                IsLocked=true,
                UniverseId=unv.Id
            };
            _apartment.InsertOne(a);
            unv.AttachedInstanceType = "APARTMENT";
            unv.AttachedInstance = a.Id;
            _universe.ReplaceOne(uvx=>uvx.Id == unv.Id, unv);

        }
        public static UniverseInstance GetUniverse(string id)
        {
            return _universe.Find(uvx => uvx.Id == id).FirstOrDefault();
        }

        public static List<AvailApartmentListItem> GetAvailApartments(string owner)
        {
            var aptItem = new List<AvailApartmentListItem>();
            var apx = _apartment.Find(ap => ap.IsLocked);
            foreach (var apt in apx.ToList())
            {
                var chx = PlayerDataService.GetCharacterFromId(apt.OwnerId);
                if (chx.IsOnline)
                    aptItem.Add(new AvailApartmentListItem()
                    {
                        firstName = chx.FirstName,
                        lastName = chx.LastName,
                        apartmentId = apt.Id
                    });
            }
            return aptItem;
        }
    }
    public class AvailApartmentListItem
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string apartmentId { get; set; }
    }
}
