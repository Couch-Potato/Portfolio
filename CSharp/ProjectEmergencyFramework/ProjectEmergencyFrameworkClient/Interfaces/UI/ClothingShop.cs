using System;
using System.Collections.Generic;
using System.Linq;
using static CitizenFX.Core.Native.API;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Equipables;
using ProjectEmergencyFrameworkClient.Services;
using ProjectEmergencyFrameworkClient.Services.Cams;

namespace ProjectEmergencyFrameworkClient.Interfaces.UI
{
    [UserInterface("clothingshop", true)]
    public class ClothingShop : UserInterface
    {

        public Dictionary<string, List<ClothingShopItem>> shopPairs = new Dictionary<string, List<ClothingShopItem>>();

        public List<string> catOrder = new List<string>();

        private List<ClothingShopItem> _items = new List<ClothingShopItem>();

        [Configuration("items")]
        public List<ClothingShopItem> shopItems { get => _items; set=> _items = value; }

        [Configuration("cat_icons")]
        public List<CategoryIconConfigurationItem> catIcons = new List<CategoryIconConfigurationItem>();

        public List<_PRESET_CFG> PlayerCFG = new List<_PRESET_CFG>();
        public List<_PRESET_CFG> PropCFG = new List<_PRESET_CFG>();

        private int _category = 0;
        private int _cloth = 0;
        private int _variation = 0;


        [Reactive("category")]
        public int category { get=>_category; set {
                _category = value;
                _variation = 0;
                _cloth = 0;
                UpdateClothingItem();
            } }

        [Reactive("clothing")]
        public int cloth { get =>_cloth; set {
                _variation = 0;
                _cloth = value;
                UpdateClothingItem();
            } 
        }

        [Reactive("variation")]
        public int variation { get =>_variation; set
            {
                _variation = value;
                UpdateClothingItem();
            }
        }
        [Reactive("buy")]
        public async void Buy()
        {
            try
            {
                var item = shopPairs[catOrder[_category]][_cloth];
                if (!await Services.TransactionService.Pay((float)item.price, $"CLOTHING PURCHASE - {item.name}"))
                    return;
                InitConfig();

                Services.InventoryService.AddItem(Services.CharacterService.MintClothingItem((ClothingItemType)item.clothingSetId, item.id, variation));
            }catch(Exception ex)
            {
                DebugService.UnhandledException(ex);
            }
            
            
        }
        [Reactive("exit")]
        public void Exit()
        {
            InterfaceController.HideInterface("clothingshop");
        }


        private void UpdateClothingItem()
        {
            try
            {
                var type = (ClothingItemType)shopPairs[catOrder[_category]][_cloth].clothingSetId;
                var item = shopPairs[catOrder[_category]][_cloth];
                switch ((ClothingItemType)item.clothingSetId)
                {
                    case ClothingItemType.Shoes:
                        SetPedComponentVariation(Game.PlayerPed.Handle, 6, item.id, _variation, 0);
                        CameraService.GetCameraOperator<ClothingCam>().CameraType = 4;
                        break;
                    case ClothingItemType.Hat:
                        SetPedPropIndex(Game.PlayerPed.Handle, 0, item.id, _variation, true);
                        CameraService.GetCameraOperator<ClothingCam>().CameraType = 1;

                        break;
                    case ClothingItemType.Glasses:
                        SetPedPropIndex(Game.PlayerPed.Handle, 1, item.id, _variation, true);
                        CameraService.GetCameraOperator<ClothingCam>().CameraType = 1;

                        break;
                    case ClothingItemType.Watch:
                        SetPedPropIndex(Game.PlayerPed.Handle, 6, item.id, _variation, true);
                        CameraService.GetCameraOperator<ClothingCam>().CameraType = 0;

                        break;
                    case ClothingItemType.Shirt:
                        SetPedComponentVariation(Game.PlayerPed.Handle, 11, item.id, _variation, 0);
                        CameraService.GetCameraOperator<ClothingCam>().CameraType = 2;

                        if (Utility.ClothingData.BestTorsoForTop[item.id] != -1)
                        {
                            SetPedComponentVariation(Game.PlayerPed.Handle, 3, Utility.ClothingData.BestTorsoForTop[item.id], 0, 0);
                        }
                        else
                        {
                            // HANDLE TOP MANAGER!!! WE CANNOT MANUALLY SET IT. 
                        }
                        break;
                    case ClothingItemType.Undershirt:
                        SetPedComponentVariation(Game.PlayerPed.Handle, 8, item.id, _variation, 0);
                        CameraService.GetCameraOperator<ClothingCam>().CameraType = 2;


                        break;
                    case ClothingItemType.Pants:
                        SetPedComponentVariation(Game.PlayerPed.Handle, 3, item.id, _variation, 0);
                        CameraService.GetCameraOperator<ClothingCam>().CameraType = 3;

                        break;
                    case ClothingItemType.Mask:
                        SetPedComponentVariation(Game.PlayerPed.Handle, 1, item.id, _variation, 0);
                        CameraService.GetCameraOperator<ClothingCam>().CameraType = 1;

                        break;
                    case ClothingItemType.Accessory:
                       
                        SetPedComponentVariation(Game.PlayerPed.Handle, 7, item.id, _variation, 0);
                        CameraService.GetCameraOperator<ClothingCam>().CameraType = 2;

                        break;
                }
            }
            catch (Exception ex)
            {
                DebugService.UnhandledException(ex);
            }
            
           

        }

