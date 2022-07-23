using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
namespace ProjectEmergencyFrameworkClient.Interact
{

    [Interactable("tg_0", "G", "Tackle", true, GenericInteractAttachment.Ped)]
    public class TackleInteract : LookatRadiusInteractable
    {
        public TackleInteract()
        {
            Radius = 3f;
            Tolerance = 30f / 2;
            RequireControlKeyDown = false;
        }
        public override async Task<bool> CanShow()
        {
            if (!IsPedRunning(Game.PlayerPed.Handle)) return false;
            return await base.CanShow();
        }
        protected override async void OnInteract()
        {
           
                RPC.RPCService.RemoteCall(NetworkGetNetworkIdFromEntity(Entity.Handle), "Tackle", Game.Player.ServerId);
                await Utility.AssetLoader.LoadAnimDict("missmic2ig_11");
                TaskPlayAnim(Game.PlayerPed.Handle, "missmic2ig_11", "mic_2_ig_11_intro_goon", 8.0f, 8.0f, 3000, 0, 0, false, false, false);
                await BaseScript.Delay(3000);
            
        }
    }
    [Interactable("tg_1", "Z", "Grab", true, GenericInteractAttachment.Ped)]
    public class GrabInteract : LookatRadiusInteractable
    {
        public GrabInteract()
        {
            Radius = 3f;
            Tolerance = 30f / 2;
            RequireControlKeyDown = true;
        }
        public override async Task<bool> CanShow()
        {
            if (IsPedRunning(Game.PlayerPed.Handle) || IsPedRunning(Entity.Handle)) return false;
            if (!((Ped)Entity).IsCuffed) return false;
            if (!Services.OrganizationService.IsOnDuty) return false;
            if (Services.OrganizationService.ConnectedOrganization.OrgType != "POLICE") return false;
            if (Services.CharacterService.IsCharacterGrabbing) return false;
            return await base.CanShow();
        }
        protected override async void OnInteract()
        {
            Services.CharacterService.IsCharacterGrabbing = true;
            await Utility.AssetLoader.LoadAnimDict("doors@");
                Game.PlayerPed.Task.PlayAnimation("doors@", "door_sweep_r_hand_medium", 9f, 2500, 2+16);
                await BaseScript.Delay(2000);
                RPC.RPCService.RemoteCall(NetworkGetNetworkIdFromEntity(Entity.Handle), "Grab", Game.Player.ServerId);
                DisableControlAction(0, 23, true);
                bool IsAttached = true;
                Services.CharacterService.IsCharacterGrabbing = true;
                Services.CharacterService.CharacterBeingGrabbed = Entity.Handle;
            //Interfaces.UI.KeybindService.RemoveKeybind("Z");
            Interfaces.UI.KeybindService.RegisterKeybind("Z", "Detach", () =>
                {
                    RPC.RPCService.RemoteCall(NetworkGetNetworkIdFromEntity(Entity.Handle), "EndGrab", Game.Player.ServerId);
                    Interfaces.UI.KeybindService.RemoveKeybind("Z");
                    IsAttached = false;
                    Services.CharacterService.IsCharacterGrabbing = false;
                    EnableControlAction(0, 23, true);
                    Game.PlayerPed.Task.ClearAll();
                });
                GrabPedInCarInteract gp = new GrabPedInCarInteract();
               /* Services.TaskService.InvokeUntilExpire(async () =>
                {
                    var veh = World.GetClosest<Vehicle>(Game.PlayerPed.Position);
                    if (veh.ClassType == VehicleClass.Emergency)
                    {
                        gp.Entity = veh;
                    }
                    return !IsAttached;
                });*/
          
           

        }
    }
}
