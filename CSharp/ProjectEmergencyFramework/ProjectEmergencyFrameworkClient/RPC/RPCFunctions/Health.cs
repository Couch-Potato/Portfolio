using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Effects;
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
    public static class Health
    {
        [RPCFunction("HelpUp")]
        public static void ClearPlayerSecondaryTasks(object data)
        {
            Game.PlayerPed.Health = 36;
            if (HealthEffectService.IsEffectRunning("health@injured"))
            {
                HealthEffectService.StopHealthEffect("health@injured");
            }
        }

        [RPCFunction("MEDICAL_HEAL")]
        public static void MedicalBill(object data)
        {
            Game.PlayerPed.Health = 100;
            HealthService.IsBeingTreated = false;
            if (HealthEffectService.IsEffectRunning("health@dying"))
            {
                HealthEffectService.StopHealthEffect("health@dying");
            }
            if (HealthEffectService.IsEffectRunning("health@injured"))
            {
                HealthEffectService.StopHealthEffect("health@injured");
            }
            Game.PlayerPed.IsInvincible = false;
            Str_1(false);
            NetworkRequestControlOfEntity(HealthService.Stretcher);
            DeleteEntity(ref HealthService.Stretcher);

            HUDService.ShowHelpText("All better now!!!", "none", 2.5f);
        }

        [RPCFunction("HANDLE_DEATH")]
        public static async void HandleDeath(object data)
        {
            HUDService.ShowHelpText("You are getting very sleeply...", "none", 2.5f);


            Game.PlayerPed.Health = 100;
            HealthService.IsBeingTreated = false;
            if (HealthEffectService.IsEffectRunning("health@dying"))
            {
                HealthEffectService.StopHealthEffect("health@dying");
            }
            if (HealthEffectService.IsEffectRunning("health@injured"))
            {
                HealthEffectService.StopHealthEffect("health@injured");
            }

            DoScreenFadeOut(10000);
            QueryService.QueryConcrete<bool>("MARK_ME_DEAD", CharacterService.CurrentCharacter.Id);
            await BaseScript.Delay(10000);
            Game.PlayerPed.IsInvincible = false;
            Str_1(false);
            NetworkRequestControlOfEntity(HealthService.Stretcher);
            DeleteEntity(ref HealthService.Stretcher);
            Interfaces.InterfaceController.ShowInterface("characterselector");

            
        }

        [RPCFunction("MedicRevive")]
        public static void MedicReviveTasks(object data)
        {
            //Game.PlayerPed.Health = 36;
            HealthService.IsBeingTreated = true;
            if (HealthEffectService.IsEffectRunning("health@dying"))
            {
                HealthEffectService.StopHealthEffect("health@dying");
            }
        }

        [RPCFunction("StretcherGetIn")]
        public static async void Str_0(object data)
        {
            var str = (int)data;
            HealthService.Stretcher = NetworkGetEntityFromNetworkId(str);
            AttachEntityToEntity(Game.PlayerPed.Handle, HealthService.Stretcher, 0, 0, 2.1f, 0, 0, 270, 0, false, false, false, false, 2, true);
            HealthService.IsCurrentlyInStretcher = true;
            await Utility.AssetLoader.LoadAnimDict("anim@gangops@morgue@table@");
            TaskService.InvokeUntilExpire(async () =>
            {
                if (!IsEntityPlayingAnim(Game.PlayerPed.Handle, "anim@gangops@morgue@table@", "ko_front", 3))
                {
                    TaskPlayAnim(Game.PlayerPed.Handle, "anim@gangops@morgue@table@", "ko_front", 8.0f, 8.0f, -1, 69, 1, false, false, false);
                }
                return !HealthService.IsCurrentlyInStretcher;
            });
        }

        [RPCFunction("EndStretcher")]
        public static void Str_1(object data)
        {
            DetachEntity(Game.PlayerPed.Handle, true, true);
            var p = new Prop(HealthService.Stretcher);
            var vec = p.Position * p.ForwardVector * -0.7f;
            p.Position = vec;
            HealthService.IsCurrentlyInStretcher = false;
        }
    }
}
