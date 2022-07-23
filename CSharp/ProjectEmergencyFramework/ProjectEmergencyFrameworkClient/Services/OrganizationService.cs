using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Utility;
using ProjectEmergencyFrameworkShared.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Services
{
    public static class OrganizationService
    {
        public static Organization ConnectedOrganization;
        public static bool IsOnDuty = false;

        public static event Action<Organization> OrganizationChanged;
        public static event Action<bool> DutyStatusChanged;
        public static Interfaces.UI.Bodycam BodyCamUI;
        private static Dictionary<string, bool> AccessCache = new Dictionary<string, bool>();

        [Queryable("CHARGING_DOCS")]
        public static void GetChargingDocs(Query q, object value)
        {
            InventoryService.AddItem("CHARGING DOCUMENTS", "/assets/inventory/receipt_charge.svg", 1, false, new
            {
                crimCase = Utility.CrappyWorkarounds.ShittyFiveMDynamicToConcrete<string>(value),
                nonTransferrable = true
            });
            q.Reply(true);
        }

        [Queryable("DEATH_DOCS")]
        public static void GetDetailDocs(Query q, object value)
        {
            InventoryService.AddItem("DEATH CERTIFICATE", "", 1, false, new
            {
                record = Utility.CrappyWorkarounds.ShittyFiveMDynamicToConcrete<string>(value),
                nonTransferrable = true
            });
            q.Reply(true);
        }

        [Queryable("HEALTH_DOCS")]
        public static void GetMedicalBill(Query q, object value)
        {
            InventoryService.AddItem("MEDICAL BILL", "", 1, false, new
            {
                record = Utility.CrappyWorkarounds.ShittyFiveMDynamicToConcrete<string>(value),
                nonTransferrable = true
            });
            q.Reply(true);
        }

        [Queryable("FINE_PAPER")]
        public static void GetFineTearsheet(Query q, object value)
        {
            var pbd = DateTime.Now;
            pbd.AddDays(7);
            float num = Utility.CrappyWorkarounds.ShittyFiveMDynamicToConcrete<float>(value);
            InventoryService.AddItem("CITATION - " + (num).ToString(), "", 1, false, new
            {
                fineAmt= num,
                nonTransferrable = true,
                desc =$"${(num).ToString()} CITATION. PAY BY: {pbd.ToString("MM/dd/yyyy")}. ISSUED BY: {CharacterService.CurrentCharacter.FirstName.Substring(0, 1)}. {CharacterService.CurrentCharacter.LastName}",
                tags="CITATION,"
            });
            q.Reply(true);
        }


        public static async void GoOnDuty(string orgId, bool showUniform = false)
        {
            var canAccess = await QueryService.QueryConcrete<bool>("CAN_ACCESS_ORG", new OrgRequest()
            {
                OrgRequestId = orgId,
                CurCharId = CharacterService.CurrentCharacter.Id
            });

            if (!canAccess) return;

            var org = await QueryService.QueryConcrete<Organization>("GET_ORG", orgId);
            ConnectedOrganization = org;
            IsOnDuty = true;
            OrganizationChanged?.Invoke(org);
            DutyStatusChanged?.Invoke(IsOnDuty);
            if (showUniform)
            {
                Interfaces.InterfaceController.ShowInterface("locker", new
                {
                    organization = org.CallableId
                });
            }
            
            Game.PlayerPed.State["dutyOrganization"] = org.CallableId;
            Game.PlayerPed.State.Set("dutyOrganizationType", org.OrgType, true);
            VoiceService.RadioServiceInit();
        }
        public static void ClearCache()
        {
            AccessCache.Clear();
        }
        public static void GoOffDuty()
        {
            Game.PlayerPed.State["dutyOrganization"] = "";
            ConnectedOrganization = null;
            IsOnDuty = false;
            DutyStatusChanged?.Invoke(false);
        }
        public static async Task<bool> IsApartOf(string orgId)
        {
            if (!AccessCache.ContainsKey(orgId))
            {
                AccessCache.Add(orgId, false);
                var query = await QueryService.QueryConcrete<bool>("CAN_ACCESS_ORG", new OrgRequest()
                {
                    OrgRequestId = orgId,
                    CurCharId = CharacterService.CurrentCharacter.Id
                });
                if (!AccessCache.ContainsKey(orgId))
                    AccessCache.Add(orgId, query);
                else
                    AccessCache[orgId] = query;
                return query;
            }else
            {
                return AccessCache[orgId];
            }
            
        }
    }
}
