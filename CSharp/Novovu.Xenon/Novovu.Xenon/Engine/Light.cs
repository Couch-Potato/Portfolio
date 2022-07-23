using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Xenon.Engine
{
    [Obsolete("Use new lighting system in Novovu.Xenon.Lighting")]
    public class Light
    {
        public enum LightTypes { Diffuse, Specular, Ambient, Emissive};

        public Vector3 Direction = new Vector3();

        public Vector4 Color = new Vector4();

        public Vector3 Color3 = new Vector3();

        public LightTypes LightType = LightTypes.Diffuse;

        public float Intensity = 1f;
        public Light(LightTypes type, Vector3 direction, Vector3 color)
        {
            Direction = direction;
            Color = new Vector4(color.X,color.Y,color.Z, 1);
            Color3 = color;
            LightType = type;

        }
        public SkinnedEffect DrawLight(SkinnedEffect effect, bool DefaultLighting, Light LevelAmbient)
        {

            //effect.LightingEnabled = true;
            if (DefaultLighting)
            {
                effect.EnableDefaultLighting();
            }
            if (LightType == LightTypes.Emissive)
            {
                effect.EmissiveColor = Color3;
            }
            effect.AmbientLightColor = LevelAmbient.Color3;
            if (!effect.DirectionalLight0.Enabled)
            {
                effect.DirectionalLight0.Enabled = true;
                if (LightType == LightTypes.Diffuse)
                {
                    effect.DirectionalLight0.DiffuseColor = Color3;
                    effect.DirectionalLight0.Direction = Direction;
                }
                if (LightType == LightTypes.Specular)
                {
                    effect.DirectionalLight0.SpecularColor = Color3;
                    effect.DirectionalLight0.Direction = Direction;
                }
            }
            if (!effect.DirectionalLight1.Enabled)
            {
                effect.DirectionalLight1.Enabled = true;
                if (LightType == LightTypes.Diffuse)
                {
                    effect.DirectionalLight1.DiffuseColor = Color3;
                    effect.DirectionalLight1.Direction = Direction;
                }
                if (LightType == LightTypes.Specular)
                {
                    effect.DirectionalLight1.SpecularColor = Color3;
                    effect.DirectionalLight1.Direction = Direction;
                }
            }
            if (!effect.DirectionalLight2.Enabled)
            {
                effect.DirectionalLight2.Enabled = true;
                if (LightType == LightTypes.Diffuse)
                {
                    effect.DirectionalLight2.DiffuseColor = Color3;
                    effect.DirectionalLight2.Direction = Direction;
                }
                if (LightType == LightTypes.Specular)
                {
                    effect.DirectionalLight2.SpecularColor = Color3;
                    effect.DirectionalLight2.Direction = Direction;
                }
            }
            return effect;
        }
        public BasicEffect DrawLight(BasicEffect effect, bool DefaultLighting, Light LevelAmbient)
        {
           
            effect.LightingEnabled = true;
            if (DefaultLighting)
            {
                effect.EnableDefaultLighting();
            }
            if (LightType == LightTypes.Emissive)
            {
                effect.EmissiveColor = Color3;
            }
            effect.AmbientLightColor = LevelAmbient.Color3;
            if (!effect.DirectionalLight0.Enabled)
            {
                effect.DirectionalLight0.Enabled = true;
                if (LightType == LightTypes.Diffuse)
                {
                    effect.DirectionalLight0.DiffuseColor = Color3;
                    effect.DirectionalLight0.Direction = Direction;
                }
                if (LightType == LightTypes.Specular)
                {
                    effect.DirectionalLight0.SpecularColor = Color3;
                    effect.DirectionalLight0.Direction = Direction;
                }
            }
            if (!effect.DirectionalLight1.Enabled)
            {
                effect.DirectionalLight1.Enabled = true;
                if (LightType == LightTypes.Diffuse)
                {
                    effect.DirectionalLight1.DiffuseColor = Color3;
                    effect.DirectionalLight1.Direction = Direction;
                }
                if (LightType == LightTypes.Specular)
                {
                    effect.DirectionalLight1.SpecularColor = Color3;
                    effect.DirectionalLight1.Direction = Direction;
                }
            }
            if (!effect.DirectionalLight2.Enabled)
            {
                effect.DirectionalLight2.Enabled = true;
                if (LightType == LightTypes.Diffuse)
                {
                    effect.DirectionalLight2.DiffuseColor = Color3;
                    effect.DirectionalLight2.Direction = Direction;
                }
                if (LightType == LightTypes.Specular)
                {
                    effect.DirectionalLight2.SpecularColor = Color3;
                    effect.DirectionalLight2.Direction = Direction;
                }
            }
            return effect;
        }
    }
}
