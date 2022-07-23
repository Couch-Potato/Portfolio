using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;

namespace ProjectEmergencyFrameworkClient.Interact
{
    [Interactable("door_0", "G", "Lock Car", true)]
    public class DoorLockInteract : LookatRadiusInteractable
    {
        public DoorLockInteract()
        {
            Radius = 3f;
            Tolerance = 30f / 2;
            RequireControlKeyDown = true;
        }
        public override async Task<bool> CanShow()
        {
            /* if (Entity.State["vehicleOwner"] != null)
             {
                 if (!Entity.State["vehicleOwner"] == Services.CharacterService.CurrentCharacter.Id){
                     return false;
                 }
             }else
             {
                 return false;
             }
 */
            if (GetVehicleDoorLockStatus(Entity.Handle) != (int)VehicleLockStatus.Unlocked)
            {
                return false;
            }

            return await base.CanShow();
        }
        protected override void OnInteract()
        {
            SetVehicleDoorsLocked(Entity.Handle, (int)VehicleLockStatus.CanBeBrokenInto);
        }
    }
    [Interactable("door_1", "G", "Unlock Car", true)]
    public class DoorUnLockInteract : LookatRadiusInteractable
    {
        public DoorUnLockInteract()
        {
            Radius = 3f;
            Tolerance = 30f / 2;
            RequireControlKeyDown = true;
        }
        public override async Task<bool> CanShow()
        {
            /* if (Entity.State["vehicleOwner"] != null)
             {
                 if (!Entity.State["vehicleOwner"] == Services.CharacterService.CurrentCharacter.Id)
                 {
                     return false;
                 }
             }
             else
             {
                 return false;
             }*/

            if (GetVehicleDoorLockStatus(Entity.Handle) != (int)VehicleLockStatus.CanBeBrokenInto)
            {
                return false;
            }
            return await base.CanShow();
        }
        protected override void OnInteract()
        {
            SetVehicleDoorsLocked(Entity.Handle, (int)VehicleLockStatus.Unlocked);
        }
    }
}
