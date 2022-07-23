using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Services;
using ProjectEmergencyFrameworkClient.Utility;
using ProjectEmergencyFrameworkShared.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;

namespace ProjectEmergencyFrameworkClient.Interfaces.UI
{
    [UserInterface("characterselector", true)]
    public class CharacterSelector : UserInterface
    {
        public override void AfterShow()
        {
            Hook("playAs", (string plyx) =>
            {
                CharacterService.SetCharacterAndSpawn(plyx);
            });

            Hook("create", (string createx) =>
            {
                Hide();
                InterfaceController.ShowInterface("birthcert");
            });
        }

        protected override async Task ConfigureAsync()
        {
            var gc = await CharacterService.GetCharacters();

            CharacterList chList = new CharacterList();

            foreach (var chx in gc)
            {
                
                chList.Add(CrappyWorkarounds.ShittyFiveMDynamicToConcrete<Character>((dynamic) chx));
            }

            SendConfiguration("configCC", chList);
        }

        protected override async Task BeforeShow()
        {
            FreezeEntityPosition(GetPlayerPed(-1), false);
            DisableAllControlActions(0);
            DisplayRadar(false);
            ShutdownLoadingScreen();
            ShutdownLoadingScreenNui();
            DoScreenFadeOut(0);
            await RoutingService.RouterSetPrivateBucket();
            SetEntityCoords(Game.PlayerPed.Handle, 405.59f, -997.18f, -99.0f, false, false, false, true);
            FreezeEntityPosition(GetPlayerPed(-1), true);
            await BaseScript.Delay(2000);
            DestroyAllCams(true);
            int camera = 0;

            camera = CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", 402.99f, -998.02f, -99.00f, 0.00f, 0.00f, 0.00f, 50.00f, false, 0);
            SetCamActive(camera, true);
            RenderScriptCams(true, false, 2000, true, true);



            await BaseScript.Delay(500);
            



            await BaseScript.Delay(500);
            SetEntityVisible(Game.PlayerPed.Handle, false, false);
            var cam3 = CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", 402.99f, -998.02f, -99.00f, 0.00f, 0.00f, 0.00f, 50.00f, false, 0);
            PointCamAtCoord(cam3, 402.99f, -998.02f, -99.00f);
            SetCamActiveWithInterp(camera, cam3, 5000, 1, 1);
            await BaseScript.Delay(5000);
            DoScreenFadeIn(2000);
        
            await BaseScript.Delay(500);
           

        }

        protected override void Cleanup()
        {

        }
    }
}
