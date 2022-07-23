using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
namespace ProjectEmergencyFrameworkClient.Services
{
    public static class DoorService
    {

        public static List<DoorEntry> DoorEntry = new List<DoorEntry>();

        public static Dictionary<Entity, string> GasPumps = new Dictionary<Entity, string>();
        public static Dictionary<uint, DoorObject> Doors = new Dictionary<uint, DoorObject>();
        public static Dictionary<uint, string> PumpLookup = new Dictionary<uint, string>();
        public static Dictionary<int, int> DoorLookup2 = new Dictionary<int, int>();
        static int curDoorId = 5001;

        [ExecuteAt(ExecutionStage.OnResourceStart)]
        public static void GetAllGasPumps()
        {
            PumpLookup.Add((uint)GetHashKey("prop_gas_pump_1a"), "ron");
            PumpLookup.Add((uint)GetHashKey("prop_gas_pump_1b"), "globe");
            PumpLookup.Add((uint)GetHashKey("prop_gas_pump_1c"), "ltd");
            PumpLookup.Add((uint)GetHashKey("prop_gas_pump_1d"), "xero");
            PumpLookup.Add((uint)GetHashKey("prop_vintage_pump"), "globe");


            Entity[] pumps = World.GetAllProps();
            foreach (var pump in pumps)
            {
                if (PumpLookup.ContainsKey((uint)pump.Model.Hash)){
                    GasPumps.Add(pump, PumpLookup[(uint)pump.Model.Hash]);
                }
            }
        }

        public static int ResolveDoorId(int ent, DoorEntry entry, uint model, Vector3 loc)
        {
            if (DoorLookup2.ContainsKey(ent))
            {
                return DoorLookup2[ent];
            }
            AddDoorToSystem((uint)curDoorId, model, loc.X, loc.Y, loc.Z, false, false, false);
            DoorSystemSetDoorState((uint)curDoorId, entry.IsLocked ? 1 : 0, false, false);
            DoorLookup2.Add(ent, curDoorId);
            curDoorId++;
            return curDoorId-1; 
        }

        public static void AddDoorhashesInRange(Vector3 location,float radius, uint[] models,bool isLocked, string lockOrganization = null, string lockType = null)
        {
            DoorEntry.Add(new DoorEntry()
            {
                IsLocked = isLocked,
                Location = location,
                Radius = radius,
                LockOrganization = lockOrganization,
                LockType = lockType,
                Models = models
            });
            /*Entity[] doors = World.GetAllProps();
            foreach (var door in doors)
            {
                if (models.Contains((uint)door.Model.Hash))
                {
                    if (Vector3.Distance(location, door.Position) < radius)
                    {
                        //DebugService.DebugCall("DOOR", "ADDED DOOR: " + curDoorId + $" / HASH: {((uint)door.Model.Hash)}");
                        var doorP = door.Position;
                        AddDoorToSystem(curDoorId, (uint)door.Model.Hash, doorP.X, doorP.Y, doorP.Z, false, false, false);
                        Utility.AssetLoader.LoadDoorSystemPhysics((int)curDoorId);
                        DoorSystemSetDoorState(curDoorId, isLocked ? 1 : 0, false, false);
                        Doors.Add(curDoorId, new DoorObject()
                        {
                            ModelHash= (uint)door.Model.Hash,
                            DoorId=curDoorId, 
                            Location=doorP,
                            DoorLockData = new LockingData()
                            {
                                IsLockedToSet = lockOrganization!=null || lockType!= null,
                                LockedOrganization = lockOrganization,
                                LockedType = lockType
                            },
                            LinkedEntity = door
                        });
                        curDoorId++;
                    }
                }
                
            }*/
        }
    }
    public class DoorObject
    {
        public uint ModelHash { get; set; }
        public Vector3 Location { get; set; }
        public uint DoorId { get; set; }
        public LockingData DoorLockData{get;set;}
        public Entity LinkedEntity { get; set; } 
    }
    public class DoorEntry
    {
        public Vector3 Location { get; set; }
        public float Radius { get; set; }
        public uint[] Models { get; set; }
        public bool IsLocked { get; set; }
        public string LockOrganization { get; set; }
        public string LockType { get; set; }
    }
    public class LockingData
    {
        public string LockedOrganization { get; set; }
        public string LockedType { get; set; }
        public bool IsLockedToSet { get; set; } = false;
    }
}
