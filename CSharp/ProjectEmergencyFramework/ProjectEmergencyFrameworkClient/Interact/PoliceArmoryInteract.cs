using ProjectEmergencyFrameworkClient.Services;
using ProjectEmergencyFrameworkShared.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interact
{
    [Interactable("armory", "G", "ENTER ARMORY", true)]
    public class PoliceArmoryInteract : RadiusInteractable
    {
        public PoliceArmoryInteract()
        {
            Radius = 3f;
        }
        protected override void OnInteract()
        {
            InventoryItemCollection items = new InventoryItemCollection();
            /*    items.AddItem(EquipmentService.ConstructEquipable("Handcuffs", "", new { }));
                items.AddItem(EquipmentService.ConstructEquipable("Handcuffs", "", new { }));
                items.AddItem(EquipmentService.ConstructEquipable("Handcuffs", "", new { }));
                items.AddItem(EquipmentService.ConstructEquipable("Handcuff Key", "", new { }));*/

            items.AddItemWithNewSlot("Handcuffs", "/assets/inventory/handcuffs.svg", 1, false, new {nonTransferrable=true});
            items.AddItemWithNewSlot("Handcuff Key", "/assets/inventory/handcuffs.svg", 1, false, new { nonTransferrable =true});

            items.AddItem(EquipmentService.ConstructEquipable("Radio", "", new { }, true));
            items.AddItem(EquipmentService.ConstructEquipable("Medical Kit", "", new { }));
            items.AddItem(EquipmentService.ConstructEquipable("Citation Book", "", new { }));
            //https://www.gtabase.com/images/gta-5/weapons/handguns/stun-gun.png
            items.AddItem(EquipmentService.ConstructEquipable("GUN", "__gun", new {
                name="COCK-19",
                icon= "/assets/inventory/weapon_glock.svg",
                weapon_hash=1593441988
            }, true));
            items.AddItem(EquipmentService.ConstructEquipable("GUN", "__gun", new
            {
                name = "TASER",
                icon = "/assets/inventory/weapon_taser.svg",
                weapon_hash = 911657153
            }, true));

            items.AddItem(EquipmentService.ConstructEquipable("Ammunition", "", new {
                type="HANDGUN",
                amount=24,
                icon= "/assets/inventory/ammo_grey.svg"
            }));
            items.AddItem(EquipmentService.ConstructEquipable("Bodycam", "", new { }, true));
            //STATIC@BOOKING_STATION
            items.AddItem(EquipmentService.ConstructEquipable("_placeable", "", new
            {
                name="BookingStation",
                icon= "https://media.discordapp.net/attachments/930686885959962674/998652982843412530/unknown.png",
                placeable = "STATIC@BOOKING_STATION"
            }, true));

            InventoryService.OpenCustomInventory(new ProjectEmergencyFrameworkShared.Data.Model.Container()
            {
                MaxItems = items.Count,
                Inventory = items,
                Name = "AMRORY",
                Type = "ARMORY"
            }, (List<InventoryItem> itemsRet) =>
            {
                var dif = InventoryService.InvDiff(items, itemsRet);
            });
        }
    }
}
