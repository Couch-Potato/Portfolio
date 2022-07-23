using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Services;

namespace ProjectEmergencyFrameworkClient.Interact
{
    [Interactable("cuff_0_0", "Z", "Cuff", true, GenericInteractAttachment.Ped)]
    public class CuffInteract : LookatRadiusInteractable
    {
        public CuffInteract()
        {
            Radius = 2f;
            Tolerance = 30f / 2;
            RequireControlKeyDown = false;
        }
        public override async Task<bool> CanShow()
        {
            if (new Ped(Entity.Handle).IsCuffed) return false;
           /* if (Entity.State["dutyOrganizationType"] != null)
            {
                if (Entity.State["dutyOrganizationType"] == "POLICE") return false;
            }*/
            if (InventoryService.GetEquippedItem()==null) return false;
            if (InventoryService.GetEquippedItem().name!="Handcuffs") return false;


            return await base.CanShow();
        }
        protected override async void OnInteract()
        {
            // CUFF STUFF HERE
            Services.InventoryService.RemoveItem("Handcuffs", "");
           
                await RPC.RPCService.RemoteQuery<bool>(NetworkGetNetworkIdFromEntity(Entity.Handle), "RequestAnimDict", "mp_arresting");
                await Utility.AssetLoader.LoadAnimDict("mp_arrest_paired");
                RPC.RPCService.RemoteCall(NetworkGetNetworkIdFromEntity(Entity.Handle), "DoHandcuff", false);
                TaskPlayAnim(Game.PlayerPed.Handle, "mp_arrest_paired", "cop_p2_back_right", 8.0f, -8, 3500, 16, 0, false, false, false);
                await BaseScript.Delay(3500);
                ClearPedTasks(Game.PlayerPed.Handle);
           
        }
    }
    [Interactable("cuff_0_1", "Z", "Uncuff", true, GenericInteractAttachment.Ped)]
    public class UncuffInteract : LookatRadiusInteractable
    {
        public UncuffInteract()
        {
            Radius = 3f;
            Tolerance = 30f / 2;
            RequireControlKeyDown = true;
        }
        public override async Task<bool> CanShow()
        {
            if (!new Ped(Entity.Handle).IsCuffed) return false;
            if (Entity.State["dutyOrganizationType"] != null)
            {
                if (Entity.State["dutyOrganizationType"] == "POLICE") return false;
            }
            if (!Services.InventoryService.HasItem("Handcuff Key", "")) return false;


            return await base.CanShow();
        }
        protected override void OnInteract()
        {
            // CUFF STUFF HERE
            Services.InventoryService.AddItem("Handcuffs", "");
            RPC.RPCService.RemoteCall(NetworkGetPlayerIndexFromPed(Entity.Handle), "EndHandcuff", false);
            Task.Run(async () =>
            {
                ClearPedTasksImmediately(Game.PlayerPed.Handle);
                await Utility.AssetLoader.LoadAnimDict("mp_arresting");
                TaskPlayAnim(Game.PlayerPed.Handle, "mp_arresting", "a_uncuff", 8.0f, -8, -1, 2, 0, false, false, false);
                await BaseScript.Delay(2200);
                ClearPedTasksImmediately(Game.PlayerPed.Handle);
            });
        }
    }
    [Interactable("cuff_1_0", "Z", "Ziptie", true, GenericInteractAttachment.Ped)]
    public class ZiptieInteract : LookatRadiusInteractable
    {
        public ZiptieInteract()
        {
            Radius = 3f;
            RequireControlKeyDown = true;
            Tolerance = 30f / 2;
        }
        public override async Task<bool> CanShow()
        {
            if (new Ped(Entity.Handle).IsCuffed) return false;

            if (!Services.InventoryService.HasItem("Ziptie", "")) return false;
            if (Services.InventoryService.HasItem("Handcuffs", "")) return false;

            return await base.CanShow();
        }
        protected override void OnInteract()
        {
            // CUFF STUFF HERE
            Services.InventoryService.RemoveItem("Handcuffs", "");
            Task.Run(async () =>
            {
                await RPC.RPCService.RemoteQuery<bool>(NetworkGetPlayerIndexFromPed(Entity.Handle), "RequestAnimDict", "mp_arrest_paired");
                await Utility.AssetLoader.LoadAnimDict("mp_arrest_paired");
                RPC.RPCService.RemoteCall(NetworkGetPlayerIndexFromPed(Entity.Handle), "DoHandcuff", false);
                TaskPlayAnim(Game.PlayerPed.Handle, "mp_arrest_paired", "cop_p2_back_right", 8.0f, -8, 3500, 16, 0, false, false, false);
                await BaseScript.Delay(3500);
                ClearPedTasks(Game.PlayerPed.Handle);
            });
        }
    }
    [Interactable("cuff_1_1", "Z", "Cut Ziptie", true, GenericInteractAttachment.Ped)]
    public class UnZiptieInteract : LookatRadiusInteractable
    {
        public UnZiptieInteract()
        {
            Radius = 3f;
            RequireControlKeyDown = true;
            Tolerance = 30f / 2;
        }
        public override async Task<bool> CanShow()
        {
            if (!new Ped(Entity.Handle).IsCuffed) return false;
            if (!Services.InventoryService.HasItem("Scissors", "")) return false;


            return await base.CanShow();
        }
        protected override void OnInteract()
        {
            // CUFF STUFF HERE
            RPC.RPCService.RemoteCall(NetworkGetPlayerIndexFromPed(Entity.Handle), "EndHandcuff", false);
            Task.Run(async () =>
            {
                ClearPedTasksImmediately(Game.PlayerPed.Handle);
                await Utility.AssetLoader.LoadAnimDict("mp_arresting");
                TaskPlayAnim(Game.PlayerPed.Handle, "mp_arresting", "a_uncuff", 8.0f, -8, -1, 2, 0, false, false, false);
                await BaseScript.Delay(2200);
                ClearPedTasksImmediately(Game.PlayerPed.Handle);
            });
        }
    }
    [Interactable("cuff_0_0@veh_0", "F", "Put In Vehicle", false, GenericInteractAttachment.Vehicle)]
    public class GrabPedInCarInteract : RadiusInteractable
    {
        public GrabPedInCarInteract()
        {
            Radius = 5f;
        }
        public override async Task<bool> CanShow()
        {
            if (!new Ped(Entity.Handle).IsCuffed) return false;
           

            return await base.CanShow();
        }
        protected override void OnInteract()
        {
            var veh = (Vehicle)Entity;
            VehicleSeat seatId = VehicleSeat.LeftRear;

            // If we are closest to our right hand side of the vehicle then put it in there
            if (Vector3.Distance(Game.PlayerPed.Position, veh.GetOffsetPosition(Vector3.Left*1.5f)) > Vector3.Distance(Game.PlayerPed.Position, veh.GetOffsetPosition(Vector3.Right * 1.5f)))
            {
                seatId = VehicleSeat.RightRear;
            }

            if (veh.IsSeatFree(seatId))
            {
                TaskOpenVehicleDoor(Game.PlayerPed.Handle, veh.Handle, 6000, (int)seatId, 1.47f);
                RPC.RPCService.RemoteCall(NetworkGetPlayerIndexFromPed(Services.CharacterService.CharacterBeingGrabbed), "PutInVehicle", new
                {
                    handle = veh.Handle,
                    seat = (int)seatId
                });
            }
        }
    }
}
