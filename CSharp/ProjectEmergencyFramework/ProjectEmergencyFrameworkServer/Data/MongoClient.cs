using MongoDB.Driver;
using ProjectEmergencyFrameworkShared.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkServer.Data
{
    public class DatabaseClient
    {
        internal static MongoClient dbClient;
        internal static IMongoDatabase db;

        public static void Connect()
        {
            dbClient = new MongoClient("mongodb://127.0.0.1:27017/");
            db = dbClient.GetDatabase("project_emergency");
            PlayerDataService._player = db.GetCollection<Player>("players");
            PlayerDataService._char = db.GetCollection<Character>("characters");
            PlayerDataService._vehicle = db.GetCollection<Vehicle>("vehicles");
            PlayerDataService._user = db.GetCollection<User>("users");
            OrganizationDataService._org = db.GetCollection<Organization>("organizations");
            OrganizationDataService._vehicles = db.GetCollection<Vehicle>("vehicles");
            OrganizationDataService._char = db.GetCollection<Character>("characters");
            BankService._accounts = db.GetCollection<BankAccount>("bankaccounts");
            BankService._char = db.GetCollection<Character>("characters");
            BankService._org = db.GetCollection<Organization>("organizations");
            BankService._debitCards = db.GetCollection<DebitCard>("debitcards");
            PlayerDataService._container = db.GetCollection<Container>("containers");
            PhoneDataService._phone = db.GetCollection<Phone>("phone");
            PhoneDataService._twatter = db.GetCollection<Twat>("twatter");
            EvidenceDataService._evidence = db.GetCollection<Evidence>("evidence");
            EvidenceDataService._guns = db.GetCollection<GunSerial>("guns");
            CriminalDataService._arrests = db.GetCollection<Arrests>("arrests");
            CriminalDataService._warrants = db.GetCollection<Warrant>("warrants");
            CriminalDataService._incarcer = db.GetCollection<IncarcerationRecord>("incarcerations");
            CriminalDataService._ncic = db.GetCollection<NCIC>("ncic");
            HealthDataService._deathCertificates = db.GetCollection<DeathCertificates>("deaths");
            HealthDataService._health = db.GetCollection<HealthRecord>("healthRecords");
            CriminalDataService._citation = db.GetCollection<CitationRecord>("citations");
            MessageBusService.BeginListen(db.GetCollection<MessageBus>("messagebus"));
            PersistenceService._persist = db.GetCollection<PersistentItem>("persistentitems");
        }
    }
}
