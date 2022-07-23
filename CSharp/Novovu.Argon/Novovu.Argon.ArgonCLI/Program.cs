using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Novovu.Argon;
namespace Novovu.Argon.ArgonCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create the test case builder.
            if (args.Length > 0)
            {
                if (args[0] == "-r")
                {
                    if (args.Length > 2)
                    {
                        string comp = args[1];
                        string output = args[2];
                        Console.WriteLine("Starting Composition Render of " + comp);
                        Argon.ReadComposition(comp, output);

                    }
                    else
                    {
                        Console.WriteLine("Composition is not specified.");
                    }
                }else if(args[0] == "-a")
                {
                    if (args.Length > 2)
                    {
                        string comp = args[1];
                        string output = args[2];
                        Argon.Package(comp, output);

                    }
                    else
                    {
                        Console.WriteLine("Composition folder is not specified.");
                    }
                }else if (args[0] == "-b")
                {
                    if (args.Length > 2)
                    {
                        string incomp = args[1];
                        string namespaces = args[2];
                        if (Directory.Exists("obj"))
                        {
                            Directory.Delete("obj", true);
                        }
                        Directory.CreateDirectory("obj");

                        foreach (string file in Directory.GetFiles(incomp))

                        {
                            
                            string icmp_name = new FileInfo(file).Name.Replace(new FileInfo(file).Extension, "");
                            Console.WriteLine("Rendering composition: " + icmp_name);
                            string temp = Path.GetTempFileName();
                            List<FileIncludes> x = Argon.ReadComposition(file, temp);
                            List<FileIncludes> nxd = new List<FileIncludes>();
                            string[] jsfiles;
                            foreach (FileIncludes xx in x)
                            {
                                if (xx.FileType == FileIncludes.FileTypes.Script)
                                {
                                    nxd.Add(xx);
                                }
                            }
                            jsfiles = new string[nxd.Count];
                            for (int i = 0; i < jsfiles.Length; i++)
                            {
                                jsfiles[i] = nxd[i].Name;
                            }
                            ArgonCompiler.PrepareComposition(jsfiles, temp, icmp_name, namespaces);
                        }
                        ArgonCompiler.BuildFolder(namespaces);
                        Console.WriteLine("Sucessfully built all compositions");
                        Console.WriteLine("---> " + namespaces + ".dll");
                        
                    }
                    else
                    {
                        Console.WriteLine("Composition folder is not specified.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Argument");
                }
            }
            else
            {
                Console.WriteLine("Invalid Paramters.");
            }
        }
    }
    
}
