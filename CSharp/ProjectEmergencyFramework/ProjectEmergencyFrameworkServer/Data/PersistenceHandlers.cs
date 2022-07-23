using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkServer.Data
{
    public static class PersistenceHandlers
    {
        [Queryable("INSERT_PLACEABLE")]
        public static void InsertPlaceable(Query q, object i, Player px)
        {
            var d = (dynamic)i;
            string id = PersistenceService.InsertPersistentItem(
                PersistenceService.DynamicToMVector(d.position),
                PersistenceService.DynamicToMVector(d.rotation),
                d.owner,
                d.universe,
                d.transportString,
                d.propName
            );
            d.id = id;
            QueryService.QueryConcrete<bool>(-1, "NEW_PLACEABLE", d);
            q.Reply(true);
        }

        [Queryable("GET_PLACEABLE")]
        public static void GetPlaceable(Query q, object i, Player px)
        {
            var universe = (string)i;
            q.Reply(PersistenceService.GetPersistentItemsInUniverse(universe));
        }

    }
}
