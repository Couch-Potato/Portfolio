using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Services;
using ProjectEmergencyFrameworkClient.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Debugging
{
    public static class DebugInteracts
    {
        [ExecuteAt(ExecutionStage.DevToolsStart)]
        public static void MakeDevInteracts()
        {
            if (!FrameworkController.DO_DEBUG_INTERACTS) return;
            Interact.InteractService.ConstructInteract("vehiclespawn", new Vector3(-970f, -2702f, 13.37f), new
            {
                isCopSpawn = false,
                organizationLocked = false
            });
            Interact.InteractService.ConstructInteract("vehiclespawn", new Vector3(434.2f, -1011.8f, 28.69f), new
            {
                organizationLocked = true,
                organization = "BASE::SASP",
                requireOnTeam = true,
                onlyShowOffDuty = false
            });
            Interact.InteractService.ConstructInteract("armory", new Vector3(452.9867f, -980.0710f, 30.7137f), new
            {
                organizationLocked = true,
                organization = "BASE::SASP",
                requireOnTeam = true,
                onlyShowOffDuty = false
            });
            Interact.InteractService.ConstructInteract("jail@evidence", new Vector3(478.7809f, -984.1361f, 25.0571f), new
            {
                organizationLocked = true,
                organization = "BASE::SASP",
                requireOnTeam = true,
                onlyShowOffDuty = false
            });
            Interact.InteractService.ConstructInteract("jail@mugshot", new Vector3(436.1396f, -990.1817f, 26.6524f), new
            {
                organizationLocked = true,
                organization = "BASE::SASP",
                requireOnTeam = true,
                onlyShowOffDuty = false
            });
            Interact.InteractService.ConstructInteract("jail@fingerprint", new Vector3(440.5677f, -991.2324f, 26.5683f), new
            {
                organizationLocked = true,
                organization = "BASE::SASP",
                requireOnTeam = true,
                onlyShowOffDuty = false
            });
            Interact.InteractService.ConstructInteract("computer@police", new Vector3(460.2641f, -988.9686f, 24.8237f), new
            {
                organizationLocked = true,
                organization = "BASE::SASP",
                requireOnTeam = true,
                onlyShowOffDuty = false
            });

            Interact.InteractService.ConstructInteract("cj@citation", new Vector3(440.9825f, -980.5652f, 30.9083f), new
            {

            });

            DoorService.AddDoorhashesInRange(new Vector3(440.9825f, -980.5652f, 30.9083f), 50f, new
            uint[]{
                0xF82C9473,
                0xC26DA56D,
                0xDF9AE350
            }, true, null, "POLICE");

            Interact.InteractService.ConstructInteract("onduty", new Vector3(441.4f, -983f, 30.6f), new
            {
                organizationLocked = true,
                organization = "BASE::SASP",
                requireOnTeam = false,
                onlyShowOffDuty = true
            });

            /* Interact.InteractService.ConstructInteract("weaponShop", new Vector3(-1024.593f, -2710.097f, 13.79656f), new
             {
                 items = new List<Interfaces.UI.WeaponItem>()
                 {
                     new Interfaces.UI.WeaponItem()
                     {
                         name="Carbine Rifle",
                         price=420,
                         WeaponHash=0x969C3D67,
                         icon="https://www.gtabase.com/images/jch-optimize/ng/images_gta-5_weapons_assault-rifles_special-carbine-mk2.webp"
                     }
                 }
             });*/
            /*Interact.InteractService.ConstructInteract("clothingShop", new Vector3(-1024.593f, -2710.097f, 13.79656f), new
            {
                items = new List<Interfaces.UI.ClothingShopItem>()
                {
                    new Interfaces.UI.ClothingShopItem()
                    {
                        name="Carbine Rifle",
                        price=420,
                        clothingSetId=8,
                        category="Accessory",
                        id=5,
                        variations=new List<int>(){0,1,2},
                        icon="https://www.gtabase.com/images/jch-optimize/ng/images_gta-5_weapons_assault-rifles_special-carbine-mk2.webp"
                    },
                     new Interfaces.UI.ClothingShopItem()
                    {
                        name="Carbine Rifle2",
                        price=420,
                        clothingSetId=8,
                        category="Accessory",
                        id=6,
                        variations=new List<int>(){0,1,2},
                        icon="https://www.gtabase.com/images/jch-optimize/ng/images_gta-5_weapons_assault-rifles_special-carbine-mk2.webp"
                    }
                }
            });*/
            /*  Interact.InteractService.ConstructInteract("genericShop", new Vector3(-1024.593f, -2710.097f, 13.79656f), new
              {
                  items = new List<Interfaces.UI.BasedShopItem>()
                  {
                      new Interfaces.UI.BasedShopItem()
                      {
                          name="123",
                          icon="456",
                          modifiers=new {},
                          type="ASSET",
                          price=100
                      }
                  }
              });*/
            /* Interact.InteractService.ConstructInteract("crafting", new Vector3(-1024.593f, -2710.097f, 13.79656f), new
             {
                 interfaceType = Interfaces.UI.CraftInterface.FoodCrafter
             });*/
            Interact.InteractService.ConstructInteract("testitem", new Vector3(-1024.593f, -2710.097f, 13.79656f), new { });
            Interact.InteractService.ConstructInteract("trashcan", new Vector3(435.2060f, -985.2798f, 30.3924f), new { });
        }
    }
}
