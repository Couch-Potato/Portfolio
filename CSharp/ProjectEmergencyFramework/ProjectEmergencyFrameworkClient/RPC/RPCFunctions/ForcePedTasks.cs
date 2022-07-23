using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Services;
using ProjectEmergencyFrameworkClient.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;

namespace ProjectEmergencyFrameworkClient.RPC.RPCFunctions
{
    public static class ForcePedTasks
    {
        [RPCQuery("RequestAnimDict")]
        public static async void RequestAnimationDictionary(Query q, object data)
        {
           
                string d = (string)data;
                if (!HasAnimDictLoaded(d))
                {
                    RequestAnimDict(d);
                    while (!HasAnimDictLoaded(d))
                    {
                        await BaseScript.Delay(0);
                    }
                }
                q.Reply(true);
            
           
        }

        [RPCFunction("ClearPlayerSecondaryTasks")]
        public static void ClearPlayerSecondaryTasks(object data)
        {
            
        }





        [RPCFunction("DoHandcuff")]
        public static async void DoHandcuff(object data)
        {
            
                InventoryService.SetHotbarSelected(-1);
                await BaseScript.Delay(2000);
                TaskPlayAnim(Game.PlayerPed.Handle, "mp_arresting", "idle", 8.0f, -8, -1, 49, 0, false,false,false);
                SetEnableHandcuffs(Game.PlayerPed.Handle, true);
                SetCurrentPedWeapon(Game.PlayerPed.Handle, 0, true);
                CharacterService.CharacterCuffed = true;
                
          
        }

        [RPCFunction("Grab")]
        public static async void DoGrab(object data)
        {
            int num = CrappyWorkarounds.ShittyFiveMDynamicToConcrete<int>(data);
            CharacterService.IsChracterGrabbed = true;
                CharacterService.PedGrabbingCharacter = GetPlayerPed(GetPlayerFromServerId(num));
            AttachEntityToEntity(GetPlayerPed(-1), GetPlayerPed(GetPlayerFromServerId(num)), 11816, 0.25f, 0.5f, 0.0f, 0.5f, 0.5f, 0.0f, false, false, false, false, 2, false);
            TaskService.InvokeUntilExpire(async () =>
                {
                    if (IsPedWalking(CharacterService.PedGrabbingCharacter))
                        SimulatePlayerInputGait(Game.Player.Handle, 1f, -1, 0f, true, false);
                    else
                        SimulatePlayerInputGait(Game.Player.Handle, 0f, 0, 0f, true, false);
                    if (!CharacterService.IsChracterGrabbed)
                        SimulatePlayerInputGait(Game.Player.Handle, 0f, 0, 0f, true, false);
                    return !CharacterService.IsChracterGrabbed;
                });
          

        }
        [RPCFunction("EndGrab")]
        public static async void DeGrab(object data)
        {
            
                CharacterService.IsChracterGrabbed = false;
                DetachEntity(Game.PlayerPed.Handle, true, false);
             
        }
        [RPCFunction("Tackle")]
        public static async void Tackle(object data)
        {
            
                await AssetLoader.LoadAnimDict("missmic2ig_11");
            DebugService.Watchpoint("NWX", data);
            int num = CrappyWorkarounds.ShittyFiveMDynamicToConcrete<int>(data);
                AttachEntityToEntity(GetPlayerPed(-1), GetPlayerPed(GetPlayerFromServerId(num)), 11816, 0.25f, 0.5f, 0.0f, 0.5f, 0.5f, 180.0f, false, false, false, false, 2, false);
                TaskPlayAnim(Game.PlayerPed.Handle, "missmic2ig_11", "mic_2_ig_11_intro_p_one", 8.0f, -8.0f, 3000, 0, 0, false, false, false);
                await BaseScript.Delay(3000);
                DetachEntity(Game.PlayerPed.Handle, true, false);
            
        }
        [RPCFunction("PutInVehicle")]
        public static void PutInVehicle(object data)
        {
            var dx = (dynamic)data;
            CharacterService.IsChracterGrabbed = false;
            DetachEntity(Game.PlayerPed.Handle, true, false);
            Game.PlayerPed.Task.EnterVehicle(new Vehicle(dx.handle), (VehicleSeat)dx.seat);
        }

        [RPCFunction("EndHandcuff")]
        public static async void EndHandcuff(object data)
        {
            
                await BaseScript.Delay(2000);
                ClearPedSecondaryTask(Game.PlayerPed.Handle);
                SetEnableHandcuffs(Game.PlayerPed.Handle, true);
                SetCurrentPedWeapon(Game.PlayerPed.Handle, (uint)GetHashKey("WEAPON_UNARMED"), true);
                CharacterService.CharacterCuffed = false;
       
        }
    }
}
