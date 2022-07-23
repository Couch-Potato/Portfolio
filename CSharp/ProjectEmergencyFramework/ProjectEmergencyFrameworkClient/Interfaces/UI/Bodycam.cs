using ProjectEmergencyFrameworkClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interfaces.UI
{
    [UserInterface("bodycam", false)]
    public class Bodycam : UserInterface
    {
        [Configuration("config")]
        public BodyCamConfig config { get; set; } = new BodyCamConfig();

        private PETask DateRunner;

        public bool Enabled { get; set; } = false;

        protected override Task ConfigureAsync()
        {
            var user = Services.OrganizationService.ConnectedOrganization.Members;
            string serviceNum = "???";
            foreach (var member in user)
            {
                if (member.CharacterId == Services.CharacterService.CurrentCharacter.Id)
                    serviceNum = member.ServiceNum;
            }
            if (DateRunner != null)
            {
                DateRunner = TaskService.InvokeUntilExpire(async () =>
                {
                    //Only worry about setting the date if the bodycam is enabled.
                    if (Enabled)
                    {
                        //19 JUN 2022 18 : 21 : 00 EDT
                        string time = DateTime.Now.ToString("dd MMM yyyy HH : mm : ss K");
                        config.timeStamp = time;
                        await UpdateAsync();
                    }
                    return false;
                }, "BODYCAM_DATE_SET");
            }
            config.name = $"{CharacterService.CurrentCharacter.FirstName[0]}. {CharacterService.CurrentCharacter.LastName} [{serviceNum}]";
            return base.ConfigureAsync();
        }

        protected override void Cleanup()
        {
            if (DateRunner != null) {
                TaskService.ForceExpireTask(DateRunner);
                DateRunner = null;
            }
            base.Cleanup();
        }

    }
    public class BodyCamConfig 
    {
        public string name { get; set; }
        public string timeStamp { get; set; }
    }
}
