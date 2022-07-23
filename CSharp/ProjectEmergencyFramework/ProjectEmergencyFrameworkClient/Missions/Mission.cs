using ProjectEmergencyFrameworkClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Missions
{
    public abstract class Mission : IMission
    {
        public void MissionTick()
        {
            OnMissionTick();
        }

        public void StartMission()
        {
            OnMissionInitialize();
        }

        public void StopMission()
        {
            OnMissionCleanup();
        }

        protected void Stop()
        {
            MissionService.StopMission(this);
        }

        protected abstract void OnMissionInitialize();
        protected abstract void OnMissionCleanup();

        protected virtual void OnMissionTick() { }
    }
}
