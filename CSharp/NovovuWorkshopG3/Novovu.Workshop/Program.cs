using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.ReactiveUI;
using Microsoft.ClearScript.JavaScript;
using Novovu.Workshop.ProjectModel;
using Novovu.Workshop.Views.Dialogs;
using Novovu.Xenon.Helper;
using Sentry;

namespace Novovu.Workshop
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.


        static bool DebugMode = true;
        public static void Main(string[] args) {
            if (!DebugMode)
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
            }

            ProjectStatic.LoggedInUsername = "bluejay";
            ProjectStatic.LoggedInUserImage = StreamHelper.BufferToStream(new WebClient().DownloadData("https://api.novovu.com/game/avatar/icon/5"));
            ProjectStatic.LoggedInUserDescription = "QA TESTER";

            using (SentrySdk.Init("https://d7426bad4220447aae428acc364d5642@o255256.ingest.sentry.io/5307915"))
            {
                BuildAvaloniaApp()
           .StartWithClassicDesktopLifetime(args);
            }
            
            
                

            
        }

        private static void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
           
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            File.WriteAllText("errorlogs.txt", ((Exception)e.ExceptionObject).Message);
            Process.Start("notepad.exe", "errorlogs.txt");
            Environment.Exit(1);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug()
                .UseReactiveUI();
    }
}
