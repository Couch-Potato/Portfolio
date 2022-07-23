using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Novovu.Workshop.VSCode
{
    public class VSCodeAdapter
    {
        public static bool IsLaunched = false;
        public static void LaunchScript(string script, int line = 1)
        {
            string versionstring = "win64";
           
            Debug.WriteLine(new FileInfo("VSCode/" + versionstring + "/Code.exe").FullName);
            var p = new Process();
            p.StartInfo.FileName = new FileInfo("VSCode/" + versionstring + "/Code.exe").FullName;
            p.StartInfo.Arguments = new FileInfo(script).FullName + ":" + line;
            if (IsLaunched)
                p.StartInfo.Arguments += " -r";
            else
                p.StartInfo.Arguments += " -n";
            IsLaunched = true;
            var t = new Thread(() =>
            {
                p.Start();
            });
            t.Start();
        }
        public static void LaunchDirectory(string directory)
        {
            string versionstring = "win64";
           
            Debug.WriteLine(new FileInfo("VSCode/" + versionstring + "/Code.exe").FullName);
            var p = new Process();
            p.StartInfo.FileName = new FileInfo("VSCode/" + versionstring + "/Code.exe").FullName;
            p.StartInfo.Arguments = '"' + new DirectoryInfo(directory).FullName + '"';
            if (IsLaunched)
                p.StartInfo.Arguments += " -r";
            else
                p.StartInfo.Arguments += " -n";
            IsLaunched = true;
            Debug.WriteLine(p.StartInfo.Arguments);
            var t = new Thread(() =>
            {
                p.Start();
            }); 
           t.Start();
        }
    }
}
