using Discord;
using Discord.Interactions;
using ProjectEmergencyBot.DataManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyBot.CommandHandlers
{
    [Group("user", "Manage users in the server.")]
    public class Users : InteractionModuleBase
    {
        public async Task<bool> IsAllowed()
        {
            var usx = UserManager.Lookup(Context.User.Id.ToString());
            if (usx == null)
            {
                await RespondAsync("Access denied.", ephemeral: true);
                return false;
            }

            if (usx.UserType == 0)
            {
                return true;
            }
            await RespondAsync("Access denied.", ephemeral: true);
            return false;
        }
        [SlashCommand("add", "Adds a user to the whitelist.")]
        public async Task Add(IUser User, string FirstName, string LastName)
        {
            if (!await IsAllowed()) return;
            EmbedBuilder embed = new EmbedBuilder();
            embed.Title = "User Added!";
            embed.Description = "Added a user to the whitelist.";
            embed.AddField("Name", $"{LastName.ToUpper()}, {FirstName.ToUpper()}");
            embed.AddField("Discord", User.Mention);
            var usrId = UserManager.AddWhitelist(FirstName, LastName, User.Id.ToString());

            EmbedBuilder whitelistNotify = new EmbedBuilder();
            whitelistNotify.Title = "Whitelist Notification";
            whitelistNotify.Description = "You have been added to the Project Emergency whitelist.";
            whitelistNotify.AddField("Whitelist Id", usrId);
            var guser = await Program.GetGuildUser(Program._client.GetGuild(Context.Guild.Id), User.Id);
            
            await User.SendMessageAsync("", embed: whitelistNotify.Build());
            embed.AddField("Whitelist Id", usrId);
            await RespondAsync("", embed:embed.Build(), ephemeral: true);
            await guser.ModifyAsync(x =>
            {
                x.Nickname = $"{FirstName.Substring(0, 1).ToUpper()}{FirstName.Substring(1).ToLower()} {LastName.Substring(0, 1).ToUpper()}{LastName.Substring(1).ToLower()}";
            });
        }
        [SlashCommand("clear", "Deletes a user from the whitelist.")]
        public async Task Clear(string userId)
        {
            if (!await IsAllowed()) return;

            EmbedBuilder embed = new EmbedBuilder();
            embed.Title = "User Removed.";
            embed.Description = "Removed a user from the whitelist.";
            var usr = UserManager.RemoveWhitelist(userId);
            embed.AddField("Name", $"{usr.LastName.ToUpper()}, {usr.FirstName.ToUpper()}");
            var user = await Context.Guild.GetUserAsync(ulong.Parse(usr.Discord));
            embed.AddField("Discord", user.Mention);
           

            EmbedBuilder whitelistNotify = new EmbedBuilder();
            whitelistNotify.Title = "Whitelist Notification";
            whitelistNotify.Description = "You have been removed from the Project Emergency whitelist.";

            await user.SendMessageAsync("", embed: whitelistNotify.Build());
            await RespondAsync("", embed: embed.Build(), ephemeral: true);
        }

        [UserCommand("Delist")]
        public async Task Clear4(IUser userx)
        {
            if (!await IsAllowed()) return;

            EmbedBuilder embed = new EmbedBuilder();
            embed.Title = "User Removed.";
            embed.Description = "Removed a user from the whitelist.";

            var userId = UserManager.Lookup(userx.Id.ToString()).Id;

            var usr = UserManager.RemoveWhitelist(userId);
            embed.AddField("Name", $"{usr.LastName.ToUpper()}, {usr.FirstName.ToUpper()}");
            var user = await Context.Guild.GetUserAsync(ulong.Parse(usr.Discord));
            embed.AddField("Discord", user.Mention);


            EmbedBuilder whitelistNotify = new EmbedBuilder();
            whitelistNotify.Title = "Whitelist Notification";
            whitelistNotify.Description = "You have been removed from the Project Emergency whitelist.";

            await user.SendMessageAsync("", embed: whitelistNotify.Build());
            await RespondAsync("", embed: embed.Build(), ephemeral: true);
        }
        [UserCommand("Lookup")]
        public async Task Lu4(IUser userX)
        {
            if (!await IsAllowed()) return;
            var components = new ComponentBuilder();

            var auser = UserManager.Lookup(userX.Id.ToString());
            EmbedBuilder embed = new EmbedBuilder();
            if (auser == null)
            {
                await RespondAsync("User not found.", ephemeral: true);
                return;
            }
            components.WithButton("Delist User", $"clear:{auser.Id}", ButtonStyle.Danger);
            embed.Title = $"{auser.LastName.ToUpper()}, {auser.FirstName.ToUpper()}";
            embed.WithDescription("Player details");
            embed.AddField("Whitelist Date", $"<t:{auser.WhitelistDate.ToString()}>").AddField("WhitelistId", auser.Id);
            embed.AddField("Is Banned", auser.IsBanned ? "Yes" : "No");
            embed.AddField("User Type", auser.UserType.ToString());
            if (auser.IsBanned)
            {
                embed.AddField("Ban Reason", auser.BanReason);
            }
            embed.AddField("Active Holds", auser.HasActiveHolds ? "Yes - expand for hold information." : "No");
            await RespondAsync("", embed: embed.Build(), ephemeral: true, components: components.Build());
            return;
        }


        [ComponentInteraction("clear:*", ignoreGroupNames:true)]
        public async Task Clear2(string userId)
        {
            if (!await IsAllowed()) return;

            EmbedBuilder embed = new EmbedBuilder();
            embed.Title = "User Removed.";
            embed.Description = "Removed a user from the whitelist.";
            var usr = UserManager.RemoveWhitelist(userId);
            embed.AddField("Name", $"{usr.LastName.ToUpper()}, {usr.FirstName.ToUpper()}");
            var user = await Context.Guild.GetUserAsync(ulong.Parse(usr.Discord));
            embed.AddField("Discord", user.Mention);


            EmbedBuilder whitelistNotify = new EmbedBuilder();
            whitelistNotify.Title = "Whitelist Notification";
            whitelistNotify.Description = "You have been removed from the Project Emergency whitelist.";

            await user.SendMessageAsync("", embed: whitelistNotify.Build());
            await RespondAsync("", embed: embed.Build(), ephemeral: true);
        }
        [SlashCommand("type", "Edits a user's type.")]
        public async Task SetUserType(string userId, UserType type)
        {
            if (!await IsAllowed()) return;

            UserManager.SetAccessLevel(userId, (int)type);
            
            await RespondAsync("Set access level.", ephemeral: true);
            return;
        }

        [Group("holds", "Manages a user's account holds.")]
        public class Holds : InteractionModuleBase
        {
            public async Task<bool> IsAllowed()
            {
                var usx = UserManager.Lookup(Context.User.Id.ToString());
                if (usx == null)
                {
                    await RespondAsync("Access denied.", ephemeral: true);
                    return false;
                }

                if (usx.UserType == 0)
                {
                    return true;
                }
                await RespondAsync("Access denied.", ephemeral: true);
                return false;
            }
            [SlashCommand("add", "Adds a hold to a user account.")]
            public async Task AddHold(string userId, string reason, /*[Summary(description:"The hours for the hold. Use '0' if you want to make the hold indefinite.")]*/int hours, HoldTypes hold)
            {
                if (!await IsAllowed()) return;

                var usx = UserManager.Lookup(Context.User.Id.ToString());
                var usr2 = UserManager.GetUser(userId);
                UserManager.AddHold(userId, reason, usx.Id, hours, (int)hold);

                EmbedBuilder whitelistNotify = new EmbedBuilder();
                whitelistNotify.Title = "Hold Notification.";
                whitelistNotify.Description = "A hold has been added to your account. Please contact a staff member if you need assistance.";
                whitelistNotify.AddField("Reason", reason);
                whitelistNotify.AddField("Type",
                    hold == HoldTypes.ServerJoin ? "__Server Join Prohibited__\nYou may not join the server until the hold expires or is removed." :
                    hold == HoldTypes.NoLawEnforcement ? "__No Law Enforcement__\nYou may not go onduty as any law enforcement officer until the hold expires or is removed." :
                    hold == HoldTypes.CharacterCreate ? "__No More Characters__\nYou may not create any more characters until the hold expires or is removed." :
                    hold == HoldTypes.NoFire ? "__No Law Enforcement__\nYou may not go onduty as a firefighter/EMS until the hold expires or is removed." :
                    hold == HoldTypes.NoJob ? "__No Job__\nYou may not go onduty for any job including public service jobs until the hold expires or is removed" :
                    ""
                    );
                whitelistNotify.AddField("Expiration", hours == 0 ? "Indefinite" : $"<t:{UserManager.Timestamp() + (60 * hours *60)}>");
                var guser = await Context.Guild.GetUserAsync(ulong.Parse(usr2.Discord));
               
                await guser.SendMessageAsync("", embed: whitelistNotify.Build());


                await RespondAsync("Hold added.", ephemeral: true);

            }
            [SlashCommand("remove", "Adds a hold from a user account.")]
            public async Task AddHold(string userId, /*[Summary(description:"The hold id. Use /user holds get to get a collection.", name:"Hold Id")]*/int id)
            {
                if (!await IsAllowed()) return;

                var usx = UserManager.Lookup(Context.User.Id.ToString());
                UserManager.RemoveHold(userId, id);
                await RespondAsync("Hold removed.", ephemeral: true);

            }
            [SlashCommand("get", "Gets a user's holds.")]
            public async Task AddHold(string userId)
            {
                if (!await IsAllowed()) return;

                var holds = UserManager.GetHolds(userId);

                string holdString = "";
                int holdId = 0;
                foreach (var hold in holds)
                {
                    
                    holdString += $"\nTYPE: **{hold.HoldType}**| ID: {holdId}\n ";
                    holdString += $"{hold.Reason}\n";
                    string holdTsx = $"<t:{hold.TimeStart + (hold.Hours * 60*60)}>";
                    holdString += $"**EXPIRES:** {(hold.IsIndef ? "INDEFINITE" : holdTsx)}\n";

                    var holdMaker = UserManager.GetUser(hold.Creator);

                    holdString += $"**ENTERED BY:** {holdMaker.LastName.ToUpper()}, {holdMaker.FirstName.ToUpper()}\n";
                    holdId++;
                }
                var user = UserManager.GetUser(userId);
                EmbedBuilder embed = new EmbedBuilder();
                embed.WithTitle($"Holds for: {user.LastName.ToUpper()}, {user.FirstName.ToUpper()}");
                embed.WithDescription(holdString);
                await RespondAsync("",embed:embed.Build(), ephemeral: true);

            }
            [UserCommand("Get Holds")]
            public async Task Lu4(IUser userX)
            {
                if (!await IsAllowed()) return;

                var userId = UserManager.Lookup(userX.Id.ToString()).Id;

                var holds = UserManager.GetHolds(userId);

                string holdString = "";
                int holdId = 0;
                foreach (var hold in holds)
                {

                    holdString += $"\nTYPE: **{hold.HoldType}**| ID: {holdId}\n ";
                    holdString += $"{hold.Reason}\n";
                    string holdTsx = $"<t:{hold.TimeStart + (hold.Hours * 60 * 60)}>";
                    holdString += $"**EXPIRES:** {(hold.IsIndef ? "INDEFINITE" : holdTsx)}\n";

                    var holdMaker = UserManager.GetUser(hold.Creator);

                    holdString += $"**ENTERED BY:** {holdMaker.LastName.ToUpper()}, {holdMaker.FirstName.ToUpper()}\n";
                    holdId++;
                }
                var user = UserManager.GetUser(userId);
                EmbedBuilder embed = new EmbedBuilder();
                embed.WithTitle($"Holds for: {user.LastName.ToUpper()}, {user.FirstName.ToUpper()}");
                embed.WithDescription(holdString);
                await RespondAsync("", embed: embed.Build(), ephemeral: true);
            }
        }


        public enum UserType
        {
            Administrator,
            Staff,
            Member
        }
        public enum HoldTypes
        {
            [ChoiceDisplay("Prohibt Server Join")]
            ServerJoin,
            [ChoiceDisplay("Prohibt Character Creation")]
            CharacterCreate,
            [ChoiceDisplay("Prohibt Law Enforcement Onduty")]
            NoLawEnforcement,
            [ChoiceDisplay("Prohibt Fire Onduty")]
            NoFire,
            [ChoiceDisplay("Prohibt Jobs Onduty")]

            NoJob
        }


        [SlashCommand("lookup", "Looks up a user.")]
        public async Task Lookup(string firstName = null, string lastName= null, [Summary(name:"user")]IUser userX = null)
        {
            if (!await IsAllowed()) return;

            var components = new ComponentBuilder();
            if (userX != null)
            {
              
                var auser = UserManager.Lookup(userX.Id.ToString());
                EmbedBuilder embed = new EmbedBuilder();
                if (auser == null)
                {
                    await RespondAsync("User not found.", ephemeral: true);
                    return;
                }
                components.WithButton("Delist User", $"clear:{auser.Id}", ButtonStyle.Danger);
                embed.Title = $"{auser.LastName.ToUpper()}, {auser.FirstName.ToUpper()}";
                embed.WithDescription("Player details");
                embed.AddField("Whitelist Date", $"<t:{auser.WhitelistDate.ToString()}>").AddField("WhitelistId", auser.Id);
                embed.AddField("Is Banned", auser.IsBanned ? "Yes" : "No");
                embed.AddField("User Type", auser.UserType.ToString());
                if (auser.IsBanned)
                {
                    embed.AddField("Ban Reason", auser.BanReason);
                }
                embed.AddField("Active Holds", auser.HasActiveHolds ? "Yes - expand for hold information." : "No");
                await RespondAsync("", embed: embed.Build(), ephemeral: true, components: components.Build());
                return;
            }
            if (firstName != null && lastName != null)
            {
                var auser = UserManager.Lookup(firstName, lastName);
                EmbedBuilder embed = new EmbedBuilder();

                if (auser == null)
                {
                    await RespondAsync("User not found.", ephemeral: true);
                    return;
                }

                components.WithButton("Delist User", $"clear:{auser.Id}", ButtonStyle.Danger);

                embed.Title = $"{auser.LastName.ToUpper()}, {auser.FirstName.ToUpper()}";
                embed.WithDescription("Player details");
                embed.AddField("Whitelist Date", $"<t:{auser.WhitelistDate.ToString()}>").AddField("WhitelistId", auser.Id);
                embed.AddField("Is Banned", auser.IsBanned ? "Yes" : "No");
                embed.AddField("User Type", auser.UserType.ToString());
                if (auser.IsBanned)
                {
                    embed.AddField("Ban Reason", auser.BanReason);
                }
                embed.AddField("Active Holds", auser.HasActiveHolds ? "Yes - expand for hold information." : "No");
                await RespondAsync("", embed: embed.Build(), ephemeral: true, components:components.Build());
                return;
            }
            else
            {
                await RespondAsync("You need to either specify a user, or a firstnabe and lastname.", ephemeral:true);
            }
        }
    }
}
