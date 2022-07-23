using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Missions
{
    public interface IMission
    {
        void StartMission();
        void StopMission();
        void MissionTick();
    }
}
