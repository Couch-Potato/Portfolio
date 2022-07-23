using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Xenon.Assets
{
    public class AssetLoader : ContentManager
    {
        public AssetLoader(IServiceProvider serviceProvider, string rootDirectory) : base(serviceProvider, rootDirectory)
        {
        }

        protected override Dictionary<string, object> LoadedAssets => base.LoadedAssets;

        protected override Stream OpenStream(string hash)
        {
            if (AssetManager.HashedAssets.ContainsKey(hash))
            {
                return AssetManager.HashedAssets[hash].AssetStream;
            }else
            {
                throw new InvalidAssetHashException(hash);
            }
            
        }
       
    }
    public class InvalidAssetHashException : Exception
    {
        public InvalidAssetHashException(string hash)
            : base($"The asset with the hash {hash} cannot be found in the asset cache")
        { }
    }
}
