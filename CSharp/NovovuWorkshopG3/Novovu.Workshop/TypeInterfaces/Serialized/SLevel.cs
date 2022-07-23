using Novovu.Xenon.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.TypeInterfaces.Serialized
{
    [Serializable]
    public class SLevel
    {
        public List<SGameObject> GameObjects = new List<SGameObject>();
        public static SLevel From(Level l)
        {
            SLevel sl = new SLevel();

            foreach (WGameObject wgo in l.Objects)
            {
                sl.GameObjects.Add(SGameObject.From(wgo));
            }

            return sl;
        }
    }
}
