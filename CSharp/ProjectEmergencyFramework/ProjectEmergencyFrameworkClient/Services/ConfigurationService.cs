using CitizenFX.Core;
using Newtonsoft.Json;
using ProjectEmergencyFrameworkShared.Configuration;
using ProjectEmergencyFrameworkShared.Configuration.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;

namespace ProjectEmergencyFrameworkClient.Services
{
    public static class ConfigurationService
    {
        public static PEConfiguration CurrentConfiguration { get; set; }

        public static Dictionary<string, string> ImportedIcons { get; set; } = new Dictionary<string, string>();
        public static Dictionary<string, GameItem> GameItems { get; set; } = new Dictionary<string, GameItem>();
        public static Dictionary<string, ComplexProp> ComplexProps { get; set; } = new Dictionary<string, ComplexProp>();


        public static T GetArchetype<T>(string name) where T : IArchetype
        {
            foreach (IArchetype archetype in CurrentConfiguration.ClothingShopArchetypes)
            {
                if (archetype.Name == name && archetype.GetType() == typeof(T))
                {
                    return (T)archetype;
                }
            }
            foreach (IArchetype archetype in CurrentConfiguration.GenericShopArchetypes)
            {
                if (archetype.Name == name && archetype.GetType() == typeof(T))
                {
                    return (T)archetype;
                }
            }
            foreach (IArchetype archetype in CurrentConfiguration.WeaponShopArchetypes)
            {
                if (archetype.Name == name && archetype.GetType() == typeof(T))
                {
                    return (T)archetype;
                }
            }
            foreach (IArchetype archetype in CurrentConfiguration.VehicleShopArchetypes)
            {
                if (archetype.Name == name && archetype.GetType() == typeof(T))
                {
                    return (T)archetype;
                }
            }
            foreach (IArchetype archetype in CurrentConfiguration.FreightShopArchetypes)
            {
                if (archetype.Name == name && archetype.GetType() == typeof(T))
                {
                    return (T)archetype;
                }
            }
            foreach (IArchetype archetype in CurrentConfiguration.Universes)
            {
                if (archetype.Name == name && archetype.GetType() == typeof(T))
                {
                    return (T)archetype;
                }
            }
            throw new UnknownArchetypeExcpetion(name, typeof(T));
        }

