using ProjectEmergencyFrameworkShared.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Services
{
    public static class TransactionService
    {
        public static async Task<bool> Pay(float amt, string details)
        {
            DebugService.DebugCall("TRANSACTION", "PEND_TRANSACTION");
            TransactionRequest transaction = new TransactionRequest()
            {
                Amount = amt,
                FirstName = CharacterService.CurrentCharacter.FirstName,
                LastName = CharacterService.CurrentCharacter.LastName,
                CharacterId = CharacterService.CurrentCharacter.Id,
                Memo = details,
                IsCardPurchase = false
            };
            /*{
                name = card.modifiers.name,
                    number = card.modifiers.number,
                    balance = dx.Balance
                });*/
            if (InventoryService.GetEquippedItem() != null)
            {
                if (InventoryService.GetEquippedItem().name == "Debit Card")
                {
                    transaction.IsCardPurchase = true;
                    transaction.CardId = InventoryService.CurrentlyEquipped.Modifiers.number;
                    transaction.FirstName = InventoryService.CurrentlyEquipped.Modifiers.name.Split(' ')[0];
                    transaction.LastName = InventoryService.CurrentlyEquipped.Modifiers.name.Split(' ')[1];
                }
            }
            

            bool success = await Utility.QueryService.QueryConcrete<bool>("CARD_PAY", transaction);

            if (success && !transaction.IsCardPurchase)
            {
                MoneyService.Cash -= Math.Abs(amt);
            }

            if (!success)
                HUDService.ShowHelpText("INSUFFICIENT FUNDS", "red", 2.5f);

            return success;
        }
    }
}
