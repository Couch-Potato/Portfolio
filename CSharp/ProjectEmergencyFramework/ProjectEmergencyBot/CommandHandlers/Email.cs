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
    public static class MailHolder
    {
        public static Dictionary<ulong, string> Mailboxes = new Dictionary<ulong, string>();
    }
    [Group("email", "Email various different clients.")]
    public class EmailIx : InteractionModuleBase
    {
        public async Task<bool> IsAllowed()
        {
            if (CharacterItem.CharacterDictionary.ContainsKey(Context.User.Id))
            {
                
                return true;
            };
            await RespondAsync("You need to set a character first.", ephemeral: true);
            return false;
        }
        [SlashCommand("create", "Create a personal email.")]
        public async Task Add(string prefix, EmailProvider suffix, string name)
        {
            if (!await IsAllowed()) return;
            EmailDataManager.CreateMailbox(CharacterItem.CharacterDictionary[Context.User.Id].Id, prefix, suffix, name);
            await RespondAsync("Email created.", ephemeral: true);
        }
        [SlashCommand("use", "Set the email that you wish to use.")]
        public async Task Use()
        {
            if (!await IsAllowed()) return;
            var components = new ComponentBuilder();
            var select = new SelectMenuBuilder()
            {
                CustomId = "mx2",
                Placeholder = "Set your Email",
                MaxValues = 1,
                MinValues = 1
            };
            foreach (var chx in EmailDataManager.GetEmailsForCharacter(CharacterItem.CharacterDictionary[Context.User.Id].Id))
            {
                select.AddOption($"{chx.Name} <{chx.EmailAddress}>", chx.Id, chx.ConnectedOrganization != null ? "FOR OFFICIAL USE ONLY" : "Personal Email");
            }
            components.WithSelectMenu(select);
            await RespondAsync("Select your Email.", components: components.Build(), ephemeral: true);
        }

        [SlashCommand("compose", "Create an email")]
        public async Task Compose()
        {
            if (!await IsAllowed()) return;
            if (!MailHolder.Mailboxes.ContainsKey(Context.User.Id))
            {
                await RespondAsync("You need to set an email first.",  ephemeral: true);
                return;
            }
            await Context.Interaction.RespondWithModalAsync<EmailModal>("compose_mail");
        }



        [ComponentInteraction("mx2", true)]
        public async Task MenuHandler(string[] selections)
        {
            string charId = selections.First();
            if (!MailHolder.Mailboxes.ContainsKey(Context.User.Id))
            {
                MailHolder.Mailboxes.Add(Context.User.Id, charId);
            }
            MailHolder.Mailboxes[Context.User.Id] = charId;
            await RespondAsync();
        }
        public enum EmailProvider
        {
            [ChoiceDisplay("@eyefind.info")]
            Eyefind,
            [ChoiceDisplay("@jol.com")]
            JOL,
            [ChoiceDisplay("@omail.com")]
            OMail,
            [ChoiceDisplay("@snotmail.com")]
            Snotmail
        }


        public class EmailModal : IModal
        {
            public string Title => "Compose Email";

            [InputLabel("To")]
            [ModalTextInput("to_id", placeholder: "hello@example.com", maxLength: 30)]
            public string To { get; set; }

            [InputLabel("Subject")]
            [ModalTextInput("mail_subject", placeholder: "Subject Line", maxLength: 30)]
            public string Subject { get; set; }

            // Additional paremeters can be specified to further customize the input.
            [InputLabel("Body")]
            [ModalTextInput("body", TextInputStyle.Paragraph, "Email body", maxLength: 500)]
            public string Body { get; set; }
        }

        [ModalInteraction("compose_mail", true)]
        public async Task ModalResponse(EmailModal modal)
        {
            var destMailbox = EmailDataManager.SendMail(modal.To, MailHolder.Mailboxes[Context.User.Id], modal.Subject, modal.Body);
            var destDiscord = CharacterManager.GetPlayerFromCharacter(destMailbox.ConnectedCharacter);
            var fromMailbox = EmailDataManager.GetMailboxFromId(MailHolder.Mailboxes[Context.User.Id]);
            var user = await Context.Guild.GetUserAsync(ulong.Parse(destDiscord.Discord));

            var embed = new EmbedBuilder();

            embed.WithTitle(modal.Subject)
                .WithFooter($"{fromMailbox.Name} <{fromMailbox.EmailAddress}>")
                .WithDescription($"**FROM:** {fromMailbox.Name} <{fromMailbox.EmailAddress}>\n**TO:** {destMailbox.Name} <{destMailbox.EmailAddress}>\n\n**__{modal.Subject}__**\n\n{modal.Body}")
                .WithAuthor(fromMailbox.Name)
                .WithCurrentTimestamp();
                

            await user.SendMessageAsync("", embed:embed.Build());

            await RespondAsync("Mail sent.", ephemeral: true);
        }

    }
}
