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
    public static class PersistenceService
    {
        internal static IMongoCollection<PersistentItem> _persist;

        public static string InsertPersistentItem(MVector3 position, MVector3 rotation, string owner, string universe, string ts, string propname)
        {
            var pi = new PersistentItem()
            {
                Position = position,
                Rotation = rotation,
                OwnerId = owner,
                PropName=propname,
                Universe = universe,
                transportString = ts
            };
            _persist.InsertOne(pi);
            return pi.Id;
        }
        public static Vector3 MVectorToCFX(MVector3 vector3)
        {
            return new Vector3(vector3.X, vector3.Y, vector3.Z);
        }
        public static MVector3 CFXToMVector(Vector3 vector3)
        {
            return new MVector3()
            {
                X = vector3.X,
                Y = vector3.Y,
                Z = vector3.Z
            };
        }
        public static MVector3 DynamicToMVector(dynamic vector3)
        {
            return new MVector3()
            {
                X = vector3.X,
                Y = vector3.Y,
                Z = vector3.Z
            };
        }
        public static PersistentItem[] GetPersistentItemsInUniverse(string universe)
        {
            return _persist.Find(p => p.Universe == universe).ToList().ToArray();
        }
    }
}
