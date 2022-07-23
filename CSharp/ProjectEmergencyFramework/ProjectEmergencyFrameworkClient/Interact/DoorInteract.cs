using ProjectEmergencyFrameworkClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
namespace ProjectEmergencyFrameworkClient.Interact
{
    [Interactable("door@unlock", "Z", "Unlock Door", true, GenericInteractAttachment.Door)]
    public class DoorInteract : LookatRadiusInteractable
    {
        public DoorInteract()
        {
            this.RequireControlKeyDown = true;
            this.Radius = 1.5f;
            Offset = new CitizenFX.Core.Vector3(-0.8f, 0f, 0);
        }
     
        public override async Task<bool> CanShow()
        {
            
            bool canShow = true;
            if (AttachedDoor.DoorLockData.IsLockedToSet)
            {
                if (!OrganizationService.IsOnDuty) return false;
                if (AttachedDoor.DoorLockData.LockedType != null)
                {
                    if (OrganizationService.ConnectedOrganization.OrgType != AttachedDoor.DoorLockData.LockedType)
                        return false;
                    
                }
                if (AttachedDoor.DoorLockData.LockedOrganization != null)
                {
                    if (OrganizationService.ConnectedOrganization.CallableId != AttachedDoor.DoorLockData.LockedOrganization)
                        return false;

                }
                
            }

            return await base.CanShow() && DoorSystemGetDoorState(AttachedDoor.DoorId) == 1 && canShow;
        }
        protected override async void OnInteract()
        {
            await Utility.AssetLoader.LoadDoorSystemPhysics((int)AttachedDoor.DoorId);
            DoorSystemSetDoorState(AttachedDoor.DoorId, 0, true, true);
        }
    }

    [Interactable("door@lock", "Z", "Lock Door", true, GenericInteractAttachment.Door)]
    public class DoorLocksInteract : LookatRadiusInteractable
    {
        public DoorLocksInteract()
        {
            this.RequireControlKeyDown = true;
            this.Radius = 1.5f;
            Offset = new CitizenFX.Core.Vector3(-0.8f, 0f, 0);
        }
        public override async Task<bool> CanShow()
        {
            
            bool canShow = true;
            if (AttachedDoor.DoorLockData.IsLockedToSet)
            {
                if (!OrganizationService.IsOnDuty) return false;
                if (AttachedDoor.DoorLockData.LockedType != null)
                {
                    if (OrganizationService.ConnectedOrganization.OrgType != AttachedDoor.DoorLockData.LockedType)
                        return false;

                }
                if (AttachedDoor.DoorLockData.LockedOrganization != null)
                {
                    if (OrganizationService.ConnectedOrganization.CallableId != AttachedDoor.DoorLockData.LockedOrganization)
                        return false;

                }
            }
            return await base.CanShow() && DoorSystemGetDoorState(AttachedDoor.DoorId) == 0 && canShow;
        }
        protected override async void OnInteract()
        {
            await Utility.AssetLoader.LoadDoorSystemPhysics((int)AttachedDoor.DoorId);
            DoorSystemSetDoorState(AttachedDoor.DoorId, 1, true, true);
        }
    }
}
