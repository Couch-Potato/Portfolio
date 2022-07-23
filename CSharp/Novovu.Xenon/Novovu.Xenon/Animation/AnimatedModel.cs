using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Novovu.Xenon.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using tainicom.Aether.Animation;
//using tainicom.Aether.Graphics;
//using tainicom.Aether.Content;
using Microsoft.Xna.Framework.Content;
//using MGSkinnedAnimationAux;
using Novovu.Xenon.SkinnedModelAnimator;
using System.Diagnostics;
using Novovu.Xenon.Lighting;

namespace Novovu.Xenon.Animation
{
    public class AnimatedModel
    {
        //Todo on content imported: Make it so then it wants to use the Animated Model Importer :)
        #region Fields Misc
        /// <summary>
        /// The actual underlying XNA model
        /// </summary>

        /// <summary>
        /// Extra data associated with the XNA model
        /// </summary>
        private ModelExtra modelExtra = null;

        /// <summary>
        /// The model bones
        /// </summary>
        private List<Bone> bones = new List<Bone>();


        private Matrix[] skeleton;

        private Matrix[] boneTransforms;

        /// <summary>
        /// The model asset name
        /// </summary>
        private string assetName = "";

        /// <summary>
        /// An associated animation clip player
        /// </summary>
        private AnimationPlayer player = null;

        public float SpecularPower = 10f;

        public EmissiveLight EmissiveLight;

        private bool hasSkinnedVertexType = false;
        private bool hasNormals = false;
        private bool hasTexCoords = false;
        #endregion
        #region Properties

        private Model _GameModel;
        public Model GameModel
        {
            get
            {
                return _GameModel;
            }
            set
            {
                _GameModel = value;

                modelExtra = _GameModel.Tag as ModelExtra;

                bool success = false;

                if (modelExtra != null)
                {
                    ObtainBones();

                    boneTransforms = new Matrix[bones.Count];

                    skeleton = new Matrix[modelExtra.Skeleton.Count];


                    success = true;
                }



                _animationAllowed = success;
            }
        }



        public Texture2D ModelTexture;

        //TODO Add support for skinned animations
        //public Materials.IMaterial Material;

        public List<Audio.ThreeDSound> SoundEmitters = new List<Audio.ThreeDSound>();

        public List<Particles.ParticleEmitter> ParticleEmitters = new List<Particles.ParticleEmitter>();

        public Vector3 LocationOffset;

        public Vector3 OrientationOffset;

        [Obsolete("Use scale property instead")]
        public float ScaleOffset;

        public Vector3 Scale;

        public GameObject ParentObject;

        public string Name = "";
        public List<Bone> Bones { get { return bones; } }


        public List<AnimationClip> Clips { get { return modelExtra.Clips; } }
        //Animated Model Stuffs

        //AnimationPlayer player;

        public List<string> AnimationDictionary = new List<string>();

        bool _animationAllowed = true;

        public bool AnimationAllowed { get { return _animationAllowed; } }



        #endregion

