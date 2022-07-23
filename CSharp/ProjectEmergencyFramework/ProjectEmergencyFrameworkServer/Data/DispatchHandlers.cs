using CitizenFX.Core;
using Newtonsoft.Json;
using ProjectEmergencyFrameworkShared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
namespace ProjectEmergencyFrameworkServer.Data
{
    public static class DispatchHandlers
    {
        public static Dictionary<Player, DispatchMemberDisposition> UnitDisposition = new Dictionary<Player, DispatchMemberDisposition>();

        public static Dictionary<string, DispatchCallFull> DispatchCalls = new Dictionary<string, DispatchCallFull>();

        [Queryable("TOGGLE_SERVICE_STATUS")]
        public static void DispatchServiceStatus(Query q, object i, Player px)
        {
            var request = (dynamic)i;

            if (UnitDisposition.ContainsKey(px))
            {
                UnitDisposition[px].isSupervisor = request.supervisor;
                UnitDisposition[px].isAttachedToCall = false;
                UnitDisposition[px].agency = request.agency;
                UnitDisposition[px].agencySign = request.agencyCallable;
                UnitDisposition[px].agencyType = request.agencyType;
                UnitDisposition[px].player = px;
                UnitDisposition[px].callAttached = null;
                UnitDisposition[px].isAvail = request.status;
                UnitDisposition[px].callSign = request.callSign;
            } else
            {
                UnitDisposition.Add(px, new DispatchMemberDisposition()
                {
                    isSupervisor = request.supervisor,
                    isAttachedToCall = false,
                    isAvail = request.status,
                    agency = request.agency,
                    agencySign = request.agencyCallable,
                    callSign = request.callSign,
                    agencyType = request.agencyType,
                    player=px
                });
            }
            q.Reply(true);
        }

        [Queryable("ATTACH_TRAFFIC")]
        public static void DispatchAttachTraffic(Query q, object i, Player px)
        {
            var time = DateTime.Now.ToString("HH:mm");
            var time2 = DateTime.Now.ToString("MM/dd/yy HH:mm:ss");
            var request = (dynamic)i;
            var call = CreateCall(new DispatchCallAttachment()
            {
                CallSource = "OFFICER INITIATED",
                CallNotes = $"{time2} - CALL CREATED FOR TRAFFIC STOP / POSITION UPDATE: {request.postal} / ",
                CallType = "TRAFFIC STOP",
                CallPostal = request.postal
            });
            DispatchOfficerToCall(call, px);
        }



        [Queryable("FILE_ARREST")]
        public static void FileArrest(Query q, object i, Player px)
        {
            var request = (dynamic)i;
            string arrest = CriminalDataService.FormatAndFileArrest(request);
            QueryService.Query<bool>(int.Parse(px.Handle), "CHARGING_DOCS", arrest);
            q.Reply(false);
        }

        [Queryable("GET_INCARCERATION")]
        public static void GetArrest(Query q, object i, Player px)
        {
            var request = (dynamic)i;
            var arrest = CriminalDataService.GetIncarcerationRecordFromArrest(request);
            q.Reply(arrest);
        }

        [Queryable("ENTER_PLEA")]
        public static void EnterPlea(Query q, object i, Player px)
        {
            var request = (dynamic)i;
            ProjectEmergencyFrameworkShared.Data.Model.Arrests arrest = CriminalDataService.GetArrest(request.arrest);

            arrest.Plea = request.plea;
            CriminalDataService.UpdateArrest(arrest);

            QueryService.QueryConcrete<bool>(int.Parse(px.Handle), "INCARCERATE", true);

            q.Reply(true);
        }

        [Queryable("GET_ARREST")]
        public static void GetArrestItem(Query q, object i, Player px)
        {
            var request = (dynamic)i;
            var arrest = CriminalDataService.GetArrest(request);
            q.Reply(arrest);
        }

        [Queryable("GET_CHARACTER_INCARCERATION")]
        public static void GetIncarcer(Query q, object i, Player px)
        {
            var request = (dynamic)i;
            ProjectEmergencyFrameworkShared.Data.Model.Character character = PlayerDataService.GetCharacter(request);
            ProjectEmergencyFrameworkShared.Data.Model.IncarcerationRecord record = CriminalDataService.GetIncarcerationRecord(character.IncarcerationRecord);


            q.Reply(record);
        }

