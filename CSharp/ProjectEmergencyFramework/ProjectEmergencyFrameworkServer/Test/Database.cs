using ProjectEmergencyFrameworkShared.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkServer.Test
{
    public class Database
    {
        public static bool Connect()
        {
            Data.DatabaseClient.Connect();
            return true;
        }
        public static bool LoginPlayer()
        {
            var player = Data.PlayerDataService.LoginPlayer("-1","-1", "");
            return player.LastJoinDate == player.FirstJoinDate;
        }
        public static bool BanPlayer()
        {
            Data.PlayerDataService.BanAllPlayersAlike(Data.PlayerDataService.GetPlayerFromDiscord("-1"), "Test");
            return Data.PlayerDataService.GetPlayerFromDiscord("-1").IsBanned;
        }
        public static bool UnbanPlayer() {
            Data.PlayerDataService.UnbanAlikePlayers(Data.PlayerDataService.GetPlayerFromDiscord("-1"));
            return !Data.PlayerDataService.GetPlayerFromDiscord("-1").IsBanned;
        }
        public static bool CreateCharater()
        {
            var ch = new Character();
            ch.FirstName = "test";
            ch.AttachedPlayerId = Data.PlayerDataService.GetPlayerFromDiscord("-1").Id;
            Data.PlayerDataService.CreateCharacter(ch);

            return Data.PlayerDataService.GetLivingPlayerCharacters(Data.PlayerDataService.GetPlayerFromDiscord("-1")).Length > 0;
        }
        public static bool WipePlayer()
        {
            Data.PlayerDataService.Wipe(Data.PlayerDataService.GetPlayerFromDiscord("-1"));
            return true;
        }
    }
}
