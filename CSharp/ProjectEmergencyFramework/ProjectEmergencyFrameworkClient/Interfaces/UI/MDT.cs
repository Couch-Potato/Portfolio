using ProjectEmergencyFrameworkShared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
namespace ProjectEmergencyFrameworkClient.Interfaces.UI
{
    [UserInterface("mdt", true)]
    public class MDT : UserInterface
    {
        public MDT() :base()
        {
            Services.DispatchService.CallAttachmentChanged += DispatchService_CallAttachmentChanged;
        }


        [Reactive("_hide")]
        public void exit2()
        {
            Hide();
        }


        private void DispatchService_CallAttachmentChanged(object sender, DispatchCallAttachment e)
        {
            if (e == null)
            {
                CurrentCall = new MDTCall();
                ForceUpdate();
                return;
            }
            var cpostal = Utility.PostalCodes.GetCoordsOfPostal(e.CallPostal.ToString());
            uint streetName = 0;
            uint crossingStreet = 0;
            GetStreetNameAtCoord(cpostal.X, cpostal.Y, 0, ref streetName, ref crossingStreet);
            CurrentCall = new MDTCall()
            {
                isCallAssigned = true,
                callNumber = e.CallNumber,
                source = e.CallSource,
                type = e.CallType,
                location = $"{e.CallPostal} {GetStreetNameFromHashKey(streetName)} {Utility.Zones.GetZoneFullName(GetNameOfZone(cpostal.X, cpostal.Y, 0))}",
                notes = e.CallNotes
            };
            ForceUpdate();
        }

        [Configuration("callData")]
        public MDTCall CurrentCall { get; set; } = new MDTCall(); 

        [Configuration("dl")]
        public List<MDTDriversLicenseEntry> LicensesOnHand { get; set; } = new List<MDTDriversLicenseEntry>();

        [Configuration("mdt_user")]
        public MDTUserConfiguration MDTUser { get; set; } = new MDTUserConfiguration();

        [Reactive("mdt_detach")]
        public void DetachFromCall()
        {
            Services.DispatchService.DetachFromCall();
        }

        [Reactive("toggle_status")]
        public async void ToggleStatus()
        {
            
            await Services.DispatchService.ToggleServiceStatus();
            UpdateAsync();
        }


        [Reactive("mdt_clear")]
        public void ClearCall()
        {
            Services.DispatchService.ClearEntireCall();
        }

        [Reactive("mdt_setGPS")]
        public void SetGPS()
        {
            Services.DispatchService.SetGPSWaypoint();
        }

        protected override async Task ConfigureAsync()
        {
            var user = Services.OrganizationService.ConnectedOrganization.Members;
            string serviceNum = "???";
            foreach (var member in user)
            {
                if (member.CharacterId == Services.CharacterService.CurrentCharacter.Id)
                    serviceNum = member.ServiceNum;
            }
            // PROLLY MAKE THIS BETTER LATER ON
            Services.DispatchService.Callsign = serviceNum;
            MDTUser = new MDTUserConfiguration()
            {
                agency = Services.OrganizationService.ConnectedOrganization.Abbrev,
                shortName = $"{Services.CharacterService.CurrentCharacter.FirstName.Substring(0, 1)}. {Services.CharacterService.CurrentCharacter.LastName}",
                callSign = serviceNum,
                serviceStatus = Services.DispatchService.IsAvailable ? Services.DispatchService.IsAttachedToCall ? "ATTACHED" : "IN SERVICE" : "OUT OF SERVICE",
                agencyType = Services.OrganizationService.ConnectedOrganization.OrgType
            };


            await base.ConfigureAsync();
        }

        public void ForceUpdate()
        {
            UpdateAsync();
        }
    }
    public class MDTUserConfiguration
    {
        public string shortName { get; set; }
        public string callSign { get; set; }
        public string serviceStatus { get; set; } = "OUT OF SERVICE";
        public string agency { get; set; }

        public string agencyType { get; set; }
    }
    public class MDTCall
    {
        public bool isCallAssigned { get; set; } = false;
        public string callNumber { get; set; }
        public string source { get; set; }
        public string type { get; set; }
        public string location { get; set; }
        public string notes { get; set; }
    }
    public class MDTDriversLicenseEntry
    {
        public string id { get; set; }
        public string dob { get; set; }
        public string exp { get; set; }
        public string fn { get; set; }
        public string ln { get; set; }
        public string num { get; set; }
        public string headshot { get; set; } = Utility.BaseIcons.MissingIcon;
    }
}
