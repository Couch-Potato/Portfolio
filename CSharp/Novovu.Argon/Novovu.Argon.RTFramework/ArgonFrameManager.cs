using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Argon.RTFramework
{
    public class ArgonFrameManager
    {
        ArgonUIPackage package;
        Dictionary<ArgonUI, bool> uiframes = new Dictionary<ArgonUI, bool>();
        public event EventHandler ArgonFormsLoaded;
        public ArgonFrameManager(ArgonUIPackage pck)
        {
            package = pck;
            
        }

        public void AddWebFrame(string compostionn, ArgonUI frame)
        {
            ArgonComposition comp = package.Compositions[compostionn];
            uiframes.Add(frame, false);

            frame.ArgonFormLoaded += Frame_ArgonFormLoaded;

            frame.LoadUI(comp);
           
        }

        private void Frame_ArgonFormLoaded(object sender, EventArgs e)
        {
            if (ArgonFormsLoaded != null)
            {
                uiframes[(ArgonUI)sender] = true;
                bool allFound = true;
                foreach (KeyValuePair<ArgonUI, bool> kvp in uiframes)
                {
                    if (kvp.Value == false)
                    {
                        allFound = kvp.Value;
                    }
                }
                if (allFound)
                {
                    ArgonFormsLoaded.Invoke(this, new EventArgs());
                }
            }
        }
    }
}
