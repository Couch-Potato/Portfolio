using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
namespace ProjectEmergencyFrameworkClient.Interact
{
    [Interactable("ballistics@gsrTest", "G", "GSR Test", true)]
    public class BallisticGSRTestInteract : RadiusInteractable
    {
        protected override async void OnInteract()
        {
            bool hasGsr = await RPC.RPCService.RemoteQuery<bool>(NetworkGetPlayerIndexFromPed(Entity.Handle), "GetGSR", false);
            InventoryService.AddItem("GSR TEST RESULTS", "", 1, false, new
            {
                desc=$"GSR RESULT: {(hasGsr ? "PRESENT" : "NONE")}"
            });
            HUDService.ShowHelpText("GSR RESULT: " + (hasGsr ? "PRESENT" : "NONE"), "none", 2.5f);
        }
    }

    [Interactable("ballistics@shellCasing", "G", "Shell Casing", true)]
    public class BallisticShellCasing : RadiusInteractable
    {
        bool shouldIShow = true;
        public BallisticShellCasing()
        {
            Radius = 10f;

        }
        public override async Task<bool> CanShow()
        {
            bool canShow = await base.CanShow();
            if (canShow && shouldIShow)
            {
                DrawMarker(0, Position.X, Position.Y, Position.Z, 0, 0, 0, 0, 0, 0, 1f, 1f, 1, 255, 255, 0, 255, false, true, 2, false, null, null, false);

            }
            return canShow && shouldIShow;
        }
        protected override void OnInteract()
        {
            InventoryService.AddItem("BALLISTICS RESULTS", "/assets/inventory/receipt_bkr.svg", 1, false, new
            {
                desc = $"BULLET CASING WEAPON: {Properties.name}\n FIRING PATTERN:{Properties.firingPattern}" 
            });
            InteractService.TerminateInteractAtPosition("ballistics@shellCasing", Position);
            shouldIShow = false;
        }
    }
}
