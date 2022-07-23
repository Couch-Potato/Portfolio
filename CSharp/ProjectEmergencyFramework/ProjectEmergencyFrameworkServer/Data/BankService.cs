using MongoDB.Driver;
using ProjectEmergencyFrameworkShared.Data.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkServer.Data
{
    public static class BankService
    {
        internal static IMongoCollection<Organization> _org;
        internal static IMongoCollection<Character> _char;
        internal static IMongoCollection<BankAccount> _accounts;
        internal static IMongoCollection<DebitCard> _debitCards;
        internal static IMongoCollection<Debt> _debt;
        public static BankAccount CreateAccountForPlayer(Character player, BankAccountProvider provider, string customProvider)
        {
            var bankAcct = new BankAccount()
            {
                LocalFullName = $"{player.FirstName} {player.LastName}",
                DriversLicenseId = player.DriversLicenseId,
                DOB = player.DOB,
                AccountType = BankAccountType.Individual,
                Provider = provider,
                LinkedProvider = provider == BankAccountProvider.Custom ? customProvider : null,
                LinkedCharacter = player.Id,
                Balance = 0,
                IncomingTransactions = new List<BankTransaction>(),
                OutgoingTransactions = new List<BankTransaction>()
            };
            _accounts.InsertOne(bankAcct);
            return bankAcct;
        }

        public static Debt CreateDebt(string type, float debtTotal, int weeksToPayInFull, string character)
        {
            Debt d = new Debt()
            {
                DebtType = type,
                DebtTotal = debtTotal,
                DebtIssuanceDate = Timestamp(),
                DebtPaid = 0,
                AssociatedCharacter = character,
                NextDueAmount = debtTotal / weeksToPayInFull,
                NextDueDate = Timestamp() + (uint)(60*60*24*7*weeksToPayInFull),
                IsDebtDischarged = false
            };
            _debt.InsertOne(d);
            return d;
        }


        public static void CreateAccountForOrganization(Organization org, BankAccountProvider provider, string customProvider)
        {
            _accounts.InsertOne(new BankAccount()
            {
                LocalFullName = $"{org.OrganizationName}",
                AccountType = BankAccountType.Organization,
                Provider = provider,
                LinkedProvider = provider == BankAccountProvider.Custom ? customProvider : null,
                LinkedOrganiztion = org.Id,
                Balance = 0,
                IncomingTransactions = new List<BankTransaction>(),
                OutgoingTransactions = new List<BankTransaction>()
            });
        }
        // This is a pain in the butt and needs to be simplified. The data needs to be indexed in its own data collection. 
        public static string GenerateCardForAccount(Character character, BankAccount account)
        {
            string cardId = $"{ServerCharacterHandlers.RandomString(4)}-{ServerCharacterHandlers.RandomString(4)}-{ServerCharacterHandlers.RandomString(4)}-{ServerCharacterHandlers.RandomString(4)}";
            var debitCardData = new DebitCard()
            {
                HolderFirstName = character.FirstName,
                HolderLastName = character.LastName,
                CardId = cardId,
                LinkedAccount = account.Id
            };
            _debitCards.InsertOne(debitCardData);
            account.DebitCards = new List<DebitCard>();
            account.DebitCards.Add(debitCardData);
            UpdateAccount(account);
            return cardId;
        }
        public static BankAccount GetBankAccountFromCard(string name, string id)
        {
          /*  Debug.WriteLine(name);
            Debug.WriteLine(id);*/
            var card = _debitCards.Find(acct => acct.CardId == id).FirstOrDefault();
            return GetBankAccountById(card.LinkedAccount);
        }
        public static BankAccount GetBankAccountFromCardBlind(string id)
        {
            var card = _debitCards.Find(acct => acct.CardId == id).FirstOrDefault();
            return GetBankAccountById(card.LinkedAccount);
        }
        public static BankAccount GetBankAccountById(string id)
        {
            return _accounts.Find(acct => acct.Id == id).FirstOrDefault();
        }
        public static List<BankAccount> GetAccountsForOrganization(string orgId)
        {
            return _accounts.Find(acct => acct.AccountType != BankAccountType.Individual && acct.LinkedOrganiztion == orgId).ToList();
        }
        public static List<BankAccount> GetAccountsForIndividual(string indId)
        {
            return _accounts.Find(acct => acct.AccountType == BankAccountType.Individual && acct.LinkedCharacter == indId).ToList();
        }
        public static List<BankAccount> GetAccountsForIndividualFromInstitution(string driverId, BankAccountProvider provider)
        {
            return _accounts.Find(acct => acct.AccountType == BankAccountType.Individual && acct.DriversLicenseId == driverId && acct.Provider == provider).ToList();
        }
        public static List<BankAccount> GetAccountsForIndividualFromCustomInstitution(string driverId, string providerId)
        {
            return _accounts.Find(acct => acct.AccountType == BankAccountType.Individual && acct.DriversLicenseId == driverId && acct.Provider == BankAccountProvider.Custom && acct.LinkedProvider == providerId).ToList();
        }

        public static float Deposit(BankAccount account, float amount, string source, BankAccountProvider provider, string sourceBankId = null)
        {
            account.IncomingTransactions.Add(new BankTransaction()
            {
                TransactionMode = TransactionMode.Deposit,
                Memo = "DEPOSIT - " + source,
                TransferingAccount = provider == BankAccountProvider.Custom ? sourceBankId : null,
                RecievingAccount = account.Id,
                Amount = Math.Abs(amount),
                TransactionTime = Timestamp()
            });
            account.Balance += Math.Abs(amount);
            UpdateAccount(account);
            return account.Balance;
        }

        public static void UpdateAccount(BankAccount b)
        {
            _accounts.ReplaceOne(acct => b.Id == acct.Id, b);
        }
        public static void Transfer(BankAccount host, float amount, string recipientRouting)
        {
            var recipBA = GetBankAccountFromCardBlind(ExpandId(recipientRouting));
            recipBA.IncomingTransactions.Add(new BankTransaction()
            {
                TransactionMode = TransactionMode.WireTransfer,
                Memo="TRNSFR - FROM " + FlattenId(host.DebitCards[0].CardId),
                TransferingAccount = host.Id,
                RecievingAccount = recipBA.Id,
                Amount = Math.Abs(amount),
                TransactionTime = Timestamp()
            });
            recipBA.Balance += Math.Abs(amount);
            UpdateAccount(recipBA);

            host.OutgoingTransactions.Add(new BankTransaction()
            {
                TransactionMode = TransactionMode.WireTransfer,
                Memo = "TRNSFR - TO " + FlattenId(host.DebitCards[0].CardId),
                TransferingAccount = host.Id,
                RecievingAccount = recipBA.Id,
                Amount = Math.Abs(amount),
                TransactionTime = Timestamp()
            });
            host.Balance -= Math.Abs(amount);
            UpdateAccount(host);
        }
        public static uint Timestamp()
        {
            return (uint)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
        public static float Withdraw(BankAccount account, float amount, string source, BankAccountProvider provider, string sourceBankId = null)
        {
            account.OutgoingTransactions.Add(new BankTransaction()
            {
                TransactionMode = TransactionMode.Withdrawal,
                Memo = "WITHDRAW - " + source,
                RecievingAccount = provider == BankAccountProvider.Custom ? sourceBankId : null,
                TransferingAccount = account.Id,
                Amount = Math.Abs(amount),
                TransactionTime = Timestamp(),
            });
            account.Balance -= Math.Abs(amount);
            UpdateAccount(account);
            return account.Balance;
        }

        public static bool TransactCard(BankAccount account, float amount, string source)
        {
           
            amount = Math.Abs(amount);
            if (account.Balance < amount)
            {
                return false;
            } 
            account.OutgoingTransactions.Add(new BankTransaction()
            {
                TransactionMode= TransactionMode.CardPay,
                RecievingAccount = null,
                TransferingAccount = account.Id,
                Amount = amount,
                Memo = source,
                TransactionTime = Timestamp()
            });
            account.Balance -= amount;
            UpdateAccount(account);
            return true;
        }

        public static string FlattenId(string inx)
        {
            return inx.Replace("-", "");
        }
        public static string ExpandId(string inx)
        {
            return $"{inx.Substring(0,4)}-{inx.Substring(4, 4)}-{inx.Substring(8, 4)}-{inx.Substring(12, 4)}";
        }
    }
}
