using MongoDB.Driver;
using ProjectEmergencyFrameworkShared.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyBot.DataManagers
{
    public static class CharacterManager
    {
        internal static IMongoCollection<Character> _characters;
        internal static IMongoCollection<Vehicle> _vehicles;
        internal static IMongoCollection<Player> _player;

        public static Vehicle FindVehicleByPlate(string plate)
        {
            return _vehicles.Find(e => e.LicensePlate == plate).FirstOrDefault();
        }
        public static Character[] FindCharactersWithName(string fn, string ln)
        {
            return _characters.Find(e => e.FirstName.ToUpper() == fn.ToUpper() && e.LastName.ToUpper() == ln.ToUpper()).ToList().ToArray();

        }
        public static Character FindCharacterWithId(string id)
        {
            return _characters.Find(e => e.Id == id || e.DriversLicenseId == id).FirstOrDefault();
        }
        public static Character[] FindCharactersBelongingToDiscord(string id)
        {
            var user = UserManager.Lookup(id);
            return _characters.Find(e=>e.AttachedPlayerId == user.LinkedPlayerId).ToList().ToArray();
        }
        public static Player GetPlayerFromCharacter(string id)
        {
            var chx = FindCharacterWithId(id);
            return _player.Find(p => p.Id == chx.AttachedPlayerId).FirstOrDefault();
        }
        public static Player GetPlayerByDiscord(string discord)
        {
            return _player.Find(p => p.Discord == discord).FirstOrDefault();
        }
    }
}
