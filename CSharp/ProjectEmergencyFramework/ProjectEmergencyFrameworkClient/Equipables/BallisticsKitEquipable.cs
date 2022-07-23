using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Interact;
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
    [Equipable("Ballistics Kit", "/assets/inventory/case_open.svg")]
    public class BallisticsKitEquipable : Equipable
    {
        IInteractable GSRTestInteract;
        const uint weapon_briefcase_02 = 0x88C78EB7;
        bool scaninprogress=false;
        public override bool DisabledPrimary => true;

        protected override void OnEquip()
        {
            //GSRTestInteract = InteractService.RegisterInteractAsGeneric("ballistics@gsrTest", GenericInteractAttachment.Ped);
            GiveWeaponToPed(Game.PlayerPed.Handle, weapon_briefcase_02, 0, false, true);
            SetCurrentPedWeapon(Game.PlayerPed.Handle, weapon_briefcase_02, true);
        }

        protected override void OnUnEquip()
        {
            //InteractService.TerminateGeneric(GSRTestInteract, GenericInteractAttachment.Ped);
            RemoveWeaponFromPed(Game.PlayerPed.Handle, weapon_briefcase_02);
            foreach (var gunshot in gunshots)
            {
                InteractService.TerminateInteractAtPosition("ballistics@shellCasing", gunshot.position);
            }
        }

        List<Gunshot> gunshots = new List<Gunshot>();


        protected override async void OnPrimaryUp()
        {
            if (scaninprogress) return;
            double tStart = CharacterService.TimestampF();

            foreach (var gunshot in gunshots)
            {
                InteractService.TerminateInteractAtPosition("ballistics@shellCasing", gunshot.position);
            }

            gunshots = await QueryService.QueryList<Gunshot>("GET_GUNSHOTS", true);

            foreach (var gunshot in gunshots)
            {
                gunshot.radius = Game.PlayerPed.Position.DistanceToSquared(gunshot.position);
            }

            double lastRadius = 0.0;
            scaninprogress = true;
            
            TaskService.InvokeUntilExpire(async () =>
            {
                double r = (CharacterService.TimestampF()-tStart) * (10.0 / 1.0);
                if (r > 10) {
                    scaninprogress = false;
                    return true;
                } 

                foreach (var gunshot in gunshots)
                {
                    if (gunshot.radius > lastRadius && gunshot.radius <= r)
                    {
                      
                        InteractService.ConstructInteract("ballistics@shellCasing", gunshot.position, new
                        {
                            name = gunshot.name,
                            firingPattern=gunshot.firingPattern
                        });
                    }
                }

                lastRadius = r;
                DrawMarker(25, Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, Game.PlayerPed.Position.Z, 0, 0, 0, 0, 0, 0, (float)r, (float)r, 1, 255, 255, 0, 255, false, true, 2, false, null, null, false);
                  
                return false;
            });
            //base.OnPrimaryUp();
        }
    }
    public class Gunshot
    {
        public Vector3 position { get; set; }
        public string name { get; set; }
        public float radius { get; set; }

        public string serialNumber { get; set; }
        public string firingPattern { get; set; }

    }
}
