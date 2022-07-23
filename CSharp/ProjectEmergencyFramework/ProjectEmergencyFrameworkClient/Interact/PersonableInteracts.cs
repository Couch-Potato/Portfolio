using ProjectEmergencyFrameworkClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
namespace ProjectEmergencyFrameworkClient.Interact
{
    [Interactable("person_ex_01@exchange01", "E", "GIVE PAPERS", true, GenericInteractAttachment.Ped)]
    public class PersonableInteracts : LookatRadiusInteractable
    {
        public PersonableInteracts()
        {
            Radius = 3f;
        }
        public override async Task<bool> CanShow()
        {
            if (!InventoryService.IsItemEquipped("CHARGING DOCUMENTS", "") && !InventoryService.IsItemEquipped("DEATH CERTIFICATE", "") && !InventoryService.IsItemEquipped("MEDICAL BILL", "")) return false;

            return await base.CanShow();
        }
        protected override void OnInteract()
        {
            DebugService.Watchpoint("GIVEPAPERS", InventoryService.GetEquippedItem());
            if (InventoryService.IsItemEquipped("CHARGING DOCUMENTS", ""))
            {
                RPC.RPCService.RemoteCall(NetworkGetNetworkIdFromEntity(Entity.Handle), "SHOW_PLEA" ,new {
                    arrestId = InventoryService.GetEquippedItem().modifiers.crimCase
                });
            }
            if (InventoryService.IsItemEquipped("MEDICAL BILL", ""))
            {
                RPC.RPCService.RemoteCall(NetworkGetNetworkIdFromEntity(Entity.Handle), "MEDICAL_HEAL", new
                {
                   healthRecord = InventoryService.GetEquippedItem().modifiers.record
                });
            }
            if (InventoryService.IsItemEquipped("DEATH CERTIFICATE", ""))
            {
                RPC.RPCService.RemoteCall(NetworkGetNetworkIdFromEntity(Entity.Handle), "HANDLE_DEATH", new
                {
                });
            }
        }
    }
}
