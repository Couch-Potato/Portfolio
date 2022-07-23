using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Argon
{
    public class JSCallbackConstructor
    {
        public delegate void InvokeDelegate(params object[] ps);
        private InvokeDelegate InvokeDelegateI;
        public JSCallbackConstructor(InvokeDelegate ivd)
        {
            InvokeDelegateI = ivd;
        }
        public void Invoke(params object[] ps)
        {
            InvokeDelegateI(ps);
        }
    }
}
