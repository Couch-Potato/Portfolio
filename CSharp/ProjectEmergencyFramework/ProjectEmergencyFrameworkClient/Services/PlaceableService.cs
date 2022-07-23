using CitizenFX.Core;
using Newtonsoft.Json;
using ProjectEmergencyFrameworkClient.Placeables;
using ProjectEmergencyFrameworkClient.Utility;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
namespace ProjectEmergencyFrameworkClient.Services
{
    public class PlaceableService
    {
       
        static List<DiscoveredItem> placablesBoilerplate = new List<DiscoveredItem>();

        [ExecuteAt(ExecutionStage.Initialized)]
        public static void InitialzePlaceables()
        {
            placablesBoilerplate = ClassDiscovery.DiscoverWithAttribute<Placeables.PlaceableAttribute>();
        }

        static List<IPlaceable> Placeables = new List<IPlaceable>();

        public static IPlaceable LoadPlaceable(string name, Vector3 position, Vector3 orientation, bool fresh = false)
        {
            foreach (var plac in placablesBoilerplate)
            {
                if (plac.GetAttribute<PlaceableAttribute>().Name == name)
                {
                    IPlaceable placeable = plac.ConstructAs<IPlaceable>();
                    placeable.Position = position;
                    placeable.Rotation = orientation;
                    placeable.Prop = ConfigurationService.ComplexProps[name].GetInstanced(position, orientation);
                    
                    if (fresh)
                        placeable.Place();
                    placeable.Instanced();
                    
                    Placeables.Add(placeable);
                    return placeable;
                }
            }
            return null;
        }
        public static void DeinstancePlaceable(IPlaceable placeable)
        {
            placeable.Cleanup();
            placeable.Prop.Delete();
            Placeables.Remove(placeable);
        }
        public static void DestroyPlaceable(IPlaceable placeable)
        {
            placeable.Destroyed();
            DeinstancePlaceable(placeable);
        }
        [Queryable("NEW_PLACEABLE")]
        public static void OnPlaceableCall(Query q, object value)
        {
            dynamic cd = Utility.CrappyWorkarounds.JSONDynamicToExpando(value);

            IPlaceable placeable = LoadPlaceable(cd.propName, DynToVec3(cd.position), DynToVec3(cd.rotation), cd.owner == CharacterService.CurrentCharacter.Id);
            placeable.OwnerId = cd.owner;
            placeable.PersistentId = cd.id;
            placeable.Universe = cd.universe;
            placeable.Properties = JsonConvert.DeserializeObject<ExpandoObject>(cd.transportString);

            q.Reply(true);
        }
        private static Vector3 DynToVec3(dynamic d)
        {
            return new Vector3(d.X, d.Y, d.Z);
        }

        [ExecuteAt(ExecutionStage.OnResourceStart)]
        public static async void LoadPersistentItems()
        {
            UniverseService.UniverseChanged += UniverseService_UniverseChanged;
            var items = await QueryService.QueryConcrete<ProjectEmergencyFrameworkShared.Data.Model.PersistentItem[]>("GET_PLACEABLE", UniverseService.CURRENT_UNIVERSE);
            foreach (var item in items)
            {
                IPlaceable placeable = LoadPlaceable(item.PropName, DynToVec3(item.Position), DynToVec3(item.Rotation), false);
                placeable.OwnerId = item.OwnerId;
                placeable.PersistentId = item.Id;
                placeable.Universe = item.Universe;
                placeable.Properties = JsonConvert.DeserializeObject<ExpandoObject>(item.transportString);
            }
        }

        private static async void UniverseService_UniverseChanged()
        {
            foreach (var plc in Placeables)
            {
                plc.Cleanup();
            }
            Placeables.Clear();
            var items = await QueryService.QueryConcrete<ProjectEmergencyFrameworkShared.Data.Model.PersistentItem[]>("GET_PLACEABLE", UniverseService.CURRENT_UNIVERSE);
            foreach (var item in items)
            {
                IPlaceable placeable = LoadPlaceable(item.PropName, DynToVec3(item.Position), DynToVec3(item.Rotation), false);
                placeable.OwnerId = item.OwnerId;
                placeable.PersistentId = item.Id;
                placeable.Universe = item.Universe;
                placeable.Properties = JsonConvert.DeserializeObject<ExpandoObject>(item.transportString);
            }
        }

        public static async void CreatePersistentPlaceable(Vector3 position, Vector3 rotation, dynamic modifiers, string propName)
        {
            string ts = JsonConvert.SerializeObject(modifiers);
            await QueryService.QueryConcrete<bool>("INSERT_PLACEABLE", new
            {
                position = new { X = position.X, Y = position.Y, Z = position.Z },
                rotation = new { X = rotation.X, Y = rotation.Y, Z = rotation.Z },
                owner = CharacterService.CurrentCharacter.Id,
                universe = UniverseService.CURRENT_UNIVERSE,
                transportString = ts,
                propName = propName
            });
        }
    }
    public class PropItem
    {
        public Vector3 Position;
        public Vector3 Rotation;

        public string HashName;
    }
    public class ComplexProp
    {
        public Vector3 Size;
        public string Name;
        public List<PropItem> Props = new List<PropItem>();
        public InstancedProp GetInstanced(Vector3 position, Vector3 rotation)
        {
            return new InstancedProp(position, rotation, this);
        }
    }
    public class PropInstance : PropItem
    {
        public Entity Entity;
    }
    public class InstancedProp
    {
        public Vector3 Position;
        public Vector3 Rotation;
        public string Name;
        public List<PropInstance> Props = new List<PropInstance>();
        public float Alpha = 1f;
        public void Update()
        {
            foreach (var prop in Props)
            {
                prop.Entity.Position = Position;
                SetEntityRotation(prop.Entity.Handle, Rotation.X, Rotation.Y, Rotation.Z, 2, true);
                Vector3 newWorld = GetOffsetFromEntityInWorldCoords(prop.Entity.Handle, prop.Position.X, prop.Position.Y, prop.Position.Z);
                prop.Entity.Position = newWorld;
                SetEntityRotation(prop.Entity.Handle, Rotation.X + prop.Rotation.X, Rotation.Y + prop.Rotation.Y, Rotation.Z + prop.Rotation.Z, 2, true);
                SetEntityAlpha(prop.Entity.Handle, (int)(Alpha * 255f), 0);
            }
        }
        public void Delete()
        {
            foreach (var prop in Props)
            {
                prop.Entity.Delete();
            }
        }
        public InstancedProp(Vector3 position, Vector3 rotation, ComplexProp prop)
        {
            Position = position;
            Rotation = rotation;
            Name = prop.Name;
            foreach (var pi in prop.Props)
            {
                PropInstance propInstance = new PropInstance()
                {
                    HashName = pi.HashName,
                    Entity = new Prop(CreateObject(GetHashKey(pi.HashName), pi.Position.X, pi.Position.Y, pi.Position.Z, false, true, false)),
                    Position = pi.Position,
                    Rotation = pi.Rotation,
                };
                Props.Add(propInstance);
            }
            Update();
        }
    }
    public class DynamicInstancedProp : InstancedProp
    {
        PETask UpdateTask;
        public DynamicInstancedProp(Vector3 position, Vector3 rotation, ComplexProp prop) : base(position, rotation, prop)
        {
            UpdateTask = TaskService.InvokeUntilExpire(async () =>
            {
                Update();
                return true;
            }, "DYN_PROP_" + Name);
        }
        public void Stop()
        {
            TaskService.ForceExpireTask(UpdateTask);
        }
        ~DynamicInstancedProp()
        {
            Stop();
        }
    }

}
