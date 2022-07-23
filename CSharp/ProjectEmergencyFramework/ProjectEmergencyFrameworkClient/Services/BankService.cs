using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectEmergencyFrameworkShared.Data.Model;

namespace ProjectEmergencyFrameworkClient.Services
{
    public static class BankService
    {
        public static async Task<BankAccount> GetBankAccountFromCard(string cardNum, string name)
        {
            return await Utility.QueryService.QueryConcrete<BankAccount>("CARD_RESOLVE", new ATMQueryObject()
            {
                name=name,
                number=cardNum
            });
        }
        public static async Task AttemptWelfare()
        {
            await Utility.QueryService.QueryConcrete<bool>("WELFARE", CharacterService.CurrentCharacter.Id);
            HUDService.ShowHelpText("$175 has been added to your bank balance because you are poor.", "green", 3f);
        }
        public static async Task<bool> AttemptWithdraw(string accountId, float amount)
        {
            bool rvx = await Utility.QueryService.QueryConcrete<bool>("WITHDRAW", new ATMTransferRequest()
            {
                accountId = accountId,
                amount = amount
            });
            //Debug.WriteLine(rvx);
            if (rvx)
            {
                MoneyService.Cash += amount;
            }
            return rvx;
        }
        public static async Task<bool> AttemptDeposit(string accountId, float amount)
        {
            if (amount > CharacterService.CurrentCharacter.CashOnHand)
            {
                return false;
            }
            bool rvx =  await Utility.QueryService.QueryConcrete<bool>("DEPOSIT", new ATMTransferRequest()
            {
                accountId = accountId,
                amount = amount
            });
            if (rvx)
            {
                MoneyService.Cash -= amount;
            }

            return rvx;
        }
    }
}
