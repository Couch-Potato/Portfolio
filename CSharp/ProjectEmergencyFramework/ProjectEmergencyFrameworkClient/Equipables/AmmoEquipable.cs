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
    [Equipable("Ammunition","")]
    public class AmmoEquipable : Equipable
    {
        public const uint Handgun = 0x1B06D571;
        public const uint SMG = 0x13532244;
        public const uint Shotgun = 0x1D073A89;
        public const uint AssaultRifle = 0xBFEFFF6D;
        public const uint LMG = 0x9D07F764;
        public const uint Heavy = 0x05FC3C11;
        public override bool DisabledPrimary => true;

        protected override void OnInstanced()
        {
            _name = this.Modifiers.type + " Ammunition";
            _icon = this.Modifiers.icon;
            this.Modifiers.desc = " Ammunition for: " + this.Modifiers.type + $" weapons. ({this.Modifiers.amount.ToString()} rounds)";
            this.Modifiers.tags = $"AMMUNITION,{this.Modifiers.type}";
        }

        protected override void OnPrimaryUp()
        {
            uint weaponType = 0;
            switch (this.Modifiers.type)
            {
                case "HANDGUN":
                    weaponType = Handgun;
                    break;
                case "SMG":
                    weaponType = SMG;
                    break;
                case "SHOTGUN":
                    weaponType = Shotgun;
                    break;
                case "ASSAULTRIFLE":
                    weaponType = AssaultRifle;
                    break;
                case "LMG":
                    weaponType = LMG;
                    break;
                case "HEAVY":
                    weaponType = Heavy;
                    break;
            }
            HUDService.ShowHelpText($"Added {Modifiers.amount} rounds.", "none", 2.5f);
            AddAmmoToPed(Game.PlayerPed.Handle, weaponType, (int)Modifiers.amount);
            Services.InventoryService.RemoveItem(this);
        }
    }
}
