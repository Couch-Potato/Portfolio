using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Xenon.Engine
{
    public class Skybox
    {
        private Model skyBox;

        /// <summary>
        /// The actual skybox texture
        /// </summary>
        public TextureCube skyBoxTexture;

        /// <summary>
        /// The effect file that the skybox will use to render
        /// </summary>
        private Effect skyBoxEffect;

        /// <summary>
        /// The size of the cube, used so that we can resize the box
        /// for different sized environments.
        /// </summary>
        //private float size = 50f;
        public Skybox(Model iso, TextureCube cubemap, Effect skbx)
        {
            skyBox = iso;
            skyBoxTexture = cubemap;
            skyBoxEffect = skbx;

        }
        public void SetSkybox(TextureCube cubemap)
        {
            skyBoxTexture = cubemap;
        }

        public void Render(Matrix view, Matrix projection, Vector3 cameraPosition, float size)
        {
         
            
            foreach (EffectPass pass in skyBoxEffect.CurrentTechnique.Passes)
            {
                // Draw all of the components of the mesh, but we know the cube really
                // only has one mesh
                foreach (ModelMesh mesh in skyBox.Meshes)
                {
                    
                    // Assign the appropriate values to each of the parameters
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                       
                        part.Effect = skyBoxEffect;


                        part.Effect.Parameters["World"].SetValue(
                            Matrix.CreateScale(size) * Matrix.CreateTranslation(cameraPosition));
                        part.Effect.Parameters["View"].SetValue(view);
                        part.Effect.Parameters["Projection"].SetValue(projection);
                        part.Effect.Parameters["SkyBoxTexture"].SetValue(skyBoxTexture);
                        part.Effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                    }
                    
                    // Draw the mesh with the skybox effect
                    mesh.Draw();
                }
            }

            // _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, primitiveCount);
        }
    }
}
