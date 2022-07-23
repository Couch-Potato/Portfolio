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
    public class AdvancedMaterial : IMaterial
    {
        private Texture2D _MaterialTexture;

        public Texture2D MaterialTexture { get { return _MaterialTexture; } set {
                _MaterialTexture = value;
                MaterialEffect.Parameters["ColorMap"].SetValue(value);
            } }

        private Texture2D _NormalMapTexture;
        public Texture2D NormalMapTexture
        {
            get { return _NormalMapTexture; }
            set
            {
                _NormalMapTexture = value;
                MaterialEffect.Parameters["NormalMap"].SetValue(value);
            }
        }
        public Effect MaterialEffect;

        public AdvancedMaterial()
        {
            MaterialEffect = Engine.Engine.CoreContentManager.Load<Effect>("AdvancedMaterial");
        }
        public Effect GetMaterialEffect(Matrix World, Matrix View, Matrix Projection, Matrix transpose)
        {
            MaterialEffect.Parameters["World"].SetValue(World);
            MaterialEffect.Parameters["View"].SetValue(View);
            MaterialEffect.Parameters["Projection"].SetValue(Projection);

          //  Light specular = MaterialBase.GetSpecular(DrawLights);
           // Light diffuse = MaterialBase.GetDiffuse(DrawLights);

         //   MaterialEffect.Parameters["AmbientColor"].SetValue(Ambient.Color);
          //  MaterialEffect.Parameters["AmbientIntensity"].SetValue(Ambient.Intensity);

         //   if (specular != null)
         //   {
        //        MaterialEffect.Parameters["SpecularColor"].SetValue(specular.Color);
         //   }

         //   if (diffuse != null)
         //   {
             //   MaterialEffect.Parameters["LightDirection"].SetValue(diffuse.Direction);
          //      MaterialEffect.Parameters["DiffuseColor"].SetValue(diffuse.Color);
            //    MaterialEffect.Parameters["DiffuseIntensity"].SetValue(diffuse.Intensity);
          //  }

          //  MaterialEffect.Parameters["EyePosition"].SetValue(level.Camera.ViewLocation);
            return MaterialEffect;
        }
    }
}
