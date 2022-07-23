using ProjectEmergencyFrameworkClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CitizenFX.Core.Native.API;
using System.Threading.Tasks;
using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Utility;

namespace ProjectEmergencyFrameworkClient.Interfaces.UI
{
    [UserInterface("vehicleselector", true)]
    public class VehicleSelector : UserInterface
    {
        public VehicleSelector()
        {
            setProperties = InterfaceController.InterfaceProperties;
        }
        private dynamic setProperties; 

        private List<VehicleItem> _vehicles = new List<VehicleItem>();

        private int _selected = 0;

        [Reactive("selectedVehicle")]
        public int SelectedVehicle { get => _selected; set => _selected = value; }

        [Configuration("vehicles")]
        public List<VehicleItem> Vehicles { get => _vehicles; set => _vehicles = value; }
        protected override async Task ConfigureAsync()
        {
            setProperties = InterfaceController.InterfaceProperties;
            if (CrappyWorkarounds.HasProperty(setProperties, "organizationLocked"))
            {
                if (setProperties.organizationLocked)
                {
                    var orgVeh = await Utility.QueryService.QueryList<VehicleItem>("GET_ORG_VEHICLES", setProperties.organization);
                    _vehicles = orgVeh;
                    return;
                }
            }
            
            var vhData = await Utility.QueryService.Query<List<object>>("GET_VEHICLES", CharacterService.CurrentCharacter.Id);
            foreach (var vh in vhData)
            {
                _vehicles.Add(Utility.CrappyWorkarounds.ShittyFiveMDynamicToConcrete<VehicleItem>(vh));
            }
        }
        bool debounce = false;
        [Reactive("spawn")]
        public void SetVehicle()
        {
            if (Vehicles.Count > 0 && !debounce)
            {
                debounce = true;
                VehicleItem sel = Vehicles[SelectedVehicle];
                VehicleService.SpawnVehicle(sel.Id, true);
            }
            InterfaceController.HideInterface("vehicleselector");
        }

        [Reactive("cancel")]
        public void Cancel()
        {

            InterfaceController.HideInterface("vehicleselector");
        }
    }
    public class VehicleItem
    {
        public string make { get; set; }
        public string model { get; set; }
        public string Id { get; set; }
        public string SpawnName { get; set; }
        public string LicensePlate { get; set; }
        public int ColorId { get; set; }
    }
}
