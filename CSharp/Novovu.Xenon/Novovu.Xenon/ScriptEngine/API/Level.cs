using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Xenon.ScriptEngine.API
{
    public class Level
    {

        #region Variable Holders

        

        //private Engine.Light _ambient;

        private List<Audio.StaticSound> _static;

        //private Camera _camera;

        private Engine.Level LevelReference;
        #endregion

        #region Properties
        public Camera Camera;

        public List<GameObject> Objects = new List<GameObject>();

        public List<Audio.StaticSound> StaticSounds
        {
            get
            {
                _static = LevelReference.StaticSounds;
                return _static;
            }
            set
            {
                _static = value;
                LevelReference.StaticSounds = _static;
            }
        }

        #endregion

       // public UIContainer GetUIContainer(string Name)
        //{
          //  foreach (UI.UIContainerHost host in LevelReference.UIContainers)
          //  {
          //      if (Name == host.ContainerName)
          //          return new UIContainer(host);
          //  }
        //    return null;
       // }


        #region Constructor

        public Level(Engine.Level level)
        {
            Camera = new Camera(level.Camera);
            LevelReference = level;
            foreach (Engine.GameObject obj in level.Objects)
            {
                Objects.Add(new GameObject(obj));
            }
            _static = level.StaticSounds;
        }

        #endregion
    }
}
