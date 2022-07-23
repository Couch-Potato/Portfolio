using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Configuration.Schema
{
    public class ApartmentConfig
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string UniverseId { get; set; }
        public Location SpawnLocation { get; set; }
        public Location VehicleSpawnLocation { get; set; }
        public Location CameraPreviewLocation { get; set; }
        public Location ApartmentAccessLocation { get; set; }
        public Location ApartmentBlipLocation { get; set; }
        public BlipInfo BlipInfo { get; set; }
    }
}
