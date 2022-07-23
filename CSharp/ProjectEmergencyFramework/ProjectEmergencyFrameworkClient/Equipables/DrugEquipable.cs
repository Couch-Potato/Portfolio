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
    public abstract class DrugEquipable:Equipable
    {
        protected abstract float THC { get; }
        protected abstract float GenHigh { get; }

        private uint LastDrugTime = 0;
        public override bool DisabledPrimary => true;

        protected abstract int Hits { get; }
        private int HitsRemaining = 0;

        public DrugEquipable() : base()
        {
            HitsRemaining = Hits;
        }

        protected override async void OnPrimaryUp()
        {
            if (LastDrugTime + (20) > CharacterService.Timestamp()) return;
            await Utility.AssetLoader.LoadAnimDict("amb@world_human_drug_dealer_hard@male@exit");
            TaskPlayAnim(Game.PlayerPed.Handle, "amb@world_human_drug_dealer_hard@male@exit", "exit", 2.0f, 2.0f, 3000, 16+32, 1.0f, false, false, false);
            
            await BaseScript.Delay(3000);
            TaskPlayAnim(Game.PlayerPed.Handle, "amb@world_human_drug_dealer_hard@male@base", "base", 2.0f, 2.0f, -1, 16+32+1, 1.0f, false, false, false);
            HitsRemaining -= 1;
            HealthService.HighTHC += THC;
            HealthService.HighOtr += GenHigh;
            LastDrugTime = CharacterService.Timestamp();
            if (HitsRemaining <= 0)
            {
                ClearPedTasks(Game.PlayerPed.Handle);
                InventoryService.RemoveItem(this);
                return;
            }
        }

        protected override async void OnEquip()
        {
            await Utility.AssetLoader.LoadAnimDict("amb@world_human_drug_dealer_hard@male@base");
            TaskPlayAnim(Game.PlayerPed.Handle, "amb@world_human_drug_dealer_hard@male@base", "base", 2.0f, 2.0f, -1, 16+32+1, 1.0f, false, false, false);
        }

        protected override void OnUnEquip()
        {
            ClearPedTasks(Game.PlayerPed.Handle);
        }
    }
    [Equipable("Weed", "/assets/inventory/drug_pot.svg")]
    public class GD_WeedEquipable : DrugEquipable
    {
        protected override float THC => 10f;

        protected override float GenHigh => 0f;

        protected override int Hits => 10;
    }
    [Equipable("Cocaine", "/assets/inventory/drug_coke.svg")]
    public class GD_CocaineEquipable : DrugEquipable
    {
        protected override float THC => 00f;

        protected override float GenHigh => 110f;

        protected override int Hits => 1;
    }
    [Equipable("Crack", "/assets/inventory/drug_crack.svg")]
    public class GD_CrackEquipable : DrugEquipable
    {
        protected override float THC => 00f;

        protected override float GenHigh => 60f;

        protected override int Hits => 3;
    }
    [Equipable("Meth", "/assets/inventory/drug_meth.svg")]
    public class GD_MethEquipable : DrugEquipable
    {
        protected override float THC => 00f;

        protected override float GenHigh => 100f;


        protected override int Hits => 3;
    }
}
