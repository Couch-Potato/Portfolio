using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Interact;
using ProjectEmergencyFrameworkClient.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
namespace ProjectEmergencyFrameworkClient.Services
{
    public static class BarterService
    {
        private static Ped _GetClosestPed()
        {
            var peds = World.GetAllPeds();
            Ped closest = new Ped(-1);
            float closestDistance = float.MaxValue;
            foreach (var x in peds)
            {
                if (x.Position.DistanceToSquared(Game.PlayerPed.Position) < closestDistance && x.Handle != Game.PlayerPed.Handle)
                {
                    closest = x;
                    closestDistance = x.Position.DistanceToSquared(Game.PlayerPed.Position);
                }
            }
            return closest;
        }

        private static int closestHandle = -1;
        private static IInteractable usedInteract;

        [ExecuteAt(ExecutionStage.Tick)]
        public static void BarterTick()
        {
            float time = GetClockHours();
            time += GetClockMinutes() / 60;
            foreach (var region in ConfigurationService.CurrentConfiguration.PedBarterRegions)
            {
                // Time Calculations
                if (!region.AllowedBarterTimeRange.AllDay)
                {
                    bool isTimeReversed = region.AllowedBarterTimeRange.To < region.AllowedBarterTimeRange.From;
                    float to = region.AllowedBarterTimeRange.To;
                    float from = region.AllowedBarterTimeRange.From;
                    if (isTimeReversed)
                    {
                        if (!(time > from || time < to))
                        {
                            continue;
                        }
                    }else
                    {
                        if (!(time > from && time < to))
                        {
                            continue;
                        }
                    }
                }

                // Region calculations

                float dist = Vector3.Distance(ConfigurationService._loc_to_vector_3(region.Location), Game.PlayerPed.Position);
                if (dist > region.Radius) continue;

                // See if our closest ped is a barter ped 

                Ped closest = _GetClosestPed();
                bool closestIsValid = false;
                if (closest == null) continue;
                foreach (var ped in region.BarterPeds)
                {
                    if (closest.Model.Hash == GetHashKey(ped.ModelName))
                    {
                        closestIsValid = true;
                    }
                }
                if (!closestIsValid) continue;
                

                // Attach the interact if it is valid. If the ped changed then clear the interact.


                if (closestHandle != closest.Handle)
                {
                    if (usedInteract != null)
                    {
                        InteractService.TerminateInteractable(usedInteract);
                    }
                }

                usedInteract = InteractService.ConstructInteract("BARTER_INTERACT", closest, new
                {
                    regionData=region
                });
                return;
            }
        }
    }
}
