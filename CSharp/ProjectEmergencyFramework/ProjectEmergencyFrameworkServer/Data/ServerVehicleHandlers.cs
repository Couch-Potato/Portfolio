using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkServer.Data
{

    public static class ServerVehicleHandlers
    {
        [Queryable("GET_VEHICLES")]
        public static void GetVehicles(Query q, object i, Player px)
        {
            var personId = (string)i;
            q.Reply(PlayerDataService.GetVehiclesForCharacter(personId));
        }

        [Queryable("GET_VEHICLE")]
        public static void GetVehicle(Query q, object i, Player px)
        {
            var vehicleId = (string)i;
            q.Reply(PlayerDataService.GetVehicle(vehicleId));
        }

        [Queryable("GET_ORG_VEHICLES")]
        public static void GetOrgVehicles(Query q, object i, Player px)
        {
            var orgId = (string)i;
            var org = OrganizationDataService.GetOrganization(orgId);
            q.Reply(PlayerDataService.GetVehiclesForCharacter(org.Id));
        }

        [Queryable("VEH_ADD")]
        public static void AddVehicle(Query q, object i, Player px)
        {
            var vehicle_2 = (dynamic)i;

            var vehicle = Utility.CrappyWorkarounds.ShittyFiveMDynamicToConcrete<ProjectEmergencyFrameworkShared.Data.Model.Vehicle>(vehicle_2);

            List<ProjectEmergencyFrameworkShared.Data.Model.InventoryItem> vehList = new List<ProjectEmergencyFrameworkShared.Data.Model.InventoryItem>();

            for (int ix = 0; ix < 15; ix++)
                vehList.Add(new ProjectEmergencyFrameworkShared.Data.Model.InventoryItem());

            var cont = PlayerDataService.CreateContainer(new ProjectEmergencyFrameworkShared.Data.Model.Container()
            {
                MaxItems = 15,
                Name = vehicle.Make + " " + vehicle.Model,
                Type = "VEHICLE_TRUNK",
                Inventory = vehList
            });

            vehicle.Container = cont.Id;
            
            PlayerDataService.CreateVehicle(vehicle);
            q.Reply(true);
        }
    }
}
