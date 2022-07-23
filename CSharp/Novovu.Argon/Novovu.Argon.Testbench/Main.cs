using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sushi.Build;
namespace Novovu.Argon.Testbench
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            InitializeChromium();
        }
        public void InitializeChromium()
        {
            CefSettings settings = new CefSettings();
            // Initialize cef with the provided settings
            Cef.Initialize(settings);
            // Create a browser component
            chromeBrowser = new ChromiumWebBrowser("https://novovu.com");
            // Add it to the form and fill it to the form window.
            this.Controls.Add(chromeBrowser);
            chromeBrowser.Dock = DockStyle.Fill;
        }
        public ChromiumWebBrowser chromeBrowser;
        private async Task Form1_Load(object sender, EventArgs e)
        {
            Sushi.Build.Sushi sushicomp = new Sushi.Build.Sushi(false);
            await sushicomp.sushiIsAwesome<bool>(new string[] { "sushay" });
        }

       
    }
}
