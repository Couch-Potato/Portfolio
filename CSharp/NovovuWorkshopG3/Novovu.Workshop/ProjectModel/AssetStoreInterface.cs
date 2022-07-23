using Avalonia.Shared.PlatformSupport;
using Novovu.Workshop.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.ProjectModel
{
    public class AssetStoreInterface
    {
        public static Xenon.Assets.AssetLoader AssetLoader;
        public static void InitAssetLoader(IServiceProvider provider)
        {
            AssetLoader = new Xenon.Assets.AssetLoader(provider, "");
        }
        public static Xenon.Assets.Asset CompileAssetItem(string item, Xenon.Assets.ParamSet set = null)
        {
            var temp = Path.GetTempFileName();
            if (!Directory.Exists("assets"))
            {
                Directory.CreateDirectory("assets");
            }

            if (File.Exists($"assets/{new FileInfo(item).Name}"))
                File.Delete($"assets/{new FileInfo(item).Name}");
            File.Copy(item, $"assets/{new FileInfo(item).Name}");
            string asset = "";
            try
            {
               // asset =await  Xenon.Assets.ContentCompiler.CompileContentByFileAsync($"assets/{new FileInfo(item).Name}", temp, "assets", set);
            }
            catch (Exception ex)
            {
                //throw new ImportFailureException(ex.)
            }
            
            try
            {

                using (Stream s = File.OpenRead(asset))
                {
                    MemoryStream AssetStream = new MemoryStream();
                    s.CopyTo(AssetStream);
                    return Xenon.Assets.AssetManager.Insert(AssetStream);
                }
            }
            catch
            {
                throw new ImportFailureException(item);
            }

        }
        public static async Task<Xenon.Assets.Asset> CompileAssetItemAsync(string item, Xenon.Assets.ParamSet set = null)
        {
            var temp = Path.GetTempFileName();
            if (!Directory.Exists("assets"))
            {
                Directory.CreateDirectory("assets");
            }
            if (File.Exists($"assets/{new FileInfo(item).Name}"))
                File.Delete($"assets/{new FileInfo(item).Name}");
            File.Copy(item, $"assets/{new FileInfo(item).Name}");
            try
            {
                string asset = await Xenon.Assets.ContentCompiler.CompileContentByFileAsync($"assets/{new FileInfo(item).Name}", temp, "assets", set);

                Debug.WriteLine(new FileInfo(item).Name);

                using (Stream s = File.OpenRead(asset))
                {

                    MemoryStream AssetStream = new MemoryStream();
                    s.CopyTo(AssetStream);
                    AssetStream.Seek(0, SeekOrigin.Begin);
                    return Xenon.Assets.AssetManager.Insert(AssetStream);
                }
                
            }
            catch (Exception ex)
            {
                File.Delete(temp);
                await new ErrorDialog(ex).ShowDialog(ProjectStatic.MainWindow);
                Environment.Exit(1);
                throw new ImportFailureException(item);

            }
           
        }
    }
    public class ImportFailureException : Exception
    {
        public ImportFailureException(string name)
            : base("The item: " + name + " could not be imported into the workshop due to a failure when preparing it for import into the asset cache.")
        {
        }

    }
}
