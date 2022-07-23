using ProjectEmergencyFrameworkClient.Services;
using ProjectEmergencyFrameworkClient.Utility;
using ProjectEmergencyFrameworkShared.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interact
{
    [Interactable("apartment@enter", "Z", "ENTER APARTMENT")]
    class ApartmentInteract : RadiusInteractable
    {
        public ApartmentInteract()
        {
            Radius = 8f;
        }
        public override async Task<bool> CanShow()
        {
            return await base.CanShow();
        }
        protected override void OnInteract()
        {
            Interfaces.InterfaceController.ShowInterface("intercom", Properties);
        }
    }
    [Interactable("apartment@exit", "Z", "EXIT APARTMENT")]
    class ExitApartmentInteract : RadiusInteractable
    {
        public ExitApartmentInteract()
        {
            Radius = 8f;
        }
        public override async Task<bool> CanShow()
        {
            return await base.CanShow();
        }
        protected override async void OnInteract()
        {
            Apartment apt = await QueryService.QueryConcrete<Apartment>("GET_APARTMENT", UniverseService.CurrentUniverseInstance.AttachedInstance);
            foreach (var apx in ConfigurationService.CurrentConfiguration.Apartments)
            {
                if (apt.ApartmentConfigName == apx.Id)
                {
                    UniverseService.TeleportBackToMain(ConfigurationService._loc_to_vector_3(apx.SpawnLocation));
                }
            }
            TaskService.InvokeOnce(() =>
            {
                InteractService.TerminateInteractable(this);
            }, "FORCE_APT_EXIT_UNBIND");
            //Interfaces.InterfaceController.ShowInterface("intercom", Properties);
        }
    }
}
