using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interfaces.UI
{
    [UserInterface("gasStation", true)]
    public class Gas:UserInterface
    {
        private string _station = "ron"; // default
        [Configuration("theme")]
        public string station { get=>_station; } // globe, xero, ron, ltd


        private FuelingData fuelingData = new FuelingData() { gallons=0, percent=0, sale=0};
        [Configuration("fuelingData")] 
        public FuelingData FuelingData { get => fuelingData; }


        [Reactive("exit")]
        public void Exit()
        {
            InterfaceController.HideInterface("gasStation");
        }

        [Reactive("begin")]
        public void Begin()
        {
            Stage = 1;
        }

        private async void InvokeFuelBegin(int amt)
        {
            Stage = 2;

            var fuelAmt = VehicleService.GetVehicleCapacityDifference(Vehicle, amt);
            var gallons = VehicleService.LitersToGallons(fuelAmt);
            var price = gallons * 2.90f; // LETS GO BRANDON!

            var isValidTransaction = await TransactionService.Pay(price, "FUEL - " + _station.ToUpper());

            if (isValidTransaction)
            {
                Stage = 3;
                
            }else
            {
                Stage = 1;
                return;
            }

            VehicleService.FuelingStatusUpdated += (float amtS, float perc) =>
            {
                float gal = VehicleService.LitersToGallons(amtS);
                float totalOfSale = gal * 2.90f;
                fuelingData = new FuelingData()
                {
                    gallons=gal,
                    sale=totalOfSale,
                    percent=perc
                };
                Update();
            };
            await VehicleService.DoVehicleRefuel(Vehicle, amt);
            Stage = 4;
        }

        private int _f = -1;

        [Reactive("fuelPercent")]
        public int FuelPercent { get =>_f; set => InvokeFuelBegin(value); }

        private int _stage = 0;

        [Configuration("stage")]
        public int Stage { get=>_stage; set
            {
                _stage = value;
                UpdateAsync();
            }
        }
        Vehicle Vehicle;
        protected override Task ConfigureAsync()
        {
            _station = Properties.station;
            Vehicle = VehicleService.GetVehicleNearestPlayer();
            if (Vehicle == null)
            {
                HUDService.ShowHelpText("No vehicle nearby!", "red", 2.5f);
                InterfaceController.HideInterface("gasStation");
            }
                
            return base.ConfigureAsync();
        }
    }
    public class FuelingData
    {
        public float gallons { get; set; }
        public float percent { get; set; }
        public float sale { get; set; }
    }
}
