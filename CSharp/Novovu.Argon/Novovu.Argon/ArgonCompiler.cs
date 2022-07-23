using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Argon
{
    public class ArgonCompiler
    {
        public static void PrepareComposition(string[] jscripts, string incomp_path, string comp_name, string name_space)
        {
            Console.WriteLine($"Preparing composition for build: {name_space}.{comp_name}");

            string reffx = File.ReadAllText(incomp_path);
            CompositionCodeParameters ccpx = new CompositionCodeParameters();
            
            foreach (string scpath in jscripts)
            {
                string reff = File.ReadAllText(scpath);
                while (reff.Contains("public function "))
                {
                    int loc = reff.IndexOf("public function ");
                    string subbed = reff.Substring(loc, reff.Length - loc);
                    string statement = subbed.Substring(0, subbed.IndexOf("}")) + "}";
                    string topStatement = statement.Substring(0, statement.IndexOf("{"));
                    topStatement = topStatement.Replace("public function ", "");
                    string fname = topStatement.Substring(0, topStatement.IndexOf("("));
                    string paramss = topStatement.Replace(fname + "(", "");
                    paramss = paramss.Replace(")", "");
                    string[] sourceParams = new string[0];
                    if (paramss != "" && !paramss.Contains(","))
                    {
                        sourceParams = new string[1];
                        sourceParams[0] = paramss;
                    }
                    else if (paramss.Contains(","))
                    {
                        sourceParams = paramss.Split(',');
                    }
                    string newstatement = statement.Replace("public function ", "function ");
                    reff = reff.Replace(statement, newstatement);
                    ccpx.Functions.Add(fname, sourceParams);
                }
                while (reff.Contains("bind "))
                {
                    int bloc = reff.IndexOf("bind ");
                    string brest = reff.Substring(bloc, reff.Length - bloc);
                    string bstatement = brest.Substring(0, brest.IndexOf(";"));
                    string bkeyword = bstatement.Split(' ')[1];
                    ccpx.Bindings.Add(bkeyword);
                    reff = reff.Replace($"bind {bkeyword};", "");
                }
                reffx.Replace(File.ReadAllText(scpath), reff);
            }

            string methods = "";
            foreach (KeyValuePair<string, string[]> kvp in ccpx.Functions)
            {
                methods += ArgonCBuild.CreateMethodBody(kvp.Key, kvp.Value);
            }
            string consts = ArgonCBuild.CreateConstructor(comp_name, ccpx.Bindings.ToArray());
            string compp = ArgonCBuild.Assmeble(consts, methods, name_space, comp_name, reffx);
            File.WriteAllText($"obj/{comp_name}x.cs", compp);
            



        }
        public static void BuildFolder(string name_space)
        {
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            ICodeCompiler icc = codeProvider.CreateCompiler();
            System.CodeDom.Compiler.CompilerParameters parameters = new CompilerParameters();
            parameters.ReferencedAssemblies.Add("Novovu.Argon.dll");
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.GenerateExecutable = false;
            parameters.OutputAssembly = name_space + ".dll";
            parameters.GenerateInMemory = false;
            //Console.WriteLine(File.ReadAllText($"obj/{comp_name}x.cs"));
            CompilerResults results = icc.CompileAssemblyFromFileBatch(parameters, Directory.GetFiles("obj"));
            foreach (string result in results.Output)
            {
                Console.WriteLine(result);
            }
        }

    }
    public class CompositionCodeParameters
    {
        public Dictionary<string, string[]> Functions = new Dictionary<string, string[]>();
        public List<string> Bindings= new List<string>();

    }
}
