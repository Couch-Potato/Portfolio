using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Novovu.Controls.NodeEditor.DefaultNodes;
using Novovu.Workshop.Controls;
using Novovu.Xenon.Engine;
using Novovu.Xenon.Renderer.Helper;
using System.Threading.Tasks;

namespace Novovu.Workshop.Views
{
    public class MaterialCreator : Window
    {
        public MaterialCreator(BasicModel attachment = null)
        {
            
            Novovu.Controls.NodeEditor.StaticAssets.ResolveImageFileHandler = async () =>
            {
                var files = await ProjectModel.ProjectStatic.OpenFile(ProjectModel.ProjectStatic.TextureFilter, "Select a texture", this);
                return files[0];
            };

            Novovu.Controls.NodeEditor.StaticAssets.RenderMaterialFileHandler = async (Xenon.Renderer.Helper.MaterialEffect efx) =>
            {
                return await Task.Run(async () =>
                {
                    return await ProjectModel.ProjectStatic.RenderMaterialAsync(efx);
                });
            };

            Novovu.Controls.NodeEditor.StaticAssets.NodeToEffect = (MaterialNode node) =>
            {
                Texture2D albedo = default;
                Texture2D roughness = default;
                Texture2D metallic = default;
                Texture2D normal = default;
                Texture2D displacement = default;
                Texture2D mask = default;

                float emissive = 0;
                float roughnessval = 0;
                float metalness = 0;

                Color color = new Color(255, 255, 255);
                if (node.Inputs[0].Connection != null)
                {
                    albedo = node.Inputs[0].Connection.Output.Get<Texture2D>();
                }
                if (node.Inputs[1].Connection != null)
                {
                    roughness = node.Inputs[1].Connection.Output.Get<Texture2D>();
                }
                if (node.Inputs[2].Connection != null)
                {
                    metallic = node.Inputs[2].Connection.Output.Get<Texture2D>();
                }
                if (node.Inputs[3].Connection != null)
                {
                    normal = node.Inputs[3].Connection.Output.Get<Texture2D>();
                }
                if (node.Inputs[4].Connection != null)
                {
                    displacement = node.Inputs[4].Connection.Output.Get<Texture2D>();
                }
                if (node.Inputs[5].Connection != null)
                {
                    mask = node.Inputs[5].Connection.Output.Get<Texture2D>();
                }
                if (node.Inputs[6].Connection != null)
                {
                    emissive = node.Inputs[6].Connection.Output.Get<float>();
                }
                if (node.Inputs[7].Connection != null)
                {
                    roughnessval = node.Inputs[7].Connection.Output.Get<float>();
                }
                if (node.Inputs[8].Connection != null)
                {
                    metalness = node.Inputs[8].Connection.Output.Get<float>();
                }
                if (node.Inputs[9].Connection != null)
                {
                    color = node.Inputs[9].Connection.Output.Get<Color>();
                }
                MaterialEffect effect = MaterialNodeEditor.CreateMaterial(color, roughnessval, metalness, albedo, normal, roughness, metallic, mask, displacement, MaterialEffect.MaterialTypes.Basic, emissive);
                return effect;
            };
            this.InitializeComponent();
            var nodeEditor = this.FindControl<MaterialNodeEditor>("editor");
            nodeEditor.Attached = attachment;
            nodeEditor.NodeWindow = this;
        }
        public MaterialCreator()
        {
            
            Novovu.Controls.NodeEditor.StaticAssets.ResolveImageFileHandler = async () =>
            {
                var files = await ProjectModel.ProjectStatic.OpenFile(ProjectModel.ProjectStatic.TextureFilter, "Select a texture", this);
                return files[0];
            };

            Novovu.Controls.NodeEditor.StaticAssets.RenderMaterialFileHandler = async (Xenon.Renderer.Helper.MaterialEffect efx) =>
            {
                return await Task.Run(async() =>
                {
                    return await ProjectModel.ProjectStatic.RenderMaterialAsync(efx);
                });
            };

            Novovu.Controls.NodeEditor.StaticAssets.NodeToEffect = (MaterialNode node) =>
            {
                Texture2D albedo = default;
                Texture2D roughness = default;
                Texture2D metallic = default;
                Texture2D normal = default;
                Texture2D displacement = default;
                Texture2D mask = default;

                float emissive = 0;
                float roughnessval = 0;
                float metalness = 0;

                Color color = new Color(255, 255, 255);
                if (node.Inputs[0].Connection != null)
                {
                    albedo = node.Inputs[0].Connection.Output.Get<Texture2D>();
                }
                if (node.Inputs[1].Connection != null)
                {
                    roughness = node.Inputs[1].Connection.Output.Get<Texture2D>();
                }
                if (node.Inputs[2].Connection != null)
                {
                    metallic = node.Inputs[2].Connection.Output.Get<Texture2D>();
                }
                if (node.Inputs[3].Connection != null)
                {
                    normal = node.Inputs[3].Connection.Output.Get<Texture2D>();
                }
                if (node.Inputs[4].Connection != null)
                {
                    displacement = node.Inputs[4].Connection.Output.Get<Texture2D>();
                }
                if (node.Inputs[5].Connection != null)
                {
                    mask = node.Inputs[5].Connection.Output.Get<Texture2D>();
                }
                if (node.Inputs[6].Connection != null)
                {
                    emissive = node.Inputs[6].Connection.Output.Get<float>();
                }
                if (node.Inputs[7].Connection != null)
                {
                    roughnessval = node.Inputs[7].Connection.Output.Get<float>();
                }
                if (node.Inputs[8].Connection != null)
                {
                    metalness = node.Inputs[8].Connection.Output.Get<float>();
                }
                if (node.Inputs[9].Connection != null)
                {
                    color = node.Inputs[9].Connection.Output.Get<Color>();
                }
                MaterialEffect effect = MaterialNodeEditor.CreateMaterial(color, roughnessval, metalness, albedo, normal, roughness, metallic, mask, displacement, MaterialEffect.MaterialTypes.Basic, emissive);
                return effect;
            };
            this.InitializeComponent();
            var nodeEditor = this.FindControl<MaterialNodeEditor>("editor");
            nodeEditor.NodeWindow = this;
        }
        public MaterialNodeEditor NodeEditor { get => this.FindControl<MaterialNodeEditor>("editor");set {
                value.Name = "editor";
                this.Content = value;
            } }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