        #region Engine Loop Methods
        public void Draw(Camera camera, GameObject parent)
        {

            for (int i = 0; i < bones.Count; i++)
            {
                Bone bone = bones[i];
                bone.ComputeAbsoluteTransform();

                boneTransforms[i] = bone.AbsoluteTransform;
            }
            for (int s = 0; s < modelExtra.Skeleton.Count; s++)
            {
                Bone bone = bones[modelExtra.Skeleton[s]];
                skeleton[s] = bone.SkinTransform * bone.AbsoluteTransform;
            }
            foreach (ModelMesh mesh in _GameModel.Meshes)
            {
                //Debug.WriteLine("Drawing...");

                foreach (var effectt in mesh.Effects)
                {
                    if (effectt.GetType() == typeof(SkinnedEffect))
                    {
                        SkinnedEffect effect = (SkinnedEffect)effectt;
                        if (EmissiveLight != null)
                        {
                            effect.EmissiveColor = EmissiveLight.Color;
                        }
                        IObjectLight[] Lights = ParentObject.LevelParent.GetLightsForObject(LocationOffset + parent.Location);
                        if (Lights[0] != null)
                        {
                            IObjectLight light = Lights[0];
                            effect.DirectionalLight0.Direction = Engine.Engine.GetVectorDirection(LocationOffset + parent.Location, light.PositionOffset + light.Parent.Location);
                            effect.DirectionalLight0.Enabled = true;
                            effect.DirectionalLight0.SpecularColor = new Vector3(
                                MathHelper.Clamp((float)(light.Color.X * LightingPhysics.GetKatt(this, light) + .3), 0, 1),
                                MathHelper.Clamp((float)(light.Color.Y * LightingPhysics.GetKatt(this, light) + .3), 0, 1),
                                MathHelper.Clamp((float)(light.Color.Z * LightingPhysics.GetKatt(this, light) + .3), 0, 1)
                                );
                            effect.DirectionalLight0.DiffuseColor = new Vector3(
                                MathHelper.Clamp((float)(light.Color.X * LightingPhysics.GetKatt(this, light)), 0, 1),
                                MathHelper.Clamp((float)(light.Color.Y * LightingPhysics.GetKatt(this, light)), 0, 1),
                                MathHelper.Clamp((float)(light.Color.Z * LightingPhysics.GetKatt(this, light)), 0, 1)
                                );
                        }
                        if (Lights[1] != null)
                        {
                            IObjectLight light = Lights[1];
                            effect.DirectionalLight1.Direction = Engine.Engine.GetVectorDirection(LocationOffset + parent.Location, light.PositionOffset + light.Parent.Location);
                            effect.DirectionalLight1.Enabled = true;
                            effect.DirectionalLight1.SpecularColor = new Vector3(
                                MathHelper.Clamp((float)(light.Color.X * LightingPhysics.GetKatt(this, light) + .3), 0, 1),
                                MathHelper.Clamp((float)(light.Color.Y * LightingPhysics.GetKatt(this, light) + .3), 0, 1),
                                MathHelper.Clamp((float)(light.Color.Z * LightingPhysics.GetKatt(this, light) + .3), 0, 1)
                                );
                            effect.DirectionalLight0.DiffuseColor = new Vector3(
                                MathHelper.Clamp((float)(light.Color.X * LightingPhysics.GetKatt(this, light)), 0, 1),
                                MathHelper.Clamp((float)(light.Color.Y * LightingPhysics.GetKatt(this, light)), 0, 1),
                                MathHelper.Clamp((float)(light.Color.Z * LightingPhysics.GetKatt(this, light)), 0, 1)
                                );
                        }
                        if (Lights[2] != null)
                        {
                            IObjectLight light = Lights[2];
                            effect.DirectionalLight2.Direction = Engine.Engine.GetVectorDirection(LocationOffset + parent.Location, light.PositionOffset + light.Parent.Location);
                            effect.DirectionalLight2.Enabled = true;
                            effect.DirectionalLight2.SpecularColor = new Vector3(
                                MathHelper.Clamp((float)(light.Color.X * LightingPhysics.GetKatt(this, light) + .3), 0, 1),
                                MathHelper.Clamp((float)(light.Color.Y * LightingPhysics.GetKatt(this, light) + .3), 0, 1),
                                MathHelper.Clamp((float)(light.Color.Z * LightingPhysics.GetKatt(this, light) + .3), 0, 1)
                                );
                            effect.DirectionalLight0.DiffuseColor = new Vector3(
                                MathHelper.Clamp((float)(light.Color.X * LightingPhysics.GetKatt(this, light)), 0, 1),
                                MathHelper.Clamp((float)(light.Color.Y * LightingPhysics.GetKatt(this, light)), 0, 1),
                                MathHelper.Clamp((float)(light.Color.Z * LightingPhysics.GetKatt(this, light)), 0, 1)
                                );
                        }
                        effect.AmbientLightColor = ParentObject.LevelParent.AmbientLight.Color;

                        Matrix Rotation = Matrix.CreateRotationX(MathHelper.ToRadians(OrientationOffset.X + parent.Orientation.X)) *
                               Matrix.CreateRotationY(MathHelper.ToRadians(OrientationOffset.Y + parent.Orientation.Y)) *
                               Matrix.CreateRotationZ(MathHelper.ToRadians(OrientationOffset.Z + parent.Orientation.Z));
                        Matrix world = Rotation * Matrix.CreateTranslation(parent.Location + LocationOffset) * Matrix.CreateScale(parent.Scale + Scale);

                        effect.World = world;
                        effect.Projection = camera.ProjectionMatrix;
                        effect.View = camera.ViewMatrix;
                    }
                    else
                    {
                        if (effectt.GetType() == typeof(BasicEffect))
                        {
                            BasicEffect effect = (BasicEffect)effectt;
                            if (EmissiveLight != null)
                            {
                                effect.EmissiveColor = EmissiveLight.Color;
                            }
                            IObjectLight[] Lights = ParentObject.LevelParent.GetLightsForObject(LocationOffset + parent.Location);
                            if (Lights[0] != null)
                            {
                                IObjectLight light = Lights[0];
                                effect.DirectionalLight0.Direction = Engine.Engine.GetVectorDirection(LocationOffset + parent.Location, light.PositionOffset + light.Parent.Location);
                                effect.DirectionalLight0.Enabled = true;
                                effect.DirectionalLight0.SpecularColor = new Vector3(
                                    MathHelper.Clamp((float)(light.Color.X * LightingPhysics.GetKatt(this, light) + .3), 0, 1),
                                    MathHelper.Clamp((float)(light.Color.Y * LightingPhysics.GetKatt(this, light) + .3), 0, 1),
                                    MathHelper.Clamp((float)(light.Color.Z * LightingPhysics.GetKatt(this, light) + .3), 0, 1)
                                    );
                                effect.DirectionalLight0.DiffuseColor = new Vector3(
                                    MathHelper.Clamp((float)(light.Color.X * LightingPhysics.GetKatt(this, light)), 0, 1),
                                    MathHelper.Clamp((float)(light.Color.Y * LightingPhysics.GetKatt(this, light)), 0, 1),
                                    MathHelper.Clamp((float)(light.Color.Z * LightingPhysics.GetKatt(this, light)), 0, 1)
                                    );
                            }
                            if (Lights[1] != null)
                            {
                                IObjectLight light = Lights[1];
                                effect.DirectionalLight1.Direction = Engine.Engine.GetVectorDirection(LocationOffset + parent.Location, light.PositionOffset + light.Parent.Location);
                                effect.DirectionalLight1.Enabled = true;
                                effect.DirectionalLight1.SpecularColor = new Vector3(
                                    MathHelper.Clamp((float)(light.Color.X * LightingPhysics.GetKatt(this, light) + .3), 0, 1),
                                    MathHelper.Clamp((float)(light.Color.Y * LightingPhysics.GetKatt(this, light) + .3), 0, 1),
                                    MathHelper.Clamp((float)(light.Color.Z * LightingPhysics.GetKatt(this, light) + .3), 0, 1)
                                    );
                                effect.DirectionalLight0.DiffuseColor = new Vector3(
                                    MathHelper.Clamp((float)(light.Color.X * LightingPhysics.GetKatt(this, light)), 0, 1),
                                    MathHelper.Clamp((float)(light.Color.Y * LightingPhysics.GetKatt(this, light)), 0, 1),
                                    MathHelper.Clamp((float)(light.Color.Z * LightingPhysics.GetKatt(this, light)), 0, 1)
                                    );
                            }
                            if (Lights[2] != null)
                            {
                                IObjectLight light = Lights[2];
                                effect.DirectionalLight2.Direction = Engine.Engine.GetVectorDirection(LocationOffset + parent.Location, light.PositionOffset + light.Parent.Location);
                                effect.DirectionalLight2.Enabled = true;
                                effect.DirectionalLight2.SpecularColor = new Vector3(
                                    MathHelper.Clamp((float)(light.Color.X * LightingPhysics.GetKatt(this, light) + .3), 0, 1),
                                    MathHelper.Clamp((float)(light.Color.Y * LightingPhysics.GetKatt(this, light) + .3), 0, 1),
                                    MathHelper.Clamp((float)(light.Color.Z * LightingPhysics.GetKatt(this, light) + .3), 0, 1)
                                    );
                                effect.DirectionalLight0.DiffuseColor = new Vector3(
                                    MathHelper.Clamp((float)(light.Color.X * LightingPhysics.GetKatt(this, light)), 0, 1),
                                    MathHelper.Clamp((float)(light.Color.Y * LightingPhysics.GetKatt(this, light)), 0, 1),
                                    MathHelper.Clamp((float)(light.Color.Z * LightingPhysics.GetKatt(this, light)), 0, 1)
                                    );
                            }
                            effect.AmbientLightColor = ParentObject.LevelParent.AmbientLight.Color;

                            Matrix Rotation = Matrix.CreateRotationX(MathHelper.ToRadians(OrientationOffset.X + parent.Orientation.X)) *
                                   Matrix.CreateRotationY(MathHelper.ToRadians(OrientationOffset.Y + parent.Orientation.Y)) *
                                   Matrix.CreateRotationZ(MathHelper.ToRadians(OrientationOffset.Z + parent.Orientation.Z));
                            Matrix world = Rotation * Matrix.CreateTranslation(parent.Location + LocationOffset) * Matrix.CreateScale(parent.Scale + Scale);

                            effect.World = world;
                            effect.Projection = camera.ProjectionMatrix;
                            effect.View = camera.ViewMatrix;
                        }
                    }


                }



                //Potential Issue: When drawing with a Material, the lighting engine may over-ride the material shader.




                foreach (var MeshPart in mesh.MeshParts)
                {
                    if (AnimationAllowed)
                    {
                        Matrix[] bones = boneTransforms;
                        if (MeshPart.Effect.GetType() == typeof(SkinnedEffect))
                        {
                            ((SkinnedEffect)MeshPart.Effect).SetBoneTransforms(skeleton);
                        }
                        else
                        {
                            //((BasicEffect)MeshPart.Effect).SetBoneTransforms(bones);
                        }


                    }
                }


                //Debug.WriteLine("Drawggg");
                mesh.Draw();

            }
        }

