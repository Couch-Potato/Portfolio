using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;

namespace ProjectEmergencyFrameworkClient.Interfaces.UI
{
    [UserInterface("carshop",true)]
    public class CarShop : UserInterface
    {
        private static Random random = new Random();
        private static string RandomStringAN(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        private CarShopConfig _cfg = new CarShopConfig();
        private string _selectedCarCat;
        private int _selectedPaint = 0;
        private int _selected = 0;

        private int VehicleHandle = 0;
        private Vector3 PlayerInitPosition;


        [Configuration("cars")]
        public Dictionary<string, List<CarShopItem>> Config { get => _cfg; }

        [Reactive("carcat")]
        public string SelectedCarCat { 
            get => _selectedCarCat; 
            set
            {
                _selectedCarCat = value;
                _selected = 0;
                SpawnCar((uint)GetHashKey(_cfg[_selectedCarCat][_selected].spawnName));
            } 
        }

        [Reactive("paint")]
        public string SelectedPaint
        {
            get => _selectedPaint.ToString(); set
            {
                _selectedPaint = int.Parse(value);
                SetVehicleColours(VehicleHandle, _selectedPaint, _selectedPaint);
            }
        }

        [Reactive("car")]
        public string SelectedCar
        {
            get => _selected.ToString(); set
            {
                _selected = int.Parse(value);
                SpawnCar((uint)GetHashKey(_cfg[_selectedCarCat][_selected].spawnName));
                // TODO
            }
        }

        private void SpawnCar(uint ModelHash)
        {
            if (VehicleHandle != 0)
            {
                SetEntityAsMissionEntity(VehicleHandle, true, true);
                DeleteVehicle(ref VehicleHandle);
            }

            RequestModel(ModelHash);
            VehicleHandle = CreateVehicle(ModelHash, 222.75f, -991.01f, -99.65f, -116f, true, false);
            SetVehicleNumberPlateText(VehicleHandle, "DEALER");
            SetVehicleOnGroundProperly(VehicleHandle);
            NetworkSetEntityInvisibleToNetwork(VehicleHandle, true);
            SetVehicleColours(VehicleHandle, _selectedPaint, _selectedPaint);
        }

        protected override async Task BeforeShow()
        {
            ClearAreaOfVehicles(236.63f, -991.17f, -96.36f, 20f, false, false, false, false, false);

            FreezeEntityPosition(GetPlayerPed(-1), false);
            DisableAllControlActions(0);
            DisplayRadar(false);
            

            DoScreenFadeOut(500);
            await BaseScript.Delay(500);
            await RoutingService.RouterSetPrivateBucket();


            SetEntityVisible(Game.PlayerPed.Handle, false, false);
            PlayerInitPosition = GetEntityCoords(GetPlayerPed(-1), true);
            SetEntityCoords(GetPlayerPed(-1), 236.63f, -991.17f, -96.36f, true, false, false, false);
            
            FreezeEntityPosition(GetPlayerPed(-1), true);
            SpawnCar((uint)GetHashKey(_cfg.Initial().spawnName));

            var cam1 = CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", 236.63f, -991.17f, -96.36f, -7.09f, 0.00f, 90.90f, 30.00f, false, 0);
            var cam2 = CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", 230.72f, -989.99f, -97.6f, -7.16f, 0.00f, 91.20f, 30.00f, false, 0);
            

            await BaseScript.Delay(1000);

            DoScreenFadeIn(500);

            SetCamActive(cam2, true);
            RenderScriptCams(true, false, 2000, true, true);

            SetCamActiveWithInterp(cam2, cam1, 5000, 1, 1);

           
            await base.BeforeShow();
        }

        private async void HandleReset()
        {
            DoScreenFadeOut(500);
            await BaseScript.Delay(500);
            RenderScriptCams(false, false, 0, true, true);
            FreezeEntityPosition(GetPlayerPed(-1), false);
            SetEntityVisible(Game.PlayerPed.Handle, true, true);
            SetEntityCoords(Game.PlayerPed.Handle, PlayerInitPosition.X, PlayerInitPosition.Y, PlayerInitPosition.Z, true, false, false, false);
            PlaceObjectOnGroundProperly(Game.PlayerPed.Handle);
            DoScreenFadeIn(500);
            EnableAllControlActions(0);
            DisplayRadar(true);
        }

        protected override async void Cleanup()
        {
            await RoutingService.RouterSetMainBucket();
            HandleReset();
            base.Cleanup();
        }
        bool wasPurchaseEffectuated = false;
        [Reactive("buy")]
        public async void HandleBuy()
        {
            if (wasPurchaseEffectuated) return;
            wasPurchaseEffectuated = true;
            if (!await Services.TransactionService.Pay(_cfg[_selectedCarCat][_selected].price, "VEHICLE_PURCHASE - " + _cfg[_selectedCarCat][_selected].make.ToUpper() + _cfg[_selectedCarCat][_selected].model.ToUpper()))
                return;
            /*RegisteredOwnerId = id, 
                LicensePlate = RandomStringAN(8),
                Make = randomCar.Make,
                Model = randomCar.Model,
                SpawnName = randomCar.SpawnName,
                ColorId = 0,
                IsGovernmentInsured = false,
                IsInsured = true,
                BelongsToOrganization = false*/
            await Services.VehicleService.AddVehicle(new ProjectEmergencyFrameworkShared.Data.Model.Vehicle()
            {
                SpawnName = _cfg[_selectedCarCat][_selected].spawnName,
                ColorId = _selectedPaint,
                IsInsured = false,
                Make = _cfg[_selectedCarCat][_selected].make,
                Model = _cfg[_selectedCarCat][_selected].model,
                RegisteredOwnerId = Services.CharacterService.CurrentCharacter.Id,
                LicensePlate = RandomStringAN(8),
                IsGovernmentInsured = true,
                BelongsToOrganization = false
            });
            InterfaceController.HideInterface("carshop");
           
           
        }

        [Reactive("cancel")] 
        public void HandleCancel()
        {
            InterfaceController.HideInterface("carshop");
        }

        [Reactive("_hide")]
        public void HandleCancel2()
        {
            InterfaceController.HideInterface("carshop");
        }
        protected override async Task ConfigureAsync()
        {
            _cfg = Properties.config;
            SelectedCarCat = _cfg.Initial().cat;

            await base.ConfigureAsync();
        }

    }
    public class CarShopConfig : Dictionary<string, List<CarShopItem>>
    {
        public void Add(CarShopItem item)
        {
            if (!ContainsKey(item.cat.ToUpper()))
            {
                Add(item.cat.ToUpper(), new List<CarShopItem>() {
                    item
                });
            } else
            {
                this[item.cat.ToUpper()].Add(item);
            }
        }
        public CarShopItem Initial()
        {
            foreach(var x in this)
            {
                return x.Value[0];
            }
            return null;
        }
    }
    public class CarShopItem
    {
        public string make { get; set; }
        public string model { get; set; }
        public string cat { get; set; }
        public int mpg { get; set; }
        public int speed { get; set; }
        public string spawnName { get; set; }
        public int price { get; set; }
        public int capacity { get; set; }
    }
}
