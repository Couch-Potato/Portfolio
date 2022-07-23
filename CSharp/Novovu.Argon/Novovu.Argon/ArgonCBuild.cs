using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Argon
{
    public class ArgonCBuild
    {
        public static string Assmeble(string init, string tasks, string namespaces, string classname, string contentx)
        {
            string content = "using System; using System.Text; using System.Threading.Tasks; using Novovu.Argon; namespace %%NAMESPACE%% { public class %%NAME%% : ARTComposition, IArgonCompositionBuild { private string b64 = \""+SafeCode(contentx)+ "\"; public string Build(object handl) { SetHandle(handl); return ArgonUtil.DecompressString(b64); } %%INIT%% %%TASKS%%} }";

            return content.Replace("%%NAMESPACE%%", namespaces).Replace("%%NAME%%", classname).Replace("%%INIT%%", init).Replace("%%TASKS%%", tasks).Replace(Environment.NewLine ,"").Replace("\n","").Replace("\r","");
        }
        private static string SafeCode(string code)
        {
            return ArgonUtil.CompressString(code);
        }
        public static string CreateMethodBody(string name, string[] paramsx)
        {
            string paramN = "";
            foreach (string s in paramsx)
            {
                if (s != "" && s!=" ") {
                    string nsx = s;
                    nsx = s.Replace(" ", "");
                    if (nsx.Contains("=")) {
                        nsx = nsx.Split('=')[0] + " = null";
                    }
                    
                    paramN += "object " + nsx + ",";
                }
            }
            paramN = paramN.TrimEnd(',');
            string paramZ = "";
            foreach (string s in paramsx)
            {
                if (s != "" && s != " ")
                {
                    
                    paramZ += s + ",";
                }
            }
            
            paramZ = paramZ.TrimEnd(',');
            //Console.WriteLine($"'{paramZ}'");
            if (paramsx.Length >0)
            {
                paramsx[0] = paramsx[0].Replace(Environment.NewLine, "").Replace("\n", "").Replace("\r", "");

                if (paramsx[0] != ""&& paramsx[0] !=" ")
                {
                    return "public async Task<T> %%NAME%%<T>(%%N%%) { return await RunJSMethod<T>(\"%%NAME%%\", %%Z%%); }".Replace("%%N%%", paramN).Replace("%%Z%%", paramZ).Replace("%%NAME%%", name);
                }else
                {
                    return "public async Task<T> %%NAME%%<T>() { return await RunJSMethod<T>(\"%%NAME%%\"); }".Replace("%%N%%", paramN).Replace("%%Z%%", paramZ).Replace("%%NAME%%", name);
                }
                
            }else
            {
                return "public async Task<T> %%NAME%%<T>() { return await RunJSMethod<T>(\"%%NAME%%\"); }".Replace("%%N%%", paramN).Replace("%%Z%%", paramZ).Replace("%%NAME%%", name);
            }
            
        }
        public static string CreateConstructor(string name, string[] paramsx)
        {
            string paramN = "";
            foreach (string s in paramsx)
            {
                paramN += "object " + s + ",";
            }
            paramN = paramN.TrimEnd(',');
            string bindString = "";
            foreach (string s in paramsx)
            {
                bindString += $"Bind(\"{s}\", {s});";
            }
            return "public %%NAME%%(%%N%%) {%%B%%}".Replace("%%N%%", paramN).Replace("%%B%%", bindString).Replace("%%NAME%%", name);

        }
    }
}
