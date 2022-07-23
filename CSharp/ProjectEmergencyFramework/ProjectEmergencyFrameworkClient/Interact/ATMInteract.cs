using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interact
{
    [Interactable("useatm", "G", "USE ATM", true, GenericInteractAttachment.Prop, 0x29264083, PropModels =new uint[]
    {
        2930269768,
        506770882,
        3424098598,
        3168729781
    })]
    public class ATMInteract : LookatRadiusInteractable
    {
        public ATMInteract()
        {
            Radius = 1.5f;
            Offset = new CitizenFX.Core.Vector3(0, 0, 1.2f);
        }
        public override async Task<bool> CanShow()
        {
            //return false;
            if (Entity == null) return false;
            return await base.CanShow();
        }
        protected override void OnInteract()
        {
            Interfaces.InterfaceController.ShowInterface("atm");
        }
    }
}
