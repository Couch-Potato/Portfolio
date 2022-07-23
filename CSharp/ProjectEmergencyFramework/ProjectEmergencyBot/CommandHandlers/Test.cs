using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyBot.CommandHandlers
{
    public class Test : InteractionModuleBase
    {
        [SlashCommand("about", "Gets data about this bot.")]
        public async Task Echo()
        {
            
            await RespondAsync("Hello World!", ephemeral:true);
        }
        [SlashCommand("menu", "Select Menu demo command")]
        public async Task MenuInput()
        {
            var components = new ComponentBuilder();
            // A SelectMenuBuilder is created
            var select = new SelectMenuBuilder()
            {
                CustomId = "menu1",
                Placeholder = "Select something", MaxValues= 1, MinValues= 1 
            };
            // Options are added to the select menu. The option values can be generated on execution of the command. You can then use the value in the Handler for the select menu
            // to determine what to do next. An example would be including the ID of the user who made the selection in the value.
            select.AddOption("abc", "abc_value");
            select.AddOption("def", "def_value");
            select.AddOption("ghi", "ghi_value");

            components.WithSelectMenu(select);

            await RespondAsync("This message has a menu!", components: components.Build(), ephemeral:true);
        }
        [ComponentInteraction("menu1")]
        public async Task MenuHandler(string[] selections)
        {
            // For the sake of demonstration, we only want the first value selected.
            //await DeleteOriginalResponseAsync();
            await RespondAsync($"You selected {selections.First()}");
        }
        public enum Animal
        {
            Cat,
            Dog,
            // You can also use the ChoiceDisplay attribute to change how they appear in the choice menu.
            [ChoiceDisplay("Guinea pig")]
            GuineaPig
        }
        [SlashCommand("food", "Tell us about your favorite food.")]
        public async Task Command() => await Context.Interaction.RespondWithModalAsync<FoodModal>("food_menu");
        [SlashCommand("blep", "Send a random adorable animal photo")]
        public async Task Blep(Animal animal)
        {
            await RespondAsync("Set animal: " + animal.ToString());
        }
        public class FoodModal : IModal
        {
            public string Title => "Fav Food";
            // Strings with the ModalTextInput attribute will automatically become components.
            [InputLabel("What??")]
            [ModalTextInput("food_name", placeholder: "Pizza", maxLength: 20)]
            public string Food { get; set; }

            // Additional paremeters can be specified to further customize the input.
            [InputLabel("Why??")]
            [ModalTextInput("food_reason", TextInputStyle.Paragraph, "Kuz it's tasty", maxLength: 500)]
            public string Reason { get; set; }
        }
        [ModalInteraction("food_menu")]
        public async Task ModalResponse(FoodModal modal)
        {
            // Build the message to send.
            string message = "hey @everyone, I just learned " +
                $"{Context.User.Mention}'s favorite food is " +
                $"{modal.Food} because {modal.Reason}.";

            // Specify the AllowedMentions so we don't actually ping everyone.
            AllowedMentions mentions = new();
            mentions.AllowedTypes = AllowedMentionTypes.Users;

            // Respond to the modal.
            await RespondAsync(message, allowedMentions: mentions, ephemeral: true);
        }
    }
    
}
