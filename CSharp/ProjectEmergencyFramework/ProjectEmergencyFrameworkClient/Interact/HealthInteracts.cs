using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Interfaces.UI;
using ProjectEmergencyFrameworkClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
namespace ProjectEmergencyFrameworkClient.Interact
{
    [Interactable("health@getup_1_1", "G", "HELP UP", true, GenericInteractAttachment.Ped)]
    public class HelpUpInteract: LookatRadiusInteractable
    {
        public HelpUpInteract()
        {
            Radius = 3f;
            RequireControlKeyDown = true;
            Tolerance = 30f / 2;
        }
        public override async Task<bool> CanShow()
        {
            var ped = new Ped(Entity.Handle);
            if (ped.Health > 35 || !ped.IsAlive || ped.Health <= 0) return false;
            return await base.CanShow();
        }

        protected override void OnInteract()
        {
            
            HUDService.ShowHelpText("ASSISTING PERSON...", "none", 3f);
            HUDService.Lock(this);
            Task.Run(async () =>
            {
                await Utility.AssetLoader.LoadAnimDict("amb@medic@standing@tendtodead@base");
                TaskPlayAnim(Game.PlayerPed.Handle, "amb@medic@standing@tendtodead@base", "base", 1000, 1000, 3000, 1, 1.0f, false, false, true);
                await BaseScript.Delay(10000);
                RPC.RPCService.RemoteCall(NetworkGetPlayerIndexFromPed(Entity.Handle), "HelpUp", false);
                HUDService.Unlock(this);
                HUDService.ShowHelpText("PERSON HAS BEEN ASSISTED", "none", 3f);
                ClearPedTasks(Game.PlayerPed.Handle);
            });
        }
    }
    [Interactable("health@getup_1_2", "G", "REVIVE", true, GenericInteractAttachment.Ped)]
    public class ReviveInteract : LookatRadiusInteractable
    {
        public ReviveInteract()
        {
            Radius = 3f;
            RequireControlKeyDown = true;
            Tolerance = 30f / 2;
        }
        public override async Task<bool> CanShow()
        {
            var ped = new Ped(Entity.Handle);
            if (ped.IsAlive && ped.Health > 0) return false;
            if (!OrganizationService.IsOnDuty) return false;
            if (OrganizationService.ConnectedOrganization.CallableId != "BASE:LSFD") return false;
            return await base.CanShow();
        }

        protected override void OnInteract()
        {

            HUDService.ShowHelpText("REVIVNG PERSON...", "none", 3f);
            HUDService.Lock(this);
            RPC.RPCService.RemoteCall(NetworkGetPlayerIndexFromPed(Entity.Handle), "MedicRevive", false);
            Task.Run(async () =>
            {
                await Utility.AssetLoader.LoadAnimDict("amb@medic@standing@tendtodead@base");
                TaskPlayAnim(Game.PlayerPed.Handle, "amb@medic@standing@tendtodead@base", "base", 1000, 1000, 3000, 1, 1.0f, false, false, true);
                await BaseScript.Delay(10000);
                RPC.RPCService.RemoteCall(NetworkGetPlayerIndexFromPed(Entity.Handle), "MedicRevive", false);
                HUDService.Unlock(this);
                HUDService.ShowHelpText("PERSON HAS BEEN ASSISTED", "none", 3f);
                ClearPedTasks(Game.PlayerPed.Handle);
            });
        }
    }

    [Interactable("health@stretcher_1_1", "G", "PUT IN STRETCHER", true, GenericInteractAttachment.Ped)]
    public class StretcherInteract : LookatRadiusInteractable
    {
        public StretcherInteract()
        {
            Radius = 5f;
            Tolerance = 30f / 2;
        }
        public override async Task<bool> CanShow()
        {
            var ped = new Ped(Entity.Handle);
            if (!HealthService.isStretcherOut) return false;
            if (HealthService.HasPersonInStretcher) return false;
            return await base.CanShow();
        }

