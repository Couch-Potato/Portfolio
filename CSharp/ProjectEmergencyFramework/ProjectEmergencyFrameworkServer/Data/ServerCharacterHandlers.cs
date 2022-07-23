using CitizenFX.Core;
using Newtonsoft.Json;
using ProjectEmergencyFrameworkServer.Utility;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkServer.Data
{
    
    public static class ServerCharacterHandlers
    {
        public static Dictionary<Player, string> CurrentCharacters = new Dictionary<Player, string>();

        private static StarterVehicleItem[] StarterVehicles =
        {
            new StarterVehicleItem()
            {
                SpawnName = "faction",
                Make = "Willard",
                Model="Faction"
            },
            new StarterVehicleItem()
            {
                SpawnName = "ingot",
                Make = "Vapid",
                Model="Ingot"
            },
            new StarterVehicleItem()
            {
                SpawnName = "vwe_minivan1",
                Make = "Vapid",
                Model="Minivan"
            },
            new StarterVehicleItem()
            {
                SpawnName = "merit",
                Make = "Declasse",
                Model="Merit"
            },
        };

        public static ProjectEmergencyFrameworkShared.Data.Model.Character GetCharacterFromPlayer(Player px)
        {
            return PlayerDataService.GetCharacter(CurrentCharacters[px]);
        }

        [Queryable("GET_CONTAINER_DATA")]
        public static void GetContainerData(Query q, object i, Player px)
        {
            q.Reply(PlayerDataService.GetContainerData((string)i));
        }

        [Queryable("UPDATE_PHYSICAL")]
        public static void UpdatePhysical(Query q, object i, Player px)
        {
            var phx = (ProjectEmergencyFrameworkShared.Data.Model.CharacterPhysicalData)i;
            var chx = PlayerDataService.GetCharacter(CurrentCharacters[px]);
            chx.Physical = phx;
            PlayerDataService.UpdateCharacter(chx);
            q.Reply(true);
        }

        [Queryable("GET_CONTAINER")]
        public static void GetInvetoryContainer(Query q, object i, Player px)
        {
            q.Reply(PlayerDataService.GetContainerInventory((string)i));
        }

        [Queryable("SET_CONTAINER")]
        public static void SetInvetoryContainer(Query q, object i, Player px)
        {
            dynamic ctx = (dynamic)i;
            var x = ctx.ItemSet;

            var invList = new List<ProjectEmergencyFrameworkShared.Data.Model.InventoryItem>();

            foreach (var xl in x)
            {
                ProjectEmergencyFrameworkShared.Data.Model.InventoryItem convItem = CrappyWorkarounds.ShittyFiveMDynamicToConcrete<ProjectEmergencyFrameworkShared.Data.Model.InventoryItem>(xl);
                convItem.modifiers = xl.modifiers;
                convItem.modifiers = null;
                convItem.transportString = JsonConvert.SerializeObject(xl.modifiers);
                invList.Add(convItem);
            }

            PlayerDataService.SetContainerInventory(ctx.ContainerId, invList);
            q.Reply("ok");
        }


        [Queryable("GET_PLAYERS")]
        public static void GetPlayers(Query q, object i, Player px)
        {
            //q.Reply(PlayerDataService.GetLivingPlayerCharacters(PlayerDataService.GetPlayerFromDiscord((string)i[0])));
        }

        [Queryable("GET_CHARACTERS")]
        public static void GetCharacters(Query q, object i, Player px)
        {
            PlayerDataService.LoginPlayer(px.Identifiers["license"], px.Identifiers["discord"], px.Handle);

            q.Reply(PlayerDataService.GetLivingPlayerCharacters(PlayerDataService.GetPlayerFromDiscord(px.Identifiers["discord"])));
        }

        [Queryable("GET_CHARACTER")]
        public static void GetCharacter(Query q, object i, Player px)
        {
            var chx = PlayerDataService.GetCharacterFromId((string)i);
            q.Reply(chx);
        }

        [Queryable("SET_CHARACTER")]
        public static void SetCharacter(Query q, object i, Player px)
        {
            var chx = PlayerDataService.GetCharacterFromId((string)i);
            var plyr = PlayerDataService.GetPlayerFromDiscord(px.Identifiers["discord"]);
            plyr.CurrenctCharacter = chx.Id;
            PlayerDataService.UpdatePlayer(plyr);
            chx.IsOnline = true;
            chx.NetworkId = px.Handle;
            PlayerDataService.UpdateCharacter(chx);
            if (CurrentCharacters.ContainsKey(px))
            {
                CurrentCharacters[px] = chx.Id;
            }else
            {
                CurrentCharacters.Add(px, chx.Id);
            }
            MessageBusService.RPCCall(ProjectEmergencyFrameworkShared.Data.Model.MessageHost.Bot, "CLIENT_OVERRIDE", new
            {
                character = chx.Id
            });
            q.Reply("ok");
        }

        [Queryable("GET_C_INVENTORY")]
        public static void GetCharacterInventory(Query q, object i, Player px)
        {
            q.Reply(PlayerDataService.GetCharacterInventory(CurrentCharacters[px]));
        }

        [Queryable("SET_C_INVENTORY")]
        public static void SetCharacterInventory(Query q, object i, Player px)
        {
            var x = (List<dynamic>)i;

            var invList = new List<ProjectEmergencyFrameworkShared.Data.Model.InventoryItem>();
            
            foreach (var xl in x)
            {
                ProjectEmergencyFrameworkShared.Data.Model.InventoryItem convItem = CrappyWorkarounds.ShittyFiveMDynamicToConcrete<ProjectEmergencyFrameworkShared.Data.Model.InventoryItem>(xl);
                convItem.modifiers = xl.modifiers;
                convItem.modifiers = null;
                convItem.transportString = JsonConvert.SerializeObject(xl.modifiers);
                invList.Add(convItem);
            }

            PlayerDataService.SetCharacterInventory(CurrentCharacters[px], invList);
            q.Reply("ok");
        }

        [Queryable("CREATE_EVIDENCE_LOCKER")]
        public static void CreateEvidenceLocker(Query q, object i, Player px)
        {
            var expando = (dynamic)i;
            q.Reply(EvidenceDataService.CreateEvidenceLocker(expando.creator, expando.agency));
        }

        [Queryable("LOWER_CASH")]
        public static void LowerCash(Query q, object i, Player px)
        {
            var cash = (float)i;
            string chara = CurrentCharacters[px];
            ProjectEmergencyFrameworkShared.Data.Model.Character character = PlayerDataService.GetCharacterFromId(chara);
            if (cash < character.CashOnHand)
            {
                character.CashOnHand = cash;
                PlayerDataService.UpdateCharacter(character);
            }
        }

        [Queryable("ADD_CASH")]
        public static void AddCash(Query q, object i, Player px)
        {
            var cash = (float)i;
            string chara = CurrentCharacters[px];
            ProjectEmergencyFrameworkShared.Data.Model.Character character = PlayerDataService.GetCharacterFromId(chara);
            if (cash <= 100f)
            {
                character.CashOnHand += cash;
                PlayerDataService.UpdateCharacter(character);
            }
        }

        [Queryable("CREATE_CONTAINER")]
        public static void CreateContainer(Query q, object i, Player px)
        {
            var expando = (dynamic)i;
            List<ProjectEmergencyFrameworkShared.Data.Model.InventoryItem> items = new List<ProjectEmergencyFrameworkShared.Data.Model.InventoryItem>();
            for (int ix = 0; ix < (int)expando.max; ix++)
            {
                items.Add(new ProjectEmergencyFrameworkShared.Data.Model.InventoryItem());
            }


            ProjectEmergencyFrameworkShared.Data.Model.Container cont = PlayerDataService.CreateContainer(new ProjectEmergencyFrameworkShared.Data.Model.Container()
            {
                MaxItems = expando.max,
                Name=expando.name,
                Type=expando.type,
                Inventory = items
            });
            q.Reply(cont.Id);
        }

        [Queryable("CREATE_CHARACTER")]
        public static void CreateCharacter(Query q, object i, Player px)
        {
            var expando = (dynamic)i;

            var rand = new Random();
            var dlid = RandomString(22);

            PlayerDataService.CreateCharacter(new ProjectEmergencyFrameworkShared.Data.Model.Character() {
                DriversLicenseId = dlid,
                FirstName = expando.FirstName,
                LastName = expando.LastName,
                DOB = new ProjectEmergencyFrameworkShared.Data.Model.DateOfBirth()
                {
                    Day = expando.DOB.Day,
                    Month = expando.DOB.Month,
                    Year = expando.DOB.Year,
                },
                Clothing = CrappyWorkarounds.ShittyFiveMDynamicToConcrete<ProjectEmergencyFrameworkShared.Data.Model.ClothingData>(expando.Clothing),
                Physical = CrappyWorkarounds.ShittyFiveMDynamicToConcrete<ProjectEmergencyFrameworkShared.Data.Model.CharacterPhysicalData>(expando.Physical),
                AttachedPlayerId = PlayerDataService.GetPlayerFromDiscord(px.Identifiers["discord"]).Id,
                DMVPhoto = expando.DMVPhoto,
            });
            var id = PlayerDataService.GetCharacter(expando.FirstName, expando.LastName, dlid).Id;

            var randomCar = RandomCar();

            List<ProjectEmergencyFrameworkShared.Data.Model.InventoryItem> vehList = new List<ProjectEmergencyFrameworkShared.Data.Model.InventoryItem>();

            for (int ix = 0; ix < 15; ix++)
                vehList.Add(new ProjectEmergencyFrameworkShared.Data.Model.InventoryItem());

            var cont = PlayerDataService.CreateContainer(new ProjectEmergencyFrameworkShared.Data.Model.Container()
            {
                MaxItems= 15,
                Name=randomCar.Make + " " + randomCar.Model,
                Type="VEHICLE_TRUNK",
                Inventory = vehList
            });

            

            PlayerDataService.CreateVehicle(new ProjectEmergencyFrameworkShared.Data.Model.Vehicle()
            {
                RegisteredOwnerId = id, 
                LicensePlate = RandomStringAN(8),
                Make = randomCar.Make,
                Model = randomCar.Model,
                SpawnName = randomCar.SpawnName,
                ColorId = 0,
                IsGovernmentInsured = false,
                IsInsured = true,
                BelongsToOrganization = false,
                Container = cont.Id
            });

            q.Reply(id);

        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string RandomStringAN(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        private static StarterVehicleItem RandomCar()
        {
            Random rand = new Random();
            return StarterVehicles[rand.Next(0, 4)];
        }

        public static void Print(this ExpandoObject dynamicObject)
        {
            var dynamicDictionary = dynamicObject as IDictionary<string, object>;

            foreach (KeyValuePair<string, object> property in dynamicDictionary)
            {
                Debug.WriteLine("{0}: {1}", property.Key, property.Value.ToString());
            }
            Debug.WriteLine();
        }
    }
    public class StarterVehicleItem
    {
        public string Make { get; set; }
        public string Model { get; set; }
        public string SpawnName { get; set; }
    }
}
