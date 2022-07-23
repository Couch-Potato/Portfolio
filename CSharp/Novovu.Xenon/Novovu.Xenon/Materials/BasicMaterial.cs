using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Novovu.Xenon.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Xenon.Materials
{
    [Obsolete("Removed static materials in newest Xenon version", true)]
    public class BasicMaterial : IMaterial
    {
        public BasicMaterial()
        {
            MaterialEffect = Engine.Engine.CoreContentManager.Load<Effect>("BasicMaterial");
        }
        #region Fields
        private float _Shininess = 1f;
        private TextureCube _Skybox;
        private Texture2D _MaterialTexture;
        private Effect MaterialEffect;
        #endregion
        #region Properties
        public float Shininess
        {
            get { return _Shininess; }
            set
            {
                _Shininess = value;
                MaterialEffect.Parameters["ShininessFactor"].SetValue(value);
            }
        }
        public TextureCube Skybox
        {
            get { return _Skybox; }
            set
            {
                _Skybox = value;
                MaterialEffect.Parameters["EnvironmentTexture"].SetValue(value);
            }
        }
        public Texture2D MaterialTexture
        {
            get { return _MaterialTexture; }
            set
            {
                _MaterialTexture = value;
                MaterialEffect.Parameters["ModelTexture"].SetValue(value);
            }
        }
        #endregion

       
        public Effect GetMaterialEffect(Matrix World, Matrix View, Matrix Projection, Matrix transpose)
        {
            MaterialEffect.Parameters["WorldMatrix"].SetValue(World);
            MaterialEffect.Parameters["ViewMatrix"].SetValue(View);
            MaterialEffect.Parameters["ProjectionMatrix"].SetValue(Projection);
            MaterialEffect.Parameters["WorldInverseTransposeMatrix"].SetValue(transpose);

          
            return MaterialEffect;
        }
    }
}
