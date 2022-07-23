using ProjectEmergencyFrameworkClient.Utility;
using System;
using System.Collections.Generic;
using CitizenFX.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;


namespace ProjectEmergencyFrameworkClient.Interfaces.UI
{
    [UserInterface("keybinds", false)]
    public class KeybindIndicator : UserInterface
    {
        public static Keymap Keys = new Keymap();

        [Configuration("keybindings")]
        public List<Keybind> Keybinds { get; set; }

        public KeybindIndicator()
        {
            Keybinds = new List<Keybind>();
        }

        public void AddKeybind(string keycode, string caption)
        {
            //DelKeybind(keycode, false);
            Keybinds.Add(new Keybind()
            {
                keycode = keycode,
                caption = caption
            });
            Update();
        }

        public void DelKeybind(string keycode, bool update = true)
        {
            int toDel = -1;
            foreach (var k in Keybinds)
            {
                toDel++;
                if (k.keycode == keycode)
                {
                    break;
                }
            }
            if (toDel > -1)
            {
                Keybinds.RemoveAt(toDel);
                if (update)
                    Update();
            }
        }
    }
    public static class KeybindService
    {
        public static KeybindIndicator KeybindInterface;
        [ExecuteAt(ExecutionStage.OnResourceStart)]
        public static void ShowKeybind()
        {
            KeybindInterface = new KeybindIndicator();
            KeybindInterface.Show();

        }
        private static List<string> ToClear = new List<string>();
        public static KeybindListeners Listeners = new KeybindListeners();

        public static void RegisterKeybind(string keybind, string caption, Action onexecute)
        {
            KeybindInterface.AddKeybind(keybind, caption);
            Listeners.Register(keybind, onexecute);
        }
        public static void RemoveKeybind(string key)
        {
            ToClear.Add(key);
            KeybindInterface.DelKeybind(key, false);
        }

        public static bool IsKeyDown(string key)
        {
            return IsControlPressed(0, KeybindIndicator.Keys[key]) || IsDisabledControlPressed(0, KeybindIndicator.Keys[key]);
        }
        public static bool IsKeyUp(string key)
        {
            return IsControlReleased(0, KeybindIndicator.Keys[key]) || IsDisabledControlReleased(0, KeybindIndicator.Keys[key]);
        }
        public static bool IsKeyJustUp(string key)
        {
            return IsControlJustReleased(0, KeybindIndicator.Keys[key]) || IsDisabledControlJustReleased(0, KeybindIndicator.Keys[key]);
        }

     
        public static async Task Tick()
        {
            foreach(var listener in Listeners)
            {
                if (!KeybindIndicator.Keys.ContainsKey(listener.Key))
                {
                    throw new Exception("NO KEYBIND REGISTRY EXISTS FOR: " + listener.Key);
                    //Debug.WriteLine("CRITICAL ERROR: NO KEYBIND REGISTRY EXISTS FOR: " + listener.Key);
                }
                if (IsControlJustReleased(0, KeybindIndicator.Keys[listener.Key]))
                {
                    listener.Value();
                }
            }
            bool wasSomethingCleared = ToClear.Count > 0;
            foreach (var item in ToClear)
            {
                Listeners.Unregister(item);
            }
            if (wasSomethingCleared)
                await KeybindInterface.UpdateAsync();
            ToClear.Clear();
        }
    }
    public class Keybind
    {
        public string keycode { get; set; }
        public string caption { get; set; }
    }
    public class Keymap : Dictionary<string, int>
    {
        public Keymap()
        {
            Add("E", 46);
            Add("Z", 48);
            Add("F", 23);
            Add("G", 47);
            Add("Q", 44);
        }
    }
    public class KeybindListeners : Dictionary<string, Action>
    {
        public void Register(string bind, Action a)
        {
            if (ContainsKey(bind))
            {
                this[bind] = a;
            }else
            {
                Add(bind, a);
            }
        }
        public void Unregister(string bind)
        {
            if (ContainsKey(bind))
                Remove(bind);
        }
    }
}
