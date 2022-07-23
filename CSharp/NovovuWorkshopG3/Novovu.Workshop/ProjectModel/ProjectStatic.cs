using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media.Imaging;
using Novovu.Workshop.TypeInterfaces;
using Novovu.Workshop.TypeInterfaces.Serialized;
using Novovu.Xenon.Engine;
using Novovu.Xenon.Renderer.Helper;
using Novovu.Xenon.XEF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.ProjectModel
{
    public class ProjectStatic
    {
        public static string LoggedInUsername;
        public static string LoggedInUserDescription;
        public static Stream LoggedInUserImage;

        public static Window MainWindow;
        public static async Task<string[]> OpenFile(List<FileDialogFilter> filter, string title = "Select a file", Window window = null)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.AllowMultiple = true;
            of.Title = title;
            of.Filters = filter;

            var wdw = window != null ? window : MainWindow;
            return await of.ShowAsync(wdw);
        }
        public static async Task<string> SaveFile(List<FileDialogFilter> filter, string title = "Save file")
        {
            SaveFileDialog sf = new SaveFileDialog()
            {
                Filters = filter,
                Title = title
            };
            return await sf.ShowAsync(MainWindow);
        }
        public static List<FileDialogFilter> ModelImportFilter
        {
            get
            {
                List<FileDialogFilter> f = new List<FileDialogFilter>();
                f.Add(new FileDialogFilter() { Extensions = {"fbx", "obj"}, Name="Model Files" });
                return f;
            }
        }
        public delegate void RecallProperties();
        public static event RecallProperties RecallAllProperties;
        public static void Recall()
        {
            RecallAllProperties?.Invoke();
        }
        public static List<FileDialogFilter> NovovuObjectFilter
        {
            get
            {
                List<FileDialogFilter> f = new List<FileDialogFilter>();
                f.Add(new FileDialogFilter() { Extensions = { "nvo" }, Name = "Novovu GameObjects" });
                return f;
            }
        }
        public static List<FileDialogFilter> NovovuComponentFilter
        {
            get
            {
                List<FileDialogFilter> f = new List<FileDialogFilter>();
                f.Add(new FileDialogFilter() { Extensions = { "nvc" }, Name = "Novovu Component" });
                return f;
            }
        }
        public static List<FileDialogFilter> NovovuLevelFilter
        {
            get
            {
                List<FileDialogFilter> f = new List<FileDialogFilter>();
                f.Add(new FileDialogFilter() { Extensions = { "nvl" }, Name = "Novovu Level" });
                return f;
            }
        }
        public static List<FileDialogFilter> TextureFilter
        {
            get
            {
                List<FileDialogFilter> f = new List<FileDialogFilter>();
                f.Add(new FileDialogFilter() { Extensions = { "jpg","jpeg","png" }, Name = "Texture" });
                return f;
            }
        }

        public delegate void ObjectEdit(object o);
        public static event ObjectEdit ObjectRemoveInvoke;
        public static void InvokeRemoval(object o)
        {
            ObjectRemoveInvoke?.Invoke(o);
        }

        public static event ObjectEdit ObjectDuplicateInvoke;

        public static void InvokeDuplicate(object o)
        {
            ObjectDuplicateInvoke?.Invoke(o);
        }

        public static event ObjectEdit ObjectClipboardCopyInvoke;

        public static void InvokeObjectClipboardCopy(object o)
        {
            ObjectClipboardCopyInvoke?.Invoke(o);
        }

        public static event ObjectEdit ObjectClipboardPasteInvoke;

        public static void InvokeObjectClipboardPaste(object o)
        {
            ObjectClipboardPasteInvoke?.Invoke(o);
        }
        public static void RenderLevel(Level l)
        {
            var path = System.IO.Path.GetTempFileName();
            Xenon.Files.SerializationProvider.Serializer = new Xenon.Files.NovovuSerializer();
            var file = Xenon.Files.SerializationProvider.Serializer.GetFile<Level>(l);
            //new Xenon.Remote.TestServer("nvt");
           // file.Save("level.nb", false);
            file.Save(path);
            var proc = new Process();
            proc.StartInfo.Arguments = "--preview 1745 981 " + path;
            proc.StartInfo.WorkingDirectory = "Rendex/win";
            proc.StartInfo.FileName = new FileInfo("Rendex/win/Novovu.Xenon.Rendex.exe").FullName;
            proc.StartInfo.RedirectStandardOutput = false;
            proc.StartInfo.CreateNoWindow = false;
            proc.StartInfo.ErrorDialog = true;
            proc.StartInfo.UseShellExecute = false;
            proc.Start();
        }
        [Obsolete("Material rendering needs to be done asynchornously otherwise there will be errors thrown.", true)]
        public static IBitmap RenderMaterial(MaterialEffect e)
        {
            var path = System.IO.Path.GetTempFileName();
            Xenon.Files.SerializationProvider.Serializer = new Xenon.Files.NovovuSerializer();
            var file = Xenon.Files.SerializationProvider.Serializer.GetFile(e);
            file.Save(path);
            var output = System.IO.Path.GetTempFileName();
            var proc = new Process();
            proc.StartInfo.Arguments = $"--render 512 512 {output} {path}";
            proc.StartInfo.WorkingDirectory = "Rendex/win";
            proc.StartInfo.FileName = new FileInfo("Rendex/win/Novovu.Xenon.Rendex.exe").FullName;
            proc.StartInfo.RedirectStandardOutput = false;
            proc.StartInfo.CreateNoWindow = false;
            proc.StartInfo.ErrorDialog = true;
            proc.StartInfo.UseShellExecute = false;
            proc.Start();
            proc.WaitForExit();

            return new Bitmap(output);
        }
        public static async Task<IBitmap> RenderMaterialAsync(MaterialEffect e)
        {
            // Handle the task on the UI thread
            var path = System.IO.Path.GetTempFileName();
            await Threading.PostUITask(() =>
            {
                
                Xenon.Files.SerializationProvider.Serializer = new Xenon.Files.NovovuSerializer();
                var file = Xenon.Files.SerializationProvider.Serializer.GetFile(e);
                file.Save(path);
            });
            var output = System.IO.Path.GetTempFileName();
            var proc = new Process();
            proc.StartInfo.Arguments = $"--render 512 512 {output} {path}";
            proc.StartInfo.WorkingDirectory = "Rendex/win";
            proc.StartInfo.FileName = new FileInfo("Rendex/win/Novovu.Xenon.Rendex.exe").FullName;
            proc.StartInfo.RedirectStandardOutput = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.ErrorDialog = true;
            proc.StartInfo.UseShellExecute = false;
            proc.Start();
            proc.WaitForExit();

            return new Bitmap(output);
        }
        public static void RunLevel(Level l)
        {
            var path = System.IO.Path.GetTempFileName();
            Xenon.Files.SerializationProvider.Serializer = new Xenon.Files.NovovuSerializer();
            var file = Xenon.Files.SerializationProvider.Serializer.GetFile<Level>(l);
            //new Xenon.Remote.TestServer("nvt");
            // file.Save("level.nb", false);
            file.Save(path);
            var proc = new Process();
            proc.StartInfo.Arguments = "--run 1745 981 " + path;
            proc.StartInfo.WorkingDirectory = "Rendex/win";
            proc.StartInfo.FileName = new FileInfo("Rendex/win/Novovu.Xenon.Rendex.exe").FullName;
            proc.StartInfo.RedirectStandardOutput = false;
            proc.StartInfo.CreateNoWindow = false;
            proc.StartInfo.ErrorDialog = true;
            proc.StartInfo.UseShellExecute = false;
            proc.Start();
        }
        public static void ExportObject(WGameObject obj, string location)
        {
            Xenon.Files.SerializationProvider.Serializer = new Xenon.Files.NovovuSerializer();
            var file = Xenon.Files.SerializationProvider.Serializer.GetFile<GameObject>(obj);
            file.Save(location);
        }
        public static void ExportLevel(Level obj, string location)
        {
            Xenon.Files.SerializationProvider.Serializer = new Xenon.Files.NovovuSerializer();
            var file = Xenon.Files.SerializationProvider.Serializer.GetFile<Level>(obj);
            file.Save(location);
        }
        public static void ExportComponent(WComponent wc, string location)
        {
            Xenon.Files.SerializationProvider.Serializer = new Xenon.Files.NovovuSerializer();
            var file = Xenon.Files.SerializationProvider.Serializer.GetFile<Component>(wc);
            file.Save(location);
        }

        public static WGameObject ImportObject(string path, Level Parent)
        {
            Xenon.Files.SerializationProvider.Serializer = new Xenon.Files.NovovuSerializer();
            var file = Xenon.Files.XenonFile.FromStream(File.OpenRead(path));
            var l = Xenon.Files.SerializationProvider.Serializer.Open<GameObject>(file);
            l.LevelParent = Parent;
            return (WGameObject)l;
        }
        public static Level ImportLevel(string path)
        {
            Xenon.Files.SerializationProvider.Serializer = new Xenon.Files.NovovuSerializer();
            var file = Xenon.Files.XenonFile.FromStream(File.OpenRead(path));
            var l = Xenon.Files.SerializationProvider.Serializer.Open<Level>(file);
            return l;
        }
    }
}
