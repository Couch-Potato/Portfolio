using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Xenon.Engine
{
    public class Camera
    {
        
        private Vector3 _orientation = new Vector3(0, 0, 0);
        public Vector3 Orientation
        {
            get
            {
                return _orientation;
            }set
            {
                _orientation = value;
                Matrix rotation = Matrix.CreateRotationX(MathHelper.ToRadians(value.X))* Matrix.CreateRotationY(MathHelper.ToRadians(value.Y));
                Vector3 lookAtOffset = Vector3.Transform(Vector3.UnitZ, rotation);
                
                _LookAtVector =  ViewLocation + lookAtOffset;


            }
        }

        public void TranslationVector(Vector3 translationVec)
        {
            Matrix rotation = Matrix.CreateRotationX(MathHelper.ToRadians(Orientation.X)) * Matrix.CreateRotationY(MathHelper.ToRadians(Orientation.Y));
            Vector3 movement = Vector3.Transform(translationVec, rotation);
            Vector3 lookAtOffset = Vector3.Transform(Vector3.UnitZ, rotation);
            ViewLocation = movement + ViewLocation;
            _LookAtVector = ViewLocation + lookAtOffset;
            
            
        }

        private Vector3 _LookAtVector = new Vector3(0, 0, 0);

        public Vector3 LookAtVector
        {
            get
            {
                return _LookAtVector;
            }set
            {
                _LookAtVector = value;
            }
        }

        public Vector3 ViewLocation = new Vector3(0, 0, 0);

        public float ViewScale = 45f;

        public float AspectRatio = 1080f / 720f;

        float nearClipPlane = 1;
        public float farClipPlane = 4000f;
        public Matrix ProjectionMatrix { get { return Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(ViewScale), AspectRatio, nearClipPlane, farClipPlane); } }

        public Ray ScreenToWorldRay(Vector2 state)
        {
            Vector3 nearPoint = new Vector3(state, 0);
            Vector3 farPoint = new Vector3(state, 1);
            nearPoint = Engine.graphicsDevice.Viewport.Unproject(nearPoint, ProjectionMatrix, ViewMatrix, Matrix.Identity);
            farPoint = Engine.graphicsDevice.Viewport.Unproject(farPoint, ProjectionMatrix, ViewMatrix, Matrix.Identity);
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            Ray r = new Ray(nearPoint, direction);

            return r;
        }
        public Vector3 ScreenToWorldPlane(Plane p, Vector2 state)
        {
            Vector3 nearPoint = new Vector3(state, 0);
            Vector3 farPoint = new Vector3(state, 1);
            nearPoint = Engine.graphicsDevice.Viewport.Unproject(nearPoint, ProjectionMatrix, ViewMatrix, Matrix.Identity);
            farPoint = Engine.graphicsDevice.Viewport.Unproject(farPoint, ProjectionMatrix, ViewMatrix, Matrix.Identity);
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            Ray ray = new Ray(nearPoint, direction);
            Vector3 n = new Vector3(0f, 1f, 0f);

            // Calculate distance of intersection point from r.origin.
            float denominator = Vector3.Dot(p.Normal, ray.Direction);
            float numerator = Vector3.Dot(p.Normal, ray.Position) + p.D;
            float t = -(numerator / denominator);

            // Calculate the picked position on the y = 0 plane.
            Vector3 pickedPosition = nearPoint + direction * t;
            return pickedPosition;
        }

        public Matrix ViewMatrix { get { return Matrix.CreateLookAt(ViewLocation, LookAtVector, Vector3.UnitY); } }
    }
}
