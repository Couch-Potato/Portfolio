using System;
using System.Collections.Generic;
using System.Linq;
using static CitizenFX.Core.Native.API;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace ProjectEmergencyFrameworkClient.Interfaces.UI
{
    [UserInterface("uniformsystem", true)]
    public class UniformInterface : UserInterface
    {
        protected override async Task BeforeShow()
        {
            // Do the preshow uniform stuff here

            FreezeEntityPosition(GetPlayerPed(-1), false);
            DisableAllControlActions(0);
            DisplayRadar(false);
            var playerPed = PlayerPedId();
            SetEntityVisible(Game.PlayerPed.Handle, true, true);

            DoScreenFadeOut(500);

            await BaseScript.Delay(500);

            DestroyAllCams(true);
            int camera = 0;
            camera = CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", 458.6599f, - 991.4388f, 30.9293f, 0.00f, 0.00f, 180.00f, 50.00f, false, 0);
            SetCamActive(camera, true);
            RenderScriptCams(true, false, 2000, true, true);

            SetEntityCoords(Game.PlayerPed.Handle, 458.9651f, -993.0538f, 29.6893f, false, false, false, true);
            SetEntityHeading(Game.PlayerPed.Handle, 0.0f);
            SetGameplayCamRelativeHeading(00f);

            for (int i = 1; i < 256; i++)
            {
                if (NetworkIsPlayerActive(i))
                {
                    SetEntityVisible(GetPlayerPed(i), false, false);
                    SetEntityVisible(PlayerPedId(), true, true);
                    
                }
            }

            

            SelectedUniform = 0;

            DoScreenFadeIn(2000);
            await BaseScript.Delay(2000);

            await base.BeforeShow();
        }
        private async void Complete()
        {
            DoScreenFadeOut(1000);
            await BaseScript.Delay(1000);
            RenderScriptCams(false, false, 0, true, true);
            EnableAllControlActions(0);
            FreezeEntityPosition(Game.PlayerPed.Handle, false);

            // Replace with original onduty interact pos

            SetEntityCoords(Game.PlayerPed.Handle, 441.4f, -983f, 30.6f, true, false, false, false);
            SetEntityHeading(Game.PlayerPed.Handle, 0.0f);
            SetEntityCollision(Game.PlayerPed.Handle, true, true);
            SetEntityVisible(Game.PlayerPed.Handle, true, true);

            if (!IsPlayerSwitchInProgress())
            {
                SwitchOutPlayer(Game.PlayerPed.Handle, 1, 1);
            }

            for (int i = 1; i < 256; i++)
            {
                if (NetworkIsPlayerActive(i))
                {
                    SetEntityVisible(GetPlayerPed(i), true, true);
                    SetEntityVisible(PlayerPedId(), true, true);

                }
            }


            await BaseScript.Delay(5000);
            DoScreenFadeIn(1000);
            SwitchInPlayer(Game.PlayerPed.Handle);
            while (GetPlayerSwitchState() != 12)
            {
                await BaseScript.Delay(0);
            }
            DisplayRadar(true);
        }

        protected override void Cleanup()
        {
            Complete();

            base.Cleanup();

        }

        private int _selected = 0;
        private List<UniformItem> _uniforms= new List<UniformItem>();

        [Reactive("selectedUniform")]
        public int SelectedUniform { get => _selected; set {
                _selected = value;
                UpdateUniform();
            } 
        }

        [Configuration("uniforms")]
        public List<UniformItem> Uniforms { get => _uniforms; set => _uniforms = value; }

        //Updates the uniform displayed
        private void UpdateUniform()
        {
            // Needs get done fully!!!
            var uniform = Uniforms[SelectedUniform];
            SetPedComponentVariation(Game.PlayerPed.Handle, 11, uniform.shirt, 0, 0);
            SetPedComponentVariation(Game.PlayerPed.Handle, 4, uniform.lower, 0, 0);
            SetPedComponentVariation(Game.PlayerPed.Handle, 6, uniform.shoes, 0, 0);
            SetPedComponentVariation(Game.PlayerPed.Handle, 3, uniform.upper, 0, 0);
            SetPedComponentVariation(Game.PlayerPed.Handle, 8, uniform.teeshirt, 0, 0);
            SetPedComponentVariation(Game.PlayerPed.Handle, 7, uniform.accessories1, 0, 0);
        }

        protected override async Task ConfigureAsync()
        {
            Uniforms.Add(new UniformItem() { 
                make = "LSPD",
                model="CLASS A",
                upper=3,
                lower=35,
                shoes=51,
                shirt=56,
                teeshirt=200
            });
            Uniforms.Add(new UniformItem()
            {
                make = "LSPD",
                model = "CLASS C",
                upper = 11,
                lower = 35,
                shoes = 51,
                shirt = 56,
                teeshirt = 191
            });
            Uniforms.Add(new UniformItem()
            {
                make = "LSPD",
                model = "RAIN JACKET",
                upper = 3,
                lower = 35,
                shoes = 51,
                shirt = 56,
                teeshirt = 200
            });
            await base.ConfigureAsync();
        }
        [Reactive("wear")]
        public void SaveUniform()
        {
            InterfaceController.HideInterface("uniformsystem");
        }

        [Reactive("cancel")]
        public void Cancel()
        {
            InterfaceController.HideInterface("uniformsystem");
        }
    }
    public class UniformItem
    {
        public string make { get; set; }
        public string model { get; set; }

        public int upper { get; set; }
        public int lower { get; set; }
        public int shirt { get; set; }
        public int teeshirt { get; set; }
        public int shoes { get; set; }
        public int accessories1 { get; set; }
        public int accessories2 { get; set; }

    }
}
