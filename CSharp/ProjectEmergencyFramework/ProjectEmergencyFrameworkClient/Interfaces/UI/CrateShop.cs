using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interfaces.UI
{
    [UserInterface("crateShop", true)]
    public class CrateShop : UserInterface
    {
        [Configuration("items")]
        public List<CratePurchaseConfigurationItem> crates { get; set; } = new List<CratePurchaseConfigurationItem>()
        {
            new CratePurchaseConfigurationItem()
            {
                icons=new List<string>(){""},
                name="Test Crate",
                type="Craftable Crate",
                price=420.69f,
                crateIcon="",
                configId="0x00"
            }
        };

        [Reactive("crateId")]
        public int SelectedCrate { get; set; } = -1;

        [Reactive("Purchase")]
        public void PurchaseCrate()
        {
            if (SelectedCrate != -1)
            {
                Interact.InteractService.ConstructInteract("cratePickup", confg_location, new
                {
                    crateId = crates[SelectedCrate].configId
                });
            }
        }

        private Vector3 confg_location = new Vector3(-1, -1, -1);

        protected override async Task ConfigureAsync()
        {
            if (CrappyWorkarounds.HasProperty(Properties, "crates"))
            {
                crates = Properties.crates;
            }
            if (CrappyWorkarounds.HasProperty(Properties, "pickupLocation"))
            {
                confg_location = Properties.pickupLocation;
            }
        }

    }
    public class CratePurchaseConfigurationItem {
        public List<string> icons { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public float price { get; set; }
        public string crateIcon { get; set; }
        public string configId { get; set; }
    }
}
