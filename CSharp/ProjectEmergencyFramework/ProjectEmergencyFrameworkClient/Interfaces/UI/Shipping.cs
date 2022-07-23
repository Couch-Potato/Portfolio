using ProjectEmergencyFrameworkClient.Services;
using ProjectEmergencyFrameworkClient.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interfaces.UI
{
    [UserInterface("shipping", true)]
    public class Shipping : UserInterface
    {
        [Configuration("shop")]
        public ShippingConfig config { get; set; } = new ShippingConfig();
        [Reactive("category")]
        public int category { get; set; } = 0;

        [Reactive("purchase")]
        public int purchase { get => 0; set
            {
                HandlePurchase(value);
            } 
        }

        private async void HandlePurchase(int id)
        {
            ShippingItem item = null; 
            switch (category)
            {
                case 0:
                    item = config.precursors[id];
                    break;
                case 1:
                    item = config.craftingModules[id];
                    break;
                case 2:
                    item = config.sellables[id];
                    break;
            }

            if (item == null)
                return;

            if (category == 2)
            {
                if (!InventoryService.HasItem(item.name, item.icon, 1))
                {
                    HUDService.ShowHelpText("You need to have this item in order to sell it.", "red", 2.5f);
                    return;
                }
                InventoryService.RemoveItem(item.name, item.icon, 1);
                QueryService.QueryConcrete<bool>("ADD_CASH", (float)item.price);
                MoneyService.Cash += (float)item.price;
            }
            if (!await Services.TransactionService.Pay(item.price, "SHIPPING DEPOT - " + item.name))
                return;

            if (category == 0)
            {
                InventoryService.AddItem(item.name, item.icon);
                return;
            }
            InventoryService.AddItem(EquipmentService.ConstructEquipable(item.name, item.icon, item.addtlProps));

        }

        protected override Task ConfigureAsync()
        {
            config = Properties.config;
            return base.ConfigureAsync();
        }
    }
    public class ShippingConfig
    {
        public List<ShippingItem> precursors { get; set; } = new List<ShippingItem>();
        public List<ShippingItem> craftingModules { get; set; } = new List<ShippingItem>();
        public List<ShippingItem> sellables { get; set; } = new List<ShippingItem>();
    }
    public class ShippingItem
    {
        public string name { get; set; }
        public string desc { get; set; }
        public string icon { get; set; }
        public float price { get; set; }
        public dynamic addtlProps { get; set; } = new { };
    }
}
