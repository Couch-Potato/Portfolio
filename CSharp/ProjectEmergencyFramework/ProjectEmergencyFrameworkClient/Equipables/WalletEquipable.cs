using ProjectEmergencyFrameworkClient.Interact;
using ProjectEmergencyFrameworkClient.Services;
using ProjectEmergencyFrameworkClient.Utility;
using ProjectEmergencyFrameworkShared.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Equipables
{
    [Equipable("Wallet", "/assets/inventory/wallet.svg")]
    public class WalletEquipable:Equipable
    {
        public override bool DisabledPrimary => true;

        int getDenom(float cash, float amt)
        {
            return (int)((cash - (cash % amt)) / amt);
        }

        protected override void OnPrimaryUp()
        {
            InventoryItemCollection walletItems = new InventoryItemCollection();
            float cash = MoneyService.Cash;

            int numHundreds = getDenom(cash, 100f);
            cash -= numHundreds * 100;


            int numTwentys = getDenom(cash, 20f);
            cash -= numTwentys * 20;

            int numTens = getDenom(cash, 10f);
            cash -= numTens * 10;

            int numFives = getDenom(cash, 5f);
            cash -= numFives * 5;

            int numOnes = getDenom(cash, 1f);
            cash -= numOnes * 1;


            for (int i = 0; i < numHundreds; i++)
            {
                walletItems.AddItem(EquipmentService.ConstructEquipable("Money", "", new
                {
                    cashValue = 100f,
                    name = "$100"
                }));
            }

            for (int i = 0; i < numTwentys; i++)
            {
                walletItems.AddItem(EquipmentService.ConstructEquipable("Money", "", new
                {
                    cashValue = 20f,
                    name = "$20"
                }));
            }

            for (int i = 0; i < numTens; i++)
            {
                walletItems.AddItem(EquipmentService.ConstructEquipable("Money", "", new
                {
                    cashValue = 10f,
                    name = "$10"
                }));
            }

            for (int i = 0; i < numFives; i++)
            {
                walletItems.AddItem(EquipmentService.ConstructEquipable("Money", "", new
                {
                    cashValue = 5f,
                    name = "$5"
                }));
            }

            for (int i = 0; i < numOnes; i++)
            {
                walletItems.AddItem(EquipmentService.ConstructEquipable("Money", "", new
                {
                    cashValue = 1f,
                    name = "$1"
                }));
            }

         

            InventoryService.OpenCustomInventory(new Container()
            {
                MaxItems = walletItems.Count,
                Inventory = walletItems,
                Name = "WALLET",
                Type = "WALLET"
            }, (List<InventoryItem> items) =>
            {
                float newCashValue = 0f;
                DebugService.Watchpoint("WALLET_ITEMS", items);
                foreach (var cashItem in items)
                {
                    if (cashItem.modifiers == null) continue;
                    if (CrappyWorkarounds.HasProperty(cashItem.modifiers, "cashValue"))
                    {
                        newCashValue += cashItem.modifiers.cashValue;
                    }
                }
                MoneyService.Cash = newCashValue;
                if (newCashValue != cash)
                {
                    QueryService.QueryConcrete<bool>("LOWER_CASH", newCashValue);
                }
                throw new WorkaroundException();
            });

        }
    }
    [Equipable("Money", "/assets/inventory/money.svg")]
    public class MoneyEquipable : Equipable
    {
        public override bool DisabledPrimary => true;

        protected override void OnPrimaryUp()
        {
            QueryService.QueryConcrete<bool>("ADD_CASH", (float)Modifiers.cashValue);
            MoneyService.Cash += (float)Modifiers.cashValue;
            InventoryService.RemoveItem(this);
        }

        protected override void OnInstanced()
        {
            _name = Modifiers.name;
            Modifiers.desc = $"A nice crisp {_name} bill.";
            Modifiers.tags = "CASH";
            Modifiers.nonTransferrable = true;
        }
        IInteractable moneyGive;
        protected override void OnEquip()
        {
            moneyGive = InteractService.RegisterInteractAsGeneric("money@give", GenericInteractAttachment.Ped, new
            {
                cashValue = Modifiers.cashValue
            });
        }
        protected override void OnUnEquip()
        {
            InteractService.TerminateGeneric(moneyGive, GenericInteractAttachment.Ped);
        }
    }
}
