using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Xenon.ScriptEngine
{
    /// <summary>
    /// Handles all global events that deal with the engine itself.
    /// Usage (JS): Input.OnKeyDown.Event.connect(function(){});
    /// </summary>
    public class ScriptEventManager
    {
        public static List<ScriptEvent> GlobalEvents = new List<ScriptEvent>();
    }
    public class ScriptEvent
    {
        public delegate void ScriptEventHandle(params object[] handles);
        public event ScriptEventHandle Event;
        public string Name;
        private bool BackwardInvokeEnabled;
        public ScriptEvent(string Name, bool backwardInvoke = false, ScriptEventHandle handler=null)
        {
            if (backwardInvoke)
            {
                Event += handler;
            }else
            {
                Event += HandleBackwardsInvoke;
            }
            this.Name = Name;
        }
        public void Invoke(params object[] handles)
        {
            Event.Invoke(handles);
        }
        private void HandleBackwardsInvoke(params object[] handles)
        {
            Logging.Logger.Log(Logging.Logger.LogTypes.Warning, this, "Cannot backwards invoke on event: " + Name);
        }
    }
}
