using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Placeables
{
    public interface IPlaceable
    {
        void Place();
        void Instanced();
        void Destroyed();
        void Cleanup();

        Vector3 Position { get; set; }
        Vector3 Rotation { get; set; }
        string OwnerId { get; set; }
        string PersistentId { get; set; }
        string Universe { get; set; }

        dynamic Properties { get; set; }
        InstancedProp Prop { get; set; }
    }
}
