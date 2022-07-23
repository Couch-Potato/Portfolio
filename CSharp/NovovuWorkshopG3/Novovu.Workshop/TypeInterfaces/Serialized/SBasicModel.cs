using Microsoft.Xna.Framework.Graphics;
using Novovu.Workshop.ProjectModel;
using Novovu.Xenon.Assets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.TypeInterfaces.Serialized
{
    [Serializable]
    public class SBasicModel
    {
        public SVector3 Position;
        public SVector3 Orientation;
        public SVector3 Scale;
        public string Name;
        public Stream Model;

        public static SBasicModel From(WBasicModel w)
        {
            var x = new SBasicModel
            {
                Position = SVector3.From(w.Position),
                Orientation = SVector3.From(w.OrientationOffset),
                Scale = SVector3.From(w.Scale),
                Name = w.Name,
                Model = AssetManager.HashedAssets[w.ModelHash].AssetStream
            };
            //AssetManager.HashedAssets[w.ModelHash].OffloadAsset();
            return x;
        }
        public WBasicModel Import(WGameObject parent)
        {
            var x = new WBasicModel();
            x.Position = Position.Import();
            x.OrientationOffset = Orientation.Import();
            x.Scale = Scale.Import();
            x.Name = Name;
            Debug.WriteLine(Model.Length);
            var a = AssetManager.Insert(Model);
            x.GameModel = AssetStoreInterface.AssetLoader.Load<Model>(a.Hash);
            x.ModelHash = a.Hash;
            x.ParentObject = parent;
            return x;
        }
    }
}
