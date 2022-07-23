using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Novovu.Xenon;
namespace Novovu.Argon
{
    public class Argon
    {
        public static void Package(string compFolder, string output)
        {
            if (Directory.Exists("out"))
            {
                Directory.Delete("out", true);
            }
            Directory.CreateDirectory("out");
            foreach(string file in Directory.GetFiles(compFolder))
            {
                FileInfo fl = new FileInfo(file);
                if (fl.Extension == ".agc")
                {
                    Console.WriteLine("Building composition: " + fl.Name);
                }
                ReadComposition(file, "out/" + (fl.Name.Replace(fl.Extension,"")) + ".agc" );
                
               
            }
            Console.WriteLine("Packing archive");
            File.WriteAllText("out/" + "ARGON_PACK", "1.0.0");

            Xenon.XEF.XEFile FILE = new Xenon.XEF.XEFile();
            FILE.ImportFile("ARGON_PACK", "out/" + "ARGON_PACK");
            foreach (string file in Directory.GetFiles("out"))
            {
                FileInfo fl = new FileInfo(file);
                Console.WriteLine("Importing " + fl.Name.Replace(fl.Extension, ""));
                FILE.ImportFile(new FileInfo(file).Name, file);
            }
            Console.WriteLine("Exporting archive");
            if (!output.Contains(".ag.xef"))
            {
                output += ".ag.xef";
            }
            Xenon.XEF.BuildXEF.BuildXEFile(FILE, output);

            Console.WriteLine("Cleaning up...");
            Directory.Delete("out/", true);
        }
        public static List<FileIncludes> ReadComposition(string comp, string output)
        {
            string com = File.ReadAllText(comp);
            string includes = com.Replace("<Includes", "﴾");
            includes = includes.Replace("</Includes", "﴾");
            string inner = includes.Split('﴾')[1];

            string outer = includes.Split('﴾')[2];
            outer = outer.TrimStart('/', '>');
            inner = inner.TrimStart('>');

            string[] inclusions = inner.Split(Environment.NewLine.ToCharArray());
            List<FileIncludes> Files = new List<FileIncludes>();
            for (int i = 0; i < inclusions.Length; i++)
            {
                try
                {
                    string src = inclusions[i].Replace("src=" + '"', "﴿");

                    src = src.Split('﴿')[1];
                    int pos = src.IndexOf('"');
                    src = src.Substring(0, pos);
                    FileIncludes.FileTypes type = FileIncludes.FileTypes.Element;
                    if (inclusions[i].Contains("<Style"))
                    {
                        type = FileIncludes.FileTypes.Style;
                    }
                    if (inclusions[i].Contains("<Element"))
                    {
                        type = FileIncludes.FileTypes.Element;
                    }
                    if (inclusions[i].Contains("<Script"))
                    {
                        type = FileIncludes.FileTypes.Script;
                    }
                    if (src.Contains("/*"))
                    {
                        src = src.Replace("*", "");

                        foreach (string file in Directory.GetFiles($"{type.ToString()}s/{src}"))
                        {

                            Console.WriteLine($"{type.ToString()}s/{src}");
                            FileIncludes fix = new FileIncludes();
                            fix.FileType = type;
                            fix.Name = file;
                            Files.Add(fix);
                        }
                    }
                    else
                    {
                        FileIncludes fix = new FileIncludes();
                        src = $"{type.ToString()}s/{src}";
                        fix.Name = src;
                        fix.FileType = type;
                        Files.Add(fix);
                    }
                }
                catch
                {

                }

            }
            Console.WriteLine("Found the following source files:");
            foreach (FileIncludes fx in Files)
            {
                Console.WriteLine($"{fx.Name}   -   {fx.FileType}");
            }
            BuildRender(outer, Files, output);
            return Files;
        }
        static string ElemConstructor = @"%%NAME%%: {Source: '%%SOURCE%%'},";
        private static string ConstructElement(string name, string source)
        {
            return ElemConstructor.Replace("%%NAME%%", name).Replace("%%SOURCE%%", source).Replace(Environment.NewLine, "");
        }
        static void BuildRender(string inner, List<FileIncludes> Files, string output)
        {
            Console.WriteLine("Building main document...");
            string doc = inner;
            Console.WriteLine("Getting Main Elements...");
            foreach (FileIncludes fi in Files)
            {

                if (fi.FileType == FileIncludes.FileTypes.Element)
                {

                    Console.WriteLine("Building Element: " + fi.Name);
                    Element e = new Element(fi.Name);
                    doc = PlugAndChug(doc, e.Name, e);
                }
            }
            Console.WriteLine("Building Javascript... (API Loader)");
            string payload = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Novovu.Argon.AFX.argonpayload.js")).ReadToEnd();
            string elemchol = "";
            foreach (FileIncludes fi in Files)
            {

                if (fi.FileType == FileIncludes.FileTypes.Element)
                {


                    Element e = new Element(fi.Name);
                    string betterSrc = e.Source;
                    betterSrc = betterSrc.Replace("[body]", "");
                    foreach (KeyValuePair<string, string> pair in e.Variables)
                    {
                        betterSrc = betterSrc.Replace("${" + pair.Key + "}", $"%%{pair.Key}%%");

                    }
                    Console.WriteLine($"Building Javascript... {e.Name}");
                    elemchol += ConstructElement(e.Name, betterSrc);
                }
            }
            doc += $"<script>{payload.Replace("$$BODY$$", elemchol)}</script>";
            string styles = "";
            Console.WriteLine("Building Styles...");
            foreach (FileIncludes fi in Files)
            {

                if (fi.FileType == FileIncludes.FileTypes.Style)
                {
                    Console.WriteLine($"Building Styles... ({fi.Name})");
                    doc = $"<style>{File.ReadAllText(fi.Name)}</style>" + doc;
                }
            }
            Console.WriteLine("Building Scripts...");
            foreach (FileIncludes fi in Files)
            {

                if (fi.FileType == FileIncludes.FileTypes.Script)
                {
                    Console.WriteLine($"Building Scripts... ({fi.Name})");
                    doc += $"<script>{File.ReadAllText(fi.Name).Replace("public function","function")}</script>";
                }
            }
            Console.WriteLine("Exporting...");
            File.WriteAllText(output, doc);
        }
        static string PlugAndChug(string input, string element, Element elemobj)
        {
            int start = input.IndexOf("<" + element);
            int end = input.IndexOf($"</{element}>");
            if (start == -1)
            {
                return input;
            }
            try
            {
                string full = input.Substring(start, end - start + element.Length + 3);
                int innerStart = full.IndexOf(">");
                int innerEnd = full.IndexOf($"</{element}>");
                string xds = full.Substring(innerStart + 1, innerEnd - innerStart - 1);
                int inputFields = full.IndexOf($"<{element}");
                int inputEnd = full.IndexOf(">");
                string paramss = full.Substring(inputFields, inputEnd - inputFields).Replace($"<{element} ", "");
                string[] paramsx = paramss.Split('"');
                elemobj.ClearAttributes();
                string handleParam = null;
                if (paramsx.Length > 1)
                {

                    for (int i = 0; i < paramsx.Length; i += 1)
                    {
                        //Console.WriteLine(paramsx[i]);
                        if (handleParam == null)
                        {
                            handleParam = paramsx[i];
                            if (handleParam.ToCharArray().Length > 0)
                            {
                                if (handleParam.ToCharArray()[0] == ' ')
                                {
                                    handleParam = handleParam.Substring(1);
                                }
                                handleParam = handleParam.Substring(0, handleParam.Length - 1);
                            }
                            else
                            {
                                handleParam = null;
                            }

                        }
                        else
                        {

                            elemobj.Variables[handleParam] = paramsx[i].Replace("\"", "").Replace("'", "");

                            handleParam = null;
                        }

                    }
                }

                input = input.Replace(full, elemobj.Build().Replace("[body]", xds));
                return PlugAndChug(input, element, elemobj);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Render failed: Invalid composition");
                return "";
            }
            
            
        }
    }
}
    public class FileIncludes
    {
        public string Name;
        public enum FileTypes { Style, Script, Element }
        public FileTypes FileType;

    }
    class Element
    {
        public string Name;
        public string Source;
        public Dictionary<string, string> Variables = new Dictionary<string, string>();
        public string Build()
        {
            string asrc = Source;
            foreach (KeyValuePair<string, string> varia in Variables)
            {
                
                asrc = asrc.Replace("${" + varia.Key + "}", varia.Value);
                if (asrc.Contains("${" + varia.Key + ":"))
            {
                string x = asrc.Substring(asrc.IndexOf("${" + varia.Key + ":"), asrc.Length - asrc.IndexOf("${" + varia.Key + ":"));
                string full = x.Substring(0, x.IndexOf('}'));
                asrc = asrc.Replace(full + "}", varia.Value);
            }
                asrc = asrc.Replace("${" + varia.Key + ":"+ varia.Value + "}", varia.Value);
            }
            return asrc;
        }
        public Element(string namefile)
        {
            Source = File.ReadAllText(namefile);
            Name = new FileInfo(namefile).Name.Replace(".ag", "");

        Dictionary<string, string> dx = scrubber(Source);
        try
        {
            foreach (KeyValuePair<string, string> kx in dx)
            {
                Variables.Add(kx.Key, kx.Value);
            }

        }
        catch
        {

        }
            
            
        }
        public void ClearAttributes()
        {
            Variables.Clear();
        Dictionary<string, string> dx = scrubber(Source);
        try
        {
            foreach (KeyValuePair<string, string> kx in dx)
            {
                Variables.Add(kx.Key, kx.Value);
            }

        }
        catch
        {

        }
    }
        private Dictionary<string, string> scrubber(string input, Dictionary<string, string> iplist = default(Dictionary<string, string>))
        {
            int start = input.IndexOf("${");
            int end = input.IndexOf("}");
            if (start == -1)
            {
                return iplist;
            }
            string name = input.Substring(start + 2, end - start - 2);
            if (iplist == default(Dictionary<string, string>))
            {
                iplist = new Dictionary<string, string>();
            }
            input = input.Replace("${" + name + "}", "");
        
            if (name.Contains(":"))
            {
         
                iplist.Add(name.Split(':')[0], name.Split(':')[1]);
                return scrubber(input, iplist);
            }else
        {
            iplist.Add(name, "");
            return scrubber(input, iplist);
        }
            
        }
    }
    class Variable
    {
        public string Name;
        public string Value;
    }