        protected override Task ConfigureAsync()
        {
            catIcons = CategoryIconConfigurationSet.Items;
            shopItems = Utility.CrappyWorkarounds.ShittyFiveMDynamicToConcrete<List<ClothingShopItem>>(Properties.items);
            foreach (var item in shopItems)
            {
                if (item.name == null)
                {
                    switch ((ClothingItemType)item.clothingSetId)
                    {
                        case ClothingItemType.Shoes:
                            item.name = Utility.ClothingData.Shoes[item.id][0];
                            break;
                        case ClothingItemType.Hat:
                            item.name = Utility.ClothingData.Hats[item.id][0];
                            break;
                        case ClothingItemType.Glasses:
                            item.name = Utility.ClothingData.Glasses[item.id][0];
                            break;
                        case ClothingItemType.Watch:
                            item.name = Utility.ClothingData.Watches[item.id][0];
                            break;
                        case ClothingItemType.Shirt:
                            item.name = Utility.ClothingData.Tops[item.id][0];
                            break;
                        case ClothingItemType.Undershirt:
                            item.name = Utility.ClothingData.Undershirts[item.id][0];
                            break;
                        case ClothingItemType.Pants:
                            item.name = Utility.ClothingData.Pants[item.id][0];
                            break;
                        case ClothingItemType.Mask:
                            item.name = Utility.ClothingData.Masks[item.id][0];
                            break;
                        case ClothingItemType.Accessory:
                            item.name = Utility.ClothingData.Accessories[item.id][0];
                            break;
                    }
                    
                }
                if (!shopPairs.ContainsKey(item.category))
                {
                    shopPairs.Add(item.category, new List<ClothingShopItem>());
                    catOrder.Add(item.category);
                }
                if (catIcons.Find(x => x.category == item.category) == null)
                {
                    catIcons.Add(new CategoryIconConfigurationItem()
                    {
                        category = item.category,
                        icon = Utility.BaseIcons.MissingIcon
                    });
                }
                shopPairs[item.category].Add(item);

            }
            return base.ConfigureAsync();
        }

