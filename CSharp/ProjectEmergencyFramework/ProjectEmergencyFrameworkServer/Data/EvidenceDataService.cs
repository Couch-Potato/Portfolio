using CitizenFX.Core;
using MongoDB.Driver;
using Newtonsoft.Json;
using ProjectEmergencyFrameworkShared.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkServer.Data
{
    public static class EvidenceDataService
    {
        internal static IMongoCollection<Evidence> _evidence;
        internal static IMongoCollection<GunSerial> _guns;
        public static string CreateGunItem(GunSerial g)
        {
            GunSerial gun = g;
            Debug.WriteLine(JsonConvert.SerializeObject(gun));
            _guns.InsertOne(gun);
            return gun.Id;
        }
        public static dynamic CreateEvidenceLocker(string creator, string agency)
        {
            List<InventoryItem> items = new List<InventoryItem>();
            for (int i = 0; i < 15; i++)
            {
                items.Add(new InventoryItem());
            }


            var container = PlayerDataService.CreateContainer(new Container()
            {
                MaxItems=15,
                Name="EVIDENCE LOCKER",
                Type="EVIDENCE",
                Inventory = items
            });

           

            var evidence = new Evidence()
            {
                Creator = creator,
                CustodialAgency=agency,
                EvidenceType="UNKNOWN - UNCATEGORIZED",
                IsEarmarkedForDestruction = false,
                IsLocked = false,
                Container = container.Id,
                Created = BankService.Timestamp()
            };

            _evidence.InsertOne(evidence);

            return new
            {
                Evidence = evidence.Id,
                Container = container.Id
            };

        }
        public static void LockEvidence(string evidenceId, string reason, string type)
        {
            Evidence evidence = _evidence.Find(ev => ev.Id == evidenceId).FirstOrDefault();
            evidence.IsLocked = true;
            evidence.LockReason = reason;
            evidence.EvidenceType = type;

            _evidence.ReplaceOne(ev => ev.Id == evidenceId, evidence);
        }
        public static void MarkForDestruction(string evidenceId, int hours)
        {
            Evidence evidence = _evidence.Find(ev => ev.Id == evidenceId).FirstOrDefault();
            evidence.IsLocked = true;
            evidence.LockReason = "MARKED FOR DESTRUCTION / SEE JUDGE FOR REVERSAL";
            evidence.IsEarmarkedForDestruction = true;
            evidence.DestructionTime = BankService.Timestamp() + (uint)(60 * hours * 60);

            _evidence.ReplaceOne(ev => ev.Id == evidenceId, evidence);
        }

        
    }
}
