using Discord;
using ProjectEmergencyBot.CommandHandlers;
using ProjectEmergencyBot.DataManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyBot.MessageBusHandlers
{
    public static class LockdownClient
    {
        [RPCHandler("CLIENT_OVERRIDE")]
        public static async void DoClientOverride(dynamic d)
        {
            string characterId = d.character;
            var plyr = CharacterManager.GetPlayerFromCharacter(characterId);
            var charx = CharacterManager.FindCharacterWithId(characterId);
            var user = await Program.GetGuildUser(Program._client.GetGuild(966751064336515142), ulong.Parse(plyr.Discord));
            CharacterItem.InvokeCC(user, charx);
            if (!CharacterItem.CharacterDictionary.ContainsKey(user.Id))
            {
                CharacterItem.CharacterDictionary.Add(user.Id, charx);
            }
            CharacterItem.CharacterDictionary[user.Id] = charx;
        }

        [RPCHandler("EMAIL_SEND")]
        public static async void DoEmail(dynamic d)
        {
            var destMailbox = EmailDataManager.SendMail(d.to, d.from, d.subject, d.body);
            var destDiscord = CharacterManager.GetPlayerFromCharacter(destMailbox.ConnectedCharacter);
            var fromMailbox = EmailDataManager.GetMailboxFromId(d.from);
            var user = await Program.GetGuildUser(Program._client.GetGuild(966751064336515142), ulong.Parse(destDiscord.Discord));

            var embed = new EmbedBuilder();

            embed.WithTitle(d.subject)
                .WithFooter($"{fromMailbox.Name} <{fromMailbox.EmailAddress}>")
                .WithDescription($"**FROM:** {fromMailbox.Name} <{fromMailbox.EmailAddress}>\n**TO:** {destMailbox.Name} <{destMailbox.EmailAddress}>\n\n**__{d.subject}__**\n\n{d.body}")
                .WithAuthor(fromMailbox.Name)
                .WithCurrentTimestamp();


            await user.SendMessageAsync("", embed: embed.Build());
        }
    }
}
