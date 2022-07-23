using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Xenon.Assets
{
    public class AssetManager
    {
        public static Dictionary<string, Asset> HashedAssets = new Dictionary<string, Asset>();
        public static Asset Insert(Stream s)
        {
           
            string hashCode = String.Format("{0:X}", s.GetHashCode());
            Logging.Logger.Log(Logging.Logger.LogTypes.Message, new AssetManager(), "Importing asset: " + hashCode);
            Asset a = new Asset(hashCode, s);
            HashedAssets.Add(hashCode, a);
            return a;
        }

    }
    public class Asset
    {
        public bool Onloaded = true;
        private Stream assetMemStream;
        public Stream AssetStream {
            get {
                if (Onloaded)
                {
                    return assetMemStream;
                }else
                {
                    LoadAsset();
                    return assetMemStream;
                }
            }
        }
        public string Hash;
        public void OffloadAsset()
        {
            Logging.Logger.Log(Logging.Logger.LogTypes.Message, this, "Offloading asset hash:" + Hash);
            if (!Directory.Exists("assets"))
            {
                Directory.CreateDirectory("assets");
            }
            if (File.Exists("assets/" + Hash + ".asset"))
            {
                File.Delete("assets/" + Hash + ".asset");
                Logging.Logger.Log(Logging.Logger.LogTypes.Warning, this, "While trying to offload an asset, it was found that there already existed an offloaded asset... Yikes. Continuing...");
            }

            using (FileStream s = File.Create("assets/" + Hash + "H.asset"))
            {
                assetMemStream.Seek(0, SeekOrigin.Begin);
                assetMemStream.CopyTo(s);
            }
            Onloaded = false;
            assetMemStream.Flush();
            assetMemStream.Dispose();
        }
        public void LoadAsset()
        {
            Logging.Logger.Log(Logging.Logger.LogTypes.Message, this, "Loading offloaded asset: " + Hash);
            assetMemStream = new MemoryStream();
            using (Stream s = File.Open("assets/" + Hash + "H.asset", FileMode.Open))
            {
                s.CopyTo(assetMemStream);
            }
            assetMemStream.Seek(0, SeekOrigin.Begin);
            File.Delete("assets/" + Hash + ".asset");
        }
        public Asset(string hash, Stream mem)
        {
            Hash = hash;
            assetMemStream = mem;
            assetMemStream.Seek(0, SeekOrigin.Begin);
            OffloadAsset();
        }
        
    }
}
