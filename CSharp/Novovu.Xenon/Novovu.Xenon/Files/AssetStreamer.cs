using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Xenon.Files
{
    class AssetStreamer : ContentManager
    {
        public AssetStreamer(IServiceProvider serviceProvider, string rootDirectory) : base(serviceProvider, rootDirectory)
    {
    }

    protected override Dictionary<string, object> LoadedAssets => base.LoadedAssets;

    protected override Stream OpenStream(string assetName)
    {
        string location = new FileInfo(assetName).FullName;

        return File.OpenRead(location);
    }
}
}
