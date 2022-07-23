
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkServer.Data
{
    public static class ServerMDTHandlers
    {
        [Queryable("MDT_VEHICLE_LOOKUP")]
        public static void VehicleLookup(Query q, object i, CitizenFX.Core.Player px)
        {
            var request = (dynamic)i;
            ProjectEmergencyFrameworkShared.Data.Model.Vehicle veh;
            ProjectEmergencyFrameworkShared.Data.Model.Character RO;
            var flags = new List<string>()
            {

            };
            if (request.vin != "-" && request.vin !="")
            {
                veh = PlayerDataService.GetVehicle(request.vin);
                if (veh == null)
                {
                    q.Reply(new
                    {
                        plate = "UKNWN",
                        flags = flags,
                        ownerFirstName = "UNKNOWN",
                        ownerLastName = "PERSON",
                        vin = "???",
                        make = "???",
                        model = "???",
                        isStolen = false
                    });
                    return;
                }
                if (veh.IsInsured)
                {
                    flags.Add("VEH INSURED");
                }
                if (veh.Imponded)
                {
                    flags.Add("~r~VEHICLE IMPOUNDED");
                }
                if (veh.Destroyed)
                {
                    flags.Add("~r~VEHICLE DESTROYED");
                }
                if (veh.IsGovernmentInsured)
                {
                    flags.Add("GOVERNMENT VEHICLE");
                }
                if (veh.BelongsToOrganization)
                {
                    flags.Add("OWNED BY ORGANIZATION");
                }
                RO = PlayerDataService.GetCharacterFromId(veh.RegisteredOwnerId);
                if (RO.IsDead)
                {
                    flags.Add("~r~OWNER DECEASED -- LIKELY STOLEN!");
                }
                if (RO.IsWanted)
                {
                    flags.Add("~r~OWNER WANTED -- LOOKUP REASON!");
                }
                q.Reply(new
                {
                    plate = veh.LicensePlate,
                    flags = flags,
                    ownerFirstName = RO.FirstName,
                    ownerLastName = RO.LastName,
                    vin = veh.Id,
                    make = veh.Make,
                    model = veh.Model,
                    isStolen = veh.IsStolen
                });
                return;
            }
            veh = PlayerDataService.GetVehicleByPlate(request.plate);
            if (veh == null)
            {
                q.Reply(new
                {
                    plate = "UKNWN",
                    flags = flags,
                    ownerFirstName = "UNKNOWN",
                    ownerLastName = "PERSON",
                    vin = "???",
                    make = "???",
                    model = "???",
                    isStolen = false
                });
                return;
            }
            if (veh.IsInsured)
            {
                flags.Add("VEH INSURED");
            }
            if (veh.Imponded)
            {
                flags.Add("~r~VEHICLE IMPOUNDED");
            }
            if (veh.Destroyed)
            {
                flags.Add("~r~VEHICLE DESTROYED");
            }
            if (veh.IsGovernmentInsured)
            {
                flags.Add("GOVERNMENT VEHICLE");
            }
            if (veh.BelongsToOrganization)
            {
                flags.Add("OWNED BY ORGANIZATION");
            }
            RO = PlayerDataService.GetCharacterFromId(veh.RegisteredOwnerId);
            if (RO.IsDead)
            {
                flags.Add("~r~OWNER DECEASED -- LIKELY STOLEN!");
            }
            if (RO.IsWanted)
            {
                flags.Add("~r~OWNER WANTED -- LOOKUP REASON!");
            }
            q.Reply(new
            {
                plate = veh.LicensePlate,
                flags = flags,
                ownerFirstName = RO.FirstName,
                ownerLastName = RO.LastName,
                vin = veh.Id,
                make = veh.Make,
                model = veh.Model,
                isStolen = veh.IsStolen
            });
            return;
        }
        [Queryable("MDT_PERSON_LOOKUP")]
        public static void PersonLookup(Query q, object i, CitizenFX.Core.Player px)
        {
            var request = (dynamic)i;
            ProjectEmergencyFrameworkShared.Data.Model.Character person = PlayerDataService.GetCharacterFromFields(request.firstName, request.dlid, request.lastName);
            var flags = new List<string>()
            {

            };
            if (person == null || person.Id == null)
            {
                flags.Add("NO RECORD!");
                q.Reply(new
                {
                    firstName = "UNKNOWN",
                    lastName = "PERSON",
                    dob = "??/??/??",
                    age = 00,
                    flags = flags,

                });
                return;
            }
           
            if (CriminalDataService.GetFines(person.Id) > 0)
            {
                flags.Add("OUTSTANDING FINES");
            }
            if (CriminalDataService.HasBackdatedFines(person.Id))
            {
                flags.Add("PAST DUE FINES");
                person.IsWanted = true;
                person.WantedReason = $"ISSUED BY THE STATE OF SAN ANDREAS / {person.LastName} : OUTSTANDING CAPIAS - MAGISTRATE COURT OF THE STATE OF SAN ANDREAS / WANTED FOR FAILING TO PAY FINES BY DEADLINE!";
            }
            var ncic = CriminalDataService.PullNCIC(person.Id);

            if (ncic.Arrests.Count > 0)
            {
                flags.Add("ARREST RECORD");
            }

            string wantedReason = person.IsWanted ? person.WantedReason : "";
            q.Reply(new
            {
                firstName = person.FirstName,
                lastName = person.LastName,
                dob=person.DOB.Month + "/" + person.DOB.Day + "/" + person.DOB.Year,
                age=00,
                flags = flags,
                wantedReason = wantedReason,
                dmvPhoto = person.DMVPhoto,
                isWanted = person.IsWanted
                
            });
        }
        [Queryable("REPORT_STOLEN")]
        public static void ReportStolen(Query q, object i, CitizenFX.Core.Player px)
        {
            var request = (dynamic)i;
            ProjectEmergencyFrameworkShared.Data.Model.Vehicle veh = PlayerDataService.GetVehicle(request.vin);
            veh.IsStolen = !veh.IsStolen;
            PlayerDataService.UpdateVehicle(veh);
            q.Reply(false);

        }
    }
}
