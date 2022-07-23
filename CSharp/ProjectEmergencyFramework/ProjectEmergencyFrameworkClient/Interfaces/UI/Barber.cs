using CitizenFX.Core;
using Newtonsoft.Json;
using ProjectEmergencyFrameworkClient.Services;
using ProjectEmergencyFrameworkClient.Services.Cams;
using ProjectEmergencyFrameworkClient.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
namespace ProjectEmergencyFrameworkClient.Interfaces.UI
{
    [UserInterface("barber", true)]
    public class Barber : UserInterface
    {
        CharacterSnapshot snapshot;
        CharacterSnapshot blank_slate;
        protected override Task BeforeShow()
        {
            CameraService.SetCamera<ClothingCam>();
            CameraService.GetCameraOperator<ClothingCam>().CameraType = ClothingCam.HEAD;
            return base.BeforeShow();
        }

        [Configuration("price")]
        public float price { get; set; } = 0f;

        [Reactive("buy")]
        public async void HandlePurchase()
        {
            // Handle purchase here.

            if (!await TransactionService.Pay(price, "BARBER - MAKEOVER"))
                return;

            blank_slate = snapshot;
            QueryService.QueryConcrete<bool>("UPDATE_PHYSICAL", snapshot.character.Physical);
        }

        [Reactive("ff")]
        public string ff { get => null; set
            {
                var pedData = JsonConvert.DeserializeObject<PedFaceFeatureData>(value);
                SetPedFaceFeature(Game.PlayerPed.Handle, pedData.id, float.Parse(pedData.scale) / 100f);
                snapshot.character.Physical.FaceFeaturesByIndex[pedData.id] = float.Parse(pedData.scale) / 100f;
                price += 1f;
                UpdateAsync();
            }
        }

        [Reactive("otf")]
        public string otf { get => null; set
            {
                var pedData = JsonConvert.DeserializeObject<PedFaceFeatureData>(value);
                var vx = int.Parse(pedData.scale);
                switch (pedData.id)
                {
                    case 0: // Age Style
                        snapshot.character.Physical.AgeingStyleId = vx;
                        SetPedHeadOverlay(Game.PlayerPed.Handle, 3, snapshot.character.Physical.AgeingStyleId, snapshot.character.Physical.AgeingStyleOpacity);
                        price += 20f;
                        UpdateAsync();
                        break;
                    case 1: // Age Opacity
                        snapshot.character.Physical.AgeingStyleOpacity = ((float)vx) / 100f;
                        SetPedHeadOverlay(Game.PlayerPed.Handle, 3, snapshot.character.Physical.AgeingStyleId, snapshot.character.Physical.AgeingStyleOpacity);
                        price += 1f;
                        UpdateAsync();
                        break;
                    case 2: // Sun Damage Style
                        snapshot.character.Physical.SunDamageStyleId = vx;
                        SetPedHeadOverlay(Game.PlayerPed.Handle, 7, snapshot.character.Physical.SunDamageStyleId, snapshot.character.Physical.SunDamageOpacity);
                        price += 20f;
                        UpdateAsync();
                        break;
                    case 3: // Sun Damage Opacity
                        snapshot.character.Physical.SunDamageOpacity = (float)vx / 100f;
                        SetPedHeadOverlay(Game.PlayerPed.Handle, 7, snapshot.character.Physical.SunDamageStyleId, snapshot.character.Physical.SunDamageOpacity);
                        price += 1f;
                        UpdateAsync();
                        break;
                    case 4: // Mole Style
                        snapshot.character.Physical.MoleStyleId = vx;

                        SetPedHeadOverlay(Game.PlayerPed.Handle, 9, snapshot.character.Physical.MoleStyleId, snapshot.character.Physical.MoleOpacity);
                        price += 20f;
                        UpdateAsync();
                        break;
                    case 5: // Mole Opacity
                        snapshot.character.Physical.MoleOpacity = (float)vx / 100f;

                        SetPedHeadOverlay(Game.PlayerPed.Handle, 9, snapshot.character.Physical.MoleStyleId, snapshot.character.Physical.MoleOpacity);
                        price += 1f;
                        UpdateAsync();
                        break;
                }
            } 
        }

        [Reactive("hair")]
        public string hair { get => null; set
            {
                var pedData = JsonConvert.DeserializeObject<PedFaceFeatureData>(value);
                var vx = int.Parse(pedData.scale);

                switch (pedData.id)
                {
                    case 0: // Hair Style
                        snapshot.character.Physical.HairId = vx;
                        SetPedComponentVariation(Game.PlayerPed.Handle, 2, vx, 0, 0);
                        price += 20f;
                        UpdateAsync();
                        break;
                    case 1: // Hair Color
                        snapshot.character.Physical.HairColorId = vx;
                        SetPedHairColor(Game.PlayerPed.Handle, vx, 0);
                        price += 20f;
                        UpdateAsync();
                        
                        break;
                }
            }
        }
        [Reactive("beard")]
        public string beard { get => null; set
            {
                var pedData = JsonConvert.DeserializeObject<PedFaceFeatureData>(value);
                var vx = int.Parse(pedData.scale);

                switch (pedData.id)
                {
                    case 0: // Beard Style
                        snapshot.character.Physical.BeardId = vx;
                        SetPedHeadOverlay(Game.PlayerPed.Handle, 1, snapshot.character.Physical.BeardId, snapshot.character.Physical.BeardOpacity);
                        SetPedHeadOverlayColor(Game.PlayerPed.Handle, 1, 1, snapshot.character.Physical.BeardColorId, snapshot.character.Physical.BeardColorId);
                        price += 20f;
                        UpdateAsync();
                        break;
                    case 1: // Beard Opacity
                        snapshot.character.Physical.BeardOpacity = (float)vx / 100f;
                        SetPedHeadOverlay(Game.PlayerPed.Handle, 1, snapshot.character.Physical.BeardId, snapshot.character.Physical.BeardOpacity);
                        SetPedHeadOverlayColor(Game.PlayerPed.Handle, 1, 1, snapshot.character.Physical.BeardColorId, snapshot.character.Physical.BeardColorId);
                        price += 1f;
                        UpdateAsync();
                        break;
                    case 2: // Beard Color
                        snapshot.character.Physical.BeardColorId = vx;
                        SetPedHeadOverlay(Game.PlayerPed.Handle, 1, snapshot.character.Physical.BeardId, snapshot.character.Physical.BeardOpacity);
                        SetPedHeadOverlayColor(Game.PlayerPed.Handle, 1, 1, snapshot.character.Physical.BeardColorId, snapshot.character.Physical.BeardColorId);
                        price += 20f;
                        UpdateAsync();
                        break;
                }
            } 
        }

        protected override Task ConfigureAsync()
        {
            snapshot = CharacterService.GetCharacterSnapshot();
            blank_slate = CharacterService.GetCharacterSnapshot();
            return base.ConfigureAsync();
        }
        protected override void Cleanup()
        {
            blank_slate.Restore();
            CameraService.Terminate();
            base.Cleanup();
        }
    }
}
