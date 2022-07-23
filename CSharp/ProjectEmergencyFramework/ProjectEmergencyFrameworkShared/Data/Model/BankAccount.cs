using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Data.Model
{
    public enum BankAccountProvider
    {
        [BsonRepresentation(BsonType.String)]
        PacificStandard,
        [BsonRepresentation(BsonType.String)]
        Fleeca,
        [BsonRepresentation(BsonType.String)]
        MazeBank,
        [BsonRepresentation(BsonType.String)]
        BlaineCountySavingsBank,
        [BsonRepresentation(BsonType.String)]
        BankOfLiberty,
        [BsonRepresentation(BsonType.String)]
        UnionDepository,
        [BsonRepresentation(BsonType.String)]
        Custom,
        [BsonRepresentation(BsonType.String)]
        FederalReserve
    }
    public enum BankAccountType
    {
        [BsonRepresentation(BsonType.String)]
        ReserveBankAccount,
        [BsonRepresentation(BsonType.String)]
        Organization,
        [BsonRepresentation(BsonType.String)]
        Governmental,
        [BsonRepresentation(BsonType.String)]
        Individual
    }
    public class BankAccount
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string DriversLicenseId { get; set; }
        public string LocalFullName { get; set; }
        public DateOfBirth DOB { get; set; }
        public BankAccountType AccountType {get;set;}
        public BankAccountProvider Provider { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string LinkedOrganiztion { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string LinkedCharacter { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string LinkedProvider { get; set; }
        public float Balance { get; set; }

        public List<BankTransaction> IncomingTransactions { get; set; }
        public List<BankTransaction> OutgoingTransactions { get; set; }

        public List<DebitCard> DebitCards { get; set; }
    }
    public enum TransactionMode
    {
        [BsonRepresentation(BsonType.String)]
        WireTransfer,
        [BsonRepresentation(BsonType.String)]
        Loan,
        [BsonRepresentation(BsonType.String)]
        BillPay,
        [BsonRepresentation(BsonType.String)]
        Deposit,
        [BsonRepresentation(BsonType.String)]
        Withdrawal,
        [BsonRepresentation(BsonType.String)]
        Ecommerce,
        [BsonRepresentation(BsonType.String)]
        CardPay,
        [BsonRepresentation(BsonType.String)]
        InitialGamePlayPayment
    }
    public class BankTransaction
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string TransferingAccount { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string RecievingAccount { get; set; }
        public UInt32 TransactionTime { get; set; }
        public string Memo { get; set; }

        public float Amount { get; set; }

        public TransactionMode TransactionMode { get; set; }
    }
    public enum CardType
    {
        Individual,
        Organization
    }

    public class DebitCard
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string HolderFirstName { get; set; }
        public string HolderLastName { get; set; }
        public string CardId { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string LinkedAccount { get; set; }
    }

    public class ATMQueryObject
    {
        public string name { get; set; }
        public string number { get; set; }
    }
    public class ATMTransferRequest
    {
        public string accountId { get; set; }
        public float amount { get; set; }
    }

    public class TransactionRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CardId { get; set; }
        public bool IsCardPurchase { get; set; }
        public float Amount { get; set; }
        public string CharacterId { get; set; }
        public string Memo { get; set; }
    }

    public class Debt
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string DebtType { get; set; }
        public string AssociatedCharacter { get; set; }
        public float DebtTotal { get; set; }
        public float DebtPaid { get; set; }
        public uint NextDueDate { get; set; }
        public uint DebtIssuanceDate { get; set; }
        public float NextDueAmount { get; set; }
        public bool IsDebtDischarged { get; set; }
    }
}
