using ProjectEmergencyFrameworkClient.Interfaces.UI;
using ProjectEmergencyFrameworkClient.Utility;
using ProjectEmergencyFrameworkShared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
namespace ProjectEmergencyFrameworkClient.Services
{
    public static class DispatchService
    {
        public static MDT MDT;
        public static bool IsMDTAble = false;
        public static bool IsAvailable = false;
        public static bool IsAttachedToCall = false;
        public static string Callsign = "";
        public static DispatchCallAttachment CallAttachment { get; private set; } = new DispatchCallAttachment();
        public static event EventHandler<DispatchCallAttachment> CallAttachmentChanged;
        public static void SetCallAttachment(DispatchCallAttachment a)
        {
            CallAttachment = a;
            CallAttachmentChanged?.Invoke(null, a);
        }

        public static async Task ToggleServiceStatus()
        {
            await QueryService.QueryConcrete<bool>("TOGGLE_SERVICE_STATUS", new
            {
                status = !IsAvailable,
                agency = Services.OrganizationService.ConnectedOrganization.Abbrev,
                agencyCallable = Services.OrganizationService.ConnectedOrganization.CallableId,
                callSign = Callsign,
                supervisor = false,
                agencyType = OrganizationService.ConnectedOrganization.OrgType
            });
            if (IsAttachedToCall)
            {
                DetachFromCall();
            }
            IsAvailable = !IsAvailable;
        }

        public static void DetachFromCall()
        {
            if (IsWaypointActive())
                SetWaypointOff();
            QueryService.QueryConcrete<bool>("DETACH_CALL", new
            {
                call = CallAttachment.CallNumber
            });
            IsAttachedToCall = false;
        }
        public static void ClearEntireCall()
        {
            if (IsWaypointActive())
                SetWaypointOff();
            QueryService.QueryConcrete<bool>("CLEAR_CALL", new
            {
                call = CallAttachment.CallNumber
            });
        }
        public static void SetGPSWaypoint()
        {
            if (IsAttachedToCall)
            {
                var pCoords = Utility.PostalCodes.GetCoordsOfPostal(CallAttachment.CallPostal.ToString());
                SetNewWaypoint(pCoords.X, pCoords.Y);
                SetUseWaypointAsDestination(true);
            }
            // DO GPS WAYPOINT
        }

        public static void DemandBackup()
        {
            QueryService.QueryConcrete<bool>("DEMAND_BACKUP_AT_LOCATION", new {
                postal = PostalCodes.GetNearestPostalToPlayer()
            });
        }
        public static void DemandEMS()
        {
            QueryService.QueryConcrete<bool>("DEMAND_EMS_AT_LOCATION", new
            {
                postal = PostalCodes.GetNearestPostalToPlayer()
            });
        }

        public static void CreateTrafficStopCall()
        {
            QueryService.QueryConcrete<bool>("ATTACH_TRAFFIC", new {
                postal = PostalCodes.GetNearestPostalToPlayer()
            });
        }

        public static void CreateCall()
        {
            QueryService.QueryConcrete<bool>("ATTACH_NEW_CALL", new {
                postal = PostalCodes.GetNearestPostalToPlayer()
            });
        }

        public static void PanicButton()
        {
            QueryService.QueryConcrete<bool>("PANIC_BUTTON", new
            {
                postal = PostalCodes.GetNearestPostalToPlayer()
            });
        }


        [Queryable("CLIENT_CALL_ATTACH")]
        public static void HandleCallAssignment(Query q, object valu2e)
        {
            IsAttachedToCall = true;
            dynamic value = Utility.CrappyWorkarounds.JSONDynamicToExpando(valu2e);
            SetCallAttachment(new DispatchCallAttachment()
            {
                CallNotes=value.CallNotes,
                CallNumber=value.CallNumber,
                CallPostal= (int)value.CallPostal,
                CallSource=value.CallSource,
                CallType=value.CallType
            });
            q.Reply(false);

        }

        [Queryable("CLIENT_CALL_UPDATE")]
        public static void HandleCallUpdate(Query q, dynamic valu2e)
        {
            dynamic value = Utility.CrappyWorkarounds.JSONDynamicToExpando(valu2e);
            SetCallAttachment(new DispatchCallAttachment()
            {
                CallNotes = value.CallNotes,
                CallNumber = value.CallNumber,
                CallPostal = (int)value.CallPostal,
                CallSource = value.CallSource,
                CallType = value.CallType
            });
            q.Reply(false);

        }

        [Queryable("CLIENT_CALL_FORCE_DETACH")]
        public static void HandleCallDetach(Query q, object value)
        {
            IsAttachedToCall = false;
            SetCallAttachment(null);
            q.Reply(false);

        }

        [ExecuteAt(ExecutionStage.Initialized)]
        public static void SetupDispatchService()
        {
            Services.OrganizationService.DutyStatusChanged += (bool isDuty) =>
            {
                if (isDuty)
                {
                    if (OrganizationService.ConnectedOrganization.OrgType == "POLICE" || OrganizationService.ConnectedOrganization.OrgType == "FIRE")
                    {
                        MDT = new MDT();
                        IsMDTAble = true;
                    }
                        
                }else
                { 
                    MDT = null;
                    IsMDTAble = false;
                }
            };
        }
    }
  
}
