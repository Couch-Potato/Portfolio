using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Peridot;
using System.Reflection;
using System.Net;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.IO;
namespace PeridotServer
{
    class Program
    {
        private static string loadInternalFile(string url)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = url;
            try
            {
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return "";
            }


        }
        static void Main(string[] args)
        {

          
            
            CSharpCodeProvider provider = new CSharpCodeProvider();
            string defaultConf = @"PeridotHttpServer peridot = new PeridotHttpServer(""public_html"",80);";
            if (!File.Exists("peridotConf.pcs"))
            {
                Directory.CreateDirectory("public_html");
                File.WriteAllText("public_html/index.html", loadInternalFile("PeridotServer.default_page.welcome.html"));
                File.WriteAllText("public_html/favicon.ico", loadInternalFile("PeridotServer.peridot.ico"));
                File.WriteAllText("peridotConf.pcs", defaultConf);
            }
            string src = @"using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Peridot;
namespace PDT
{
    public class PDTCONF
    {
        public static void Conf()
        {
            "+System.IO.File.ReadAllText("peridotConf.pcs")+@"
             Console.ReadLine();
        }
    }
}
";
            Console.WriteLine("[NOTICE] Reading configuration file");
            CompilerParameters param = new CompilerParameters { GenerateExecutable = false, GenerateInMemory = true };
            param.ReferencedAssemblies.Add("Peridot.dll");
            param.ReferencedAssemblies.Add("System.dll");
            CompilerResults results = provider.CompileAssemblyFromSource(param, src);
            if (results.Errors.Count != 0)
            {
                Console.WriteLine("[FATAL ERROR] Could not read peridotConf.pcs file.");
                foreach (CompilerError error in results.Errors)
                {
                    Console.WriteLine($"...[{error.ErrorNumber}] {error.ErrorText} in {error.Line}");
                }
                Console.ReadLine();
                return;
            }
            object o = results.CompiledAssembly.CreateInstance("PDT.PDTCONF");
            MethodInfo mi = o.GetType().GetMethod("Conf");
            mi.Invoke(o, null);
        }
    }
}
