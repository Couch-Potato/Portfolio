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
    public abstract class PropEquipable : Equipable
    {
        protected abstract float DrunkAmt { get; }
        public abstract string Prop { get; }
        public abstract int PropBone { get; }
        public abstract string AnimationSet { get; }
        public abstract string AnimationName { get; }
        public abstract Vector3 PropPosition { get; }
        public abstract Vector3 PropRotation { get; }

        public abstract bool IsScarce { get; }

        private int PropEnt = -1;

        private bool isEquipped = false;

        uint lastSip = 0;

        protected override async void OnEquip()
        {
            isEquipped = true;
            ClearPedTasks(Game.PlayerPed.Handle);
            int normalFlag = 16 + 32;
            if (IsScarce) normalFlag += 1;
            await Utility.AssetLoader.LoadAnimDict(AnimationSet);
            TaskPlayAnim(Game.PlayerPed.Handle, AnimationSet, AnimationName, 2.0f, 2.0f, -1, normalFlag, 0, false, false, false);
            var pCoords = Game.PlayerPed.Position;
            PropEnt = CreateObject(GetHashKey(Prop), pCoords.X, pCoords.Y, pCoords.Z + 0.2f, true, true, true);

            TaskService.InvokeUntilExpire(async () =>
            {
                if (lastSip + 30 < CharacterService.Timestamp())
                {
                    lastSip = CharacterService.Timestamp();
                    HealthService.DrunkPercent += DrunkAmt;
                }
                return !isEquipped;
            },"PROP_EFFECT_EQUIP");

            AttachEntityToEntity(PropEnt, Game.PlayerPed.Handle, GetPedBoneIndex(Game.PlayerPed.Handle, PropBone),
                PropPosition.X,
                PropPosition.Y,
                PropPosition.Z,
                PropRotation.X,
                PropRotation.X,
                PropRotation.X,
                true, true, false, true, 1, true
                );

        }

        protected override void OnUnEquip()
        {
            isEquipped = false;
            ClearPedTasks(Game.PlayerPed.Handle);
            DeleteEntity(ref PropEnt);
        }
    }
    [Equipable("Coffee", "/assets/inventory/coffee.svg")]
    public class GP_CoffeeEquipable : PropEquipable
    {
        public override string Prop => "p_amb_coffeecup_01";

        public override int PropBone => 28422;

        public override string AnimationSet => "amb@world_human_drinking@coffee@male@idle_a";

        public override string AnimationName => "idle_c";

        public override Vector3 PropPosition => new Vector3(0, 0, 0f);

        public override Vector3 PropRotation => new Vector3(0, 0, 0);

        public override bool IsScarce => false;

        protected override float DrunkAmt => 0f;
    }
    [Equipable("Whiskey", "/assets/inventory/whiskey.svg")]
    public class GP_WhiskeyEquipable : PropEquipable
    {
        public override string Prop => "prop_drink_whisky";

        public override int PropBone => 28422;

        public override string AnimationSet => "amb@world_human_drinking@coffee@male@idle_a";

        public override string AnimationName => "idle_c";

        public override Vector3 PropPosition => new Vector3(0.01f, -0.01f, -0.06f);

        public override Vector3 PropRotation => new Vector3(0, 0, 0);
        public override bool IsScarce => false;

        protected override float DrunkAmt => 20f;


    }
    [Equipable("Beer", "/assets/inventory/beer.svg")]
    public class GP_BeerEquipable : PropEquipable
    {
        public override string Prop => "prop_amb_beer_bottle";

        public override int PropBone => 28422;

        public override string AnimationSet => "amb@world_human_drinking@coffee@male@idle_a";

        public override string AnimationName => "idle_c";

        public override Vector3 PropPosition => new Vector3(0, 0, 0f);

        public override Vector3 PropRotation => new Vector3(0, 0, 0);
        public override bool IsScarce => false;

        protected override float DrunkAmt => 8f;


    }
    [Equipable("Cup", "/assets/inventory/coffee.svg")]
    public class GP_CupEquipable : PropEquipable
    {
        public override string Prop => "prop_plastic_cup_02";

        public override int PropBone => 28422;

        public override string AnimationSet => "amb@world_human_drinking@coffee@male@idle_a";

        public override string AnimationName => "idle_c";

        public override Vector3 PropPosition => new Vector3(0, 0, 0f);

        public override Vector3 PropRotation => new Vector3(0, 0, 0);
        public override bool IsScarce => false;

        protected override float DrunkAmt => 0f;


    }
    [Equipable("Soda", "/assets/inventory/soda.svg")]
    public class GP_SodaEquipable : PropEquipable
    {
        public override string Prop => "prop_ecola_can";

        public override int PropBone => 28422;

        public override string AnimationSet => "amb@world_human_drinking@coffee@male@idle_a";

        public override string AnimationName => "idle_c";

        public override Vector3 PropPosition => new Vector3(0, 0, 0f);

        public override Vector3 PropRotation => new Vector3(0, 0, 130);
        public override bool IsScarce => false;
        protected override float DrunkAmt => 0f;

    }
    [Equipable("Donut", "/assets/inventory/donut.svg")]
    public class GP_DonutEquipable : PropEquipable
    {
        public override string Prop => "prop_amb_donut";

        public override int PropBone => 18905;

        public override string AnimationSet => "mp_player_inteat@burger";

        public override string AnimationName => "mp_player_int_eat_burger";

        public override Vector3 PropPosition => new Vector3(0.13f, 0.05f, 0.02f);

        public override Vector3 PropRotation => new Vector3(-50f, 16f, 60);
        public override bool IsScarce => true;

        protected override float DrunkAmt => 0f;

    }
    [Equipable("Burger", "/assets/inventory/burger.svg")]
    public class GP_BurgerEquipable : PropEquipable
    {
        public override string Prop => "prop_cs_burger_01";

        public override int PropBone => 18905;

        public override string AnimationSet => "mp_player_inteat@burger";

        public override string AnimationName => "mp_player_int_eat_burger";

        public override Vector3 PropPosition => new Vector3(0.13f, 0.05f, 0.02f);

        public override Vector3 PropRotation => new Vector3(-50f, 16f, 60);
        public override bool IsScarce => true;

        protected override float DrunkAmt => 0f;


    }
    [Equipable("Sandwich", "/assets/inventory/sandwich.svg")]
    public class GP_SandwichEquipable : PropEquipable
    {
        public override string Prop => "prop_sandwich_01";

        public override int PropBone => 18905;

        public override string AnimationSet => "mp_player_inteat@burger";

        public override string AnimationName => "mp_player_int_eat_burger";

        public override Vector3 PropPosition => new Vector3(0.13f, 0.05f, 0.02f);

        public override Vector3 PropRotation => new Vector3(-50f, 16f, 60);
        public override bool IsScarce => true;

        protected override float DrunkAmt => 0f;

    }
    [Equipable("Candybar", "/assets/inventory/candy.svg")]
    public class GP_CandybarEquipable : PropEquipable
    {
        public override string Prop => "prop_choc_ego";

        public override int PropBone => 60309;

        public override string AnimationSet => "mp_player_inteat@burger";

        public override string AnimationName => "mp_player_int_eat_burger";

        public override Vector3 PropPosition => new Vector3(0,0,0);

        public override Vector3 PropRotation => new Vector3(0,0,0);
        public override bool IsScarce => true;

        protected override float DrunkAmt => 0f;


    }
    [Equipable("Wine", "/assets/inventory/wine.svg")]
    public class GP_WineEquipable : PropEquipable
    {
        public override string Prop => "prop_drink_redwine";

        public override int PropBone => 18905;

        public override string AnimationSet => "mp_player_inteat@burger";

        public override string AnimationName => "mp_player_int_eat_burger";

        public override Vector3 PropPosition => new Vector3(0.1f, -0.03f, 0.03f);

        public override Vector3 PropRotation => new Vector3(100.0f, 0,-10);
        public override bool IsScarce => false;

        protected override float DrunkAmt => 8f;


    }
    [Equipable("Champagne", "/assets/inventory/champage.svg")]
    public class GP_ChampEquipable : PropEquipable
    {
        public override string Prop => "prop_drink_champ";

        public override int PropBone => 18905;

        public override string AnimationSet => "mp_player_inteat@burger";

        public override string AnimationName => "mp_player_int_eat_burger";

        public override Vector3 PropPosition => new Vector3(0.1f, -0.03f, 0.03f);

        public override Vector3 PropRotation => new Vector3(100.0f, 0, -10);
        public override bool IsScarce => false;

        protected override float DrunkAmt => 10f;

    }
    [Equipable("Cigar", "/assets/inventory/cigar.svg")]
    public class GP_CigarEquipable : PropEquipable
    {
        public override string Prop => "prop_cigar_02";

        public override int PropBone => 47419;

        public override string AnimationSet => "amb@world_human_smoking@male@male_a@enter";

        public override string AnimationName => "enter";

        public override Vector3 PropPosition => new Vector3(0.010f, 0.0f, 0.0f);

        public override Vector3 PropRotation => new Vector3(50.0f, 0.0f, -80.0f);
        public override bool IsScarce => false;

        protected override float DrunkAmt => 0f;


    }
    [Equipable("Cigarette", "/assets/inventory/cigarette.svg")]
    public class GP_CigaretteEquipable : PropEquipable
    {
        public override string Prop => "prop_cs_ciggy_01";

        public override int PropBone => 28422;

        public override string AnimationSet => "amb@world_human_aa_smoke@male@idle_a";

        public override string AnimationName => "idle_b";

        public override Vector3 PropPosition => new Vector3(0,0,0);

        public override Vector3 PropRotation => new Vector3(0,0,0);
        public override bool IsScarce => false;

        protected override float DrunkAmt => 0f;


    }
}
