using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Services;
using ProjectEmergencyFrameworkClient.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interact
{
    [Interactable("onduty", "E", "GO ON DUTY")]
    public class OnDutyInteract : RadiusInteractable
    {
        public OnDutyInteract()
        {
            Radius = 15f;
        }
        public override async Task<bool> CanShow()
        {
            return await base.CanShow();
        }
        protected override void OnInteract()
        {
            OrganizationService.GoOnDuty(Properties.organization, true);
        }
    }
}