        [Queryable("GET_CHARACTER_ARREST")]
        public static void GetCArrest(Query q, object i, Player px)
        {
            var request = (dynamic)i;
            ProjectEmergencyFrameworkShared.Data.Model.Character character = PlayerDataService.GetCharacter(request);
            ProjectEmergencyFrameworkShared.Data.Model.IncarcerationRecord record = CriminalDataService.GetIncarcerationRecord(character.IncarcerationRecord);
            ProjectEmergencyFrameworkShared.Data.Model.Arrests arrest = CriminalDataService.GetArrest(record.Arrest);

            q.Reply(arrest);
        }

        [Queryable("MARK_CHARACTER_FREE")]
        public static void MarkFree(Query q, object i, Player px)
        {
            var request = (dynamic)i;

            ProjectEmergencyFrameworkShared.Data.Model.Character character = PlayerDataService.GetCharacter(request);
            character.IsIncarcerated = false;
            character.IncarcerationRecord = null;

            PlayerDataService.UpdateCharacter(character);

            q.Reply(true);
        }


        [Queryable("FILE_DEATH")]
        public static void FileDeath(Query q, object i, Player px)
        {
            var request = (dynamic)i;
            string arrest = HealthDataService.FileDeathCertificate(request);
            QueryService.Query<bool>(int.Parse(px.Handle), "DEATH_DOCS", arrest);
            q.Reply(false);

        }

        [Queryable("MARK_ME_DEAD")]
        public static void MarkMeDead(Query q, object i, Player px)
        {
            var request = (dynamic)i;
            ProjectEmergencyFrameworkShared.Data.Model.Character chars = PlayerDataService.GetCharacterFromId(request);
            chars.IsDead = true;
            PlayerDataService.UpdateCharacter(chars);
            q.Reply(false);

        }

        [Queryable("POST_FINES")]
        public static void PostFines(Query q, object i, Player px)
        {
            var request = (dynamic)i;
            CriminalDataService.PostFines(request);
            q.Reply(true);
        }

        [Queryable("POST_BAIL")]
        public static void PostBail(Query q, object i, Player px)
        {
            var request = (dynamic)i;
            CriminalDataService.PostBail(request);
            q.Reply(true);
        }

        [Queryable("GET_FINES")]
        public static void GetFines(Query q, object i, Player px)
        {
            var request = (dynamic)i;
            q.Reply(CriminalDataService.GetFines(request));
        }

        [Queryable("MAKE_CITATION")]
        public static void MakeCitation(Query q, object i, Player px)
        {
            var request = (dynamic)i;
            float fineAmt = CriminalDataService.MakeCitation(request);
            q.Reply(false);
            QueryService.Query<bool>(int.Parse(px.Handle), "FINE_PAPER", fineAmt);
        }

        [Queryable("FILE_MED")]
        public static void FileMed(Query q, object i, Player px)
        {
            var request = (dynamic)i;
            string arrest = HealthDataService.FileHealthReport(request);
            QueryService.Query<bool>(int.Parse(px.Handle), "MED_DOCS", arrest);
            q.Reply(false);

        }

        [Queryable("ATTACH_NEW_CALL")]
        public static void DispatchAttachNew(Query q, object i, Player px)
        {
            var time = DateTime.Now.ToString("HH:mm");
            var time2 = DateTime.Now.ToString("MM/dd/yy HH:mm:ss");
            var request = (dynamic)i;
            var call = CreateCall(new DispatchCallAttachment()
            {
                CallSource = "OFFICER INITIATED",
                CallNotes = $"{time2} - CALL CREATED BY OFFICER / POSITION UPDATE: {request.postal} / ",
                CallType = "OFFCR CREATED CALL",
                CallPostal = request.postal
            });
            DispatchOfficerToCall(call, px);
        }

        [Queryable("CLEAR_CALL")]
        public static void DispatchClear(Query q, object i, Player px)
        {
            var request = (dynamic)i;
            var dx = UnitDisposition[px].callAttached.CallNumber;
            foreach (var unit in DispatchCalls[UnitDisposition[px].callAttached.CallNumber].UnitsAttached)
            {
                DetachOfficer(unit.player, UnitDisposition[px].callAttached.CallNumber, false);
                UnitDisposition[px].isAttachedToCall = false;
                UnitDisposition[px].callAttached = null;
               
            }
            DispatchCalls.Remove(dx);

        }

        [Queryable("DETACH_CALL")]
        public static void DispatchDetach(Query q, object i, Player px)
        {
            var request = (dynamic)i;
            DispatchCalls[UnitDisposition[px].callAttached.CallNumber].UnitsAttached.Remove(UnitDisposition[px]);
            DetachOfficer(px, UnitDisposition[px].callAttached.CallNumber, true);
            UnitDisposition[px].isAttachedToCall = false;
            UnitDisposition[px].callAttached = null;
            
            q.Reply(true);
        }

