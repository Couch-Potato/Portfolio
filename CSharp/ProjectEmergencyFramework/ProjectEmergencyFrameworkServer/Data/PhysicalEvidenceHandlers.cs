using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkServer.Data
{
    public static class PhysicalEvidenceHandlers
    {
        public static List<Gunshot> gunshots = new List<Gunshot>();
        [Queryable("GUNSHOT")]
        public static void HandleBullet(Query q, object i, Player px)
        {
            var request = (dynamic)i;

            gunshots.Add(new Gunshot()
            {
                position = px.Character.Position,
                name = request.name,
                firingPattern=request.firingPattern
            });

            q.Reply(true);
        }

        [Queryable("GET_GUNSHOTS")]
        public static void HandleBulletRequest(Query q, object i, Player px)
        {
            var request = (dynamic)i;


            q.Reply(gunshots);
        }

        [Queryable("GET_GUN_SERIAL")]
        public static void HandleGunRegistry(Query q, object i, Player px)
        {
            var request = (dynamic)i;


            q.Reply(EvidenceDataService.CreateGunItem(new ProjectEmergencyFrameworkShared.Data.Model.GunSerial()
            {
                WeaponName=request.name,
                Purchaser=request.purchaser,
                Organization=request.organization
            }));
        }
    }
    public class Gunshot
    {
        public Vector3 position { get; set; }
        public string name { get; set; }
        public string firingPattern { get; set; }

    }
}
