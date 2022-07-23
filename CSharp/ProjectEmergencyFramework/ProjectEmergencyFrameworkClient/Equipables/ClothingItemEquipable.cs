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
    public enum ClothingItemType
    {
        Shoes,
        Hat,
        Glasses,
        Watch,
        Shirt,
        Undershirt,
        Pants,
        Mask,
        Accessory
    }
    [Equipable("_CLOTHING_ITEM", "_CLOTH")]
    public class ClothingItemEquipable : Equipable
    {
        int ClothType;
        int Drawable;
        int ForceBadge = -1;
        int ForceBadge_Variant = -1;
        int Variant;
        int ModId = -1;
        bool isProp = false;
        protected override void OnInstanced()
        {
            
            Drawable = (int)Modifiers.drawable;
            Variant = (int)Modifiers.variant;
            ClothType = (int)Modifiers.clothType;
            if (Utility.CrappyWorkarounds.HasProperty(Modifiers, "badge"))
            {
                ForceBadge = Modifiers.badge;
                ForceBadge_Variant = Modifiers.badgeVariant;
            }
            //DebugService.Watchpoint("ACCESS", ClothType);
            switch ((ClothingItemType)(int)ClothType)
            {
                case ClothingItemType.Shoes:
                    _name = Utility.ClothingData.Shoes[Drawable][Variant];
                    Modifiers.desc = _name + " [Shoes]";
                    Modifiers.tags = "CLOTHING,SHOES";
                    ModId = 6;
                    SetPedComponentVariation(Game.PlayerPed.Handle, 6, Drawable, Variant, 0);
                    break;
                case ClothingItemType.Hat:
                    _name = Utility.ClothingData.Hats[Drawable][Variant];
                    SetPedPropIndex(Game.PlayerPed.Handle, 0, Drawable, Variant, true);
                    Modifiers.desc = _name + " [Hat]";
                    Modifiers.tags = "CLOTHING,HAT";
                    ModId = 0;
                    isProp = true;
                    break;
                case ClothingItemType.Accessory:
                    _name = Utility.ClothingData.Accessories[Drawable][Variant];
                    Modifiers.desc = _name + " [Accessory]";
                    Modifiers.tags = "CLOTHING,ACCESSORY";
                    SetPedComponentVariation(Game.PlayerPed.Handle, 7, Drawable, Variant, 0);
                    ModId = 7;
                    break;
                case ClothingItemType.Glasses:
                    _name = Utility.ClothingData.Glasses[Drawable][Variant];
                    Modifiers.desc = _name + " [Glasses]";
                    Modifiers.tags = "CLOTHING,GLASSES";
                    SetPedPropIndex(Game.PlayerPed.Handle, 1, Drawable, Variant, true);
                    ModId = 1;
                    isProp = true;
                    break;
                case ClothingItemType.Watch:
                    _name = Utility.ClothingData.Watches[Drawable][Variant];
                    Modifiers.desc = _name + " [Watch]";
                    Modifiers.tags = "CLOTHING,WATCH";
                    SetPedPropIndex(Game.PlayerPed.Handle, 6, Drawable, Variant, true);
                    ModId = 6;
                    isProp = true;
                    break;
                case ClothingItemType.Shirt:
                    _name = Utility.ClothingData.Tops[Drawable][Variant];
                    Modifiers.desc = _name + " [Shirt]";
                    Modifiers.tags = "CLOTHING,SHIRT";
                    SetPedComponentVariation(Game.PlayerPed.Handle, 11, Drawable, Variant, 0);
                    if (Utility.ClothingData.BestTorsoForTop[Drawable] != -1)
                    {
                        SetPedComponentVariation(Game.PlayerPed.Handle, 3, Utility.ClothingData.BestTorsoForTop[Drawable], 0, 0);
                    }
                    else
                    {
                        // HANDLE TOP MANAGER!!! WE CANNOT MANUALLY SET IT. 
                    }
                    ModId = 11;
                    break;
                case ClothingItemType.Undershirt:
                    _name = Utility.ClothingData.Undershirts[Drawable][Variant];
                    Modifiers.desc = _name + " [Undershirt]";
                    Modifiers.tags = "CLOTHING,UNDERSHIRT";
                    SetPedComponentVariation(Game.PlayerPed.Handle, 8, Drawable, Variant, 0);
                    ModId = 8;
                    break;
                case ClothingItemType.Pants:
                    _name = Utility.ClothingData.Pants[Drawable][Variant];
                    Modifiers.desc = _name + " [Pants]";
                    Modifiers.tags = "CLOTHING,PANTS";
                    SetPedComponentVariation(Game.PlayerPed.Handle, 4, Drawable, Variant, 0);
                    ModId = 4;
                    break;
                case ClothingItemType.Mask:
                    _name = Utility.ClothingData.Masks[Drawable][Variant];
                    Modifiers.desc = _name + " [Mask]";
                    Modifiers.tags = "CLOTHING,MASK";
                    SetPedComponentVariation(Game.PlayerPed.Handle, 1, Drawable, Variant, 0);
                    ModId = 1;
                    break;
            }
         /*   if (ForceBadge != -1)
            {
                    SetPedComponentVariation(Game.PlayerPed.Handle, 10, ForceBadge, ForceBadge_Variant, 0);
            }*/
            /*
                        _name = Utility.ClothingData.;*/
            _icon = this.Modifiers.icon;
        }

        protected override void OnEquip()
        {
            if (!isProp)
            {
                SetPedComponentVariation(Game.PlayerPed.Handle, ModId, 0, 0, 0);
                //Shirtless so lets handle the torso for that.
                if (ModId == 11)
                {
                    SetPedComponentVariation(Game.PlayerPed.Handle, 3, 15, 0, 0);
                }
                // Put us in undies if we have not pants
                if (ModId == 4)
                {
                    SetPedComponentVariation(Game.PlayerPed.Handle, 4, 24, 0, 0);
                }
            }else
            {
                SetPedPropIndex(Game.PlayerPed.Handle, ModId, 0, 0, false);
            }
            if (ForceBadge != -1)
            {
                SetPedComponentVariation(Game.PlayerPed.Handle, 10, 0, 0, 0);
            }
            base.OnEquip();
        }

        protected override void OnDeInstanced()
        {
            if (!isProp)
            {
                SetPedComponentVariation(Game.PlayerPed.Handle, ModId, 0, 0, 0);
                //Shirtless so lets handle the torso for that.
                if (ModId == 11)
                {
                    SetPedComponentVariation(Game.PlayerPed.Handle, 3, 15, 0, 0);
                }
                // Put us in undies if we have not pants
                if (ModId == 4)
                {
                    SetPedComponentVariation(Game.PlayerPed.Handle, 4, 24, 0, 0);
                }
            }
            else
            {
                SetPedPropIndex(Game.PlayerPed.Handle, ModId, 0, 0, false);
            }
            if (ForceBadge != -1)
            {
                SetPedComponentVariation(Game.PlayerPed.Handle, 10, 0, 0, 0);
            }
            base.OnDeInstanced();
        }

        protected override void OnUnEquip()
        {
            if (!isProp)
            {
                SetPedComponentVariation(Game.PlayerPed.Handle, ModId, Drawable, Variant, 0);
                if (ModId == 11)
                {
                    SetPedComponentVariation(Game.PlayerPed.Handle, 3, Utility.ClothingData.BestTorsoForTop[Drawable], 0, 0);
                }
            }else
            {
                SetPedPropIndex(Game.PlayerPed.Handle, ModId, Drawable, Variant, true);
            }
            if (ForceBadge != -1)
            {
                SetPedComponentVariation(Game.PlayerPed.Handle, 10, ForceBadge, ForceBadge_Variant, 0);
            }
            base.OnUnEquip();
        }

        ~ClothingItemEquipable()
        {
            DeInstance();
        }
    }
}
