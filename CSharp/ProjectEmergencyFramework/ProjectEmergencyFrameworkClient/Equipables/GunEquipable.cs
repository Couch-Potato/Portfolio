using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Services;
using ProjectEmergencyFrameworkClient.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;

namespace ProjectEmergencyFrameworkClient.Equipables
{
    [Equipable("GUN","__gun", false)]
    public class GunEquipable : Equipable
    {
        uint WeaponHash;
        protected override async void OnInstanced()
        {
            _name = this.Modifiers.name;
            _icon = this.Modifiers.icon;
            WeaponHash = Convert.ToUInt32(this.Modifiers.weapon_hash);
            
            if (!CrappyWorkarounds.HasProperty(this.Modifiers, "serialNumber"))
            {
                this.Modifiers.firingPattern = RandomUtil.RandomString(8);
                if (CrappyWorkarounds.HasProperty(Modifiers, "doNotTrackWeapon"))
                {
                    if (Modifiers.doNotTrackWeapon)
                    {
                        this.Modifiers.serialNumber = "UNKNOWN";
                        this.Modifiers.desc = $"SERIAL NUMBER SCRATCHED OFF";
                        return;
                    }
                }
                if (!IsInDiscreteMode)
                {
                    await handleInstance();
                    InventoryService.ForceUpdateEquipable(this);
                }
                DebugService.Watchpoint("GUN2", Modifiers);
            }
            // Handle the holster stuff (put the gun in the holster when it is instanced)
            foreach (var holster in ConfigurationService.CurrentConfiguration.Holsters)
            {
                // If it applies to our weapon.

                if (holster.WeaponName == _name)
                {
                    // Check to see if we have the out of holster component on, and if we do then put the gun inside of it.
                    var holsterId = GetPedDrawableVariation(Game.PlayerPed.Handle, holster.WeaponOutOfHolster.ComponentId);
                    if (holsterId == holster.WeaponOutOfHolster.DrawableId)
                    {
                        var texture = GetPedTextureVariation(Game.PlayerPed.Handle, holster.WeaponOutOfHolster.ComponentId);
                        SetPedComponentVariation(Game.PlayerPed.Handle, holster.WeaponInHolster.ComponentId, holster.WeaponInHolster.DrawableId, texture, 0);
                    }
                }
            }

        }
       /* private ProjectEmergencyFrameworkShared.Configuration.Schema.Holster _holstercfg = null;*/
        private async Task handleInstance()
        {
            if (OrganizationService.IsOnDuty)
            {
                this.Modifiers.serialNumber = await QueryService.QueryConcrete<string>("GET_GUN_SERIAL", new
                {
                    name = Name,
                    hash = WeaponHash,
                    purchaser = CharacterService.CurrentCharacter.Id,
                    organization = OrganizationService.ConnectedOrganization.CallableId
                });
                this.Modifiers.desc = $"SERIAL NUMBER: {this.Modifiers.serialNumber}";
            }
            else
            {
                this.Modifiers.serialNumber = await QueryService.QueryConcrete<string>("GET_GUN_SERIAL", new
                {
                    name = Name,
                    hash = WeaponHash,
                    purchaser = CharacterService.CurrentCharacter.Id,
                    organization = ""
                });
                this.Modifiers.desc = $"SERIAL NUMBER: {this.Modifiers.serialNumber}";
            }

        }
        protected override void OnEquip()
        {

            GiveWeaponToPed(Game.PlayerPed.Handle, WeaponHash, 0, false, true);
            SetCurrentPedWeapon(Game.PlayerPed.Handle, WeaponHash, true);
            // Handle the holster stuff (take the gun out of the holster)
            foreach (var holster in ConfigurationService.CurrentConfiguration.Holsters)
            {
                // If it applies to our weapon.

                if (holster.WeaponName == _name)
                {
                    // Check to see if we have the out of holster component on, and if we do then put the gun inside of it.
                    var holsterId = GetPedDrawableVariation(Game.PlayerPed.Handle, holster.WeaponInHolster.ComponentId);
                    if (holsterId == holster.WeaponInHolster.DrawableId)
                    {
                        var texture = GetPedTextureVariation(Game.PlayerPed.Handle, holster.WeaponInHolster.ComponentId);
                        SetPedComponentVariation(Game.PlayerPed.Handle, holster.WeaponOutOfHolster.ComponentId, holster.WeaponOutOfHolster.DrawableId, texture, 0);
                    }
                }
            }
           
            base.OnEquip();
        }
        protected override void OnUnEquip()
        {
            RemoveWeaponFromPed(Game.PlayerPed.Handle, WeaponHash);
            // Handle the holster stuff (put the gun in the holster when it is unequipped)
            foreach (var holster in ConfigurationService.CurrentConfiguration.Holsters)
            {
                // If it applies to our weapon.

                if (holster.WeaponName == _name)
                {
                    // Check to see if we have the out of holster component on, and if we do then put the gun inside of it.
                    var holsterId = GetPedDrawableVariation(Game.PlayerPed.Handle, holster.WeaponInHolster.ComponentId);
                    if (holsterId == holster.WeaponOutOfHolster.DrawableId)
                    {
                        var texture = GetPedTextureVariation(Game.PlayerPed.Handle, holster.WeaponOutOfHolster.ComponentId);
                        SetPedComponentVariation(Game.PlayerPed.Handle, holster.WeaponInHolster.ComponentId, holster.WeaponInHolster.DrawableId, texture, 0);
                    }
                }
            }
            base.OnUnEquip();
        }

        protected override void OnPrimaryUp()
        {
            // If weapon is a gun
            if (GetWeaponDamageType(WeaponHash) == 3)
            {
                if (CharacterService.Timestamp() - CharacterService.LastShotTime < 3) return;
                CharacterService.LastShotTime = CharacterService.Timestamp();
                QueryService.QueryConcrete<bool>("GUNSHOT", new
                {
                    name = this.Name,
                    serial= Modifiers.serialNumber,
                    firingPattern =Modifiers.firingPattern
                });
            }
        }
        protected override void OnDeInstanced()
        {
            // Handle the holster stuff (take the gun out of the holster)
            foreach (var holster in ConfigurationService.CurrentConfiguration.Holsters)
            {
                // If it applies to our weapon.

                if (holster.WeaponName == _name)
                {
                    // Check to see if we have the out of holster component on, and if we do then put the gun inside of it.
                    var holsterId = GetPedDrawableVariation(Game.PlayerPed.Handle, holster.WeaponInHolster.ComponentId);
                    if (holsterId == holster.WeaponInHolster.DrawableId)
                    {
                        var texture = GetPedTextureVariation(Game.PlayerPed.Handle, holster.WeaponInHolster.ComponentId);
                        SetPedComponentVariation(Game.PlayerPed.Handle, holster.WeaponOutOfHolster.ComponentId, holster.WeaponOutOfHolster.DrawableId, texture, 0);
                    }
                }
            }
            base.OnDeInstanced();
        }
    }
}
