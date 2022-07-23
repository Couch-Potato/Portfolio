using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Novovu.Xenon.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Xenon.Materials
{
    public class MaterialBase
    {
       

        public Texture2D Texture;

        public float Shininess=100f;


        public float BumpConstant=1f;

        public Texture2D NormalMap;

        public Vector3 TintColor = new Vector3(0, 0, 0);

        private Effect baseEffect;
        public MaterialBase(Texture2D texture, Texture2D bumpmap, TextureCube skybox, float Shininess = 100f, float BumpCons=1f)
        {

            ContentManager manage = Engine.Engine.CoreContentManager;

            baseEffect = manage.Load<Effect>("NovovuMaterialShader");

           // baseEffect.Parameters["ModelTexture"].SetValue(texture);

            //baseEffect.Parameters["NormalMap"].SetValue(bumpmap);
            //baseEffect.Parameters["SkyboxTexture"].SetValue(skybox);


            this.Shininess = Shininess;

            BumpConstant = BumpCons;

            Texture = texture;

            NormalMap = bumpmap;
        }
        public static Engine.Light GetDiffuse(List<Engine.Light> DrawLights)
        {
            foreach (Light light in DrawLights)
            {
                if (light.LightType == Light.LightTypes.Diffuse)
                    return light;
            }
            return null;
        }
        public static Engine.Light GetSpecular(List<Engine.Light> DrawLights)
        {
            foreach (Light light in DrawLights)
            {
                if (light.LightType == Light.LightTypes.Specular)
                    return light;
            }
            return null;
        }

        public Effect GetBaseEffect(Matrix world, Matrix view, Matrix projection,Matrix transpose, Engine.Light Ambient, List<Engine.Light> DrawLights, Vector3 CameraLocation)
        {
            Engine.Light Diffuse = GetDiffuse(DrawLights);
            Light Specular = GetSpecular(DrawLights);

            //Import the effect hereeee

            baseEffect.Parameters["World"].SetValue(world);
            baseEffect.Parameters["View"].SetValue(view);
            baseEffect.Parameters["Projection"].SetValue(projection);

           // baseEffect.Parameters["AmbientColor"].SetValue(Ambient.Color);
            //baseEffect.Parameters["AmbientIntensity"].SetValue(Ambient.Intensity);

            baseEffect.Parameters["WorldInverseTranspose"].SetValue(transpose);

            if (Diffuse != null)
            {
               // baseEffect.Parameters["DiffuseLightDirection"].SetValue(Diffuse.Direction);
               // baseEffect.Parameters["DiffuseColor"].SetValue(Diffuse.Color);
               // baseEffect.Parameters["DiffuseIntensity"].SetValue(Diffuse.Intensity);
            }

            //baseEffect.Parameters["Shininess"].SetValue(Shininess);

            if (Specular != null)
            {
                //baseEffect.Parameters["SpecularColor"].SetValue(Specular.Color);
               // baseEffect.Parameters["SpecularIntensity"].SetValue(Specular.Intensity);
            }
            
            //baseEffect.Parameters["ViewVector"].SetValue(CameraLocation);

            //baseEffect.Parameters["CameraPosition"].SetValue(CameraLocation);
            return baseEffect;
        }
    }
}
