using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Xenon.Editor
{
    public class Geometry
    {
        public readonly VertexPositionNormalTexture[] Vertices;
        public readonly short[] Indices;
        public Geometry(Vector3[] positions, Vector3[] normals, short[] indices)
        {
            Indices = indices;
            var texdata = new[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1) };
            Vertices = new VertexPositionNormalTexture[positions.Length];
            for (int i = 0; i < positions.Length; i++)
                Vertices[i] = new VertexPositionNormalTexture(positions[i], normals[i], texdata[i % 3]);
        }
        public BoundingBox GetBoundingBox(Matrix world)
        {
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            foreach (VertexPositionNormalTexture vpnt in Vertices)
            {
                Vector3 tpos = Vector3.Transform(vpnt.Position, world);
                min = Vector3.Min(min, tpos);
                max = Vector3.Max(max, tpos);
            }
            return new BoundingBox(min, max);
        }
    }
}
