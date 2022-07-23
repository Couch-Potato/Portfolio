using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp.WinForms;
using CefSharp;
using Novovu.Argon;
using System.Diagnostics;

namespace Novovu.Argon.WinForms
{
    public partial class ArgonCompDisplay : UserControl
    {
        ChromiumWebBrowser webBrowser = new ChromiumWebBrowser();
        public ArgonCompDisplay()
        {
            InitializeComponent();
            CefSharpSettings.WcfEnabled = true;
            this.Resize += ArgonCompDisplay_Resize;
        }

        private void ArgonCompDisplay_Resize(object sender, EventArgs e)
        {
            //webBrowser.Size = this.Size;
            //webBrowser.Refresh();
        }

        public Size RequestedSize
        {
            get
            {
                return webBrowser.Size;
            }
        }

        private string srctext = "";
        public void RunComposition(IArgonCompositionBuild comp)
        {
            Controls.Add(webBrowser);
            webBrowser.Dock = DockStyle.Top;
            webBrowser.Size = this.Size;
            string text = comp.Build(webBrowser);
            srctext = text;
            webBrowser.IsBrowserInitializedChanged += WebBrowser_IsBrowserInitializedChanged;

            webBrowser.LoadingStateChanged += WebBrowser_LoadingStateChanged;
        }

        private void WebBrowser_IsBrowserInitializedChanged(object sender, EventArgs e)
        {
  
            if (webBrowser.IsBrowserInitialized)
            {
                srctext += @"<style>body{overflow:hidden;}<style>";
                webBrowser.GetBrowser().MainFrame.LoadHtml(srctext);
            }
        }

        private void WebBrowser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (e.IsLoading == false)
            {
                
                //CompositionLoaded.Invoke(this, new EventArgs());
            }
        }

        //public event EventHandler CompositionLoaded;
    }
}
