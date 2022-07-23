using ProjectEmergencyFrameworkClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interact
{
    [Interactable("uniformLocker", "G", "CHANGE UNIFORM", false)]
    internal class UniformLockerInteract : RadiusInteractable
    {
        public UniformLockerInteract()
        {
            Radius = 5f;
        }

        protected override void OnInteract()
        {
            Interfaces.InterfaceController.ShowInterface("locker", new
            {
                organization = OrganizationService.ConnectedOrganization
            });
        }
    }
}
