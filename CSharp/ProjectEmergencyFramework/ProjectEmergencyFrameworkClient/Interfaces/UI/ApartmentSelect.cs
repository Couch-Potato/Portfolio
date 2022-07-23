using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Services;
using ProjectEmergencyFrameworkClient.Services.Cams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
namespace ProjectEmergencyFrameworkClient.Interfaces.UI
{
    [UserInterface("apartmentSelect", true)]
    public class ApartmentSelect : UserInterface
    {
        [Configuration("config")]
        public List<ASItem> config { get; set; } = new List<ASItem>();
        protected override Task BeforeShow()
        {
            CameraService.SetCamera<ApartmentCam>(false, 0, new
            {
                position = Vector3.Zero,
                point = Vector3.Zero
            });
            return base.BeforeShow();
        }

        private int _id= 0;
        [Reactive("apt")]
        public int AptId
        {
            get => _id;
            set
            {
                _id = value;
                CameraService.GetCameraOperator<ApartmentCam>().Modifiers.position = ConfigurationService.CurrentConfiguration.Apartments[_id].CameraPreviewLocation;
                CameraService.GetCameraOperator<ApartmentCam>().Modifiers.point = ConfigurationService.CurrentConfiguration.Apartments[_id].SpawnLocation;

            }
        }
        protected override void Cleanup()
        {
            CameraService.Terminate();
            base.Cleanup();
        }
        [Reactive("buy")]
        public void Buy()
        {
            Hide();
            InterfaceController.ShowInterface("characterbuilder", new
            {
                sex = Properties.sex,
                firstName = Properties.firstName,
                lastName = Properties.lastName,
                mm = Properties.mm,
                dd = Properties.dd,
                yy = Properties.yy,
                aptId = _id
            });
        }
        protected override Task ConfigureAsync()
        {
            config.Clear();
            
            foreach (var apx in ConfigurationService.CurrentConfiguration.Apartments)
            {
                var vx = ConfigurationService._loc_to_vector_3(apx.ApartmentAccessLocation);
                uint streetName = 0;
                uint crossingStreet = 0;
                GetStreetNameAtCoord(vx.X, vx.Y, 0, ref streetName, ref crossingStreet);
                config.Add(new ASItem
                {
                    address = $"{Utility.PostalCodes.GetNearestPostalToCoords(vx)} {GetStreetNameFromHashKey(streetName)} {Utility.Zones.GetZoneFullName(GetNameOfZone(vx.X, vx.Y, 0))}, SA",
                    apartmentName = apx.Name
                });
            }
            CameraService.GetCameraOperator<ApartmentCam>().Modifiers.position = ConfigurationService.CurrentConfiguration.Apartments[0].CameraPreviewLocation;
            CameraService.GetCameraOperator<ApartmentCam>().Modifiers.point = ConfigurationService.CurrentConfiguration.Apartments[0].SpawnLocation;

            return base.ConfigureAsync();
        }
    }
    public class ASItem
    {
        public string apartmentName { get; set; }
        public string address { get; set; }
    }
}
