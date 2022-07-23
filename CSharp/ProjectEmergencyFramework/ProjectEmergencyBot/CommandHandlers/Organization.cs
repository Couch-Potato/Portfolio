using Discord.Interactions;
using ProjectEmergencyBot.DataManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyBot.CommandHandlers
{
    [Group("organization", "Organization commands")]
    public class OrganizationItem : InteractionModuleBase
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
        public async Task<bool> IsAllowed2()
        {
            if (CharacterItem.CharacterDictionary.ContainsKey(Context.User.Id))
            {

                return true;
            };
            await RespondAsync("You need to set a character first.", ephemeral: true);
            return false;
        }
        [SlashCommand("add", "Adds a character to an organization.")]
        public async Task Add(string id, string orgId)
        {
            if (!await IsAllowed()) return;

            OrganizationManager.AddCharacterToOrganization(orgId, id);
            await RespondAsync("Added to organization.", ephemeral: true);
        }
        [SlashCommand("remove", "Removes a character from an organization.")]
        public async Task Remove(string id, string orgId)
        {
            if (!await IsAllowed()) return;

            OrganizationManager.RemoveCharacterFromOrganization(orgId, id);
            await RespondAsync("Removed from organization.", ephemeral: true);
        }
        [SlashCommand("email", "Creates an organization email.")]
        public async Task emailc(string orgId)
        {
            if (!await IsAllowed2()) return;

            if (!OrganizationManager.IsCharacterMemberOf(CharacterItem.CharacterDictionary[Context.User.Id].Id, orgId))
            {
                await RespondAsync("You are not a member of this organization.", ephemeral: true);
                return;
            }

            var orgz = OrganizationManager.GetOrganization(orgId);

            if (!orgz.AllowEmails)
            {
                await RespondAsync("This organization does not allow emails.", ephemeral: true);
                return;
            }
            var newEmail = orgz.EmailTemplate;
            var chx = CharacterItem.CharacterDictionary[Context.User.Id];
            newEmail = newEmail.Replace("%fi%", chx.FirstName.Substring(0, 1).ToLower());
            newEmail = newEmail.Replace("%last%", chx.LastName.ToLower());




            EmailDataManager.CreateOrgMailbox(orgz.CallableId, CharacterItem.CharacterDictionary[Context.User.Id].Id, newEmail, $"{chx.LastName.ToUpper()}, {chx.FirstName.ToUpper()} [{orgz.Abbrev}]");

            await RespondAsync("Email created.", ephemeral: true);
        }
    }
}
