using ProjectEmergencyFrameworkClient.Services;
using ProjectEmergencyFrameworkClient.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interact
{
    [Interactable("BARTER_INTERACT", "Z", "Barter")]
    public class BarterInteract:LookatRadiusInteractable
    {
        Random r = new Random();
        protected override void OnInteract()
        {
            foreach (var item in linkedRegion.PurchaseOrders)
            {
                if (InventoryService.IsItemEquipped(item.ItemName))
                {
                    // Get a price for the item
                    int price = (int)item.BasePrice + r.Next(0, (int)(item.PriceFlexibility));

                    QueryService.QueryConcrete<bool>("ADD_CASH", price);
                    MoneyService.Cash += (float)price;

                    InventoryService.RemoveItem(item.ItemName, InventoryService.GetEquippedItem().icon, 1);

                    break;
                }
            }



        }

        public BarterInteract()
        {
            Radius = 3f;
            Tolerance = 30f / 2;
            Offset = new CitizenFX.Core.Vector3(0, 0, .7f);
        }

        ProjectEmergencyFrameworkShared.Configuration.Schema.PedBarterRegion linkedRegion;

        public override async Task<bool> CanShow()
        {
            if (linkedRegion == null)
            {
                linkedRegion = Properties.regionData;
            }

            bool doesHaveItemOut = false;
            foreach (var item in linkedRegion.PurchaseOrders)
            {
                if (InventoryService.IsItemEquipped(item.ItemName))
                {
                    doesHaveItemOut = true;
                }
            }

            if (!doesHaveItemOut) return false;



            return await base.CanShow();
        }
    }
}
