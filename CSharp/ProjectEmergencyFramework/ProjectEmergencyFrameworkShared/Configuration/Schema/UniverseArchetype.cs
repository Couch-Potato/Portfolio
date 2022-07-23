using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Configuration.Schema
{
    public class UniverseArchetype : IArchetype
    {
        public string Name { get; set; }
        public bool IsInterior { get; set; }
        public string InteriorName { get; set; }
        public Location SpawnCoords { get; set; }
    }
}
