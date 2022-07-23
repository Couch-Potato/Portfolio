using Novovu.Xenon.Engine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.TypeInterfaces.Serialized
{
    [Serializable]
    public class SGameObject
    {
        public SVector3 Position;
        public SVector3 Orientation;
        public SVector3 Scale;
        public string Name;
        public List<SBasicModel> Models = new List<SBasicModel>();
        public List<SComponent> Components = new List<SComponent>();
        public List<SGameObject> Children = new List<SGameObject>();

        public static SGameObject From(WGameObject w)
        {
            Debug.WriteLine(w.Name);
            SGameObject sgo = new SGameObject();
            sgo.Name = w.Name;
            sgo.Orientation = SVector3.From(w.OrientationOffset);
            sgo.Position = SVector3.From(w.Position);
            sgo.Scale = SVector3.From(w.Scale);
            foreach (WBasicModel mdl in w.Models)
            {
                sgo.Models.Add(SBasicModel.From(mdl));
            }
            foreach (WComponent wc in w.Components)
            {
                //sgo.Components.Add(SComponent.From(wc));
            }
            
            return sgo;
        }
        public WGameObject Import(Level parent)
        {
            WGameObject wg = new WGameObject();
            wg.Name = Name;
            wg.OrientationOffset = Orientation.Import();
            wg.Scale = Scale.Import();
            wg.Position = Position.Import();
            wg.LevelParent = parent;
            foreach (SBasicModel m in Models)
            {
                wg.Models.Add(m.Import(wg));
            }
            foreach (SComponent wc in Components)
            {
                //wg.Components.Add(wc.Import(wg));
            }

            return wg;
        }
    }
}
