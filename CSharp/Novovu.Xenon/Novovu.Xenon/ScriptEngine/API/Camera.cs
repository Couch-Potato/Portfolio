using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Xenon.ScriptEngine.API
{
    public class Camera
    {
        #region Variable Holders
        private Vector3 _pos;
        private Vector3 _orein;
        private Engine.Camera CameraObject;
        private float _scale;
        #endregion

        #region Properties
        public Vector3 Position
        {
            get
            {
                _pos = CameraObject.ViewLocation;
                return _pos;
            }
            set
            {
                _pos = value;
                CameraObject.ViewLocation = _pos;
            }
        }
        public Vector3 Orientation
        {
            get
            {
                _orein = CameraObject.LookAtVector;
                return _pos;
            }
            set
            {
                _orein = value;
                CameraObject.LookAtVector = _orein;
            }
        }
        public float ViewScale
        {
            get
            {
                _scale = CameraObject.ViewScale;
                return _scale;
            }
            set
            {
                _scale = value;
                CameraObject.ViewScale = value;
            }
        }
        #endregion

        #region Constructor
        public Camera(Engine.Camera cam)
        {
            _pos = cam.ViewLocation;
            _orein = cam.LookAtVector;
            _scale = cam.ViewScale;

            CameraObject = cam;
        }
        #endregion
    }
}
