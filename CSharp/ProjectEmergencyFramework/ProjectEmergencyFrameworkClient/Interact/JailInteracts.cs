using ProjectEmergencyFrameworkClient.Services;
using ProjectEmergencyFrameworkClient.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
namespace ProjectEmergencyFrameworkClient.Interact
{
    [Interactable("jail@mugshot", "G", "MUGSHOT SUSPECT")]
    public class MugshotInteract : RadiusInteractable
    {
       
        public MugshotInteract()
        {
            this.Radius = 2f;
        }

        public override async Task<bool> CanShow()
        {
            return await base.CanShow() && Services.CharacterService.IsCharacterGrabbing;
        }

        protected override async void OnInteract()
        {
            var mugshot = await Services.HeadshotService.GetHeadshotOfPed(CharacterService.CharacterBeingGrabbed);
            InventoryService.AddItem("MUGSHOT", mugshot, 1, false, new
            {
                desc="MUGSHOT ID: MGS-" + DateTime.Now.ToString("MMddyyyy-HHmmss"),
                tags="MUGSHOT,EVIDENCE",
                id = DateTime.Now.ToString("MMddyyyy-HHmmss"),
                url = mugshot
            });
        }
    }

    [Interactable("jail@evidence", "G", "EVIDENCE LOCKER")]
    public class EvidenceInteract : RadiusInteractable
    {

        public EvidenceInteract()
        {
            this.Radius = 6f;
        }

        public override async Task<bool> CanShow()
        {
            return await base.CanShow() && OrganizationService.IsOnDuty;
        }

        protected override async void OnInteract()
        {
            dynamic dx = await QueryService.QueryConcrete<dynamic>("CREATE_EVIDENCE_LOCKER", new { 
                creator = CharacterService.CurrentCharacter.Id,
                agency = OrganizationService.ConnectedOrganization.CallableId
            });
            DebugService.Watchpoint("EVIDENCEOPEN", dx);
            dynamic dx3 = CrappyWorkarounds.JSONDynamicToExpando(dx);
            InventoryService.AddItem("EVIDENCE RECEIPT", "/assets/inventory/receipt_evidence.svg", 1, false, new
            {
                desc="EVIDENCE ID: EVD-" + dx3.Evidence,
                tags="EVIDENCE,EVIDENCE RECEIPT",
                id=dx3.Evidence
            });
            InventoryService.OpenNetworkedContainer(dx3.Container);
        }
    }

    [Interactable("jail@fingerprint", "G", "FINGERPRINT SUSPECT")]
    public class FingerprintInteract : RadiusInteractable
    {

        public FingerprintInteract()
        {
            this.Radius = 2f;
        }

        public override async Task<bool> CanShow()
        {
            return await base.CanShow() && Services.CharacterService.IsCharacterGrabbing;
        }

        protected override async void OnInteract()
        {
            string otherchar = await RPC.RPCService.RemoteQuery<string>(NetworkGetNetworkIdFromEntity(CharacterService.CharacterBeingGrabbed), "GET_ID", false);
            InventoryService.AddItem("FINGERPRINT", "/assets/inventory/receipt_fingerprint.svg", 1, false, new
            {
                desc = "FINGERPRINT ID: FGP-" + DateTime.Now.ToString("MMddyyyy-HHmmss"),
                tags = "EVIDENCE,FINGERPRINT",
                id = DateTime.Now.ToString("MMddyyyy-HHmmss"),
                character = otherchar
            });

        }
    }
}