        [Queryable("PANIC_BUTTON")]
        public static void DispatchPanicButton(Query q, object i, Player px)
        {
            var request = (dynamic)i;
            var time = DateTime.Now.ToString("HH:mm");
            var time2 = DateTime.Now.ToString("MM/dd/yy HH:mm:ss");
            var call = CreateCall(new DispatchCallAttachment()
            {
                CallSource = "PANIC BUTTON",
                CallNotes = $"{time2} - PANIC BUTTON PRESSED / {UnitDisposition[px].callSign} REQUESTED ALL-CALL @ {time} / POSITION UPDATE: {request.postal} / ",
                CallType = "PANIC BUTTON",
                CallPostal = request.postal
            });
            DispatchOfficerToCall(call, px);
            var pos = GetEntityCoords(GetPlayerPed(px.Handle));
            for (int ix =0;ix<10;ix++)
            {
                DispatchClosestAvailableOfficer(request.postal, pos, call);
            }
        }

        public static void DetachOfficer(Player officer, string callId, bool doUpdate)
        {
            if (doUpdate)
            {
                var time = DateTime.Now.ToString("HH:mm");
                DispatchCalls[callId].DispatchCall.CallNotes += $"UNIT DETACHED: {UnitDisposition[officer].callSign} [{time}] / ";
                PushUpdateCall(callId);
            }
            QueryService.Query<bool>(int.Parse(officer.Handle), "CLIENT_CALL_FORCE_DETACH", true);
        }

        [Queryable("DEMAND_EMS_AT_LOCATION")]
        public static void DispatchEMS(Query q, object i, Player px)
        {
            var request = (dynamic)i;
            var time = DateTime.Now.ToString("HH:mm");
            var time2 = DateTime.Now.ToString("MM/dd/yy HH:mm:ss");
            if (!UnitDisposition.ContainsKey(px))
                return;

            if (UnitDisposition[px].isAttachedToCall)
            {
                var pos = GetEntityCoords(GetPlayerPed(px.Handle));
                if (request.postal != DispatchCalls[UnitDisposition[px].callAttached.CallNumber].DispatchCall.CallPostal)
                {
                    DispatchCalls[UnitDisposition[px].callAttached.CallNumber].DispatchCall.CallNotes += $"POSITION UPDATE: {request.postal} / ";
                }
                DispatchCalls[UnitDisposition[px].callAttached.CallNumber].DispatchCall.CallNotes += $"{UnitDisposition[px].callSign} REQUESTED EMS @ {time} / ";
                DispatchClosestAvailableEMS(request.postal, pos, UnitDisposition[px].callAttached.CallNumber);
                PushUpdateCall(UnitDisposition[px].callAttached.CallNumber);
            }
            else
            {

                var call = CreateCall(new DispatchCallAttachment()
                {
                    CallSource = "OFFICER",
                    CallNotes = $"{time2} - CALL CREATED FOR EMS REQUEST / {UnitDisposition[px].callSign} REQUESTED EMS @ {time} / POSITION UPDATE: {request.postal} / ",
                    CallType = "MEDICAL",
                    CallPostal = request.postal
                });
                DispatchOfficerToCall(call, px);
                var pos = GetEntityCoords(GetPlayerPed(px.Handle));
                DispatchClosestAvailableEMS(request.postal, pos, call);
            }

            q.Reply(true);
        }

