using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
namespace ProjectEmergencyFrameworkClient.Effects
{
    [HealthEffect("drug@weed_01", "green", "HIGH", "/assets/effects/weed.svg")]
    public class DrugEffectWeed : IHealthEffect
    {
        public void OnEffectStart()
        {
            AnimpostfxPlay("ChopVision", 0, true);
        }

        public void OnEffectStop()
        {
            AnimpostfxStop("ChopVision");
        }

        public void Tick()
        {
            
        }
    }
    [HealthEffect("drug@generic_01", "blue", "HIGH", "/assets/effects/high.svg")]
    public class DrugEffectGeneric01 : IHealthEffect
    {
        public void OnEffectStart()
        {
            AnimpostfxPlay("DrugsMichaelAliensFightIn", 5000, false);
            AnimpostfxPlay("DrugsMichaelAliensFight", 0, true);
            SetPedIsDrunk(Game.PlayerPed.Handle, true);
            ShakeGameplayCam("DRUNK_SHAKE", 1f);
            HealthService.WalkState = WalkState.Drunk;
        }

        public void OnEffectStop()
        {
            AnimpostfxStop("DrugsMichaelAliensFight");
            AnimpostfxPlay("DrugsMichaelAliensFightOut", 5000, false);
            SetPedIsDrunk(Game.PlayerPed.Handle, false);
            HealthService.WalkState = WalkState.Normal;
            StopGameplayCamShaking(true);
        }

        public void Tick()
        {

        }
    }
    [HealthEffect("drug@generic_02", "blue", "HIGH", "/assets/effects/high.svg")]
    public class DrugEffectGeneric02 : IHealthEffect
    {
        public void OnEffectStart()
        {
            AnimpostfxPlay("DrugsTrevorClownsFightIn", 5000, false);
            AnimpostfxPlay("DrugsTrevorClownsFight", 0, true);
            SetPedIsDrunk(Game.PlayerPed.Handle, true);
            ShakeGameplayCam("DRUNK_SHAKE", 1f);
            HealthService.WalkState = WalkState.Drunk;
        }

        public void OnEffectStop()
        {
            AnimpostfxStop("DrugsTrevorClownsFight");
            AnimpostfxPlay("DrugsTrevorClownsFightOut", 5000, false);
            SetPedIsDrunk(Game.PlayerPed.Handle, false);
            HealthService.WalkState = WalkState.Normal;
            StopGameplayCamShaking(true);
        }

        public void Tick()
        {

        }
    }
    [HealthEffect("drug@generic_03", "blue", "HIGH", "/assets/effects/high.svg")]
    public class DrugEffectGeneric03 : IHealthEffect
    {
        public void OnEffectStart()
        {
            AnimpostfxPlay("Rampage", 0, true);
            SetPedIsDrunk(Game.PlayerPed.Handle, true);
            ShakeGameplayCam("DRUNK_SHAKE", 1f);
            HealthService.WalkState = WalkState.Drunk;
        }

        public void OnEffectStop()
        {
            AnimpostfxStop("Rampage");
            AnimpostfxPlay("RampageOut", 5000, false);
            SetPedIsDrunk(Game.PlayerPed.Handle, false);
            HealthService.WalkState = WalkState.Normal;
            StopGameplayCamShaking(true);
        }

        public void Tick()
        {

        }
    }
    [HealthEffect("drug@drunk", "blue", "DRUNK", "/assets/effects/drunk.svg")]
    public class DrugEffectDrunk : IHealthEffect
    {
        public void OnEffectStart()
        {
            SetPedIsDrunk(Game.PlayerPed.Handle, true);
            ShakeGameplayCam("DRUNK_SHAKE", 1f);
            HealthService.WalkState = WalkState.Drunk;
        }

        public void OnEffectStop()
        {
            SetPedIsDrunk(Game.PlayerPed.Handle, false);
            HealthService.WalkState = WalkState.Normal;
            StopGameplayCamShaking(true);
        }

        public void Tick()
        {

        }
    }
}
