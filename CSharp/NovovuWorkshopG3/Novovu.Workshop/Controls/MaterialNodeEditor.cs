using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Novovu.Controls.NodeEditor.DefaultNodes;
using Novovu.Xenon.Renderer.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Novovu.Workshop.ViewModels;
using Novovu.Workshop.Views;
using Novovu.Workshop.ProjectModel;
using Novovu.Xenon.Engine;
using System.Diagnostics;

namespace Novovu.Workshop.Controls
{
    public class MaterialNodeEditor : Novovu.Controls.NodeEditor.NodeEditor
    {
        public string Name;
        public bool Saved = false;
        public MaterialNode RootNode;
        public BasicModel Attached;
        public MaterialCreator NodeWindow;
        public MaterialNodeEditor() : base()
        {
            RootNode = new MaterialNode();
            CreateBox(RootNode);
        }

        public Xenon.Renderer.Helper.MaterialEffect Material
        {
            get {
                Texture2D albedo = default;
                Texture2D roughness = default;
                Texture2D metallic = default;
                Texture2D normal = default;
                Texture2D displacement = default;
                Texture2D mask = default;

                float emissive = 0;
                float roughnessval = 0;
                float metalness = 0;

                Color color = new Color(255,255,255);
                if (RootNode.Inputs[0].Connection != null)
                {
                    albedo = RootNode.Inputs[0].Connection.Output.Get<Texture2D>();
                }
                if (RootNode.Inputs[1].Connection != null)
                {
                    roughness = RootNode.Inputs[1].Connection.Output.Get<Texture2D>();
                }
                if (RootNode.Inputs[2].Connection != null)
                {
                    metallic = RootNode.Inputs[2].Connection.Output.Get<Texture2D>();
                }
                if (RootNode.Inputs[3].Connection != null)
                {
                    normal = RootNode.Inputs[3].Connection.Output.Get<Texture2D>();
                }
                if (RootNode.Inputs[4].Connection != null)
                {
                    displacement = RootNode.Inputs[4].Connection.Output.Get<Texture2D>();
                }
                if (RootNode.Inputs[5].Connection != null)
                {
                    mask = RootNode.Inputs[5].Connection.Output.Get<Texture2D>();
                }
                if (RootNode.Inputs[6].Connection != null)
                {
                    emissive = RootNode.Inputs[6].Connection.Output.Get<float>();
                }
                if (RootNode.Inputs[7].Connection != null)
                {
                    roughnessval = RootNode.Inputs[7].Connection.Output.Get<float>();
                }
                if (RootNode.Inputs[8].Connection != null)
                {
                    metalness = RootNode.Inputs[8].Connection.Output.Get<float>();
                }
                if (RootNode.Inputs[9].Connection != null)
                {
                    color = RootNode.Inputs[9].Connection.Output.Get<Color>();
                }
                MaterialEffect effect = CreateMaterial(color, roughnessval, metalness, albedo, normal, roughness, metallic, mask, displacement, MaterialEffect.MaterialTypes.Basic, emissive);
                return effect;
            }
        }
        bool ControlDown = false;
        protected override void OnKeyDown(Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == Avalonia.Input.Key.LeftCtrl)
                ControlDown = true;
            // base.OnKeyDown(e);
        }
        protected override void OnKeyUp(Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == Avalonia.Input.Key.S && ControlDown)
            {
                Save();
            }
            if (e.Key == Avalonia.Input.Key.LeftCtrl)
                ControlDown = false;
            // base.OnKeyUp(e);
        }

        protected override void Save()
        {
            HandleSave();
        }

        private async void HandleSave()
        {
            if (string.IsNullOrEmpty(Name))
            {
                LoadDialogResponse response = await new ItemNameDialog("Enter Material Name").ShowDialog(NodeWindow);

                if (response.DialogResult == LoadDialogResponse.Result.Confirmed)
                {
                    Name = response.Value;
                }
                NodeWindow.Title = "Material Creator - " + Name;
            }
            bool ItemFound = false;
            foreach (var item in ProjectAssets.GetTab("Materials").Items)
            {
                if (item.Name == Name)
                {
                    item.Attachment = Material;
                    item.Icon = RootNode.MaterialOutput;
                    ItemFound = true;
                }
            }
            if (!ItemFound)
            {
                var item = new AssetViewer.AssetItem(ProjectAssets.GetTab("Materials"));
                item.Attachment = Material;
                item.Name = Name;
                item.Icon = RootNode.MaterialOutput;
                ProjectAssets.GetTab("Materials").Items.Add(item);
                
            }
            Attached.Material = Material;

        }

        public static MaterialEffect CreateMaterial(Color color, float roughness, float metallic, Texture2D albedoMap = null, Texture2D normalMap = null, Texture2D roughnessMap = null, Texture2D metallicMap = null, Texture2D mask = null, Texture2D displacementMap = null, MaterialEffect.MaterialTypes type = 0, float emissiveStrength = 0)
        {
            MaterialEffect mat = new MaterialEffect();
            mat.Initialize(color, roughness, metallic, albedoMap, normalMap, roughnessMap, metallicMap, mask, displacementMap, type, emissiveStrength);
            return mat;
        }
    }
}