        [Queryable("DEMAND_BACKUP_AT_LOCATION")]
        public static void DispatchBackup(Query q, object i, Player px)
        {
            var request = (dynamic)i;
            var time = DateTime.Now.ToString("HH:mm");
            var time2 = DateTime.Now.ToString("MM/dd/yy HH:mm:ss");
            if (!UnitDisposition.ContainsKey(px))
                 return;

            if (UnitDisposition[px].isAttachedToCall)
            {
                var pos = GetEntityCoords(GetPlayerPed(px.Handle));
                if (int.Parse(request.postal) != DispatchCalls[UnitDisposition[px].callAttached.CallNumber].DispatchCall.CallPostal)
                {
                    DispatchCalls[UnitDisposition[px].callAttached.CallNumber].DispatchCall.CallNotes += $"POSITION UPDATE: {request.postal} / ";
                    DispatchCalls[UnitDisposition[px].callAttached.CallNumber].DispatchCall.CallPostal = int.Parse(request.postal);
                }
                DispatchCalls[UnitDisposition[px].callAttached.CallNumber].DispatchCall.CallNotes += $"{UnitDisposition[px].callSign} REQUESTED BACKUP @ {time} / ";
                DispatchClosestAvailableOfficer(request.postal, pos, UnitDisposition[px].callAttached.CallNumber);
                
                PushUpdateCall(UnitDisposition[px].callAttached.CallNumber);
            }
            else
            {
               
                var call = CreateCall(new DispatchCallAttachment()
                {
                    CallSource="OFFICER BACKUP",
                    CallNotes=$"{time2} - CALL CREATED FOR BACKUP REQUEST / {UnitDisposition[px].callSign} REQUESTED BACKUP @ {time} / POSITION UPDATE: {request.postal} / ",
                    CallType="BACKUP REQUEST",
                    CallPostal = int.Parse(request.postal)
                });
                DispatchOfficerToCall(call, px);
                var pos = GetEntityCoords(GetPlayerPed(px.Handle));
                DispatchClosestAvailableOfficer(request.postal, pos, call);
            }

            q.Reply(true);
        }
        public static string CreateCall(DispatchCallAttachment callAttachment)
        {
            string callId = ServerCharacterHandlers.RandomString(6);
            callAttachment.CallNumber = callId;
            DispatchCalls.Add(callId, new DispatchCallFull { DispatchCall = callAttachment, UnitsAttached = new List<DispatchMemberDisposition>() });
            return callId;
        }

        public static void DispatchClosestAvailableOfficer(string postal, Vector3 ppos, string callId)
        {
            float minDist = float.MaxValue;
            Player closestUnit = null;
            foreach (var x in UnitDisposition)
            {
                
                var pos = GetEntityCoords(GetPlayerPed(x.Value.player.Handle));
                var dist = ppos.DistanceToSquared(pos);
                if (x.Value.isAvail && !x.Value.isAttachedToCall && dist < minDist && x.Value.agencyType == "POLICE")
                {
                    closestUnit = x.Key;
                    minDist = dist;
                }
            }
            if (closestUnit != null)
            {
                DispatchOfficerToCall(callId, closestUnit);
            }
        }
        public static void DispatchClosestAvailableEMS(string postal, Vector3 ppos, string callId)
        {
            float minDist = float.MaxValue;
            Player closestUnit = null;
            foreach (var x in UnitDisposition)
            {

                var pos = GetEntityCoords(GetPlayerPed(x.Value.player.Handle));
                var dist = ppos.DistanceToSquared(pos);
                if (x.Value.isAvail && !x.Value.isAttachedToCall && dist < minDist && x.Value.agencyType == "FIRE")
                {
                    closestUnit = x.Key;
                    minDist = dist;
                }
            }
            if (closestUnit != null)
            {
                DispatchOfficerToCall(callId, closestUnit);
            }
        }
        public static void PushUpdateCall(string callId)
        {
            foreach (var units in DispatchCalls[callId].UnitsAttached)
            {
                //Debug.WriteLine(JsonConvert.SerializeObject(DispatchCalls[callId].DispatchCall));

                QueryService.Query<bool>(int.Parse(units.player.Handle), "CLIENT_CALL_UPDATE", DispatchCalls[callId].DispatchCall);
                units.callAttached = DispatchCalls[callId].DispatchCall;
            }
        }
        public static void DispatchOfficerToCall(string callId, Player px)
        {
            UnitDisposition[px].isAttachedToCall = true;
            UnitDisposition[px].callAttached = DispatchCalls[callId].DispatchCall;
            var time = DateTime.Now.ToString("HH:mm");
            DispatchCalls[callId].DispatchCall.CallNotes += $"UNIT ATTACHED: {UnitDisposition[px].callSign} [{time}] / ";
            DispatchCalls[callId].UnitsAttached.Add(UnitDisposition[px]);
            PushUpdateCall(callId);
            ServerVoiceHandlers.JoinPlayerToFrequency(px, "CALL_" + callId);
            QueryService.Query<bool>(int.Parse(px.Handle), "CLIENT_CALL_ATTACH", DispatchCalls[callId].DispatchCall);
        }



    }
    public class DispatchMemberDisposition
    {
        public Player player;
        public bool isAvail = false;
        public string agency;
        public string agencyType;
        public string agencySign;
        public bool isAttachedToCall;
        public bool isSupervisor;
        public string callSign;
        public ProjectEmergencyFrameworkShared.Data.DispatchCallAttachment callAttached;
    }
    public class DispatchCallFull
    {
        public List<DispatchMemberDisposition> UnitsAttached = new List<DispatchMemberDisposition>();
        public ProjectEmergencyFrameworkShared.Data.DispatchCallAttachment DispatchCall;
    }
}
