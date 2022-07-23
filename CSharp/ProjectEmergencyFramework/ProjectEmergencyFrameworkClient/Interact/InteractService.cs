using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Interfaces.UI;
using ProjectEmergencyFrameworkClient.Services;
using ProjectEmergencyFrameworkClient.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
namespace ProjectEmergencyFrameworkClient.Interact
{
    public class InteractService
    {
        private static List<IInteractable> Interactables = new List<IInteractable>();
        private static Dictionary<IInteractable, bool> ShownDict = new Dictionary<IInteractable, bool>();
        private static Dictionary<string, Type> InteractTemplates = new Dictionary<string, Type>();
        private static BillboardInteract BillboardInteract = new BillboardInteract();
        private static MultiInteract m_interact = new MultiInteract();
        private static List<IInteractable> _generic_vehicle_incs = new List<IInteractable>();
        private static List<IInteractable> _generic_ped_incs = new List<IInteractable>();
        private static List<IInteractable> _generic_door_incs = new List<IInteractable>();
        private static List<IInteractable> _generic_gas_incs = new List<IInteractable>();
        private static List<IInteractable> _generic_props_incs = new List<IInteractable>();

        public static IInteractable ConstructInteract(string name, Vector3 positon, dynamic props)
        {
            foreach (var pair in InteractTemplates)
            {
                if (pair.Key == name)
                {
                    var inst = (IInteractable)Activator.CreateInstance(pair.Value);
                    inst.Position = positon;
                    inst.Properties = props;
                    Interactables.Add(inst);
                    ShownDict.Add(inst, false);
                    return inst;
                }
            }
            return null;
        }

        public static void TerminateInteractAtPosition(string name, Vector3 position)
        {
            IInteractable found = null;
            foreach (var intc in Interactables)
            {
                var tp = intc.GetType().GetCustomAttribute<InteractableAttribute>();
                if (intc.Position == position && tp.InteractName == name)
                {
                    found = intc;
                }
            }
            if (found != null)
            {
                TerminateInteractable(found);
            }
        }

        public static IInteractable RegisterInteractAsGeneric(string name, GenericInteractAttachment type, dynamic props = null)
        {
            foreach (var pair in InteractTemplates)
            {
                if (pair.Key == name)
                {
                    var inst = (IInteractable)Activator.CreateInstance(pair.Value);
                    if (props != null)
                        inst.Properties = props;
                    if (type == GenericInteractAttachment.Ped)
                    {
                        _generic_ped_incs.Add(inst);
                    }else if (type == GenericInteractAttachment.Vehicle)
                    {
                        _generic_vehicle_incs.Add(inst);
                    }
                    Interactables.Add(inst);
                    ShownDict.Add(inst, false);
                    return inst;
                }
            }
            return null;
        }
        public static void TerminateGeneric(IInteractable inst, GenericInteractAttachment type)
        {
            if (type == GenericInteractAttachment.Ped)
            {
                _generic_ped_incs.Remove(inst);
            }
            else if (type == GenericInteractAttachment.Vehicle)
            {
                _generic_vehicle_incs.Remove(inst);
            }
            TerminateInteractable(inst);
        }

        public static IInteractable ConstructInteract(string name, Entity item, dynamic props)
        {
            foreach (var pair in InteractTemplates)
            {
                if (pair.Key == name)
                {
                    var inst = (IInteractable)Activator.CreateInstance(pair.Value);
                    inst.Entity = item;
                    inst.Properties = props;
                    Interactables.Add(inst);
                    ShownDict.Add(inst, false);
                    return inst;
                }
            }
            return null;
        }
        private Ped[] GetLivingPeds()
        {
            return World.GetAllPeds()?
            .Where(x => (x?.IsAlive ?? false))?
            .ToArray();
        }
        private Vehicle[] GetAllVehs()
        {
            return World.GetAllVehicles()?
            .Where(x => (x?.IsAlive ?? false))?
            .ToArray();
        }

        private static Prop _GetNearestPropOfModel(uint model)
        {
            int obj = GetClosestObjectOfType(Game.PlayerPed.Position.X,
                        Game.PlayerPed.Position.Y,
                        Game.PlayerPed.Position.Z, 20f, model, false, false, false);
            if (obj != -1)
            {
                return new Prop(obj);
            }
            else
            {
                return null;
            }
        }

        private static Ped _GetClosestPed()
        {
            var peds = World.GetAllPeds();
            Ped closest = new Ped(-1);
            float closestDistance = float.MaxValue;
            foreach (var x in peds)
            {
                if (x.Position.DistanceToSquared(Game.PlayerPed.Position) < closestDistance && x.Handle != Game.PlayerPed.Handle) {
                    closest = x;
                    closestDistance = x.Position.DistanceToSquared(Game.PlayerPed.Position);
                }
            }
            return closest;
        }

