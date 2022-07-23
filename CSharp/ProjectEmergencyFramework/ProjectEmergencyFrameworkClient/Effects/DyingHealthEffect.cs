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
    [HealthEffect("health@dying", "red", "DYING", "/assets/effects/dying.svg")]
    public class DyingHealthEffect : IHealthEffect
    {
        public void OnEffectStart()
        {
            HUDService.ForceHUDUnlock();
            HUDService.ShowHelpText("YOU ARE DYING. YOU NEED MEDICAL ATTENTION. PRESS ~G~ TO RESPAWN.", "none");
            HUDService.Lock(this);
        }

        public void OnEffectStop()
        {
            HUDService.ForceHUDUnlock();
            HUDService.HideHelpText();
        }

        public void Tick()
        {
            if (IsControlJustReleased(0, 47))
            {
                CharacterService.RespawnCharacter();
            }
            SetPedToRagdoll(Game.PlayerPed.Handle, 1000, 1000, 0, false, false, false);
        }
    }
    [HealthEffect("health@injured", "red", "INJURED", "/assets/effects/injured.svg")]
    public class InjuredHealthEffect : TimedHealthEffect, IHealthEffect
    {
        public InjuredHealthEffect()
        {
            EffectDuration = 60f;
        }
        public new void OnEffectStart()
        {
            HUDService.ForceHUDUnlock();
            HUDService.ShowHelpText("YOU ARE INJURED. WAIT 60 SECONDS OR ASK SOMEONE FOR HELP", "none");
            HUDService.Lock(this);
            base.OnEffectStart();
        }

        public new void OnEffectStop()
        {
            HUDService.ForceHUDUnlock();
            HUDService.HideHelpText();
            Game.PlayerPed.Health = 35;
        }

        public new void Tick()
        {
            if (60 - ((int)Timestamp() - TimeStart) <= 0)
            {
                HealthEffectService.StopHealthEffect(this);
            }
            float timeLeft = EffectDuration - (TimeStart - Timestamp());
            HUDService.Unlock(this);
            HUDService.ForceHUDUnlock();
            HUDService.SetHelpText($"YOU ARE INJURED. WAIT {60-((int)Timestamp() - TimeStart)} SECONDS OR ASK SOMEONE FOR HELP");
            HUDService.Lock(this);
            SetPedToRagdoll(Game.PlayerPed.Handle, 1000, 1000, 0, false, false, false);
            base.Tick();
        }
    }
}
