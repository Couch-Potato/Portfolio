using Newtonsoft.Json;
using ProjectEmergencyFrameworkShared.Configuration;
using ProjectEmergencyFrameworkShared.Configuration.Schema;
using ProjectEmergencyFrameworkShared.Configuration.Schema.Menyoo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProjectEmergencyConfigurator
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                Console.WriteLine("No args specified.");
            if (args[0] == "--generate")
            {
                CreateDefaultBuild();
            }
            if (args[0] == "--build")
            {
                DiscoverBuild();
            }
            if (args[0] == "--debug")
            {

            }
        }
        public static void WriteToFile<T>(T a, string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            TextWriter writer = new StreamWriter(filename);
            serializer.Serialize(writer, a);
            writer.Close();
        }
        public static void DiscoverBuild()
        {
            if (!Directory.Exists("game"))
            {
                Console.WriteLine("No game files found.");
                return;
            }
            PEConfiguration config = new PEConfiguration()
            {
                ClothingShops = HandleDirectoryDiscovery<ClothingShop>("game"),
                ClothingShopArchetypes = HandleDirectoryDiscovery<ClothingShopArchetype>("game"),
                CraftingRecipes = HandleDirectoryDiscovery<CraftingRecipe>("game"),
                Banks = HandleDirectoryDiscovery<Bank>("game"),
                DoorAreas = HandleDirectoryDiscovery<DoorConfig>("game"),
                GenericShopArchetypes = HandleDirectoryDiscovery<GenericShopArchetype>("game"),
                VehicleShopArchetypes = HandleDirectoryDiscovery<VehicleShopArchetype>("game"),
                WeaponShopArchetypes = HandleDirectoryDiscovery<WeaponShopArchetype>("game"),
                GameItems = HandleDirectoryDiscovery<GameItem>("game"),
                GenericInteracts = HandleDirectoryDiscovery<GenericInteractItem>("game"),
                GenericShops = HandleDirectoryDiscovery<GenericShop>("game"),
                ItemIcons = HandleDirectoryDiscovery<ItemIcon>("game"),
                PoliceStations = HandleDirectoryDiscovery<PoliceConfig>("game"),
                Uniforms = HandleDirectoryDiscovery<UniformConfig>("game"),
                VehicleShops = HandleDirectoryDiscovery<VehicleShop>("game"),
                WeaponShops = HandleDirectoryDiscovery<WeaponShop>("game"),
                Markers = HandleDirectoryDiscovery<Markers>("game"),
                Holsters = HandleDirectoryDiscovery<Holster>("game"),
                PropPlacements = HandleDirectoryDiscovery<PropPlacementConfig>("game"),
                BarberShops = HandleDirectoryDiscovery<BarberShop>("game"),
                TattooShops = HandleDirectoryDiscovery<TattooShop>("game"),
                ComputerAreas = HandleDirectoryDiscovery<ComputerAreaConfig>("game"),
                Apartments = HandleDirectoryDiscovery<ApartmentConfig>("game"),
                Props = new List<HydratedPropConfig>()
            };
            Console.WriteLine("Preparing to sanitize and load menyoo prop files...");
            var propCfgs = HandleDirectoryDiscovery<PropConfig>("game");
            foreach (var a in propCfgs)
            {
                Console.WriteLine("Preparing: " +a.Name);
                Console.WriteLine("Importing: " + a.PropFile);
                XmlSerializer serializer = new XmlSerializer(typeof(SpoonerPlacements));
                FileStream fs = new FileStream("game/props/" + a.PropFile, FileMode.Open);
                SpoonerPlacements placements;
                HydratedPropConfig propConfig = new HydratedPropConfig()
                {
                    Name = a.Name,
                    CreateStaticPropOnStart = a.CreateStaticPropOnStart,
                    Placements = new List<Placement>()
                };
              /*  try
                {*/

                    placements = (SpoonerPlacements)serializer.Deserialize(fs);
                    Console.WriteLine("Loading placements for " + a.Name + $" size({placements.Count})");

                    foreach (var obj in placements)
                    {
                    try
                    {
                      
                            var plca = (Placement)obj;
                            Console.WriteLine("PLACEMENT LOADED: " + plca.HashName);

                            propConfig.Placements.Add(plca);
                        
                    }
                    catch
                    {

                    }
                       
                    }
                    fs.Dispose();
               /* }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex);
                    Console.WriteLine($"Failed to import specified propfile for {a.Name} ({a.PropFile}): {ex.Message}");
                    fs.Dispose();
                    continue;
                }*/
                config.Props.Add(propConfig);
            }
            File.WriteAllText("build.json", JsonConvert.SerializeObject(config));
         
        }
        public static List<T> HandleDirectoryDiscovery<T>(string dirc, List<T> list = null)
        {
            if (list == null)
                list = new List<T>();
            foreach (var dc in Directory.GetDirectories(dirc))
            {
                HandleDirectoryDiscovery(dc, list);
            }
            XmlSerializer serializer = new XmlSerializer(typeof(T));
           
            foreach (var dx in Directory.GetFiles(dirc))
            {
                if (dx.Contains(".xml"))
                {
                    FileStream fs = new FileStream(dx, FileMode.Open);
                    try
                    {
                        
                        var item = serializer.Deserialize(fs);
                        list.Add((T)item);
                        Console.WriteLine($"Found {typeof(T).Name}:{dx}");
                        fs.Dispose();
                    }
                    catch(Exception ex)
                    {
                        //Console.WriteLine(ex);
                        fs.Dispose();
                    }
                    
                }
            }
            return list;
        }
        
        public static void CreateDefaultBuild()
        {
            if (!Directory.Exists("game"))
            {
                Directory.CreateDirectory("game");
                Directory.CreateDirectory("game/vehicle_shop");
                Directory.CreateDirectory("game/weapon_shop");
                Directory.CreateDirectory("game/clothing_shop");
                Directory.CreateDirectory("game/generic_shop");
                Directory.CreateDirectory("game/generic_items");
                Directory.CreateDirectory("game/misc_intc");
                Directory.CreateDirectory("game/icons");
                Directory.CreateDirectory("game/crafting");
                Directory.CreateDirectory("game/stations");
                Directory.CreateDirectory("game/doors");
                Directory.CreateDirectory("game/uniforms");
                Directory.CreateDirectory("game/markers");
                Directory.CreateDirectory("game/icons/img");
                Directory.CreateDirectory("game/holsters");
                Directory.CreateDirectory("game/props");
                Directory.CreateDirectory("game/props/menyoo");
                Directory.CreateDirectory("game/props/placement");
                Directory.CreateDirectory("game/barber");
                Directory.CreateDirectory("game/tattoo");
                Directory.CreateDirectory("game/computers");
                Directory.CreateDirectory("game/apartments");
                Directory.CreateDirectory("game/npc");
                Directory.CreateDirectory("game/npc/barter");
                Directory.CreateDirectory("game/freight");
                Directory.CreateDirectory("game/universes");
                // Weapon Shop
                WriteToFile(new WeaponShopArchetype()
                {
                    Name = "AMMUNATION@STANDARD",
                    Items = new List<WeaponShopItem>() {
                        new WeaponShopItem()
                        {
                            Name="M4A1",
                            IconId="WEAPON_M4A1@INV_ITEM",
                            Price=420,
                            WeaponHash=0x969C3D67
                        }
                    }
                }, "game/weapon_shop/std_arch_ammunation.xml");

                WriteToFile(new WeaponShop()
                {
                    Name = "AMMUNATION",
                    Memo = "VINEWOOD AMMUNATION",
                    Archetype = "WEAPONSHOP_AMMUNATION@STANDARD"
                    ,BlipInfo=new BlipInfo()
                    {
                        Color=0,
                        Id=0,
                        Text="This is an example blip"
                    }
                }, "game/weapon_shop/ammunation_vinewood.xml");

                // Vehicle Shop

                WriteToFile(new VehicleShopArchetype()
                {
                    Name = "SANSUPERAUTO@STANDARD",
                    Items = new List<VehicleShopItem>()
                    {
                        new VehicleShopItem()
                        {
                            Make="Lel",
                            Model="Xd",
                            Price=420,
                            SpawnName="police1"
                        }
                    }
                }, "game/vehicle_shop/std_arch_sansuperauto.xml");

                WriteToFile(new VehicleShop()
                {
                    Name = "SIMEON'S DEALERSHIP",
                    Memo = "SIMEON DEALERSHOP NEAR PILLBOX",
                    Archetype = "SANSUPERAUTO@STANDARD",
                    BlipInfo = new BlipInfo()
                    {
                        Color = 0,
                        Id = 0,
                        Text = "This is an example blip"
                    }
                }, "game/vehicle_shop/sansuperauto_simeon.xml");

                // Clothing Shop
                WriteToFile(new ClothingShopArchetype()
                {
                    Name = "SUBURBAN@STANDARD",
                    Items = new List<ClothingShopItem>()
                    {
                        new ClothingShopItem()
                        {
                            ClothingId=69,
                            
                        }
                    }
                }, "game/clothing_shop/std_arch_suburban.xml");

                WriteToFile(new ClothingShop()
                {
                    Name = "SUBURBAN",
                    Archetype = "SUBURBAN@STANDARD",
                    Memo = "Suburban at murietta heights, LS",
                    BlipInfo = new BlipInfo()
                    {
                        Color = 0,
                        Id = 0,
                        Text = "This is an example blip"
                    }
                }, "game/clothing_shop/suburban_murietta.xml");

                // Generic
                WriteToFile(new GameItem()
                {
                    Name = "TestItem",
                    IconId = "GENERIC_TESTITEM@INV_ITEM",
                    SmartItemId = "Weed",
                    Properties = new GlobalPropertyList()
                    {
                        desc = "This is a test item, this desc will show on the tooltip, the tags are also shown only on the tooltip.",
                        tags = new List<string>()
                        {
                            "TEST"
                        }
                    }
                }, "game/generic_items/test_item.xml");

                WriteToFile(new GenericShopArchetype()
                {
                    Name = "CORNERSTORE@STANDARD",
                    Items = new List<GenericShopItem>()
                    {
                        new GenericShopItem()
                        {
                            Name="Cool Test Item",
                            Category="Test",
                            Price=420,
                            GenericItemName="TestItem"
                        }
                    }
                }, "game/generic_shop/arch_corner_store.xml");

                WriteToFile(new GenericShop()
                {
                    Name = "Kims' Convience",
                    Archetype = "CORNERSTORE@STANDARD",
                    Memo = "in the hood or something",
                    BlipInfo = new BlipInfo()
                    {
                        Color = 0,
                        Id = 0,
                        Text = "This is an example blip"
                    }
                }, "game/generic_shop/cornerstore_kims_convience.xml");

                // Icons

                WriteToFile(new ItemIcon()
                {
                    IconImageName = "/img/weapon_m4.png",
                    IconId = "WEAPON_M4A1@INV_ITEM"
                }, "game/icons/weapon_m4a1.xml");

                WriteToFile(new ItemIcon()
                {
                    IconImageName = "/img/test.png",
                    IconId = "GENERIC_TESTITEM@INV_ITEM"
                }, "game/icons/generic_testitem.xml");

                // Misc 

                WriteToFile(new Bank()
                {
                    Name = "Paleto Bay Bank",
                    Memo = "Bank in the stix",
                    BlipInfo = new BlipInfo()
                    {
                        Color = 0,
                        Id = 0,
                        Text = "This is an example blip"
                    }
                }, "game/misc_intc/paleto_bank.xml");

                WriteToFile(new GasStation()
                {
                    Name = "LTD Grove",
                    Location = new Location(),
                    GasStationType = "ltd"
                }, "game/misc_intc/gas_ltd_grove.xml");

                WriteToFile(new GenericInteractItem()
                {
                    Name = "Onduty Interact",
                    Location = new Location() { X = 1.5f },
                    InteractId = "onduty",
                    GlobalPropertyList = new GlobalPropertyList()
                }, "game/misc_intc/random_generic.xml");

                WriteToFile(new CraftingRecipe()
                {
                    RecipeName = "Bitches",
                    RecipeType = "food",
                    ItemResult = "TestItem",
                    RequiredItems = new List<RecipeItem>()
                    {
                        new RecipeItem(){ IconId="123", Name="Weed"},
                         new RecipeItem(){ IconId="123", Name="Weed"},
                          new RecipeItem(){ IconId="123", Name="Weed"},
                    }
                }, "game/crafting/random_recipe.xml");

                WriteToFile(new PoliceConfig()
                {
                    Name = "MissionRow PD",
                    OrganizationId = "BASE::SASP",
                    BookingComputer = new Location(),
                    OnDutyLocation = new Location(),
                    EvidenceLocker = new Location(),
                    Fingerprint = new Location(),
                    Mugshot = new Location(),
                    PayFines = new Location(),
                    PersonnelLocker = new Location(),
                    VehicleSpawner = new Location(),
                    Armory = new Location()
                }, "game/stations/mission_row.xml");

                WriteToFile(new DoorConfig()
                {
                    Name = "MissionRow PD Doors",
                    AutoCloseTime = 3f,
                    DoAutoClose = true,
                    Location = new Location(),
                    LockedOrganization = null,
                    LockedType = "POLICE",
                    Models = new List<uint>()
                    {
                         0xF82C9473,
                         0xC26DA56D,
                         0xDF9AE350
                    },
                    Radius = 50f
                }, "game/doors/mission_row_pd.xml");


                WriteToFile(new UniformConfig()
                {
                    OrganizationId = "BASE::SASP",
                    Uniform = new List<UniformLoadout>()
                    {
                        new UniformLoadout()
                        {
                            Name="SASP CLASS A",
                            Shirt=new UniformItem(){Texture=1, Drawable=5}
                        },
                         new UniformLoadout()
                        {
                            Name="SASP CLASS B",
                            Pants=new UniformItem(){Texture=1, Drawable=5}
                        }
                    }
                }, "game/uniforms/sasp.xml");

                WriteToFile(new Markers()
                {
                    new LocationBlipInfo()
                    {
                        Color=1,
                        Id=1,
                        Text="Hello World!",
                        Location=new Location()
                        {
                            X=420,
                            Y=520,
                            Z=69
                        }
                    },new LocationBlipInfo()
                    {
                        Color=1,
                        Id=1,
                        Text="Hello World!",
                        Location=new Location()
                        {
                            X=420,
                            Y=520,
                            Z=69
                        }
                    },new LocationBlipInfo()
                    {
                        Color=1,
                        Id=1,
                        Text="Hello World!",
                        Location=new Location()
                        {
                            X=420,
                            Y=520,
                            Z=69
                        }
                    },
                }, "game/markers/government.xml");

                WriteToFile(new Holster()
                {
                    WeaponInHolster=new HolsterComponent()
                    {
                        ComponentId=8,
                        DrawableId=254
                    },
                    WeaponOutOfHolster = new HolsterComponent()
                    {
                        ComponentId = 8,
                        DrawableId = 254
                    },
                }, "game/holsters/pistol.xml");
                WriteToFile(new PropConfig()
                {
                    Name="STATIC@BOOKING_STATION",
                    PropFile="menyoo/missionrowbooking.xml",
                    CreateStaticPropOnStart = false
                }, "game/props/booking_station.xml");
                WriteToFile(new PropPlacementConfig
                {
                    PlacementName = "MPRD BOOKING STATION",
                    PropName = "STATIC@BOOKING_STATION",
                    WorldPlacement = new WorldPlacement()
                    {
                        Location = new Location(),
                        Orientation = new Location()
                    },

                }, "game/props/placement/mrpd_booking.xml");
                WriteToFile(new BarberShop()
                {
                    Name = "Barber Shop Vinweood",
                    BlipInfo = new BlipInfo()
                    {
                        Color = 0,
                        Id = 0,
                        Text = "This is an example blip"
                    },
                    Location=new Location()
                }, "game/barber/vinewood.xml");
                WriteToFile(new TattooShop()
                {
                    Name = "Tattoo Shop Vinweood",
                    BlipInfo = new BlipInfo()
                    {
                        Color = 0,
                        Id = 0,
                        Text = "This is an example blip"
                    },
                    Location = new Location()
                }, "game/tattoo/vinewood.xml");
                WriteToFile(new ComputerAreaConfig()
                {
                    ComputerType="JUDGE",
                    Name="DAVIS COURT HOUSE COMPUTERS",
                    Location = new Location(),
                    ModelHashes= new List<uint>()
                    {
                        0x0, 0x0
                    },
                    Radius=50f
                }, "game/computers/judge_davis.xml");
                WriteToFile(new ApartmentConfig()
                {
                    Id="APARTMENT@DEL_PERRO",
                    Name="Del Perro Apartment Complex",
                    BlipInfo= new BlipInfo()
                    {
                        Color = 0,
                        Id = 0,
                        Text = "This is an example blip"
                    },
                    CameraPreviewLocation = new Location(),
                    SpawnLocation = new Location(),
                    VehicleSpawnLocation = new Location(),
                    ApartmentAccessLocation = new Location(),
                    ApartmentBlipLocation = new Location(),
                    UniverseId="UNIVERSE@STANDARD_APT"
                }, "game/apartments/del_perro_apc.xml");
                WriteToFile(new NPCPed()
                {
                    Heading=0f,
                    IsSpawnedOnStart=false,
                    Location=new Location(),
                    MissionTrigger="GREEK_START",
                    Name="The Greek",
                    PedModel="mp_m_greek01"
                }, "game/npc/npc_test.xml");
                WriteToFile(new PedBarterRegion()
                {
                    AllowedBarterTimeRange=new TimeRange()
                    {
                        From=22.00f,
                        To=4.00f
                    },
                    BarterPeds = new List<PedConfig>()
                    {
                        new PedConfig()
                        {
                            ModelName="mp_m_greek01"
                        }
                    },
                    Location=new Location(),
                    Name="DrugDealer_POOR",
                    Radius=100f,
                    PurchaseOrders=new List<PurchaseOrder>()
                    {
                        new PurchaseOrder()
                        {
                            BasePrice=100f,
                            ItemName="Drugs",
                            PriceFlexibility=10f,
                            PurchaseChance=100
                        }
                    }
                },"game/npc/barter/region_grove.xml");
                WriteToFile(new FreightShopArchetype()
                {
                    Name="freight@standard",
                    CraftingModulesForSale=new List<CraftingModuleShopConfig>()
                    {
                        new CraftingModuleShopConfig()
                        {
                            Description="Craft shit",
                            Icon="icon@craftmodule",
                            ModuleType="food",
                            Name="Kitchen",
                            Price=1000f,
                            Tier=1
                        }
                    },
                    ItemsBuying = new List<FreightShopItem>()
                    {
                        new FreightShopItem()
                        {
                            Description="Random ass item",
                            Price=200f,
                            GenericItemName="Item"
                        }
                    },
                    ItemsForSale = new List<FreightShopItem>()
                    {
                        new FreightShopItem()
                        {
                            Description="Random ass item",
                            Price=200f,
                            GenericItemName="Item"
                        }
                    },
                    
                },"game/freight/arch_std.xml");

                WriteToFile(new FreightShop()
                {
                    Archetype="freight@standard",
                    Location=new Location(),
                    Name="Port of LS Freight Shop",
                    Skin="jetsam"
                }, "game/freight/port_of_ls.xml");

                WriteToFile(new UniverseArchetype()
                {
                    IsInterior = false,
                    SpawnCoords = new Location(),
                    Name = "universe@std_apartment"
                }, "game/universes/apartment.xml");

            }
        }
    }
}