        private void GivePlayerClothingItem()
        {
            Services.InventoryService.AddItem(Services.CharacterService.MintClothingItem(Equipables.ClothingItemType.Shoes, 0, 0));
        }
        private void InitConfig()
        {
            PlayerCFG.Clear();
            for (int i = 0; i < 12; i++)
            {
                PlayerCFG.Add(new _PRESET_CFG()
                {
                    item = GetPedDrawableVariation(Game.PlayerPed.Handle, i),
                    variation = GetPedTextureVariation(Game.PlayerPed.Handle, i)
                });
            }
            PropCFG.Clear();
            PropCFG.Add(new _PRESET_CFG()
            {
                item = GetPedPropIndex(Game.PlayerPed.Handle,0),
                variation = GetPedPropTextureIndex(Game.PlayerPed.Handle,0)
            });
            PropCFG.Add(new _PRESET_CFG()
            {
                item = GetPedPropIndex(Game.PlayerPed.Handle, 1),
                variation = GetPedPropTextureIndex(Game.PlayerPed.Handle, 1)
            });
            PropCFG.Add(new _PRESET_CFG()
            {
                item = GetPedPropIndex(Game.PlayerPed.Handle, 6),
                variation = GetPedPropTextureIndex(Game.PlayerPed.Handle, 6)
            });
        }
        private void RestoreConfig()
        {
            for (int i=0;i<12;i++)
            {
                SetPedComponentVariation(Game.PlayerPed.Handle, i, PlayerCFG[i].item, PlayerCFG[i].variation, 0);
            }
            SetPedPropIndex(Game.PlayerPed.Handle, 0, PlayerCFG[0].item, PropCFG[0].variation, true);
            SetPedPropIndex(Game.PlayerPed.Handle, 1, PlayerCFG[1].item, PropCFG[1].variation, true);
            SetPedPropIndex(Game.PlayerPed.Handle, 6, PlayerCFG[2].item, PropCFG[2].variation, true);

        }

        protected override Task BeforeShow()
        {
            CameraService.SetCamera<ClothingCam>();
            InitConfig();
            // Todo:

            // Flip character camera around
            // Stop character from moving
            // Fade to black??
            UpdateClothingItem();

            FreezeEntityPosition(GetPlayerPed(-1), true);
            DisableAllControlActions(0);
            DisplayRadar(false);


            // Do camera business

            /*var cam = CreateCam("DEFAULT_SCRIPTED_CAMERA", false);
            var coordsCam = GetOffsetFromEntityGivenWorldCoords(PlayerId(), 0.0f, 0.5f, 0.65f);

            var coordsPly = GetEntityCoords(PlayerPedId(), true);
            SetCamCoord(cam, coordsCam.X, coordsCam.Y, coordsCam.Z);
            PointCamAtCoord(cam, coordsPly.X, coordsPly.Y, coordsPly.Z + 0.65f);

            SetCamActive(cam, true);

            RenderScriptCams(true, true, 500, true, true);*/

            return base.BeforeShow();
        }
        protected override void Cleanup()
        {

            /*RenderScriptCams(false, true, 500, true, true);*/
            FreezeEntityPosition(GetPlayerPed(-1), false);
            CameraService.Terminate();
            DisplayRadar(true);
            RestoreConfig();
            base.Cleanup();
        }
    }
    public class _PRESET_CFG
    {
        public int item { get; set; }
        public int variation { get; set; }
    }
    public class ClothingShopData
    {
        public List<ClothingShopItem> Shoes { get; set; }
        public List<ClothingShopItem> Hats { get; set; }
        public List<ClothingShopItem> Shirts { get; set; }
        public List<ClothingShopItem> Pants { get; set; }
    }

    public static class CategoryIconConfigurationSet
    {
        public static List<CategoryIconConfigurationItem> Items { get; set; } = new List<CategoryIconConfigurationItem>()
        {
            new CategoryIconConfigurationItem() {
                category="SHIRT",
                icon=Utility.BaseIcons.MissingIcon
            },
            new CategoryIconConfigurationItem() {
                category="PANTS",
                icon=Utility.BaseIcons.MissingIcon
            },new CategoryIconConfigurationItem() {
                category="MASK",
                icon=Utility.BaseIcons.MissingIcon
            },
            new CategoryIconConfigurationItem() {
                category="SHOES",
                icon=Utility.BaseIcons.MissingIcon
            }
        };
    }
    public class CategoryIconConfigurationItem
    {
        public string category { get; set; }
        public string icon { get; set; }
    }

    public class ClothingShopItem
    {
        public int clothingSetId { get; set; }
        public int id { get; set; }
        public string icon { get; set; }
        public string category { get; set; }
        public string name { get; set; }
        public int price { get; set; }
        public List<int> variations { get; set; }
    }
}
