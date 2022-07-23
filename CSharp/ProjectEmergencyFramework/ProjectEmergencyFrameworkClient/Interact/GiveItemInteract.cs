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
    [Interactable("generic@giveItem", "Q", "GIVE ITEM", true, GenericInteractAttachment.Ped)]
    public class GiveItemInteract : LookatRadiusInteractable
    {
        public GiveItemInteract()
        {
            Radius = 3f;
            RequireControlKeyDown = true;
        }
        public override async Task<bool> CanShow()
        {
            if (InventoryService.GetEquippedItem() is null) return false;
            var item = InventoryService.GetEquippedItem();
            if (item == null) return false;
            if (item.modifiers == null) return false;
            if (CrappyWorkarounds.HasProperty(item.modifiers, "nonTransferrable"))
            {
                if (item.modifiers.nonTransferrable)
                {
                    return false;
                }
            }
            return await base.CanShow();
        }
        protected override void OnInteract()
        {
            var item = InventoryService.GetEquippedItem();
            RPC.RPCService.RemoteCall(NetworkGetNetworkIdFromEntity(Entity.Handle), "GIVE_ITEM", item);
            InventoryService.RemoveItem(item.name, item.icon,1,item.modifiers);
        }
    }
}
