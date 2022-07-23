using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Configuration.Schema
{
    public class PoliceConfig
    {
        public string OrganizationId { get; set; }
        public string Name { get; set; }
        public Location OnDutyLocation { get; set; }
        public Location BookingComputer { get; set; }
        public Location EvidenceLocker { get; set; }
        public Location PersonnelLocker { get; set; }
        public Location Fingerprint { get; set; }
        public Location Mugshot { get; set; }
        public Location PayFines { get; set; }
        public Location VehicleSpawner { get; set; }
        public Location Armory { get; set; }


    }
}
