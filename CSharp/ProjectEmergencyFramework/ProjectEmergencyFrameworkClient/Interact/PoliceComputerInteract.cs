using ProjectEmergencyFrameworkClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interact
{
    [Interactable("computer@police", "G", "USE COMPUTER (POLICE)", true)]
    public class PoliceComputerInteract : RadiusInteractable
    {
        public PoliceComputerInteract()
        {
            Radius = 2f;
        }
        public override async Task<bool> CanShow()
        {
            return await base.CanShow() && OrganizationService.IsOnDuty;
        }
        protected override void OnInteract()
        {
            Interfaces.InterfaceController.ShowInterface("desktopCop");
        }
    }
}
