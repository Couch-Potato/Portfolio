using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Novovu.Argon;
namespace AXDX.A
{
    public class ArgonCompName : ARTComposition, IArgonCompositionBuild
    {
        private string b64 = "";

        public ArgonCompName(object binda, object bindb)
        {
            Bind("binda", binda);
            Bind("bindb", bindb);
        }

        public string Build(object handl)
        {
            SetHandle(handl);
            return Encoding.Unicode.GetString(Convert.FromBase64String(b64));
        }

        public async Task<T> DoRandomStuff<T>(object p1, object p2)
        {
            return await RunJSMethod<T>("DoRandomStuff", p1, p2);
        }
    }
}
