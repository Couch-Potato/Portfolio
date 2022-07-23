using ProjectEmergencyFrameworkClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interfaces.UI
{
    [UserInterface("atm", true)]
    public class ATM : UserInterface
    {

        private List<ATMCard> _cards = new List<ATMCard>();

        private Dictionary<string, string> lookup = new Dictionary<string, string>();

        private string state = "";

        [Configuration("errorState")]
        public string errorState { get => state; set => state = value; }

        [Configuration("cards")]
        public List<ATMCard> Cards { get => _cards; }

        private string _account = "";
        private string _setMoney = "";

        [Reactive("account")]
        public string account { get => _account; set => _account = value; }

        [Reactive("money")]
        public string setMoney { get=>_setMoney; set=>_setMoney = value; }

        [Reactive("deposit")]
        public void deposit()
        {
            depositAsync();
        }

        private async void depositAsync()
        {
            DebugService.Watchpoint("ATMDEP", setMoney);
            var x = await Services.BankService.AttemptDeposit(lookup[account], float.Parse(setMoney));
            if (x)
            {
                errorState = "Success";
            }else
            {
                errorState = "Insufficient Funds.";
            }
            UpdateAsync();
        }

        [Reactive("withdraw")]
        public void withdraw()
        {
            withdrawAsync();
        }

        private async void withdrawAsync()
        {
            var x = await Services.BankService.AttemptWithdraw(lookup[account], float.Parse(setMoney));
            if (x)
            {
                errorState = "Success";
            }
            else
            {
                errorState = "Insufficient Funds.";
            }
            UpdateAsync();
        }

        [Reactive("exit")]
        public void exit()
        {
            InterfaceController.HideInterface("atm");
        }

        [Reactive("_hide")]
        public void exit2()
        {
            InterfaceController.HideInterface("atm");
        }

        protected override async Task ConfigureAsync()
        {
            // First we need to get cards.
            var cards = Services.InventoryService.GetInventoryItemsOfName("Debit Card");

            foreach (var card in cards)
            {
                ProjectEmergencyFrameworkShared.Data.Model.BankAccount dx = await Services.BankService.GetBankAccountFromCard(card.modifiers.number, card.modifiers.name);
                _cards.Add(new ATMCard()
                {
                    name = card.modifiers.name,
                    number = card.modifiers.number,
                    balance = dx.Balance
                });
                DebugService.Watchpoint("DBX", card.modifiers.number);
                lookup.Add(card.modifiers.number, dx.Id);
            }

            await base.ConfigureAsync();
        }
    }
    public class ATMCard
    {
        public string name { get; set; }
        public string number { get; set; }
        public float balance { get; set; }
    }
}
