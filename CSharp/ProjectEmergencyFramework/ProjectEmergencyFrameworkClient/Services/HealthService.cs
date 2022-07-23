using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Effects;
using ProjectEmergencyFrameworkClient.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
namespace ProjectEmergencyFrameworkClient.Services
{
    public enum WalkState
    {
        Normal,
        Drunk,
        Injured
    }
    public static class HealthService
    {
        //EMS / STRETCHER
        public static bool HasPersonInStretcher = false;
        public static Ped PersonInStretcher;

        public static bool IsCurrentlyInStretcher = false;
        public static Ped StretcherOwner;

        public static bool HasStretcherEquipped = false;
        public static int Stretcher;
        public static bool isStretcherOut = false;
        // OTHER

        public static bool IsStretcherOutOfCustody = false;

        public static bool IsBeingTreated = false;
        public static bool PedCanWalk = true;
        public static bool IsAlive = true;
        private static WalkState _walkState = WalkState.Normal;

        public static float HighTHC = 0f;
        public static float HighOtr = 0f;
        public static float DrunkPercent = 0f;

        static uint LastDegredation = CharacterService.Timestamp();

        public static WalkState WalkState { get => _walkState;set {
                if (_walkState == value) return;
                _walkState = value;
                switch (value)
                {
                    case WalkState.Normal:
                        SetWalk("move_m@multiplayer");
                        break;
                    case WalkState.Drunk:
                        SetWalk("move_m@drunk@verydrunk");
                        break;
                    case WalkState.Injured:
                        SetWalk("move_characters@jimmy@slow@");
                        break;
                }
            } 
        }
        private static async void SetWalk(string walk)
        {
            await AssetLoader.LoadAnimDict(walk);
            SetPedMovementClipset(Game.PlayerPed.Handle, walk, 0.2f);
        }


        [ExecuteAt(ExecutionStage.OnResourceStart)]
        public static void HealthServiceInit()
        {

        }

        static bool IsHighEffectPlaying = false;
        static string HighEffect = "";
        [ExecuteAt(ExecutionStage.Tick)]
        public static async void HealthServiceTick()
        {
            SetPlayerHealthRechargeMultiplier(Game.Player.Handle, 0f);
            if (Game.PlayerPed.Health < 50)
            {
                WalkState = WalkState.Injured;
            }
            if (IsBeingTreated)
            {
                SetPedToRagdoll(Game.PlayerPed.Handle, 1000, 1000, 0, false, false, false);
            }
            if ((Game.PlayerPed.IsDead || Game.PlayerPed.Health == 0 ) && !IsBeingTreated)
            {
                if (HealthEffectService.IsEffectRunning("health@injured"))
                {
                    HealthEffectService.StopHealthEffect("health@injured");
                }
                if (!HealthEffectService.IsEffectRunning("health@dying"))
                {
                    HealthEffectService.StartHealthEffect("health@dying");
                }
                SetEnableHandcuffs(Game.PlayerPed.Handle, true);
                Framework.FrameworkController.EXP["spawnmanager"].setAutoSpawn(false);
                Game.PlayerPed.Resurrect();
                SetEntityHealth(Game.PlayerPed.Handle, 200);
                Game.PlayerPed.IsInvincible = true;
                IsAlive = false;
            }
            if (Game.PlayerPed.Health < 35 && !IsBeingTreated && IsAlive && HighOtr < 100f)
            {
                if (!HealthEffectService.IsEffectRunning("health@injured"))
                {
                    HealthEffectService.StartHealthEffect("health@injured");
                }
            }

            if (HighOtr >= 100f && !IsHighEffectPlaying)
            {
                IsHighEffectPlaying = true;
                Random rnd = new Random();
                int effectId = rnd.Next(0, 2);
                if (effectId == 0)
                {
                    HealthEffectService.StartHealthEffect("drug@generic_01");
                    HighEffect = "drug@generic_01";
                }
                if (effectId == 1)
                {
                    HealthEffectService.StartHealthEffect("drug@generic_02");
                    HighEffect = "drug@generic_02";

                }
                if (effectId == 2)
                {
                    HealthEffectService.StartHealthEffect("drug@generic_03");
                    HighEffect = "drug@generic_03";
                }
            }

            if (HighTHC >= 100f)
            {
                if (!HealthEffectService.IsEffectRunning("drug@weed_01"))
                {
                    HealthEffectService.StartHealthEffect("drug@weed_01");
                }
            }

            if (DrunkPercent >= 100f)
            {
                if (!HealthEffectService.IsEffectRunning("drug@drunk"))
                {
                    HealthEffectService.StartHealthEffect("drug@drunk");
                }
            }

            if (HighOtr < 100f && IsHighEffectPlaying)
                HealthEffectService.StopHealthEffect(HighEffect);

            if (HighTHC < 100f && HealthEffectService.IsEffectRunning("drug@weed_01"))
                HealthEffectService.StopHealthEffect("drug@weed_01");

            if (DrunkPercent < 100f && HealthEffectService.IsEffectRunning("drug@drunk"))
                HealthEffectService.StopHealthEffect("drug@drunk");

            if (LastDegredation + (30) < CharacterService.Timestamp())
            {
                DrunkPercent -= 10;
                HighOtr -= 10;
                HighTHC -= 10;
                LastDegredation = CharacterService.Timestamp();
            }

            /* if (Game.PlayerPed.IsDead)
             {
                 SetEnableHandcuffs(Game.PlayerPed.Handle, true);
                 Framework.FrameworkController.EXP["spawnmanager"].setAutoSpawn(false);
                 SetEntityHealth(Game.PlayerPed.Handle, 200);
             }*/
        }
    }

}
