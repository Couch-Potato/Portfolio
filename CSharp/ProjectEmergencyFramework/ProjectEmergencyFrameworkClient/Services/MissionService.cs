using ProjectEmergencyFrameworkClient.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Services
{
    public static class MissionService
    {
        private static List<DiscoveredItem> missions;
        private static List<Missions.IMission> activeMissions = new List<Missions.IMission>();
        [ExecuteAt(ExecutionStage.Initialized)]
        public static void InitMissionService()
        {
            missions = ClassDiscovery.DiscoverWithAttribute<Missions.MissionAttribute>();
        }

        public static void StartMission(string name)
        {
            foreach (var mission in missions)
            {
                if (mission.GetAttribute<Missions.MissionAttribute>().Name == name)
                {
                    if (mission.GetAttribute<Missions.MissionAttribute>().AllowOnlyMission)
                    {
                        foreach (var msx in activeMissions)
                        {
                            msx.StopMission();
                        }
                        activeMissions.Clear();
                    }
                    var newms = mission.ConstructAs<Missions.IMission>();
                    newms.StartMission();
                    activeMissions.Add(newms);
                    TaskService.InvokeUntilExpire(async () =>
                    {
                        if (!activeMissions.Contains(newms))
                            return true;
                        newms.MissionTick();
                        return false;
                    });
                }
            }
        }
        public static void StopMission(Missions.IMission mission)
        {
            activeMissions.Remove(mission);
            mission.StopMission();
        }
    }
}
