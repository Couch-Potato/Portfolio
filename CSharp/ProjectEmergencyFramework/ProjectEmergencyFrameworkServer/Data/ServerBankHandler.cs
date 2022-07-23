using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkServer.Data
{
    public static class ServerBankHandler
    {
        [Queryable("GENERATE_BANK_ACCOUNT")]
        public static void MakeBank(Query q, object i, Player px)
        {
            var character = ServerCharacterHandlers.GetCharacterFromPlayer(px);
            var acct = BankService.CreateAccountForPlayer(character, ProjectEmergencyFrameworkShared.Data.Model.BankAccountProvider.MazeBank, null);
            var card = BankService.GenerateCardForAccount(character, acct);
            q.Reply(new ProjectEmergencyFrameworkShared.Data.Model.DebitCard()
            {
                HolderFirstName = character.FirstName,
                HolderLastName = character.LastName,
                CardId = card
            });
        }
        [Queryable("CARD_RESOLVE")]
        public static void CardResolve(Query q, object i, Player px)
        {
           
            var bT = (dynamic)i;
            q.Reply(BankService.GetBankAccountFromCard(bT.name, bT.number));
        }

        [Queryable("DEPOSIT")]
        public static void Deposit(Query q, object i, Player px)
        {
            var bT = (dynamic)i;

            // First lets get the character.
            var character = ServerCharacterHandlers.GetCharacterFromPlayer(px);

            if (character.CashOnHand < bT.amount)
            {
                q.Reply(false);
                return;
            }

            
            var bankData = BankService.GetBankAccountById(bT.accountId);
            character.CashOnHand -= bT.amount;
            BankService.Deposit(bankData,
                bT.amount,
                "FLEECA-ATM_LOCATION-UNK", // ADD PLD!!!!
                ProjectEmergencyFrameworkShared.Data.Model.BankAccountProvider.Fleeca
                );
            PlayerDataService.UpdateCharacter(character);
            q.Reply(true);
           // q.Reply(BankService.GetBankAccountFromCard(bT.name, bT.number));
        }

        [Queryable("WELFARE")]
        public static void Welfare(Query q, object i, Player px)
        {
            var character = ServerCharacterHandlers.GetCharacterFromPlayer(px);
            var bankData = BankService.GetAccountsForIndividual(character.Id);
            if (bankData.Count > 0)
            {
                BankService.Deposit(bankData[0], 175, "WELFARE - GOVERMENT", ProjectEmergencyFrameworkShared.Data.Model.BankAccountProvider.FederalReserve);
            }
        }

        [Queryable("WITHDRAW")]
        public static void Withdraw(Query q, object i, Player px)
        {
            var bT = (dynamic)i;

            var bankData = BankService.GetBankAccountById(bT.accountId);

            if (bankData.Balance < bT.amount)
            {
                q.Reply(false);
                return;
            }

            var character = ServerCharacterHandlers.GetCharacterFromPlayer(px);
            character.CashOnHand += bT.amount;
            BankService.Withdraw(bankData,
                bT.amount,
                "FLEECA-ATM_LOCATION-UNK", // ADD PLD!!!!
                ProjectEmergencyFrameworkShared.Data.Model.BankAccountProvider.Fleeca
                );
            PlayerDataService.UpdateCharacter(character);
            q.Reply(true);
            //q.Reply(BankService.GetBankAccountFromCard(bT.name, bT.number));
        }

        [Queryable("CARD_PAY")]
        public static void CardProcess(Query q, object i, Player px)
        {
            var bT = (dynamic)i;

            // If it is a card purchase

            if (bT.IsCardPurchase)
            {
                var bank = BankService.GetBankAccountFromCard($"{bT.FirstName} {bT.LastName}", bT.CardId);
                q.Reply(BankService.TransactCard(bank, bT.Amount, bT.Memo));
                return;
            }
            var character = PlayerDataService.GetCharacterFromId(bT.CharacterId);
            if (character.CashOnHand < bT.Amount)
            {
                q.Reply(false);
                return;
            }
            character.CashOnHand -= Math.Abs(bT.Amount);
            PlayerDataService.UpdateCharacter(character);
            q.Reply(true);
        }


    }
}
