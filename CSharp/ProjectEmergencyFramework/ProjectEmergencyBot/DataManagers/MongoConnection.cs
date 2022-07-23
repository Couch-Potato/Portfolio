using MongoDB.Driver;
using ProjectEmergencyFrameworkShared.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyBot.DataManagers
{
    public class MongoConnection
    {
        internal static MongoClient dbClient;
        internal static IMongoDatabase db;
        public static void StartMongoConnection()
        {
            dbClient = new MongoClient("mongodb://127.0.0.1:27017/");
            db = dbClient.GetDatabase("project_emergency");
            UserManager._users = db.GetCollection<User>("users");
            CharacterManager._characters = db.GetCollection<Character>("characters");
            CharacterManager._vehicles = db.GetCollection<Vehicle>("vehicles");
            CharacterManager._player = db.GetCollection<Player>("players");

            OrganizationManager._org = db.GetCollection<Organization>("organizations");
            EmailDataManager._mail = db.GetCollection<Email>("email");
            MessageBusService.BeginListen(db.GetCollection<MessageBus>("messagebus"));
        }
    }
}
