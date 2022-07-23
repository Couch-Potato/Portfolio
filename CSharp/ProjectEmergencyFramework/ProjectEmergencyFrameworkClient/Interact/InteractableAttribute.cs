using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interact
{
    public enum GenericInteractAttachment
    {
        NoAttachment,
        Ped,
        Vehicle,
        Door,
        Gas,
        Prop
    }
    public class InteractableAttribute:Attribute
    {
        public string InteractName;
        public string Keybind;
        public string Caption;
        public bool IsBillboard;
        public uint PropModel;
        public uint[] PropModels;
        public GenericInteractAttachment Attachment;
        public InteractableAttribute(string InteractName, string key, string cap, bool isBillboard = false, GenericInteractAttachment attch = GenericInteractAttachment.NoAttachment, uint propModel = 0)
        {
            Keybind = key;
            Caption = cap;
            this.InteractName = InteractName;
            IsBillboard = isBillboard;
            Attachment = attch;
            this.PropModel = propModel;
        }
    }
}
