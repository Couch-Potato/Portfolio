using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ClearScript.JavaScript;
using Microsoft.ClearScript.V8;


namespace Novovu.Xenon.ScriptEngine
{
    public class ScriptManager
    {
        public List<Script> scripts = new List<Script>();
    }
    public class Printer
    {
        string scr_name;

        public Printer(string nme)
        {
            scr_name = nme;
        }

        public void Print(string message)
        {
            Logging.Logger.Log(Logging.Logger.LogTypes.Script, this, message);
        }
    }
    public class APIBuilder
    {
        public static V8ScriptEngine BuildAPIEngine(Engine.Game g, string scriptName)
        {
            V8ScriptEngine engine = new V8ScriptEngine();
            engine.AddHostObject("Game", new API.Game(g));
            engine.AddHostType(typeof(Microsoft.Xna.Framework.Vector3));
            engine.AddHostObject("Output", new Printer(scriptName));
            //Add event manager
            //engine.AddHostObject("UI")
            foreach (ScriptEvent ev in ScriptEventManager.GlobalEvents)
            {
                engine.AddHostObject(ev.Name, ev);
            }
            return engine;
        }
    }
}
