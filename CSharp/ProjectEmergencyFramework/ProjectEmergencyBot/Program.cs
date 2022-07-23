using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ProjectEmergencyBot.DataManagers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace ProjectEmergencyBot
{
    public class DiscordConfig
    {
        public enum DiscordTypes
        {
            Public,
            StaffOnly,
            OrganizationLocked
        }
        public ulong DiscordId { get; set; }
        public string LinkedOrganization { get; set; }
        public bool IsInCharacter { get; set; }
        public DiscordTypes DiscordType { get; set; }
    }
	public class Program
	{
		public static Task Main(string[] args) => new Program().MainAsync();
        public static Dictionary<ulong, DiscordConfig> Discords = new Dictionary<ulong, DiscordConfig>();
		internal static DiscordSocketClient _client;
		private InteractionService _interactionService;
        private IServiceProvider _services;
		private IServiceProvider ConfigureServices()
		{
			// this returns a ServiceProvider that is used later to call for those services
			// we can add types we have access to here, hence adding the new using statement:
			// using csharpi.Services;
			return new ServiceCollection()
				.AddSingleton<DiscordSocketClient>()
				.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
				.BuildServiceProvider();
		}
		public async Task MainAsync()
		{
            Discords.Add(966750783104229448, new DiscordConfig()
            {
                LinkedOrganization ="BASE::SASP",
                IsInCharacter = true,
                DiscordId = 966750783104229448,
                DiscordType = DiscordConfig.DiscordTypes.OrganizationLocked
            });
            Discords.Add(966751064336515142, new DiscordConfig()
            {
                IsInCharacter = false,
                DiscordId = 966751064336515142,
                DiscordType = DiscordConfig.DiscordTypes.StaffOnly
            });

            CommandHandlers.CharacterItem.CharacterChanged += CharacterItem_CharacterChanged;


            _client = new DiscordSocketClient();
            DataManagers.MongoConnection.StartMongoConnection();
			_client.Log += Log;
			_client.Ready += Client_Ready;

            _client.UserJoined += _client_UserJoined;


            _interactionService = new InteractionService(_client.Rest);
            _services = ConfigureServices();
            _interactionService.SlashCommandExecuted += SlashCommandExecuted;
            _interactionService.ContextCommandExecuted += ContextCommandExecuted;
            _interactionService.ComponentCommandExecuted += ComponentCommandExecuted;
            _client.InteractionCreated += HandleInteraction;
            await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
			//  You can assign your bot token to a string, and pass that in to connect.
			//  This is, however, insecure, particularly if you plan to have your code hosted in a public repository.
			var token = "";

			// Some alternative options would be to keep your token in an Environment Variable or a standalone file.
			// var token = Environment.GetEnvironmentVariable("NameOfYourEnvironmentVariable");
			// var token = File.ReadAllText("token.txt");
			// var token = JsonConvert.DeserializeObject<AConfigurationClass>(File.ReadAllText("config.json")).Token;

			await _client.LoginAsync(TokenType.Bot, token);
			await _client.StartAsync();

			// Block this task until the program is closed.
			await Task.Delay(-1);
		}

        private async void CharacterItem_CharacterChanged(IUser arg1, ProjectEmergencyFrameworkShared.Data.Model.Character arg2)
        {
            foreach (var kvp in Discords)
            {
                var guild = _client.GetGuild(kvp.Key);
                if (kvp.Value.IsInCharacter)
                {
                    if (OrganizationManager.IsCharacterMemberOf(arg2.Id, kvp.Value.LinkedOrganization))
                    {
                        var role = await GetGuildRole(guild, "OrgMember");
                        var usr = await GetGuildUser(guild, arg1.Id);
                        var sn = OrganizationManager.GetCharacterServiceNumber(arg2.Id, kvp.Value.LinkedOrganization);
                        await usr.ModifyAsync(n =>
                        {
                            n.Nickname = $"{arg2.FirstName.Substring(0, 1).ToUpper()}. {arg2.LastName.Substring(0, 1).ToUpper()}{arg2.LastName.Substring(1).ToLower()} [{sn}]";
                        });
                        await usr.AddRoleAsync(role);
                    }else
                    {
                       
                        var role = await GetGuildRole(guild, "OrgMember");
                        var usr = await GetGuildUser(guild, arg1.Id);
                        await usr.ModifyAsync(n =>
                        {
                            n.Nickname = usr.Username;
                        });
                        await usr.RemoveRoleAsync(role);
                    }
                }
            }
        }
        public static async Task<IGuildUser> GetGuildUser(SocketGuild guild, ulong id)
        {
            var usrs = guild.GetUsersAsync();
            await foreach (var usxr in usrs)
            {
                foreach (var usr in usxr)
                {
                    if (usr.Id == id)
                        return usr;
                }
                
            }
            return null;
        }
        private async Task<IRole> GetGuildRole(SocketGuild guild, string name)
        {
            foreach (var role in guild.Roles)
            {
                if (role.Name == name)
                    return role;
            }
            return (await guild.CreateRoleAsync(name));
        }

        private Task _client_UserJoined(SocketGuildUser arg)
        {
            throw new NotImplementedException();
        }

        private Task Log(LogMessage msg)
		{
			Console.WriteLine(msg.ToString());
			return Task.CompletedTask;
		}
        public async Task Client_Ready()
        {
			
			await _interactionService.RegisterCommandsToGuildAsync(966751064336515142);
		}
        private Task ComponentCommandExecuted(ComponentCommandInfo arg1, Discord.IInteractionContext arg2, IResult arg3)
        {
            if (!arg3.IsSuccess)
            {
                switch (arg3.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        // implement
                        break;
                    case InteractionCommandError.UnknownCommand:
                        // implement
                        break;
                    case InteractionCommandError.BadArgs:
                        // implement
                        break;
                    case InteractionCommandError.Exception:
                        // implement
                        break;
                    case InteractionCommandError.Unsuccessful:
                        // implement
                        break;
                    default:
                        break;
                }
            }

            return Task.CompletedTask;
        }

        private Task ContextCommandExecuted(ContextCommandInfo arg1, Discord.IInteractionContext arg2, IResult arg3)
        {
            if (!arg3.IsSuccess)
            {
                switch (arg3.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        // implement
                        break;
                    case InteractionCommandError.UnknownCommand:
                        // implement
                        break;
                    case InteractionCommandError.BadArgs:
                        // implement
                        break;
                    case InteractionCommandError.Exception:
                        // implement
                        break;
                    case InteractionCommandError.Unsuccessful:
                        // implement
                        break;
                    default:
                        break;
                }
            }

            return Task.CompletedTask;
        }

        private Task SlashCommandExecuted(SlashCommandInfo arg1, Discord.IInteractionContext arg2, IResult arg3)
        {
            if (!arg3.IsSuccess)
            {
                switch (arg3.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        // implement
                        break;
                    case InteractionCommandError.UnknownCommand:
                        // implement
                        break;
                    case InteractionCommandError.BadArgs:
                        // implement
                        break;
                    case InteractionCommandError.Exception:
                        // implement
                        break;
                    case InteractionCommandError.Unsuccessful:
                        // implement
                        break;
                    default:
                        break;
                }
            }

            return Task.CompletedTask;
        }

        private async Task HandleInteraction(SocketInteraction arg)
        {
            try
            {
                // create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules
                var ctx = new SocketInteractionContext(_client, arg);
                await _interactionService.ExecuteCommandAsync(ctx, _services);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // if a Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
                // response, or at least let the user know that something went wrong during the command execution.
                if (arg.Type == InteractionType.ApplicationCommand)
                {
                    await arg.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
                }
            }
        }
    }

}
