using ProjectEmergencyFrameworkClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interact
{
    [Interactable("NPC_TALK", "G", "TALK")]
    public class NPCTalk : LookatRadiusInteractable
    {
        Random r = new Random();
        protected override void OnInteract()
        {

            type?.NPCTalk();

        }

        public NPCTalk()
        {
            Radius = 3f;
            Tolerance = 30f / 2;
            Offset = new CitizenFX.Core.Vector3(0, 0, .7f);
        }

        NPCType type = null;

        public override async Task<bool> CanShow()
        {
            if (type == null)
            {
                type = Properties.npcType;
            }



            return await base.CanShow();
        }
    }
}
