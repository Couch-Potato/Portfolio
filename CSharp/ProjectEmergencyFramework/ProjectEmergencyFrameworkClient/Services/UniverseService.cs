using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Utility;
using ProjectEmergencyFrameworkShared.Configuration.Schema;
using ProjectEmergencyFrameworkShared.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
namespace ProjectEmergencyFrameworkClient.Services
{
    public class UniverseService
    {
        public static string CURRENT_UNIVERSE = "main";
        public static event Action UniverseChanged;
        public static UniverseInstance CurrentUniverseInstance;
        public static async void TeleportToUniverse(string univId)
        {
            DoScreenFadeOut(500);
            var universe = await GetUniverse(univId);
            CurrentUniverseInstance = universe;
            var unv = ConfigurationService.GetArchetype<UniverseArchetype>(universe.UniverseType);
            CURRENT_UNIVERSE = "UNV_" + universe.Id;
            UniverseChanged?.Invoke();

            await BaseScript.Delay(500);
            if (!unv.IsInterior)
            {
                await RoutingService.RouterSetNamedBucket("UNV_" + universe.Id);
                Game.PlayerPed.Position = ConfigurationService._loc_to_vector_3(unv.SpawnCoords);
                DoScreenFadeIn(500);

            }
            else
            {
                throw new NotImplementedException("Interior universes have not been implemented yet. Sorry charlie.");
            }
        }
        public static async Task<UniverseInstance> GetUniverse(string id)
        {
            return await QueryService.QueryConcrete<UniverseInstance>("GET_UNIVERSE", id);
        }
        public static async void TeleportBackToMain(Vector3 position)
        {
            CURRENT_UNIVERSE = "main";
            DoScreenFadeOut(500);
            UniverseChanged?.Invoke();

            await BaseScript.Delay(500);

            await RoutingService.RouterSetMainBucket();
            Game.PlayerPed.Position = position;
            DoScreenFadeIn(500);

        }

    }
}
