using Discord;
using Discord.Interactions;
using ProjectEmergencyBot.DataManagers;
using ProjectEmergencyFrameworkShared.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyBot.CommandHandlers
{
    [Group("dmv", "The Department of Motor Vehicles.")]
    public class DMV : InteractionModuleBase
    {
        public async Task<bool> IsAllowed()
        {
            var usx = UserManager.Lookup(Context.User.Id.ToString());
            if (usx == null)
            {
                await RespondAsync("Access denied. You need to be an administrator to run that command.", ephemeral: true);
                return false;
            }

            if (usx.UserType == 0)
            {
                return true;
            }
            await RespondAsync("Access denied.", ephemeral: true);
            return false;
        }
        [SlashCommand("lookup", "Looks for characters based on their first and last names.")]
        public async Task Lookup(string firstName, string lastName)
        {
            if (!await IsAllowed()) return;
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle("DMV Lookup Results");
            embed.WithDescription(ChrToString(CharacterManager.FindCharactersWithName(firstName, lastName)));
            await RespondAsync("", embed: embed.Build(), ephemeral: true);
        }

        [SlashCommand("person", "Gets person data based off their drivers license id, or their character id.")]
        public async Task LookupPerson(string id)
        {
            if (!await IsAllowed()) return;
            var chx = CharacterManager.FindCharacterWithId(id);
            if (chx == null)
            {
                await RespondAsync("Character not found.", ephemeral: true);
                return;
            }
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle($"{chx.LastName.ToUpper()}, {chx.FirstName.ToUpper()}")
                .AddField("Is Dead", chx.IsDead, true)
                .AddField("Is Wanted", chx.IsWanted ? chx.WantedReason : "false", true)
                .AddField("Is Incarcerated", chx.IsIncarcerated, true)
                .AddField("Drivers License", chx.DriversLicenseId)
                .AddField("Is Online", chx.IsOnline, true)
                .AddField("Id", chx.Id)
                .WithImageUrl(chx.DMVPhoto)
                .WithDescription("Lookup result");
            await RespondAsync("", embed: embed.Build(), ephemeral: true);
        }

        [SlashCommand("plate", "Looks for a vehicle based on the license plate.")]
        public async Task Plate(string plate)
        {
            if (!await IsAllowed()) return;
            var veh = CharacterManager.FindVehicleByPlate(plate);
            if (veh == null)
            {
                await RespondAsync("Character not found.", ephemeral: true);
                return;
            }
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle($"{veh.Make} {veh.Model}")
                .AddField("Owner: ", veh.RegisteredOwnerId)
                .AddField("Spawn Name:", veh.SpawnName)
                .AddField("Stolen:", veh.IsStolen)
                .AddField("Is Government:", veh.IsGovernmentInsured)
                .AddField("Trunk Container:", veh.Container);
                
            await RespondAsync("", embed: embed.Build(), ephemeral: true);
        }
        private string ChrToString(Character[] ch)
        {
            string str = "";
            foreach (var chx in ch)
            {
                str += $"**{chx.FirstName.ToUpper()} {chx.LastName.ToUpper()}**\nDrivers License: {chx.DriversLicenseId}\nCharacterId: {chx.Id}\nOwner: {chx.AttachedPlayerId}\n";
                str += "======================================";
            }
            return str.Trim('n').Trim('/').Trim();
        }
    }
}
