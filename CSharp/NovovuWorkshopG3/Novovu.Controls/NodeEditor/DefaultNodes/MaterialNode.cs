using Avalonia;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Controls.NodeEditor.DefaultNodes
{
    public class MaterialNode : NodeBox
    {
        public MaterialNode() : base()
        {
            Title = "Material Shader";
            Type = "Shader Pass";
            Inputs = new List<NodePoint>()
            {
                new NodePoint("Albedo", "Texture", 0, this, NodePoint.PointFlowTypes.Input),
                new NodePoint("RoughnessMap", "Texture", 1, this, NodePoint.PointFlowTypes.Input),
                new NodePoint("MetallicMap", "Texture", 2, this, NodePoint.PointFlowTypes.Input),
                new NodePoint("NormalMap", "Texture", 3, this, NodePoint.PointFlowTypes.Input),
                new NodePoint("DisplacementMap", "Texture", 4, this, NodePoint.PointFlowTypes.Input),
                new NodePoint("Mask", "Texture", 5, this, NodePoint.PointFlowTypes.Input),
                new NodePoint("EmissiveStrength", "Intensity", 6, this, NodePoint.PointFlowTypes.Input),
                new NodePoint("Roughness", "Intensity", 7, this, NodePoint.PointFlowTypes.Input, 0f),
                new NodePoint("Metalness", "Intensity", 8, this, NodePoint.PointFlowTypes.Input, 0f),
                new NodePoint("DiffuseColor", "Color", 9, this, NodePoint.PointFlowTypes.Input),
            };
            Size = new Avalonia.Size(200, 425);
            NodeYOffset = 125;
            this.ReadyFeaturePaint += MaterialNode_ReadyFeaturePaint;
            foreach (var point in Inputs)
            {
                point.GeneratedValueChanged += ReRender;
            }

            //Render our first job
            RenderJob();
        }
        bool IsReRendering = false;
        public IBitmap MaterialOutput = new Bitmap("Assets/tex_default.jpg");
        public void ReRender(object sender, EventArgs e)
        {
            // We only wish to do one render job at a time.
            if (!IsReRendering)
            {
                // Set our output to our default texture
                MaterialOutput = new Bitmap("Assets/tex_default.jpg");
                IsReRendering = true;
                RenderJob();
            }
        }
        public async void RenderJob()
        {
            var bitmap = await StaticAssets.RenderMaterial(StaticAssets.ConvertNodeToEffect(this));
            MaterialOutput = bitmap;
            IsReRendering = false;
        }
        private void MaterialNode_ReadyFeaturePaint(Avalonia.Media.DrawingContext context, VectorCamera cam)
        {
            var Position = cam.Position + this.Position + new Avalonia.Point(50, 40);
            context.DrawImage(MaterialOutput, new Avalonia.Rect(new Point(0, 0), MaterialOutput.Size), new Rect(Position, new Size(100, 100)), Avalonia.Visuals.Media.Imaging.BitmapInterpolationMode.LowQuality);
        }
    }
}
