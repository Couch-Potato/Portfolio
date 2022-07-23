using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Dock.Model;
using Novovu.Workshop.ProjectModel;
using Novovu.Workshop.Views;
using Novovu.Workshop.Workspace;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using Avalonia;
using Novovu.Workshop.DockSystem;

namespace Novovu.Workshop.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public const string DocumentsDockId = "Files";


        private IFactory _factory;
        private IDock _layout;

        public IFactory Factory
        {
            get => _factory;
            set => this.RaiseAndSetIfChanged(ref _factory, value);
        }

        public IDock Layout
        {
            get => _layout;
            set => this.RaiseAndSetIfChanged(ref _layout, value);
        }

        private Encoding GetEncoding(string path)
        {
            using (var reader = new StreamReader(path, Encoding.Default, true))
            {
                if (reader.Peek() >= 0)
                {
                    reader.Read();
                }
                return reader.CurrentEncoding;
            }
        }


        private Workspace.Workspace selectedWS;
        public Workspace.Workspace SelectedWS
        {
            get => selectedWS;
            set { 
                this.RaiseAndSetIfChanged(ref selectedWS, value);
                value.Link(this);
                var factory = new MainWindowDockFactory(value);
                Layout?.Close();
                Factory = factory;
                Layout = factory.CreateLayout();
                Factory.InitLayout(Layout);
            }
        }





        private Window GetWindow()
        {
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                return desktopLifetime.MainWindow;
            }
            return null;
        }

        private WorkspaceSelectorViewModel WorkspaceSelectorViewModel { get; }

        public string VSCodeVersion = "0000";

        public MainWindowViewModel()
        {
            WorkspaceRegistry.ActivationChanged += WorkspaceRegistry_ActivationChanged;
            WorkspaceSelectorViewModel = new WorkspaceSelectorViewModel(this);

            bool doInstallVS = false;
            if (!Directory.Exists("VSCode"))
            {
                doInstallVS = true;
            }else
            {
                if (!File.Exists("VSCode/.version"))
                {
                    doInstallVS = true;
                    Directory.Delete("VSCode", true);
                }else
                {
                    if (File.ReadAllText("VSCode/.version") != VSCodeVersion)
                    {
                        doInstallVS = true;
                        Directory.Delete("VSCode", true);
                    }
                }
            }
            if (doInstallVS)
            {
                InstallVSCode();
            }
            
        }
        private void WorkspaceRegistry_ActivationChanged(Workspace.Workspace w)
        {
            SelectedWS = w;
        }

        private void InstallVSCode()
        {
            LoadDialog d = new LoadDialog("Preparing VS Code...");
            d.ShowDialog(ProjectStatic.MainWindow);
            WebClient wc = new WebClient();
            var tmp = Path.GetTempFileName();
            wc.DownloadProgressChanged += (object sender, DownloadProgressChangedEventArgs e)=>
            {
                d.SetStatus($"Downloading new binaries... ({(e.BytesReceived/1024)/1024}MB/{(e.TotalBytesToReceive / 1024)/1024}MB)", e.ProgressPercentage * .9);
                if (e.ProgressPercentage == 100)
                {
                    d.SetStatus($"Extracting...", 90);
                }
            };
            wc.DownloadFileCompleted += (object sender, System.ComponentModel.AsyncCompletedEventArgs e)=> {
                
                ZipFile.ExtractToDirectory(tmp, AppDomain.CurrentDomain.BaseDirectory);
                File.WriteAllText("VSCode/.version", VSCodeVersion);
                File.Delete(tmp);
                d.Close();
            };
            wc.DownloadFileAsync(new Uri("https://artifacts.novovu.com/VSCode.zip"), tmp);
        }


  
        public string Greeting => "Welcome to Avalonia!";
    }
}
