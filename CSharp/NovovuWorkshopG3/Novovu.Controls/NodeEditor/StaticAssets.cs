using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Novovu.Controls.NodeEditor.DefaultNodes;
using Novovu.Xenon.Renderer.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Controls.NodeEditor
{
    public class StaticAssets
    {
        public static Bitmap ConnectionUnfilled;
        public static Bitmap ConnectionFilled;
        public static Bitmap ConnectionHighlight;
        public static Bitmap TextureDefault;

        public delegate string OpenImageFileHandler();
        public static Func<Task<string>> ResolveImageFileHandler = async ()=> { throw new NotImplementedException("This delegate must be handled before it can be called."); };

        public delegate IBitmap RenderMaterialHandler();
        public static Func<MaterialEffect, Task<IBitmap>> RenderMaterialFileHandler = async (MaterialEffect effect) => { throw new NotImplementedException("This delegate must be handled before it can be called."); };

        public static Func<MaterialNode, MaterialEffect> NodeToEffect = (MaterialNode node)=>{ throw new NotImplementedException("This delegate must be handled before it can be called."); };
        public async static Task<string> ResolveImageFile()
        {
            return await ResolveImageFileHandler();
        }
        public async static Task<IBitmap> RenderMaterial(MaterialEffect material)
        {
            return await RenderMaterialFileHandler(material);
        }
        public static MaterialEffect ConvertNodeToEffect(MaterialNode node)
        {
            return NodeToEffect(node);
        }

        public static bool IsLoaded = false;
        public static void Load()
        {
            if (IsLoaded)
                return;
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            ConnectionUnfilled = new Bitmap(File.OpenRead("Assets/connection_unfilled.png"));
            ConnectionFilled = new Bitmap(File.OpenRead("Assets/connection_filled.png"));
            ConnectionHighlight = new Bitmap(File.OpenRead("Assets/connection_highlight.png"));
            TextureDefault = new Bitmap(File.OpenRead("Assets/tex_default.jpg"));

            //ConnectionUnfilled = new Bitmap(assets.Open(new Uri("avares://NodeEditorTest/Assets/connection_unfilled.png")));
            // ConnectionFilled = new Bitmap(assets.Open(new Uri("avares://NodeEditorTest/Assets/connection_filled.png")));
            //  ConnectionHighlight = new Bitmap(assets.Open(new Uri("avares://NodeEditorTest/Assets/connection_highlight.png")));
            IsLoaded = true;
        }
    }
}