        protected override void OnInteract()
        {
            HealthService.IsStretcherOutOfCustody = false;
            HealthService.HasPersonInStretcher = true;
            HealthService.PersonInStretcher = new Ped(Entity.Handle);
            RPC.RPCService.RemoteCall(NetworkGetPlayerIndexFromPed(Entity.Handle), "StretcherGetIn", NetworkGetNetworkIdFromEntity(HealthService.Stretcher));
        }
    }
    [Interactable("health@stretcher_1_2", "G", "PUT INTO VEHICLE", true, GenericInteractAttachment.Vehicle)]
    public class StretcherVehInteract : LookatRadiusInteractable
    {
        public StretcherVehInteract()
        {
            Radius = 5f;
            Tolerance = 30f / 2;
        }
        public override async Task<bool> CanShow()
        {
            var veh = new Vehicle(Entity.Handle);
            if (!HealthService.isStretcherOut) return false;
            if (veh.ClassType != VehicleClass.Emergency) return false;
            if (!veh.State.Get("ambulance")) return false;
            if (veh.State.Get("hasStretcher")) return false;
            return await base.CanShow();
        }

        protected override async void OnInteract()
        {
            var veh = new Vehicle(Entity.Handle);
            veh.Doors[VehicleDoorIndex.BackLeftDoor].Open();
            veh.Doors[VehicleDoorIndex.BackLeftDoor].Open();
            await BaseScript.Delay(1000);
            HealthService.IsStretcherOutOfCustody = true;
            
            DetachEntity(HealthService.Stretcher, true, true);
            veh.State.Set("hasStretcher", true, true);
            veh.State.Set("stretcher", NetworkGetNetworkIdFromEntity(HealthService.Stretcher), true);
            AttachEntityToEntity(HealthService.Stretcher, veh.Handle, 0, 0.0f, -3.7f, 0.0f, 0.0f,0.0f, 90.0f, false, false, true, false, 2, true);
            FreezeEntityPosition(HealthService.Stretcher, true);
            InventoryService.RemoveItem("Stretcher", "");
            await BaseScript.Delay(1000);
            veh.Doors[VehicleDoorIndex.BackLeftDoor].Close();
            veh.Doors[VehicleDoorIndex.BackLeftDoor].Close();
        }
    }
    [Interactable("health@stretcher_1_3", "G", "GET STRETCHER", true, GenericInteractAttachment.Vehicle)]
    public class StretcherVeh2Interact : LookatRadiusInteractable
    {
        public StretcherVeh2Interact()
        {
            Radius = 5f;
            Tolerance = 30f / 2;
        }
        public override async Task<bool> CanShow()
        {
            var veh = new Vehicle(Entity.Handle);
            if (!HealthService.isStretcherOut) return false;
            if (veh.ClassType != VehicleClass.Emergency) return false;
            if (!veh.State.Get("ambulance")) return false;
            if (!veh.State.Get("hasStretcher")) return false;
            return await base.CanShow();
        }

        protected override async void OnInteract()
        {
            var veh = new Vehicle(Entity.Handle);
            veh.Doors[VehicleDoorIndex.BackLeftDoor].Open();
            veh.Doors[VehicleDoorIndex.BackLeftDoor].Open();
            await BaseScript.Delay(1000);
            var stretcher = NetworkGetEntityFromNetworkId(veh.State.Get("stretcher"));
            DetachEntity(stretcher, true, true);
            FreezeEntityPosition(stretcher, false);
            Vector3 c = GetEntityCoords(stretcher, false);
            SetEntityCoords(stretcher, c.X, c.Y, c.Z, false, false, false, false);
            PlaceObjectOnGroundProperly(stretcher);
            HealthService.Stretcher = stretcher;
            HealthService.isStretcherOut = true;
            AttachEntityToEntity(HealthService.Stretcher, Game.PlayerPed.Handle, GetPedBoneIndex(Game.PlayerPed.Handle, 28422), 0.0f, -0.6f, -1.43f, 180, 170, 90, false, false, false, true, 2, true);
            KeybindService.RegisterKeybind("G", "STOP PUSHING", () =>
            {
                HealthService.isStretcherOut = false;
                DetachEntity(HealthService.Stretcher, true, true);
            });
            TaskService.InvokeUntilExpire(async () =>
            {
                
                if (!IsEntityPlayingAnim(Game.PlayerPed.Handle, "anim@heists@box_carry@", "idle", 3))
                {
                    TaskPlayAnim(Game.PlayerPed.Handle, "anim@heists@box_carry@", "idle", 8.0f, 8.0f, -1, 50, 0, false, false, false);
                }
                return !HealthService.isStretcherOut;
            });

        }
    }
}
