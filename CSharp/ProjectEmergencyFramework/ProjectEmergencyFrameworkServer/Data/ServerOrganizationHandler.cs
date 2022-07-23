using CitizenFX.Core;
using Newtonsoft.Json;
using ProjectEmergencyFrameworkServer.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkServer.Data
{
    public static class ServerOrganizationHandler
    {
        [Queryable("CAN_ACCESS_ORG")]
        public static void GetCanOrg(Query q, object i, Player px)
        {
            var a = (dynamic)i;
            var request = CrappyWorkarounds.ShittyFiveMDynamicToConcrete<ProjectEmergencyFrameworkShared.Data.Model.OrgRequest>(a);
            
            if (PlayerDataService.HasActiveHoldOfType(px.Identifiers["discord"], ProjectEmergencyFrameworkShared.Data.Model.HoldType.NoJob))
            {
                q.Reply(false);
                return;
            }

            ProjectEmergencyFrameworkShared.Data.Model.Organization org = OrganizationDataService.GetOrganization(request.OrgRequestId);

            if (org == null)
            {
                q.Reply(false);
                return;
            }

            if (org.OrgType == "POLICE")
            {
                if (PlayerDataService.HasActiveHoldOfType(px.Identifiers["discord"], ProjectEmergencyFrameworkShared.Data.Model.HoldType.NoLawEnforcement))
                {
                    q.Reply(false);
                    return;
                }
            }

            if (org.OrgType == "FIRE")
            {
                if (PlayerDataService.HasActiveHoldOfType(px.Identifiers["discord"], ProjectEmergencyFrameworkShared.Data.Model.HoldType.NoFire))
                {
                    q.Reply(false);
                    return;
                }
            }

            var data = OrganizationDataService.IsCharacterMemberOf(request.CurCharId, request.OrgRequestId);
            q.Reply(data);
        }

        [Queryable("GET_ORG_VEH")]
        public static void GetOrgVehicles(Query q, object i, Player px)
        {
            var request = (string)i;
            q.Reply(PlayerDataService.GetVehiclesForCharacter(request));
        }

        [Queryable("GET_ORG")]
        public static void GetOrg(Query q, object i, Player px)
        {
            var request = (string)i;
            q.Reply(OrganizationDataService.GetOrganization(request));
        }

        [Queryable("GET_ORG_VEH_PLATE")]
        public static void GetOrgPlate(Query q, object i, Player px)
        {
            var a = (dynamic)i;
            var request = CrappyWorkarounds.ShittyFiveMDynamicToConcrete<ProjectEmergencyFrameworkShared.Data.Model.OrgRequest>(a);

            var org = OrganizationDataService.GetOrganization(request.OrgRequestId);

            foreach (var member in org.Members)
            {
                if (member.CharacterId == request.CurCharId)
                {
                    q.Reply(org.Abbrev + member.ServiceNum);
                    return;
                }
                
            }
        }
    }
}
