using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
namespace ProjectEmergencyFrameworkClient.Equipables
{
    public abstract class ContainerEquipable : Equipable
    {
        public override bool DisabledPrimary => true;

        protected override async void OnInstanced()
        {
            if (!Utility.CrappyWorkarounds.HasProperty(Modifiers, "containerId"))
            {
                Modifiers.containerId = await InventoryService.CreateNetworkedContainer(15, "CONTAINER", "EQUIP_CONTAINER");
            }
        }
        protected override void OnPrimaryUp()
        {
            InventoryService.OpenNetworkedContainer(Modifiers.containerId);
        }
    }
    [Equipable("Briefcase", "/assets/inventory/briefcase.svg")]
    public class C_BriefcaseEquipable : ContainerEquipable
    {
        const uint weapon_briefcase_02 = 0x01B79F17;
        protected override void OnInstanced()
        {
            GiveWeaponToPed(Game.PlayerPed.Handle, weapon_briefcase_02, 0, false, true);
            SetCurrentPedWeapon(Game.PlayerPed.Handle, weapon_briefcase_02, true);
            base.OnInstanced();
        }
        protected override void OnDeInstanced()
        {
            RemoveWeaponFromPed(Game.PlayerPed.Handle, weapon_briefcase_02);
        }
    }
    [Equipable("Duffelbag", "/assets/inventory/duffelbag.svg")]
    public class C_DuffelbagEquipable : ContainerEquipable
    {
        protected override void OnInstanced()
        {
            SetPedComponentVariation(Game.PlayerPed.Handle, 5, 45, 0, 0);
            base.OnInstanced();

        }
        protected override void OnDeInstanced()
        {
            SetPedComponentVariation(Game.PlayerPed.Handle, 5, 0, 0, 0);
        }
        protected override void OnEquip()
        {
            SetPedComponentVariation(Game.PlayerPed.Handle, 5, 44, 0, 0);
        }
        protected override void OnUnEquip()
        {
            SetPedComponentVariation(Game.PlayerPed.Handle, 5, 45, 0, 0);
        }
    }
}
