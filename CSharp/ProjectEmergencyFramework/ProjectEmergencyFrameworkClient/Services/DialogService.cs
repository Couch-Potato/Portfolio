using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Services
{
    public class DialogService
    {
        private static Interfaces.UI.Dialog Instance;
        public static void ShowDialog(DialogHandler handler)
        {
            Instance = new Interfaces.UI.Dialog();
            Instance.OnOptionSelected += (int i) =>
            {
                handler.Prompt.Options[i].Action(handler);
            };
            Instance.setConfig(new Interfaces.UI.DialogConfig()
            {
                options = handler.Prompt.ToStrList(),
                prompt = new Interfaces.UI.DialogPrompt()
                {
                    name = handler.Name,
                    icon = handler.Icon,
                    text = handler.Prompt.Prompt
                }
            });
            Instance.Show();
        }
        public static void HideAllDialogs()
        {
            Instance.Hide();
        }

    }
    public class DialogHandler
    {
        public string Name { get; set; }
        public string Icon { get; set; }

        public DialogPrompt Prompt { get; set; }
        public DialogHandler() { }
        public DialogHandler(DialogHandler parent, DialogPrompt prompt)
        {
            Name = parent.Name;
            Icon = parent.Icon;
            Prompt = prompt;
        }
    }
    public class DialogPrompt
    {
        public string Prompt { get; set; }
        public DialogOption[] Options { get; set; }

        public DialogPrompt(string prompt, DialogOption[] option)
        {
            Prompt = prompt;
            Options = option;
        }
        public List<string> ToStrList()
        {
            List<string> d = new List<string>();
            foreach (var option in Options)
            {
                d.Add(option.Text);
            }
            return d;
        }
    }
    public class DialogOption
    {
        public Action<DialogHandler> Action { get; set; }
        public string Text { get; set; }
        public DialogOption(string text, Action<DialogHandler> action)
        {
            Text = text;
            Action = action;
        }
        public void Invoke(DialogHandler parent)
        {
            Action(parent);
        }
    }
}
