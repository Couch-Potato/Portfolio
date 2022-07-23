using CitizenFX.Core;
using ProjectEmergencyFrameworkServer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
namespace ProjectEmergencyFrameworkServer
{
    public class Whitelist : BaseScript
    {
        public Whitelist()
        {
            EventHandlers["playerConnecting"] += new Action<Player, string, dynamic, dynamic>(OnPlayerConnecting);
            EventHandlers["playerDropped"] += new Action<Player, string>(OnPlayerDropped);
        }
        private void OnPlayerDropped([FromSource] Player player, string reason)
        {
            var idx = PlayerDataService.FindUserById(player.Identifiers["discord"]);
            if (idx != null)
            {
                var dpd = PlayerDataService.GetPlayerFromDiscord(idx.Discord);
                dpd.IsOnline = false;
                dpd.NetworkId = null;
                if (dpd.CurrenctCharacter != null)
                {
                    var chx = PlayerDataService.GetCharacterFromId(dpd.CurrenctCharacter);
                    chx.IsOnline = false;
                    chx.NetworkId = null;
                    PlayerDataService.UpdateCharacter(chx);
                }
                if (ServerCharacterHandlers.CurrentCharacters.ContainsKey(player))
                {
                    ServerCharacterHandlers.CurrentCharacters.Remove(player);
                }
                dpd.CurrenctCharacter = null;
                PlayerDataService.UpdatePlayer(dpd);

                
            }
        }
        private async void OnPlayerConnecting([FromSource] Player player, string playerName, dynamic setKickReason, dynamic deferrals)
        {
            deferrals.defer();

            await Delay(0);

            deferrals.update("Verifying your membership status...");

            await Delay(1000);

            var idx = PlayerDataService.FindUserById(player.Identifiers["discord"]);
            if (idx != null)
            {
                if (idx.IsBanned)
                {
                    deferrals.done($"You are banned. {idx.BanReason}");
                    return;
                }
                string HoldInfo = "";
                bool hasHoldsEngaged = false;
                if (idx.HasActiveHolds)
                {
                    foreach (var hold in idx.Holds)
                    {
                        if (hold.HoldType == ProjectEmergencyFrameworkShared.Data.Model.HoldType.ServerJoin)
                        {
                            if (hold.IsIndef)
                            {
                                deferrals.done($"You have an active hold on your account which is preventing you from joining. Reason: {hold.Reason}");
                                return;
                            }else
                            {
                                var ts = BankService.Timestamp();
                                if (ts < hold.TimeStart + (60 * 60 * hold.Hours))
                                {
                                    uint remainSeconds = (uint)(hold.TimeStart + (60 * 60 * hold.Hours) - ts);
                                    uint mins = remainSeconds / 60;
                                    uint hours = (uint)Math.Floor(mins / 60f);
                                    uint minsDesplay = mins - (hours * 60);
                                    deferrals.done($"You have an active hold on your account which is preventing you from joining. Reason: {hold.Reason} | Time Remaining: {hours} hours, {minsDesplay} minutes.");
                                    return;
                                }
                            }

                        }else
                        {
                            if (hold.IsIndef)
                            {
                                HoldInfo += $"{hold.HoldType.ToString()}: {hold.Reason} | ";
                                hasHoldsEngaged = true;
                            }else
                            {
                                var ts = BankService.Timestamp();
                                if (ts < hold.TimeStart + (60 * 60 * hold.Hours))
                                {
                                    uint remainSeconds = (uint)(hold.TimeStart + (60 * 60 * hold.Hours) - ts);
                                    uint mins = remainSeconds / 60;
                                    uint hours = (uint)Math.Floor(mins / 60f);
                                    uint minsDesplay = mins - (hours * 60);
                                    HoldInfo += $"{hold.HoldType.ToString()}: {hold.Reason} Time Remaining: {hours} hours, {minsDesplay} minutes. | ";
                                    hasHoldsEngaged = true;

                                }
                            }
                        }

                    }
                }

                if (hasHoldsEngaged)
                {
                    deferrals.update($"Welcome, {idx.FirstName} {idx.LastName}. Inserting you in game. YOU HAVE ACTIVE HOLDS: {HoldInfo}");
                }else
                {
                    deferrals.update($"Welcome, {idx.FirstName} {idx.LastName}. Inserting you in game.");
                }
                if (idx.UserType == ProjectEmergencyFrameworkShared.Data.Model.UserType.Administrator)
                {
                    ExecuteCommand($"add_principal identifier.discord:{idx.Discord} vMenu.Staff");
                    ExecuteCommand($"add_principal identifier.discord:{idx.Discord} group.admin");
                    ExecuteCommand($"add_principal identifier.discord:{idx.Discord} group.moderator");
                }
                if (idx.UserType == ProjectEmergencyFrameworkShared.Data.Model.UserType.Staff)
                {
                    ExecuteCommand($"add_principal identifier.discord:{idx.Discord} vMenu.Staff");
                    ExecuteCommand($"add_principal identifier.discord:{idx.Discord} group.moderator");
                }
                await Delay(3000);
                deferrals.done();
            }
            else
            {
                deferrals.done("No whitelist record found. Contact a Project Emergency staff member if this is incorrect.");
                return;
            }
        }
    }
}
