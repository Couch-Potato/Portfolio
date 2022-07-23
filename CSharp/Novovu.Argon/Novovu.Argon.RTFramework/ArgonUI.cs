using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
namespace Novovu.Argon.RTFramework
{
    public partial class ArgonUI: UserControl
    {
        ChromiumWebBrowser host;
        public event EventHandler ArgonFormLoaded;
        public void LoadUI(ArgonComposition comp)
        {
            host = new ChromiumWebBrowser();
            host.LoadHtml(comp.Source);
            host.LoadingStateChanged += Host_LoadingStateChanged;
        }
        bool loadFinished = false;
        private void Host_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (e.IsLoading == false && !loadFinished)
            {
                loadFinished = true;
                ArgonFormLoaded.Invoke(this, null);
            }
        }

        public ArgonUI()
        {
            InitializeComponent();
        }
    }
}
