using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
namespace ProjectEmergencyFrameworkClient.Interfaces.UI
{
    [UserInterface("birthcert", true)]
    public class BirthCertificate : UserInterface
    {
        [Reactive("sex")]
        public string Sex { get; set; } = "MALE";

        [Reactive("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [Reactive("lastName")]
        public string LastName { get; set; } = string.Empty;

        [Reactive("mm")]
        public string mm { get; set; } = string.Empty;
        [Reactive("dd")]
        public string dd { get; set; } = string.Empty;
        [Reactive("yy")]
        public string yy { get; set; } = string.Empty;

        [Reactive("finish")]
        public void Finish()
        {
            Hide();
            InterfaceController.ShowInterface("apartmentSelect", new
            {
                sex=Sex,
                firstName=FirstName,
                lastName=LastName,
                mm=mm,
                dd=dd,
                yy=yy
            });
        }
        public override void AfterShow()
        {
            DoScreenFadeIn(1000);
        }
        protected override async Task BeforeShow()
        {
            DoScreenFadeOut(1000);
            await BaseScript.Delay(1000);
        }
    }
}
