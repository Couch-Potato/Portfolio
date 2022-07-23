using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Novovu.Xenon.ScriptEngine;
using DynamicData;

namespace Novovu.Workshop.TypeInterfaces.Serialized
{
    [Serializable]
    public class SComponent
    {
        /*public string Name;
        public List<SScript> Scripts = new List<SScript>();
        public SScript EntryPoint;
        public string EntryPointName;
        public SScript Properties;
        public string PropertiesPointName;
        public static SComponent From(WComponent w)
        {
            SComponent sc = new SComponent();
            sc.Name = w.TreeItemName;
            sc.EntryPoint = new SScript()
            {
                Name = new FileInfo(w.Dir + "/" + w.Entry).Name,
                Content = File.ReadAllText(w.Dir + "/" + w.Entry)
            };
            sc.EntryPointName = new FileInfo(w.Dir + "/" + w.Entry).Name;
            sc.PropertiesPointName = new FileInfo(w.Dir + "/" + w.PropertiesScriptPoint).Name;
            sc.Properties = new SScript()
            {
                Name = new FileInfo(w.Dir + "/" + w.Entry).Name,
                Content = File.ReadAllText(w.Dir + "/" + w.Entry)
            };
            foreach (string s in Directory.GetFiles(w.Dir))
            {
             //   Script ws = Script.Open(s);
               // SScript scr = new SScript()
              //  {
              //      Name = ws.Name,
              //      Content = ws.Source
             //   };
             //   sc.Scripts.Add(scr);
            }
            return sc;
        }
        public WComponent Import(WGameObject parent)
        {
            
            WComponent w = new WComponent();
            w.Entry = EntryPointName;
            w.PropertiesScriptPoint = PropertiesPointName;
            w.name = Name;
            w.ParentObject = parent;
            List<Script> slist = new List<Script>();
            foreach (SScript s in Scripts)
            {
                Script ws = new Script()
                {
                    Source = s.Content,
                    Name = s.Name
                };
                slist.Add(ws);
            }
            if (!Directory.Exists("Components"))
                Directory.CreateDirectory("Components");
            if (Directory.Exists("Components/" + w.name))
                Directory.Delete("Components/" + w.name, true);
            Directory.CreateDirectory("Components/" + w.name);
            w.Dir = "Components/" + w.name + "/";
            foreach (SScript scr in Scripts)
            {
                File.WriteAllText(w.Dir + scr.Name, scr.Content);
            }
            w.EntryPointScript = new Script()
            {
                Name = EntryPoint.Name,
                Source = EntryPoint.Content
            };
            w.PropertiesScript = new Script()
            {
                Name = Properties.Name,
                Source = Properties.Content
            };
            w.Scripts = slist.ToArray();
            w.GenerateProperties();
            return w;
        }*/
    }
}
