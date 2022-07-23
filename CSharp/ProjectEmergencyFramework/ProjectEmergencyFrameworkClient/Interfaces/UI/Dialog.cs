using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interfaces.UI
{
    [UserInterface("dialog", true)]
    public class Dialog : UserInterface
    {
        private int _v=-1;
        private DialogConfig _cfg = new DialogConfig() { options=new List<string>(), prompt=new DialogPrompt() { icon="", name="LOADING", text="Please wait a moment..."} };

        [Reactive("selectedOption")]
        public int selected { get => _v; set => handleOptionSet(value); }

        [Configuration("dialogConfig")]
        public DialogConfig config { get=>_cfg; private set => _cfg = value; }

        protected override Task ConfigureAsync()
        {
            
            return base.ConfigureAsync();
        }

        private void handleOptionSet(int op)
        {
            OnOptionSelected?.Invoke(op);
        }

        public void setConfig(DialogConfig config)
        {
            this.config = config;
            Update();
        }

        public event Action<int> OnOptionSelected;
    }
    public class DialogConfig
    { 
        public List<string> options { get; set; }
        public DialogPrompt prompt { get; set; }
    }
    public class DialogPrompt
    {
        public string icon { get; set; }
        public string name { get; set; }
        public string text { get; set; }
    }
}
