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
    [Group("character", "Character based commands.")]
    public class CharacterItem : InteractionModuleBase
    {
        public static event Action<IUser, Character> CharacterChanged;
        public static Dictionary<ulong, Character> CharacterDictionary = new Dictionary<ulong, Character>();

        [SlashCommand("set", "Set your character")]
        public async Task Set()
        {

            var plyr = CharacterManager.GetPlayerByDiscord(Context.User.Id.ToString());
            if (plyr.IsOnline)
            {
                await RespondAsync("You cannot select a different character when you are in server.", ephemeral: true);
                return;
            }

            var components = new ComponentBuilder();
            var select = new SelectMenuBuilder()
            {
                CustomId = "mx1",
                Placeholder = "Set your character",
                MaxValues = 1,
                MinValues = 1
            };
            foreach (var chx in CharacterManager.FindCharactersBelongingToDiscord(Context.User.Id.ToString()))
            {
                select.AddOption($"{chx.FirstName} {chx.LastName}", chx.Id, $"CharacterId: {chx.Id} | Drivers License: {chx.DriversLicenseId}");
            }
            components.WithSelectMenu(select);
            await RespondAsync("Select your character.", components: components.Build(), ephemeral: true);
        }
        [ComponentInteraction("mx1", true)]
        public async Task MenuHandler(string[] selections)
        {
            string charId = selections.First();
            var charx = CharacterManager.FindCharacterWithId(charId);
            CharacterChanged?.Invoke(Context.User, charx);
            if (!CharacterDictionary.ContainsKey(Context.User.Id))
            {
                CharacterDictionary.Add(Context.User.Id, charx);
            }
            CharacterDictionary[Context.User.Id] = charx;
            await RespondAsync();
        }
        public static void InvokeCC(IUser user, Character c)
        {
            CharacterChanged?.Invoke(user, c);
        }
    }
}
