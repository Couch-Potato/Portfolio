using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
namespace ProjectEmergencyFrameworkClient.Equipables
{
    [Equipable("Medical Kit", "/assets/inventory/health.svg")]
    public class MedicalKitEquipable : Equipable
    {
        protected override async void OnPrimaryUp()
        {
            if (Game.PlayerPed.Health > 70)
            {
                HUDService.ShowHelpText("YOU NEED TO BE MORE INJURED TO USE A MED KIT.", "red", 2.5f);
                return;
            }
            await Utility.AssetLoader.LoadAnimDict("random@train_tracks");
            HUDService.ShowHelpText("HEALING...", "none", 2.5f);
            Game.PlayerPed.Task.PlayAnimation("random@train_tracks", "idle_e", 8.0f, 8000, AnimationFlags.UpperBodyOnly);
            await BaseScript.Delay(8000);
            Game.PlayerPed.Task.ClearAnimation("random@train_tracks", "idle_e");
            Game.PlayerPed.Health = 70;
            HUDService.ShowHelpText("YOU HAVE BEEN HEALED!", "green", 2.5f);
        }
    }
}
