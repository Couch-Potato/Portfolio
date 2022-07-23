using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Services;
using ProjectEmergencyFrameworkClient.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;

namespace ProjectEmergencyFrameworkClient.Interact
{
    public abstract class Interactable : IInteractable
    {
        private dynamic _props;
        private Vector3 _position;
        private int _chunk;
        private Entity _entity;
        public bool IsEntityAttached = false;
        public bool IsDoorAttached = false;
        public Vector3 Offset = new Vector3(0,0,0);
        public dynamic Properties { get => _props; set => _props = value; }
        public Vector3 Position { get
            {
                if (IsDoorAttached) return DoorAttached.LinkedEntity.GetOffsetPosition(Offset);
                if (IsEntityAttached) return _entity.GetOffsetPosition(Offset);
                return _position+ Offset;
            } set => _position = value; }

        public Entity Entity { get=>_entity; set{
                _entity = value;
                IsEntityAttached = true;
                
            }
        }
        private DoorObject DoorAttached;
        public bool RequireControlKeyDown = false;
        public int ChunkId { get => _chunk; set => _chunk = value; }
        public ChunkingMethod ChunkingMethod => ChunkingMethod.PositionCalculated;

        public DoorObject AttachedDoor { get => DoorAttached; set {
                IsDoorAttached = true;
                DoorAttached = value;
            } }

        public virtual async Task<bool> CanShow()
        {
            if (RequireControlKeyDown)
            {
                if (!IsControlPressed(0, 19) && !IsDisabledControlPressed(0, 19)) return false;
            }

            if (!CrappyWorkarounds.HasProperty(Properties, "organizationLocked")) return true;
           
            if (Properties.organizationLocked)
            {
                if (Properties.requireOnTeam)
                {
                    if (OrganizationService.IsOnDuty)
                    {
                        return Properties.organization == OrganizationService.ConnectedOrganization.CallableId;
                    }
                    else
                    {
                        return false;
                    }
                }

                if (Properties.onlyShowOffDuty && CharacterService.CharacterSelected)
                {
                    if (!OrganizationService.IsOnDuty)
                    {
                        var isApart = await OrganizationService.IsApartOf(Properties.organization);
                        
                        return isApart;
                    }
                }
                return false;
            }
            return true;
            
        }

        public void Interact()
        {
            OnInteract();
        }

        protected abstract void OnInteract();
    }
}
