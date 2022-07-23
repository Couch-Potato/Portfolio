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
    [Equipable("_placeable", "")]
    public class PlaceableEquipable : Equipable
    {
        string Placeable;
        InstancedProp prop;
        float yaw = 0f;
        public override bool DisabledPrimary => true;
        protected override void OnInstanced()
        {
            Placeable = Modifiers.placeable;
            _name = this.Modifiers.name;
            _icon = this.Modifiers.icon;
        }
        PETask EquipUpdate;
        protected override void OnEquip()
        {
            IsEquipped = true;
            var propx = ConfigurationService.ComplexProps[Placeable];
            float offY = propx.Size.Y / 2;

            Vector3 offset = GetOffsetFromEntityInWorldCoords(Game.PlayerPed.Handle, 0f, offY + 1f, 0f);

            prop = propx.GetInstanced(offset, new Vector3(yaw, 0, 0));
            prop.Alpha = .5f;
            EquipUpdate = TaskService.InvokeUntilExpire(async () =>
            {
                prop.Position = GetOffsetFromEntityInWorldCoords(Game.PlayerPed.Handle, 0f, offY + 1f, 0f);
                prop.Rotation = new Vector3(yaw, 0, 0);
                prop.Update();
                return false;
            });

        }
        bool IsEquipped = false;
        protected override void OnUnEquip()
        {
            if (!IsEquipped) return;
            IsEquipped = false;
            TaskService.ForceExpireTask(EquipUpdate);
            prop.Delete();
            base.OnUnEquip();
        }
        protected override void OnPrimaryUp()
        {
            var propx = ConfigurationService.ComplexProps[Placeable];
            float offY = propx.Size.Y / 2;
            Vector3 offset = GetOffsetFromEntityInWorldCoords(Game.PlayerPed.Handle, 0f, offY + 1f, 0f);
            OnUnEquip();
            InventoryService.RemoveItem(this);
            PlaceableService.CreatePersistentPlaceable(offset, new Vector3(yaw, 0, 0), new { }, Placeable);
            base.OnPrimaryUp();
        }
    }
}
