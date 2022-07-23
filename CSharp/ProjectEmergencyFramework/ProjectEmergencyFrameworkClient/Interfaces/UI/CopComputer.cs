using ProjectEmergencyFrameworkClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interfaces.UI
{
    [UserInterface("desktopCop")]
    public class CopComputer : UserInterface
    {
        [Configuration("fingerprints")]
        public List<KVPair> fingerpints { get; set; } = new List<KVPair>();
        [Configuration("mugshots")]
        public List<KVPair> mugshots { get; set; } = new List<KVPair>();
        [Configuration("evidence")]
        public List<KVPair> evidence { get; set; } = new List<KVPair>();

        [Configuration("userData")]
        public DesktopSession session { get; set; } = new DesktopSession();


        [Reactive("_hide")]
        public void exit2()
        {
            InterfaceController.HideInterface("desktopCop");
        }

        protected override Task ConfigureAsync()
        {
            var user = Services.OrganizationService.ConnectedOrganization.Members;
            string serviceNum = "???";
            foreach (var member in user)
            {
                if (member.CharacterId == Services.CharacterService.CurrentCharacter.Id)
                    serviceNum = member.ServiceNum;
            }
            session = new DesktopSession()
            {
                agency = OrganizationService.ConnectedOrganization.Abbrev,
                unitNum = serviceNum,
                name = $"{CharacterService.CurrentCharacter.FirstName.Substring(0, 1)}. {CharacterService.CurrentCharacter.LastName}",
                userId = CharacterService.CurrentCharacter.Id,
                type=OrganizationService.ConnectedOrganization.OrgType
            };

            foreach (var evidenceItem in InventoryService.GetInventoryItemsOfName("EVIDENCE RECEIPT"))
            {
                evidence.Add(new KVPair
                {
                    key = $"EVD-{evidenceItem.modifiers.id}",
                    value = $"{evidenceItem.modifiers.id},"
                });
            }

            foreach (var evidenceItem in InventoryService.GetInventoryItemsOfName("FINGERPRINT"))
            {
                fingerpints.Add(new KVPair
                {
                    key = $"FGP-{evidenceItem.modifiers.id}",
                    value = evidenceItem.modifiers.character
                });
            }
            foreach (var evidenceItem in InventoryService.GetInventoryItemsOfName("MUGSHOT"))
            {
                mugshots.Add(new KVPair
                {
                    key = $"MGS-{evidenceItem.modifiers.id}",
                    value = evidenceItem.modifiers.url
                });
            }
            DebugService.Watchpoint("EVIDENCE", new
            {
                mugshots=mugshots,
                evidence=evidence,
                fingerpints=fingerpints
            });
            return base.ConfigureAsync();
        }
    }
    public class KVPair
    {
        public string key { get;set; }
        public string value { get;set; }
    }
    public class DesktopSession
    {
        public string name { get; set; }
        public string unitNum { get; set; }
        public string agency { get; set; }
        public string userId { get; set; }

        public string type { get; set; }
    }

}
