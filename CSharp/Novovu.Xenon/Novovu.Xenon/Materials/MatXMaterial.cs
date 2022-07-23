using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ClearScript.V8;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Novovu.Xenon.Engine;

namespace Novovu.Xenon.Materials
{
    public class MatXMaterial : IMaterial
    {
        private Effect MaterialEffect;
        MatX.StructuredMaterial mat;
        private Dictionary<string, object> _shade = new Dictionary<string, object>();
        V8ScriptEngine engine;
        Dictionary<string, object> Shader { get{ return _shade; }
            set
            {
                _shade = value;
                foreach (KeyValuePair<string,object> kvp in value)
                {
                    if (kvp.Value.GetType() == typeof(bool))
                    {
                        MaterialEffect.Parameters[kvp.Key].SetValue((bool)kvp.Value);
                    }
                    if (kvp.Value.GetType() == typeof(Vector4))
                    {
                        MaterialEffect.Parameters[kvp.Key].SetValue((Vector4)kvp.Value);
                    }
                    if (kvp.Value.GetType() == typeof(Vector4[]))
                    {
                        MaterialEffect.Parameters[kvp.Key].SetValue((Vector4[])kvp.Value);
                    }
                    if (kvp.Value.GetType() == typeof(Vector3))
                    {
                        MaterialEffect.Parameters[kvp.Key].SetValue((Vector3)kvp.Value);
                    }
                    if (kvp.Value.GetType() == typeof(Vector3[]))
                    {
                        MaterialEffect.Parameters[kvp.Key].SetValue((Vector3[])kvp.Value);
                    }
                    if (kvp.Value.GetType() == typeof(Vector2))
                    {
                        MaterialEffect.Parameters[kvp.Key].SetValue((Vector2)kvp.Value);
                    }
                    if (kvp.Value.GetType() == typeof(Vector2[]))
                    {
                        MaterialEffect.Parameters[kvp.Key].SetValue((Vector2[])kvp.Value);
                    }
                    if (kvp.Value.GetType() == typeof(Texture))
                    {
                        MaterialEffect.Parameters[kvp.Key].SetValue((Texture)kvp.Value);
                    }
                    if (kvp.Value.GetType() == typeof(Single[]))
                    {
                        MaterialEffect.Parameters[kvp.Key].SetValue((Single[])kvp.Value);
                    }
                    if (kvp.Value.GetType() == typeof(Matrix[]))
                    {
                        MaterialEffect.Parameters[kvp.Key].SetValue((Matrix[])kvp.Value);
                    }
                    if (kvp.Value.GetType() == typeof(Matrix))
                    {
                        MaterialEffect.Parameters[kvp.Key].SetValue((Matrix)kvp.Value);
                    }
                    if (kvp.Value.GetType() == typeof(int))
                    {
                        MaterialEffect.Parameters[kvp.Key].SetValue((int)kvp.Value);
                    }
                    if (kvp.Value.GetType() == typeof(Quaternion))
                    {
                        MaterialEffect.Parameters[kvp.Key].SetValue((Quaternion)kvp.Value);
                    }
                    if (kvp.Value.GetType() == typeof(float))
                    {
                        MaterialEffect.Parameters[kvp.Key].SetValue((float)kvp.Value);
                    }
                }
            }
        }
     
        public MatXMaterial(string name, GameObject objc, Level lvl, BasicModel bdml)
        {
            Logging.Logger.Log(Logging.Logger.LogTypes.Message, "Xenon.MatX", "Imporing material: " + name);
            mat = MatX.MaterialManager.ImportMaterial(name);
            Logging.Logger.Log(Logging.Logger.LogTypes.Message, "Xenon.MatX." + mat.MaterialName, "Importing shader effect");
            Assets.Asset Shader = Assets.AssetManager.Insert(mat.ShaderFile);
            MaterialEffect = Engine.Engine.AssetLoader.Load<Effect>(Shader.Hash);
            Shader.OffloadAsset();
            Logging.Logger.Log(Logging.Logger.LogTypes.Message, "Xenon.MatX." + mat.MaterialName, "Building shader script enviornment");

            engine = new V8ScriptEngine();
            
            engine.AddHostType(typeof(Vector3));
            engine.AddHostType(typeof(Matrix));
            engine.AddHostType(typeof(Vector4));

            engine.AddHostType(typeof(Vector3[]));
            engine.AddHostType(typeof(Matrix[]));
            engine.AddHostType(typeof(Vector4[]));
            engine.AddHostType(typeof(Quaternion));
          


            engine.AddHostObject("Shader", Shader);
            engine.AddHostObject("GameObject", new ScriptEngine.API.GameObject(objc));
            engine.AddHostObject("Model", new ScriptEngine.API.Model(bdml));
            engine.AddHostObject("Level", new ScriptEngine.API.Level(lvl));

            Logging.Logger.Log(Logging.Logger.LogTypes.Message, "Xenon.MatX." + mat.MaterialName, "Executing shading script");

            engine.Execute(mat.MaterialJS);

            Logging.Logger.Log(Logging.Logger.LogTypes.Message, "Xenon.MatX." + mat.MaterialName, "Running init() function");

            engine.Execute("init();");
        }
        public Effect GetMaterialEffect(Matrix World, Matrix View, Matrix Projection, Matrix transpose)
        {

            DrawParameters paramsx = new DrawParameters()
            {
                World = World,
                View = View,
                Projection = Projection,
                Transpose = transpose
            };

            string ohash2 = String.Format("{0:X}", paramsx.GetHashCode());

            engine.AddHostObject(ohash2, paramsx);


            engine.Execute($"draw({ohash2})");


            return MaterialEffect;
        }
    }
    public class DrawParameters
    {
        public Matrix World;
        public Matrix View;
        public Matrix Projection;
        public Matrix Transpose;

    }
   
}
