using ProjectEmergencyFrameworkClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Equipables
{
    [Equipable("Bodycam", "https://media.discordapp.net/attachments/930686885959962674/998652741276663808/unknown.png")]
    public class BodycamEquipable : Equipable
    {
        public override bool DisabledPrimary => true;
        protected override void OnInstanced()
        {
            if (OrganizationService.BodyCamUI == null)
            {
                OrganizationService.BodyCamUI = new Interfaces.UI.Bodycam();
                OrganizationService.BodyCamUI.Show();
            }
            base.OnInstanced();
        }
        public bool IsBodycamEnabled = true;
        protected override void OnDeInstanced()
        {
            if (OrganizationService.BodyCamUI == null)
            {
                OrganizationService.BodyCamUI = new Interfaces.UI.Bodycam();
                OrganizationService.BodyCamUI.Hide();
            }
            base.OnDeInstanced();
        }
        protected override void OnPrimaryUp()
        {
            if (IsBodycamEnabled)
            {
                OrganizationService.BodyCamUI.Hide();
                HUDService.ShowHelpText("Bodycam Disabled.", "none", 2.5f);
            }
            else
            {
                OrganizationService.BodyCamUI.Show();
                HUDService.ShowHelpText("Bodycam Disabled.", "none", 2.5f);

            }
            IsBodycamEnabled = !IsBodycamEnabled;
            OrganizationService.BodyCamUI.Enabled = IsBodycamEnabled;
            base.OnPrimaryUp();
        }
    }
}
