using ProjectEmergencyFrameworkClient.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Services
{
    public static class MoneyService
    {
        private static bool IsMoneyShowing = false;
        private static Interfaces.UI.Money money;

        public static float Cash
        {
            get => CharacterService.CurrentCharacter.CashOnHand;
            set
            {
                /* 
                 * WHATEVER YOU DO, TRY TO RESIST THE URGE TO NETWORK THIS FUNCTION
                 * All cash operations need to be natural in the form of a transaction to prevent against exploitation. 
                 * Having a simple "setcash" handled is kinda a big vulnerability.
                 */
                CharacterService.CurrentCharacter.CashOnHand = value;
                money.CashAmt = value;
            }
        }

        [ExecuteAt(ExecutionStage.OnResourceStart)]
        public static void Start()
        {
            money = new Interfaces.UI.Money();
            CharacterService.CharacterChanged += CharacterService_CharacterChanged;
        }

        private static async void CharacterService_CharacterChanged(ProjectEmergencyFrameworkShared.Data.Model.Character newCharacter)
        {
            IsMoneyShowing = true;
            money.OverrideSetMoney(newCharacter.CashOnHand);
            await money.Show();
            money.ShowMoneyBriefly();
        }
    }
}
