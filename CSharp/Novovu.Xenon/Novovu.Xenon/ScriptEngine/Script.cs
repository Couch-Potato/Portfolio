using Microsoft.ClearScript.V8;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Xenon.ScriptEngine
{
    public class Script
    {
        public string Source;

        public string Name;

        public object Parent;

        public Script(string src,object parent, string name="script_gen")
        {
            Name = name;
            Source = src;
            Parent = parent;
        }

        public V8ScriptEngine Enviornment;

        public void Run(Engine.Game g,string src = null)
        {
         
            Logging.Logger.Log(Logging.Logger.LogTypes.Message, this, "Started Script Init Process");
            if (src == null)
            {
                src = File.ReadAllText(Source);
            }
            Logging.Logger.Log(Logging.Logger.LogTypes.Message, this, "Assembled Script");
            V8ScriptEngine eng = APIBuilder.BuildAPIEngine(g, Name);
            eng.AddHostObject("Parent", Parent);
            Logging.Logger.Log(Logging.Logger.LogTypes.Message, this, "Built API Model");
            Enviornment = eng;
            try
            {
                eng.Execute(src);
            }
            catch (Microsoft.ClearScript.ScriptEngineException ex)
            {
                Logging.Logger.Log(Logging.Logger.LogTypes.Error, this, $"({Name}) "+ ex.ErrorDetails);
            }
            
            Logging.Logger.Log(Logging.Logger.LogTypes.Message, this, "Executed Script");
        }
    }
}