        private static DoorObject _GetClosestDoor()
        {

            DoorObject door_used = null;
            float distance = float.MaxValue;

            foreach (var doorEntry in DoorService.DoorEntry)
            {
                foreach (var model in doorEntry.Models)
                {
                    int obj = GetClosestObjectOfType(
                        Game.PlayerPed.Position.X,
                        Game.PlayerPed.Position.Y,
                        Game.PlayerPed.Position.Z,
                        doorEntry.Radius,
                        model,
                        false,
                        false,
                        false
                        );
                    //DebugService.Watchpoint("DOOR RESOLVE", obj);

                    if (obj != -1)
                    {
                        var loc_distance = Vector3.Distance(Game.PlayerPed.Position, GetEntityCoords(obj, false));
                        if (loc_distance < distance)
                        {
                            door_used = new DoorObject()
                            {
                                DoorId = (uint)DoorService.ResolveDoorId(obj, doorEntry, model, GetEntityCoords(obj, false)),
                                DoorLockData = new LockingData()
                                {
                                    IsLockedToSet = doorEntry.LockOrganization != null | doorEntry.LockType != null,
                                    LockedOrganization = doorEntry.LockOrganization,
                                    LockedType = doorEntry.LockType
                                },
                                LinkedEntity = new Prop(obj),
                                Location = GetEntityCoords(obj, false),
                                ModelHash = model
                            };
                            distance = loc_distance;
                        }
                    }

                }

            }
            if (door_used != null)
            {
                Vector3 lx = door_used.Location;
                DrawMarker(0, lx.X, lx.Y, lx.Z, 0, 0, 0, 0, 0, 0, 1, 1, 1, 255, 255, 255, 255, true, true, 2, false, null, null, false);
            }
            

            return door_used;
        }
        private static Entity _GetClosestGas()
        {
            float closestDistance = float.MaxValue;
            Entity closest = null;
            foreach (var x in DoorService.GasPumps)
            {
                if (x.Key.Position.DistanceToSquared(Game.PlayerPed.Position) < closestDistance)
                {
                    closestDistance = x.Key.Position.DistanceToSquared(Game.PlayerPed.Position);
                    closest = x.Key;
                }
            }
            return closest;
        }


        private static Vehicle GetClosestVehicle()
        {
            var peds = World.GetAllVehicles();
            Vehicle closest = new Vehicle(-1);
            float closestDistance = float.MaxValue;
            foreach (var x in peds)
            {
                if (x.Position.DistanceToSquared(Game.PlayerPed.Position) < closestDistance)
                {
                    closest = x;
                    closestDistance = x.Position.DistanceToSquared(Game.PlayerPed.Position);
                }
            }
            return closest;
        }