        public void DoParticles(GameObject parent)
        {
            foreach (Particles.ParticleEmitter particle in ParticleEmitters)
            {
                particle.Emit(ParentObject.LevelParent.Camera.ViewMatrix, ParentObject.LevelParent.Camera.ProjectionMatrix);
            }
        }

        public void Update(GameTime time)
        {
            if (AnimationAllowed)
            {
                player?.Update(time);
            }
            foreach (Particles.ParticleEmitter particle in ParticleEmitters)
            {
                particle.Update(time, LocationOffset + particle.LocationOffset + ParentObject.Location);
            }
        }
        #endregion

        #region Animation Methods
        public AnimationPlayer PlayClip(AnimationClip clip, bool looping = true, int keyframestart = 0, int keyframeend = 0, int fps = 24)
        {
            // Create a clip player and assign it to this model
            player = new AnimationPlayer(clip, this, looping, keyframestart, keyframeend, fps);
            return player;
        }
        public AnimationClip[] GetAnimationClips()
        {
            if (AnimationAllowed)
            {
                AnimationClip[] Clips = new AnimationClip[modelExtra.Clips.Count];
                int i = 0;
                foreach (var item in modelExtra.Clips)
                {
                    Clips[i] = item;
                    i++;
                }
                return Clips;
            }
            else
            {
                return new AnimationClip[0];
            }

        }
        public string[] GetAnimationNames()
        {
            if (AnimationAllowed)
            {
                string[] Clips = new string[modelExtra.Clips.Count];
                int i = 0;
                foreach (var item in modelExtra.Clips)
                {
                    Clips[i] = item.Name;
                    i++;
                }
                return Clips;
            }
            else
            {
                return new string[0];
            }

        }
        #endregion

        #region Engine Helper Methods
        private void ObtainBones()
        {
            bones.Clear();
            foreach (ModelBone bone in _GameModel.Bones)
            {
                // Create the bone object and add to the heirarchy
                Bone newBone = new Bone(bone.Name, bone.Transform, bone.Parent != null ? bones[bone.Parent.Index] : null);

                // Add to the bones for this model
                bones.Add(newBone);
            }
        }
        public Bone FindBone(string name)
        {
            foreach (Bone bone in Bones)
            {
                if (bone.Name == name)
                    return bone;
            }

            return null;
        }
        #endregion

    }
}