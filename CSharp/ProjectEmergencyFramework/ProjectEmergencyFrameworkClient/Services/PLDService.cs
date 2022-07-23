using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Interfaces.UI;
using ProjectEmergencyFrameworkClient.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
namespace ProjectEmergencyFrameworkClient.Services
{
    public static class PLDService
    {
        static PLD pld = null; 
        [ExecuteAt(ExecutionStage.OnResourceStart)]
        public static void StartPLDService()
        {
            pld = new PLD();
            pld.Show();
        }

        [ExecuteAt(ExecutionStage.Tick)]
        public static void PLDTick()
        {
            
            uint streetName = 0;
            uint crossingStreet = 0;
            GetStreetNameAtCoord(Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, 0, ref streetName, ref crossingStreet);
            pld.location = $"{GetStreetNameFromHashKey(streetName)} {Utility.Zones.GetZoneFullName(GetNameOfZone(Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, 0))}".ToUpper();
            pld.postal = Utility.PostalCodes.GetNearestPostalToPlayer();
            var heading = Game.PlayerPed.Heading;
            string dir = "N";
            dir = isInRange(heading, 0, 45) ? "N" : isInRange(heading, 90, 45) ? "E" : isInRange(heading, 180, 45) ? "S" : isInRange(heading, 270, 45) ? "W" : isInRange(heading, 360, 45) ? "N" : "N";
            pld.direction = dir;
            pld.UpdateAsync();
        }

        public static void TogglePLDLocation(bool top)
        {
            pld.onTop = top;
            pld.UpdateAsync();
        }
        private static bool isInRange(float heading, float target, float tolerance)
        {
            return heading > target - tolerance && heading < target + tolerance;
        }
    }
}
