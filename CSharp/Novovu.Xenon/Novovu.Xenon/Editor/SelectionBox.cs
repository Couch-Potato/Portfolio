using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Novovu.Xenon.Engine;

namespace Novovu.Xenon.Editor
{
    public class SelectionBox : IMiscDrawable
    {
        BoundingBox SetBox;
        public SelectionBox(BoundingBox setBox)
        {
            _selectionBoxEffect = new BasicEffect(Engine.Engine.graphicsDevice);
            SetBox = setBox;
        }

        public void Draw(Camera c)
        {

            CreateSelectionBox(SetBox);

            const float boxScale = 2f;

            var _graphics = Engine.Engine.graphicsDevice;

                _selectionBoxEffect.View = c.ViewMatrix;
                _selectionBoxEffect.Projection = c.ProjectionMatrix;
                _selectionBoxEffect.World = Matrix.Identity;

                _selectionBoxEffect.CurrentTechnique.Passes[0].Apply();
                _graphics.DrawUserPrimitives(PrimitiveType.LineList, _selectionBoxVertices.ToArray(), 0,
                                             _selectionBoxVertices.Count / 2);

        }
        private BasicEffect _selectionBoxEffect;
        private List<VertexPositionColor> _selectionBoxVertices = new List<VertexPositionColor>();
        private void CreateSelectionBox(BoundingBox box)
        {
            Color lineColor = Color.Black;
            const float lineLength = 8f;

            _selectionBoxVertices.Clear();

            //Vector3 min = new Vector3(float.MaxValue), max = new Vector3(float.MinValue);
            //SelectionBox = new BoundingBox();
            //foreach (var selectable in Selection)
            //{
            //  min = Vector3.Min(min, selectable.BoundingBox.Min);
            //  max = Vector3.Max(max, selectable.BoundingBox.Max);
            //}

            //// convert to local-space
            //min -= _position;
            //max -= _position;

            //SelectionBox = new BoundingBox(min, max);
            //Vector3[] boundingBoxCorners = SelectionBox.GetCorners();


            BoundingBox boundingBox = box;
                Vector3[] boundingBoxCorners = boundingBox.GetCorners();

                #region Create Corners
                // --- Corner 0 --- // 
                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[0], lineColor));
                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[0] +
                                                                  new Vector3(0,
                                                                              (boundingBoxCorners[3].Y - boundingBoxCorners[0].Y) /
                                                                              lineLength, 0), lineColor));

                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[0], lineColor));
                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[0] +
                                                                  new Vector3(0, 0,
                                                                              (boundingBoxCorners[4].Z - boundingBoxCorners[0].Z) /
                                                                              lineLength), lineColor));

                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[0], lineColor));
                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[0] +
                                                                  new Vector3(
                                                                    (boundingBoxCorners[1].X - boundingBoxCorners[0].X) / lineLength,
                                                                    0, 0), lineColor));


                // --- Corner 1 --- // 
                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[1], lineColor));
                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[1] +
                                                                  new Vector3(0,
                                                                              (boundingBoxCorners[2].Y - boundingBoxCorners[1].Y) /
                                                                              lineLength, 0), lineColor));

                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[1], lineColor));
                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[1] +
                                                                  new Vector3(0, 0,
                                                                              (boundingBoxCorners[5].Z - boundingBoxCorners[1].Z) /
                                                                              lineLength), lineColor));

                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[1], lineColor));
                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[1] +
                                                                  new Vector3(
                                                                    (boundingBoxCorners[0].X - boundingBoxCorners[1].X) / lineLength,
                                                                    0, 0), lineColor));


                // --- Corner 2 --- // 
                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[2], lineColor));
                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[2] +
                                                                  new Vector3(0,
                                                                              (boundingBoxCorners[1].Y - boundingBoxCorners[2].Y) /
                                                                              lineLength, 0), lineColor));

                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[2], lineColor));
                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[2] +
                                                                  new Vector3(0, 0,
                                                                              (boundingBoxCorners[6].Z - boundingBoxCorners[2].Z) /
                                                                              lineLength), lineColor));

                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[2], lineColor));
                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[2] +
                                                                  new Vector3(
                                                                    (boundingBoxCorners[3].X - boundingBoxCorners[2].X) / lineLength,
                                                                    0, 0), lineColor));


                // --- Corner 3 --- // 
                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[3], lineColor));
                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[3] +
                                                                  new Vector3(0,
                                                                              (boundingBoxCorners[0].Y - boundingBoxCorners[3].Y) /
                                                                              lineLength, 0), lineColor));

                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[3], lineColor));
                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[3] +
                                                                  new Vector3(0, 0,
                                                                              (boundingBoxCorners[7].Z - boundingBoxCorners[3].Z) /
                                                                              lineLength), lineColor));

                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[3], lineColor));
                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[3] +
                                                                  new Vector3(
                                                                    (boundingBoxCorners[2].X - boundingBoxCorners[3].X) / lineLength,
                                                                    0, 0), lineColor));


                // --- Corner 4 --- // 
                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[4], lineColor));
                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[4] +
                                                                  new Vector3(0,
                                                                              (boundingBoxCorners[7].Y - boundingBoxCorners[4].Y) /
                                                                              lineLength, 0), lineColor));

                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[4], lineColor));
                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[4] +
                                                                  new Vector3(0, 0,
                                                                              (boundingBoxCorners[0].Z - boundingBoxCorners[4].Z) /
                                                                              lineLength), lineColor));

                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[4], lineColor));
                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[4] +
                                                                  new Vector3(
                                                                    (boundingBoxCorners[5].X - boundingBoxCorners[4].X) / lineLength,
                                                                    0, 0), lineColor));


                // --- Corner 5 --- // 
                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[5], lineColor));
                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[5] +
                                                                  new Vector3(0,
                                                                              (boundingBoxCorners[6].Y - boundingBoxCorners[5].Y) /
                                                                              lineLength, 0), lineColor));

                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[5], lineColor));
                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[5] +
                                                                  new Vector3(0, 0,
                                                                              (boundingBoxCorners[1].Z - boundingBoxCorners[5].Z) /
                                                                              lineLength), lineColor));

                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[5], lineColor));
                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[5] +
                                                                  new Vector3(
                                                                    (boundingBoxCorners[4].X - boundingBoxCorners[5].X) / lineLength,
                                                                    0, 0), lineColor));

                // --- Corner 6 --- // 
                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[6], lineColor));
                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[6] +
                                                                  new Vector3(0,
                                                                              (boundingBoxCorners[5].Y - boundingBoxCorners[6].Y) /
                                                                              lineLength, 0), lineColor));

                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[6], lineColor));
                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[6] +
                                                                  new Vector3(0, 0,
                                                                              (boundingBoxCorners[2].Z - boundingBoxCorners[6].Z) /
                                                                              lineLength), lineColor));

                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[6], lineColor));
                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[6] +
                                                                  new Vector3(
                                                                    (boundingBoxCorners[7].X - boundingBoxCorners[6].X) / lineLength,
                                                                    0, 0), lineColor));


                // --- Corner 7 --- // 
                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[7], lineColor));
                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[7] +
                                                                  new Vector3(0,
                                                                              (boundingBoxCorners[4].Y - boundingBoxCorners[7].Y) /
                                                                              lineLength, 0), lineColor));

                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[7], lineColor));
                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[7] +
                                                                  new Vector3(0, 0,
                                                                              (boundingBoxCorners[3].Z - boundingBoxCorners[7].Z) /
                                                                              lineLength), lineColor));

                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[7], lineColor));
                _selectionBoxVertices.Add(new VertexPositionColor(boundingBoxCorners[7] +
                                                                  new Vector3(
                                                                    (boundingBoxCorners[6].X - boundingBoxCorners[7].X) / lineLength,
                                                                    0, 0), lineColor));
                #endregion
            
        }
    }
}
