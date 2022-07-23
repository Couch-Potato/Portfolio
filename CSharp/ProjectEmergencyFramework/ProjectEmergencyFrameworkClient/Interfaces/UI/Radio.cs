using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interfaces.UI
{
    [UserInterface("radio")]
    public class Radio : UserInterface
    {
        [Configuration("radio")]
        public RadioConfig Config { get; set; } = new RadioConfig();

        protected override async Task ConfigureAsync()
        {
            Config.isPolice = Services.OrganizationService.ConnectedOrganization.OrgType == "POLICE";
            //return base.ConfigureAsync();
        }

        public void ShowRadio()
        {
            Config.isActive = true;
            Update();
        }
        public void HideRadio()
        {
            Config.isActive = false;
            Update();
        }
    }
    public class RadioConfig
    {
        public int page { get; set; } = 0;
        public int selected { get; set; } = 0;
        public bool isActive { get; set; } = false;
        public bool isPolice { get; set; } = false;
    }
}