        [ExecuteAt(ExecutionStage.Tick)]
        public static async void InteractTick()
        {
            var ped = _GetClosestPed();
            var vehicle = GetClosestVehicle();
            var door = _GetClosestDoor();
            var gas = _GetClosestGas();

            if (vehicle != null)
            {
                foreach (var inter in _generic_vehicle_incs)
                {
                    inter.Entity = vehicle;
                }
            }

            if (ped != null)
            {
                foreach (var inter in _generic_ped_incs)
                {
                    inter.Entity = ped;
                }
            }

            if (gas != null)
            {
                foreach (var inter in _generic_gas_incs)
                {
                    inter.Entity = gas;
                }
            }


            if (door != null)
            {
                foreach (var doorx in _generic_door_incs)
                {
                    doorx.AttachedDoor = door;
                }
            }
            foreach (var prop in _generic_props_incs)
            {
                var tp = prop.GetType().GetCustomAttribute<InteractableAttribute>();
                // DebugService.Watchpoint("PROPP", tp);
                if (tp.PropModel != 0)
                {

                    var px = _GetNearestPropOfModel(tp.PropModel);
                    if (px != null)
                    {
                        prop.Entity = px;
                        // DebugService.Watchpoint("!!!", "!!!!!!");
                    }
                }

                if (tp.PropModels != null)
                {

                    foreach (var pm in tp.PropModels)
                    {

                        var px = _GetNearestPropOfModel(pm);

                        if (px != null)
                        {
                            if (px.Model.Hash != 0)
                                prop.Entity = px;
                            // DebugService.Watchpoint("!!!", "!!!!!!");
                        }
                    }
                }

               
            }

            int scW = 0;
            int scH = 0;

            GetActiveScreenResolution(ref scW, ref scH);
            foreach (var intf in Interactables)
            {
                bool canShow = false;
                try
                {
                    var tp = intf.GetType().GetCustomAttribute<InteractableAttribute>();
                   
                    if (tp.Attachment != GenericInteractAttachment.NoAttachment && (intf.Entity == null && intf.AttachedDoor==null))
                    {
                        continue;
                    }
                    canShow = await intf.CanShow();
                }
                catch (Exception ex)
                {
                var tp = intf.GetType().GetCustomAttribute<InteractableAttribute>();
                    DebugService.UnhandledException(ex);

                    //Debug.WriteLine(tp.InteractName + ": " + ex.Message);
                }
                
                if (canShow)
                {
                    if (!ShownDict[intf])
                    {
                        var tp = intf.GetType().GetCustomAttribute<InteractableAttribute>();
                        ShownDict[intf] = true;
                        float screenX = 0f;
                        float screenY = 0f;

                        GetScreenCoordFromWorldCoord(intf.Position.X, intf.Position.Y, intf.Position.Z, ref screenX, ref screenY);

                        if (tp.IsBillboard)
                        {
                            var billboard = new BillboardInteractItem()
                            {
                                caption = tp.Caption,
                                keyBind = tp.Keybind,
                                active = false,
                                locked = false,
                                location = new BillboardLocation()
                                {
                                    x = screenX * scW,
                                    y = screenY * scH
                                }
                            };
                            BillboardInteract.AddInteract(billboard);
                            Services.TaskService.InvokeUntilExpire(async () =>
                            {
                               
                                float oldScreenX, oldScreenY;
                                oldScreenX = screenX;
                                oldScreenY = screenY;
                                GetScreenCoordFromWorldCoord(intf.Position.X, intf.Position.Y, intf.Position.Z, ref screenX, ref screenY);
                                if (KeybindService.IsKeyDown(tp.Keybind))
                                {
                                    BillboardInteract.UpdateInteractActiveStatus(tp.Keybind, tp.Caption, new BillboardLocation() { x = oldScreenX * scW, y = oldScreenY * scH }, true);
                                }
                                if (KeybindService.IsKeyJustUp(tp.Keybind))
                                {
                                    BillboardInteract.UpdateInteractActiveStatus(tp.Keybind, tp.Caption, new BillboardLocation() { x = oldScreenX * scW, y = oldScreenY * scH }, false);
                                }
                                BillboardInteract.UpdateInteractLocation(tp.Keybind, tp.Caption, new BillboardLocation() { x = oldScreenX * scW, y = oldScreenY * scH },
                                    new BillboardLocation()
                                    {
                                        x= screenX * scW,
                                        y= screenY * scH
                                    });
                                var canShow2 = await intf.CanShow();
                                if (!canShow2)
                                {
                                    BillboardInteract.RemoveInteract(billboard);
                                }
                                if (!canShow2) return true;
                                return false;
                            });
                        }
                        Interfaces.UI.KeybindService.RegisterKeybind(tp.Keybind, tp.Caption, async () =>
                        {
                            try
                            {
                                intf.Interact();
                            }
                            catch (Exception ex)
                            {
                                DebugService.UnhandledException(ex);
                            }
                            
                           /* Interfaces.UI.KeybindService.RemoveKeybind(tp.Keybind);*/
                        });
                    }
                }else if (ShownDict[intf])
                {
                    ShownDict[intf] = false;
                    var tp = intf.GetType().GetCustomAttribute<InteractableAttribute>();
                    Interfaces.UI.KeybindService.RemoveKeybind(tp.Keybind);
                  
                }
            }
        }
        public static void TerminateInteractable(IInteractable intf)
        {
            if (ShownDict.ContainsKey(intf))
            {
                if (ShownDict[intf])
                {
                    
                    try {
                        var tp = intf.GetType().GetCustomAttribute<InteractableAttribute>();
                        Interfaces.UI.KeybindService.RemoveKeybind(tp.Keybind);
                        ShownDict.Remove(intf);
                    }
                    catch
                    {

                    }
                }
                
            }
            if (Interactables.Contains(intf))
                Interactables.Remove(intf);
        }

        [ExecuteAt(ExecutionStage.Initialized)]
        public static void DiscoverInteractables()
        {
            foreach (var typ in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (typ.GetCustomAttribute<InteractableAttribute>() != null)
                {
                    var tp = typ.GetCustomAttribute<InteractableAttribute>();
                    if (!InteractTemplates.ContainsKey(tp.InteractName))
                    {
                        DebugService.DebugCall("INTERACT", "Registered interact: " + tp.InteractName);
                        InteractTemplates.Add(tp.InteractName, typ);

                        if (tp.Attachment == GenericInteractAttachment.Ped)
                        {
                            var inst = (IInteractable)Activator.CreateInstance(typ);
                            _generic_ped_incs.Add(inst);
                            Interactables.Add(inst);
                            ShownDict.Add(inst, false);
                        }
                        if (tp.Attachment == GenericInteractAttachment.Vehicle)
                        {
                            var inst = (IInteractable)Activator.CreateInstance(typ);
                            _generic_vehicle_incs.Add(inst);
                            Interactables.Add(inst);
                            ShownDict.Add(inst, false);
                        }
                        if (tp.Attachment == GenericInteractAttachment.Door)
                        {
                            var inst = (IInteractable)Activator.CreateInstance(typ);
                            _generic_door_incs.Add(inst);
                            Interactables.Add(inst);
                            ShownDict.Add(inst, false);
                        }
                        if (tp.Attachment == GenericInteractAttachment.Gas)
                        {
                            var inst = (IInteractable)Activator.CreateInstance(typ);
                            _generic_gas_incs.Add(inst);
                            Interactables.Add(inst);
                            ShownDict.Add(inst, false);
                        }
                        if (tp.Attachment == GenericInteractAttachment.Prop)
                        {
                            var inst = (IInteractable)Activator.CreateInstance(typ);
                            _generic_props_incs.Add(inst);
                            Interactables.Add(inst);
                            ShownDict.Add(inst, false);
                        }
                    }
                    
                    
                }
            }
        }

    }
}