        public static void ConfigureCurrentSession(string name)
        {
            //
            string config_str = LoadResourceFile("project_emergency_framework", "build.json");
            PEConfiguration config = JsonConvert.DeserializeObject<PEConfiguration>(config_str);
            CurrentConfiguration = config;
            ImportedIcons.Clear();


            ImportedIcons.Add("NULL", "/assets/missing_texture.webp");
            DebugService.SetDebugOwner("CONFIGURE_CLIENT");
            DebugService.SetDebugHandler("PRECONFIGURE");
            try
            {
                foreach (var icon in config.ItemIcons)
                {
                    ImportedIcons.Add(icon.IconId, icon.IconImageName);
                }
                foreach (var genericItems in config.GameItems)
                {
                    GameItems.Add(genericItems.Name, genericItems);
                }
            }
            catch (Exception ex)
            {
                DebugService.UnhandledException(ex);
            }
            DebugService.ClearDebugHandler();
            foreach (var shop in config.ClothingShops)
            {
                DebugService.SetDebugHandler($"CLOTHING_SHOP_{shop.Name.Replace(" ", "_").ToUpper()}");
                DebugService.DebugCall("CONFIGURE", $"CLOTHING_SHOP_{shop.Name.Replace(" ", "_").ToUpper()}");
                try
                {
                    MakeMarker(shop.BlipInfo, shop.Location);
                    /*var archetype = GetArchetype<ClothingShopArchetype>(shop.Archetype);
                    List<Interfaces.UI.ClothingShopItem> clothingItems = new List<Interfaces.UI.ClothingShopItem>();
                    foreach (var item in archetype.Items)
                    {
                        clothingItems.Add(new Interfaces.UI.ClothingShopItem()
                        {
                            price = (int)item.Price,
                            clothingSetId = item.ClothingItemType,
                            icon = ImportedIcons["NULL"],
                            id = item.ClothingId,
                            name = $"ITEM ID:{item.ClothingId}",
                            variations = item.Variations,
                            category = ((Equipables.ClothingItemType)item.ClothingItemType).ToString().ToUpper()
                        });
                    }
                    Interact.InteractService.ConstructInteract("clothingShop", _loc_to_vector_3(shop.Location), new
                    {
                        items = clothingItems
                    });*/
                }
                catch (Exception ex)
                {
                    DebugService.UnhandledException(ex);
                }
                DebugService.ClearDebugHandler();

            }
            foreach (var shop in config.WeaponShops)
            {
                DebugService.SetDebugHandler($"WEAPON_SHOP_{shop.Name.Replace(" ", "_").ToUpper()}");
                DebugService.DebugCall("CONFIGURE", $"WEAPON_SHOP_{shop.Name.Replace(" ", "_").ToUpper()}");
                try
                {
                    MakeMarker(shop.BlipInfo, shop.Location);
                    var archetype = GetArchetype<WeaponShopArchetype>(shop.Archetype);
                    var items = new List<Interfaces.UI.WeaponItem>();
                    foreach (var item in archetype.Items)
                    {
                        items.Add(new Interfaces.UI.WeaponItem
                        {
                            icon = ImportedIcons[item.IconId],
                            name = item.Name,
                            price = item.Price,
                            WeaponHash = item.WeaponHash
                        });
                    }
                    Interact.InteractService.ConstructInteract("weaponShop", _loc_to_vector_3(shop.Location), new
                    {
                        items = items
                    });
                }
                catch (Exception ex)
                {
                    DebugService.UnhandledException(ex);
                }
                DebugService.ClearDebugHandler();

            }
            foreach (var shop in config.VehicleShops)
            {
                DebugService.SetDebugHandler($"VEHICLE_SHOP_{shop.Name.Replace(" ", "_").ToUpper()}");
                DebugService.DebugCall("CONFIGURE", $"VEHICLE_SHOP_{shop.Name.Replace(" ", "_").ToUpper()}");
                DebugService.ClearDebugHandler();

                try
                {
                    MakeMarker(shop.BlipInfo, shop.Location);
                    var archetype = GetArchetype<VehicleShopArchetype>(shop.Archetype);
                    var csc = new Interfaces.UI.CarShopConfig();
                    foreach (var item in archetype.Items)
                    {
                        csc.Add(new Interfaces.UI.CarShopItem()
                        {
                            make = item.Make,
                            model = item.Model,
                            cat = ((VehicleClass)GetVehicleClassFromName((uint)GetHashKey(item.SpawnName))).ToString().ToUpper(),
                            capacity = 4,
                            mpg = 20,
                            speed = (int)GetVehicleClassMaxSpeed(GetVehicleClassFromName((uint)GetHashKey(item.SpawnName))),
                            spawnName = item.SpawnName,
                            price = (int)item.Price
                        });
                    }
                    Interact.InteractService.ConstructInteract("carshop", _loc_to_vector_3(shop.Location), new
                    {
                        config = csc
                    });
                }
                catch (Exception ex)
                {
                    DebugService.UnhandledException(ex);
                }
                DebugService.ClearDebugHandler();

            }
            foreach (var shop in config.GenericShops)
            {
                DebugService.SetDebugHandler($"GENERIC_SHOP_{shop.Name.Replace(" ", "_").ToUpper()}");
                DebugService.DebugCall("CONFIGURE", $"GENERIC_SHOP_{shop.Name.Replace(" ", "_").ToUpper()}");
                DebugService.ClearDebugHandler();

                try
                {
                    MakeMarker(shop.BlipInfo, shop.Location);
                    var archetype = GetArchetype<GenericShopArchetype>(shop.Archetype);
                    var items = new List<Interfaces.UI.BasedShopItem>();
                    foreach (var item in archetype.Items)
                    {
                        items.Add(new Interfaces.UI.BasedShopItem()
                        {
                            price = item.Price,
                            name = item.Name,
                            type = item.Category,
                            icon = ImportedIcons[GameItems[item.GenericItemName].IconId],
                            modifiers = GameItems[item.GenericItemName].Properties,
                            smartItemName = GameItems[item.GenericItemName].SmartItemId
                        });

                    }
                    Interact.InteractService.ConstructInteract("genericShop", _loc_to_vector_3(shop.Location), new
                    {
                        items = items
                    });
                }
                catch (Exception ex)
                {
                    DebugService.UnhandledException(ex);
                }
                DebugService.ClearDebugHandler();

            }
            foreach (var bank in config.Banks)
            {
                DebugService.SetDebugHandler($"BANK_{bank.Name.Replace(" ", "_").ToUpper()}");
                DebugService.DebugCall("CONFIGURE", $"BANK_{bank.Name.Replace(" ", "_").ToUpper()}");
                DebugService.ClearDebugHandler();

                try
                {
                    Interact.InteractService.ConstructInteract("mazeBank", _loc_to_vector_3(bank.Location), new { });
                    MakeMarker(bank.BlipInfo, bank.Location);
                }
                catch (Exception ex)
                {
                    DebugService.UnhandledException(ex);
                }
                DebugService.ClearDebugHandler();

            }
            foreach (var pd in config.PoliceStations)
            {
                DebugService.SetDebugHandler($"POLICE_STN_{pd.Name.Replace(" ", "_").ToUpper()}");
                DebugService.DebugCall("CONFIGURE", $"POLICE_STN_{pd.Name.Replace(" ", "_").ToUpper()}");

                try
                {
                    _utilConstruct_org(pd.EvidenceLocker, "jail@evidence", pd.OrganizationId);
                    _utilConstruct_org(pd.VehicleSpawner, "vehiclespawn", pd.OrganizationId);
                    _utilConstruct_org(pd.Mugshot, "jail@mugshot", pd.OrganizationId);
                    _utilConstruct_org(pd.Fingerprint, "jail@fingerprint", pd.OrganizationId);
                    _utilConstruct_org(pd.BookingComputer, "computer@police", pd.OrganizationId);
                    Interact.InteractService.ConstructInteract("cj@citation", _loc_to_vector_3(pd.PayFines), new { });
                    Interact.InteractService.ConstructInteract("onduty", _loc_to_vector_3(pd.OnDutyLocation), new
                    {
                        organizationLocked = true,
                        organization = pd.OrganizationId,
                        requireOnTeam = false,
                        onlyShowOffDuty = true
                    });
                    Interact.InteractService.ConstructInteract("uniformLocker", _loc_to_vector_3(pd.OnDutyLocation), new
                    {
                        organizationLocked = true,
                        organization = pd.OrganizationId,
                        requireOnTeam = true,
                    });
                }
                catch (Exception ex)
                {
                    DebugService.UnhandledException(ex);
                }
                DebugService.ClearDebugHandler();

            }
            foreach (var da in config.DoorAreas)
            {
                DebugService.SetDebugHandler($"DOOR_{da.Name.Replace(" ", "_").ToUpper()}");
                DebugService.DebugCall("CONFIGURE", $"DOOR_{da.Name.Replace(" ", "_").ToUpper()}");

                try
                {
                    var loc = new Vector3(da.Location.X, da.Location.Y, da.Location.Z);
                    DoorService.AddDoorhashesInRange(loc, da.Radius, da.Models.ToArray(), true, da.LockedOrganization, da.LockedType);
                }
                catch (Exception ex)
                {
                    DebugService.UnhandledException(ex);
                }
                DebugService.ClearDebugHandler();

            }
            foreach (var marker in config.Markers)
            {
                foreach (var bp in marker)
                {
                    try
                    {
                        DebugService.SetDebugHandler($"MARKER_{bp.Text.Replace(" ", "_").ToUpper()}");
                        DebugService.DebugCall("CONFIGURE", $"MARKER_{bp.Text.Replace(" ", "_").ToUpper()}");

                        MakeMarker(bp, bp.Location);
                        DebugService.ClearDebugHandler();

                    }
                    catch (Exception ex)
                    {
                        DebugService.UnhandledException(ex);
                        DebugService.ClearDebugHandler();

                    }
                }
            }
            foreach (var npc in config.NPCs)
            {
                DebugService.SetDebugHandler($"NPC_{npc.Name.Replace(" ", "_").ToUpper()}");
                DebugService.DebugCall("CONFIGURE", $"REGISTER_NPC_{npc.Name.Replace(" ", "_").ToUpper()}");
                NPCService.RegisterNPC(new NPCType()
                {
                    Name = npc.Name,
                    PedModel = npc.PedModel,
                    NPCTalk = () =>
                    {
                        MissionService.StartMission(npc.MissionTrigger);
                    }
                });
                if (npc.IsSpawnedOnStart)
                {
                    DebugService.DebugCall("CONFIGURE", $"SPAWN_NPC_{npc.Name.Replace(" ", "_").ToUpper()}");
                    NPCService.SpawnNPC(npc.Name, _loc_to_vector_3(npc.Location), npc.Heading);
                }
                DebugService.ClearDebugHandler();

            }
            foreach (var tattoo in config.TattooShops)
            {
                DebugService.SetDebugHandler($"TATTOO_{tattoo.Name.Replace(" ", "_").ToUpper()}");
                DebugService.DebugCall("CONFIGURE", $"TATTOO_{tattoo.Name.Replace(" ", "_").ToUpper()}");
                Interact.InteractService.ConstructInteract("ui@open@shop", _loc_to_vector_3(tattoo.Location), new
                {
                    ui = "tattoo",
                    props = new {}
                });
                MakeMarker(tattoo.BlipInfo, tattoo.Location);
                DebugService.ClearDebugHandler();

            }
            foreach (var tattoo in config.BarberShops)
            {
                DebugService.SetDebugHandler($"BARBER_{tattoo.Name.Replace(" ", "_").ToUpper()}");
                DebugService.DebugCall("CONFIGURE", $"BARBER_{tattoo.Name.Replace(" ", "_").ToUpper()}");
                Interact.InteractService.ConstructInteract("ui@open@shop", _loc_to_vector_3(tattoo.Location), new
                {
                    ui = "barber",
                    props = new { }
                });
                MakeMarker(tattoo.BlipInfo, tattoo.Location);
                DebugService.ClearDebugHandler();

            }
            foreach (var tattoo in config.Props)
            {
                DebugService.SetDebugHandler($"PROP_{tattoo.Name.Replace(" ", "_").ToUpper()}");
                DebugService.DebugCall("CONFIGURE", $"PROP_{tattoo.Name.Replace(" ", "_").ToUpper()}");
                try
                {
                    foreach (var prop in config.Props)
                    {
                        Vector3 min = new Vector3(float.MaxValue);
                        Vector3 max = new Vector3(float.MinValue);

                        var complexProp = new ComplexProp();
                        complexProp.Name = prop.Name;
                        complexProp.Props = new List<PropItem>();
                        foreach (var item in prop.Placements)
                        {
                            Vector3 mi = Vector3.Zero;
                            Vector3 ma = Vector3.Zero;
                            GetModelDimensions((uint)GetHashKey(item.HashName), ref mi, ref ma);
                            if (mi.LengthSquared() > 0)
                            {
                                ma = ma - mi;
                                mi = Vector3.Zero;
                            }
                            /*ma += new Vector3(item.PositionRotation.X, item.PositionRotation.Y, item.PositionRotation.Z);
                            mi += new Vector3(item.PositionRotation.X, item.PositionRotation.Y, item.PositionRotation.Z);
*/
                            if (mi.LengthSquared() < min.LengthSquared())
                                min = mi;
                            if (ma.LengthSquared() > max.LengthSquared())
                                max = ma;
                        }
                        Vector3 size = max - min;
                        Vector3 origin = new Vector3(min.X + (size.X / 2), min.Y + (size.Y / 2), min.Z);
                        foreach (var px in prop.Placements)
                        {
                            var vx = new Vector3(px.PositionRotation.X, px.PositionRotation.Y, px.PositionRotation.Z);
                            var offset = vx - origin;
                            px.PositionRotation.X = offset.X;
                            px.PositionRotation.Y = offset.Y;
                            px.PositionRotation.Z = offset.Z;
                            complexProp.Props.Add(new PropItem()
                            {
                                HashName=px.HashName,
                                Position = offset,
                                Rotation = new Vector3(px.PositionRotation.Yaw, px.PositionRotation.Pitch, px.PositionRotation.Roll)
                            });
                        }
                        complexProp.Size = size;
                        ComplexProps.Add(complexProp.Name, complexProp);
                    }
                    
                }catch (Exception ex)
                {
                    DebugService.UnhandledException(ex);
                }
                DebugService.ClearDebugHandler();

            }
            foreach (var freightShop in config.FreightShops)
            {
                DebugService.SetDebugHandler($"FREIGHT_{freightShop.Name.Replace(" ", "_").ToUpper()}");
                DebugService.DebugCall("CONFIGURE", $"FREIGHT_{freightShop.Name.Replace(" ", "_").ToUpper()}");

                try
                {
                    Interfaces.UI.ShippingConfig cfg = new Interfaces.UI.ShippingConfig();

                    var archetype = GetArchetype<FreightShopArchetype>(freightShop.Archetype);

                    foreach (var ix in archetype.ItemsBuying)
                    {
                        cfg.precursors.Add(new Interfaces.UI.ShippingItem()
                        {
                            name = GameItems[ix.GenericItemName].Name,
                            icon = ImportedIcons[GameItems[ix.GenericItemName].IconId],
                            desc=ix.Description,
                            price=ix.Price
                        });
                    }
                    foreach (var ix in archetype.ItemsForSale)
                    {
                        cfg.sellables.Add(new Interfaces.UI.ShippingItem()
                        {
                            name = GameItems[ix.GenericItemName].Name,
                            icon = ImportedIcons[GameItems[ix.GenericItemName].IconId],
                            desc = ix.Description,
                            price = ix.Price
                        });
                    }
                    foreach (var ix in archetype.CraftingModulesForSale)
                    {
                        cfg.craftingModules.Add(new Interfaces.UI.ShippingItem()
                        {
                            name = ix.Name,
                            icon = ix.Icon,
                            desc = ix.Description,
                            price = ix.Price,
                            addtlProps = new
                            {
                                tier = ix.Tier,
                                type = ix.ModuleType
                            }
                        });
                    }

                    Interact.InteractService.ConstructInteract("ui@open@shop", _loc_to_vector_3(freightShop.Location), new
                    {
                        ui = "shipping",
                        props = new
                        {
                            config = cfg
                        }
                    });

                }
                catch (Exception ex)
                {
                    DebugService.UnhandledException(ex);
                }
                DebugService.ClearDebugHandler();


            }
            foreach (var apartment in config.Apartments)
            {
                DebugService.SetDebugHandler($"APARTMENT_{apartment.Name.Replace(" ", "_").ToUpper()}");
                DebugService.DebugCall("CONFIGURE", $"APARTMENT_{apartment.Name.Replace(" ", "_").ToUpper()}");

                try
                {
                    MakeMarker(apartment.BlipInfo, apartment.ApartmentBlipLocation);

                    Interact.InteractService.ConstructInteract("vehiclespawn", _loc_to_vector_3(apartment.VehicleSpawnLocation), new
                    {
                        isCopSpawn = false,
                        organizationLocked = false
                    });

                    Interact.InteractService.ConstructInteract("apartment@enter", _loc_to_vector_3(apartment.ApartmentAccessLocation), new { });



                }
                catch (Exception ex)
                {
                    DebugService.UnhandledException(ex);

                }

                DebugService.ClearDebugHandler();


            }
            DebugService.ClearDebugOwner();
        }
        public static void MakeMarker(BlipInfo bp, Location position)
        {
            var blip = AddBlipForCoord(position.X, position.Y, position.Z);
            SetBlipDisplay(blip, 6);
            SetBlipDisplay(blip, 4);
            SetBlipScale(blip, 0.9f);
            SetBlipAsShortRange(blip, true);
            SetBlipSprite(blip, bp.Id);
            SetBlipFlashes(blip, false);
            BeginTextCommandSetBlipName("STRING");
            AddTextComponentString(bp.Text);
            EndTextCommandSetBlipName(blip);
            if (bp.Color != -1)
            {
                SetBlipColour(blip, bp.Color);
            }
        }

        private static void _utilConstruct_org(Location l, string name, string organization)
        {
            var loc = new Vector3(l.X, l.Y, l.Z);

            Interact.InteractService.ConstructInteract(name, loc, new
            {
                organizationLocked = true,
                organization = organization,
                requireOnTeam = true,
                onlyShowOffDuty = false
            });
        }
        internal static Vector3 _loc_to_vector_3(Location l)
        {
            var loc = new Vector3(l.X, l.Y, l.Z);
            return loc;
        }


    }
    public class UnknownArchetypeExcpetion : Exception
    {
        public UnknownArchetypeExcpetion(string name, Type type) : base($"Could not find archetype '{name} of type {type.Name}'") { }
    }
   
}
