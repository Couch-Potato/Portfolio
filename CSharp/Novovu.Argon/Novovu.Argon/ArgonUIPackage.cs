using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Novovu.Xenon.XEF;
namespace Novovu.Argon
{
    public class ArgonUIPackage
    {
        public Dictionary<string, ArgonComposition> Compositions = new Dictionary<string, ArgonComposition>();
        public static ArgonUIPackage LoadFromFile(string fileName)
        {
            Logger.Log(new ArgonUIPackage(), "Loading new Argon archive.");
            XEFile UIPACK = BuildXEF.LoadXEF(fileName);

            if (UIPACK.Files.ContainsKey("ARGON_PACK"))
            {
                string agv;
                using (StreamReader sr = new StreamReader(UIPACK.GetStream("ARGON_PACK")))
                {
                    agv = sr.ReadToEnd();
                };
                Logger.Log(new ArgonUIPackage(), "Argon version: " + agv);

            }
            else
            {
                throw new InvalidPackageException(fileName);
            }

            Logger.Log(new ArgonUIPackage(), "Importing compositions...");
            ArgonUIPackage uip = new ArgonUIPackage();
            foreach (KeyValuePair<string, byte[]> file in UIPACK.Files)
            {
                if (file.Key.Contains(".agc"))
                {
                    string name = file.Key.Replace(".agc", "");
                    Logger.Log(new ArgonUIPackage(), "Importing " + name);
                    uip.Compositions.Add(name, new ArgonComposition(name, file.Value));
                }
            }
            Logger.Log(new ArgonUIPackage(), "Argon Archive Loaded.");
            return uip;
        }

    }
    public class InvalidPackageException:Exception
    {
        public InvalidPackageException(string filename)
            : base("The package file you were wishing to open is not a valid Argon package. (" + filename + ")")
        {
           
        }
    }
}
