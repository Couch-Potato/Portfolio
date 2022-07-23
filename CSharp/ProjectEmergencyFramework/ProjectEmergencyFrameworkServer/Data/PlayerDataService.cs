using MongoDB.Bson;
using MongoDB.Driver;
using ProjectEmergencyFrameworkServer.Utility;
using ProjectEmergencyFrameworkShared.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkServer.Data
{
    public static class PlayerDataService
    {
        internal static IMongoCollection<Player> _player;
        internal static IMongoCollection<Character> _char;
        internal static IMongoCollection<Vehicle> _vehicle;
        internal static IMongoCollection<Container> _container;
        internal static IMongoCollection<User> _user;
        public static List<InventoryItem> GetContainerInventory(string containerId)
        {
            var cnt = _container.Find(ch => ch.Id == containerId).FirstOrDefault();
            var inv = cnt.Inventory;

            if (inv == null)
            {
                inv = new List<InventoryItem>();
                for (int i = 1; i < cnt.MaxItems; i++)
                {
                    inv.Add(new InventoryItem());
                }
            }

            return inv;
        }
        public static void SetContainerInventory(string containerId, List<InventoryItem> items)
        {
            var cnt = _container.Find(ch => ch.Id == containerId).FirstOrDefault();
            cnt.Inventory = items;
            _container.ReplaceOne(ch => ch.Id == containerId, cnt);
        }
        public static Container GetContainerData(string containerId)
        {
            var cnt = _container.Find(ch => ch.Id == containerId).FirstOrDefault();
            cnt.Inventory = new List<InventoryItem>();
            return cnt;
        }
        public static Container CreateContainer(Container c)
        {
            var newC = c;
            _container.InsertOne(newC);
            return newC;
        }

        /// <summary>
        /// Gets a player from their discord id
        /// </summary>
        /// <param name="discordId">Their Discord</param>
        /// <returns>The Player</returns>
        public static Player GetPlayerFromDiscord(string discordId)
        {
            return _player.Find(plyr => plyr.Discord == discordId).FirstOrDefault();
        }
        /// <summary>
        /// Gets a player from their steamid
        /// </summary>
        /// <param name="steamId">Their Steam Id</param>
        /// <returns>The Player</returns>
        public static Player GetPlayerFromSteam(string steamId)
        {
            return _player.Find(plyr => plyr.Steam == steamId).FirstOrDefault();
        }

        public static void UpdatePlayer (Player p)
        {
            _player.ReplaceOne(px => p.Id == px.Id, p);
        }

        /// <summary>
        /// Gets a character and returns its id
        /// </summary>
        /// <param name="id">Character id</param>
        /// <returns>Character</returns>
        public static Character GetCharacterFromId(string id)
        {
            return _char.Find(chx => chx.Id == id).FirstOrDefault();
        }

        public static Character GetCharacterFromFields(string fn, string dlid, string ln)
        {

            return _char.Find(chx => (fn != "-" && fn.ToUpper() == chx.FirstName.ToUpper() && ln != "-" && chx.LastName.ToUpper() == ln.ToUpper() && dlid != "-" && chx.DriversLicenseId.ToUpper() == dlid.ToUpper()) || (fn != "-" && fn.ToUpper() == chx.FirstName.ToUpper() && ln != "-" && chx.LastName.ToUpper() == ln.ToUpper()) || (dlid != "-" && chx.DriversLicenseId.ToUpper() == dlid.ToUpper())).FirstOrDefault();
        }

        public static User FindUserById(string discord)
        {
            if (discord == null) return null;
            return _user.Find(x => x.Discord == discord).FirstOrDefault();

        }
        public static void UpdateUser(User p)
        {
            _user.ReplaceOne(px => p.Id == px.Id, p);
        }
        public static bool HasActiveHoldOfType(string discord, HoldType type)
        {
            var idx = FindUserById(discord);
            if (idx.HasActiveHolds)
            {
                foreach (var hold in idx.Holds)
                {
                    if (hold.HoldType == type)
                    {
                        if (hold.IsIndef)
                        {
                            return true;
                        }
                        else
                        {
                            var ts = BankService.Timestamp();
                            if (ts < hold.TimeStart + (60 * 60 * hold.Hours))
                            {
                                return true;
                            }
                        }

                    }

                }
            }
            return false;
        }

        /// <summary>
        /// Logs a player into the server.
        /// </summary>
        /// <param name="steam">Their steam id</param>
        /// <param name="discordId">Their discord id</param>
        /// <returns>The Player</returns>
        public static Player LoginPlayer(string steam, string discordId, string netid)
        {
            var plyr = _player.Find(_p => _p.Steam == steam && _p.Discord == discordId).FirstOrDefault();
            var userData = FindUserById(discordId);
            if (plyr != null)
            {
                Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                plyr.LastJoinDate = unixTimestamp;
                plyr.NetworkId = netid;
                plyr.IsOnline = true;
                if (userData.LinkedPlayerId != plyr.Id)
                {
                    userData.LinkedPlayerId = plyr.Id;
                    UpdateUser(userData);
                }
                _player.ReplaceOne(_p => _p.Steam == steam && _p.Discord == discordId, plyr);
                return plyr;
            }else
            {
                Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                var pyx = new Player();
                pyx.Discord = discordId;
                pyx.Steam = steam;
                pyx.NetworkId = netid;
                pyx.IsOnline = true;
                pyx.LastJoinDate = unixTimestamp;
                pyx.FirstJoinDate = unixTimestamp;
                
                _player.InsertOne(pyx);
                    userData.LinkedPlayerId = pyx.Id;
                    UpdateUser(userData);
                return pyx;
            }
        }
        public static Player[] GetAlikePlayers(Player player)
        {
            return _player.Find(_p => _p.Steam == player.Steam || _p.Discord == player.Discord).ToList().ToArray();
        }
        public static void BanAllPlayersAlike(Player player, string reason)
        {
            foreach (var plyr in GetAlikePlayers(player))
            {
                BanIndividualPlayer(plyr, reason);
            }
        }
        public static void BanIndividualPlayer(Player player, string reason)
        {
            player.IsBanned = true;
            player.BanReason = reason;
            _player.ReplaceOne(_p => _p.Steam == player.Steam && _p.Discord == player.Discord, player);
        }
        public static void SendEmail(string to, string from, string subject, string body)
        {
            MessageBusService.RPCCall(MessageHost.Bot, "EMAIL_SEND", new
            {
                to= to,
                from = from,
                subject = subject,
                body = body
            });
        }
        public static void UnbanIndividualPlayer(Player player)
        {
            player.IsBanned = false;
            player.BanReason = null;
            _player.ReplaceOne(_p => _p.Steam == player.Steam && _p.Discord == player.Discord, player);
        }
        public static void UnbanAlikePlayers(Player player)
        {
            foreach (var plyr in GetAlikePlayers(player))
            {
                UnbanIndividualPlayer(plyr);
            }
        }
        public static void Wipe(Player player)
        {
            _player.DeleteOne(_p => _p.Steam == player.Steam && _p.Discord == player.Discord);
            _char.DeleteMany(_ch => _ch.AttachedPlayerId == player.Id);
        }

        public static Character[] GetPlayerCharacters(Player plyr)
        {
            return _char.Find(chx => chx.AttachedPlayerId == plyr.Id).ToList().ToArray();
        }
        public static Character[] GetLivingPlayerCharacters(Player plyr)
        {
            return _char.Find(chx => chx.AttachedPlayerId == plyr.Id && !chx.IsDead).ToList().ToArray();
        }

        public static Character GetCharacter(string first, string last, string dlid)
        {
            return _char.Find(chx => chx.FirstName == first && chx.LastName == last && chx.DriversLicenseId == dlid).FirstOrDefault();
        }
        public static Character GetCharacter(string id)
        {
            return _char.Find(chx => chx.Id == id).FirstOrDefault();
        }

        public static float GetCharacterBankBalance(string id)
        {
            return _char.Find(chx => chx.Id == id).FirstOrDefault().CashOnHand;
        }

        public static float AddCashBalance(string id, float amount)
        {
            var character = GetCharacter(id);
            float currentBalance = character.CashOnHand;
            // Using abs to prevent funky operations when trying to manipulate signs.
            float newBalance = currentBalance + Math.Abs(amount);
            character.CashOnHand = newBalance;
            UpdateCharacter(character);
            return newBalance;
        }
        public static float RemoveCashBalance(string id, float amount)
        {
            var character = GetCharacter(id);
            float currentBalance = character.CashOnHand;
            // Using abs to prevent funky operations when trying to manipulate signs.
            float newBalance = currentBalance - Math.Abs(amount);
            character.CashOnHand = newBalance;
            UpdateCharacter(character);
            return newBalance;
        }


        public static Character FindCharacterBattery(dynamic d)
        {
            if (CrappyWorkarounds.HasProperty(d, "dlid")  && !string.IsNullOrWhiteSpace(d.dlid))
            {
                var dlid = (string)d.dlid;
                var chars = _char.Find(ch => ch.DriversLicenseId == dlid).FirstOrDefault();
                if (chars != null) return chars;
            }
           /* if (CrappyWorkarounds.HasProperty(d, "firstName") && !string.IsNullOrWhiteSpace(d.firstName) && CrappyWorkarounds.HasProperty(d, "lastName") && !string.IsNullOrWhiteSpace(d.lastName) && CrappyWorkarounds.HasProperty(d, "dob") && !string.IsNullOrWhiteSpace(d.dob))
            {
                var dob = (string)d.dob;
                if (dob.)
                var chars = _char.Find(ch => ch.DriversLicenseId == dlid).FirstOrDefault();
                if (chars != null) return chars;
            }*/
            return null;
        }
        public static void UpdateCharacter(Character c)
        {
            _char.ReplaceOne(chx => c.Id == chx.Id, c);
        }

        public static void UpdateVehicle(Vehicle v)
        {
            _vehicle.ReplaceOne(veh => v.Id == veh.Id, v);
        }

        public static void CreateCharacter(Character c)
        {
            _char.InsertOne(c);
        }

        public static void CreateVehicle(Vehicle v)
        {
            _vehicle.InsertOne(v);
        }

        public static Vehicle[] GetVehiclesForCharacter(string id)
        {
            return _vehicle.Find(veh => veh.RegisteredOwnerId == id).ToList().ToArray();
        }

        public static Vehicle GetVehicle(string vehId)
        {
            var ghost = new ObjectId();
            if (!ObjectId.TryParse(vehId, out ghost)) return null;
            return _vehicle.Find(veh => veh.Id == vehId).FirstOrDefault();
        }

        public static Vehicle GetVehicleByPlate(string plate)
        {
            return _vehicle.Find(veh => veh.LicensePlate == plate).FirstOrDefault();
        }

        public static List<InventoryItem> GetCharacterInventory(string id)
        {
            var inv = _char.Find(ch => ch.Id == id).FirstOrDefault().Inventory;

            if (inv == null)
            {
                inv = new List<InventoryItem>();
                for (int i=0;i<19;i++)
                {
                    inv.Add(new InventoryItem());
                }
            }

            return inv;
        }

        public static void SetCharacterInventory(string id, List<InventoryItem> items)
        {
            var cv = GetCharacterFromId(id);
            cv.Inventory = items;
            UpdateCharacter(cv);
        }
    }
}
