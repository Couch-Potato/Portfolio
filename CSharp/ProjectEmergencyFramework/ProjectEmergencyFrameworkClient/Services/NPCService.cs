using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
namespace ProjectEmergencyFrameworkClient.Services
{
    public static class NPCService
    {
        private static Dictionary<string, NPCType> npcReg = new Dictionary<string, NPCType>();
        public static List<ActiveNPC> NPCs = new List<ActiveNPC>();
        public static void RegisterNPC(NPCType type)
        {
            npcReg.Add(type.Name, type);
        }
        public static bool IsNPCRegistered(string name)
        {
            return npcReg.ContainsKey(name);
        }
        public static void DespawnNPC(ActiveNPC npc)
        {
            NPCs.Remove(npc);
            npc.Ped.Delete();
            Interact.InteractService.TerminateInteractable(npc.Interactable);
        }
        public static async Task<ActiveNPC> SpawnNPC(string name, Vector3 location, float heading)
        {
            // first we need to see if a ped with this type has already been spawned. 
            foreach (var ped in World.GetAllPeds())
            {
                if (ped.State.Get("MISSION_NAME") != null && ped.IsAlive)
                {
                    if (ped.State.Get("MISSION_NAME") == name)
                    {
                        // We already have a NPC spawned. This is probably due to some replication stuff. 
                        // Lets figure out its distance from the specified location before we decide if we need to spawn another one.
                        if (Vector3.Distance(location, ped.Position) > 10f)
                        {
                            

                        }else
                        {
                            // Lets just attach the NPC interact.
                            var intcs = Interact.InteractService.ConstructInteract("NPC_TALK", ped, new {
                                npcType = npcReg[name]
                            });
                            var anpcs = new ActiveNPC()
                            {
                                NPCType = npcReg[name],
                                Ped = ped,
                                Interactable = intcs
                            };
                            NPCs.Add(anpcs);
                            return anpcs;
                        }
                    }
                }
            }
            // We need to spawn.
            await Utility.AssetLoader.LoadModel(npcReg[name].PedModel);


            int newPed = CreatePed(4, (uint)GetHashKey(npcReg[name].PedModel), location.X, location.Y, location.Z, heading, true, true);
            var pedD = new Ped(newPed);
            pedD.IsInvincible = true;
            pedD.State.Set("MISSION_NAME", name, true);

            var intc = Interact.InteractService.ConstructInteract("NPC_TALK", pedD, new
            {
                npcType = npcReg[name]
            });
            var anpc = new ActiveNPC()
            {
                NPCType = npcReg[name],
                Ped = pedD, 
                Interactable = intc
            };
            NPCs.Add(anpc);
            return anpc;
        }
    }
    public class NPCType
    {
        public string Name { get; set; }
        public Action NPCTalk { get; set; }
        public string PedModel { get; set; }
    }
    public class ActiveNPC
    {
        public NPCType NPCType { get; set; }
        public Ped Ped { get; set; }

        public Interact.IInteractable Interactable { get; set; }
    }
}
