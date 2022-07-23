using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Novovu.Xenon.Engine;

namespace Novovu.Xenon.Materials
{
    [Obsolete("Removed static materials in newest Xenon version", true)]
    public class ToonMaterial : IMaterial
    {
        #region Fields
        private float[] _Thresholds = { 0.95f, 0.5f, 0.2f, 0.03f };
        private float[] _BrightnessLevels = { 1.0f, 0.8f, 0.6f, 0.35f, 0.01f };

        private Effect MaterialEffect;

        private Texture2D _MaterialTexture;
        #endregion
        #region Properties
        public float[] Thresholds {
            get { return _Thresholds; }
            set {
                _Thresholds = value;
                MaterialEffect.Parameters["ToonThresholds"].SetValue(value);
            } }
        public float[] BrightnessLevels
        {
            get { return _BrightnessLevels; }
            set
            {
                _BrightnessLevels = value;
                MaterialEffect.Parameters["ToonBrightnessLevels"].SetValue(value);
            }
        }
        public Texture2D MaterialTexture
        {
            get { return _MaterialTexture; }
            set
            {
                _MaterialTexture = value;
                MaterialEffect.Parameters["colorTexture"].SetValue(value);
            }
        }
        #endregion


        public ToonMaterial()
        {
            MaterialEffect = Engine.Engine.CoreContentManager.Load<Effect>("ToonMaterial");
        }

        public Effect GetMaterialEffect(Matrix World, Matrix View, Matrix Projection, Matrix transpose)
        {
            
            return MaterialEffect;
        }
    }
}
