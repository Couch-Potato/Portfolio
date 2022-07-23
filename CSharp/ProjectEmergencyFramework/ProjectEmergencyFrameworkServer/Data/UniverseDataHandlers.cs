using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkServer.Data
{
    public static class UniverseDataHandlers
    {
        [Queryable("GET_CHARACTER_PRIMARY_APARTMENT")]
        public static void GET_CHARACTER_PRIMARY_APARTMENT(Query q, object i, Player px)
        {
            q.Reply(UniverseDataService.GetPrimaryApartment((string)i));
        }
        [Queryable("GET_APARTMENT")]
        public static void GET_APARTMENT(Query q, object i, Player px)
        {
            q.Reply(UniverseDataService.GetApartment((string)i));
        }
        [Queryable("CREATE_APARTMENT")]
        public static void CREATE_APARTMENT(Query q, object i, Player px)
        {
            dynamic ni = i;
            UniverseDataService.CreateApartment(ni.config, ni.universe, ni.owner);
            q.Reply(true);
        }
        [Queryable("GET_UNIVERSE")]
        public static void GET_UNIVERSE(Query q, object i, Player px)
        {
            q.Reply(UniverseDataService.GetUniverse((string)i));
        }
        [Queryable("GET_UNIVERSE")]
        public static void GET_AVAIL_APARTMENTS(Query q, object i, Player px)
        {
            q.Reply(UniverseDataService.GetAvailApartments(ServerCharacterHandlers.CurrentCharacters[px]));
        }
    }
}
