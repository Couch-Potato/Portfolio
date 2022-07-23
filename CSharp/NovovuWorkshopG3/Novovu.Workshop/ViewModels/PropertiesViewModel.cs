using Avalonia.Input;
using Dock.Model.Controls;
using Microsoft.Xna.Framework;
using Novovu.Workshop.Models;
using Novovu.Workshop.ProjectModel;
using Novovu.Workshop.Shared;
using Novovu.Workshop.TypeInterfaces;
using Novovu.Workshop.Workspace;
using Novovu.Xenon.Engine;
using Novovu.Xenon.ScriptEngine;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace Novovu.Workshop.ViewModels
{
    public class PropertiesViewModel: Tool, DockSystem.IDockNameable
    {
        private WorkspaceInterface WSIF;
        public string Title = "Properties";
        private GlobalProperty Resolve(Property p)
        {
            GlobalProperty px = new GlobalProperty();
            switch (p.AttributePropertyType)
            {
                case (Property.PropertyType.String):
                    px.PropertyType = GlobalProperty.Types.String;
                    px.String = (string)p.Get();
                    ProjectStatic.RecallAllProperties += () =>
                    {
                        px.String = (string)p.Get();
                    };
                    px.OnItemChanged += () =>
                    {
                        p.Set(px.String);
                    };
                    break;
                case (Property.PropertyType.Vector3):
                    px.PropertyType = GlobalProperty.Types.Vector3;
                    Vector3 vx = (Vector3)p.Get();
                    px.Vector = new Shared.Types.Vector3() { X = vx.X.ToString(), Y = vx.Y.ToString(), Z = vx.Z.ToString() };
                    ProjectStatic.RecallAllProperties += () =>
                    {
                        px.Vector = new Shared.Types.Vector3() { X = vx.X.ToString(), Y = vx.Y.ToString(), Z = vx.Z.ToString() };
                    };
                    px.Vector.OnChanged += () =>
                    {
                        try
                        {
                            var x = float.Parse(px.Vector.X);
                            var y = float.Parse(px.Vector.Y);
                            var z = float.Parse(px.Vector.Z);
                            p.Set(new Vector3(x, y, z));
                        }catch
                        {

                        }
                       
                    };
                    break;
                case (Property.PropertyType.Execute):
                    px.PropertyType = GlobalProperty.Types.Command;
                    px.Command = (ExecutableProperty)p.Get();
                    break;
            }
            
            px.PropertyName = p.Name;
            return px;
        }
        public void ClearView()
        {
            PropertyCatModel = null;
        }

        public object SelectedObject;
        public PropertyCategory DefaultCateogry;

        public void DisplayProperty(object obj)
        {
            //First lets get the basic propertyinfo
            PropertyCategory category = new PropertyCategory();
            category.PropertyName = "Basic Properties";
            category.PropertySet = obj.GetType().Name;
            foreach (System.Reflection.PropertyInfo f in obj.GetType().GetProperties())
            {
                if (f.GetCustomAttributesData().Count > 0)
                {
                    foreach (var att in f.GetCustomAttributesData())
                    {
                        if (att.AttributeType == typeof(Shared.Property))
                        {
                            Property p = Property.BuildPropertyFromData(att, obj, f.Name);
                            category.Properties.Add(Resolve(p));
                        }
                        
                    }
                    
                }
                
            }
            PropertyCatModel = new PropertyViewCategoryViewModel(new ObservableCollection<PropertyCategory>() { category });
            if (obj is WGameObject)
            {
                var wb = (WGameObject)obj;
                foreach (var comp in wb.Components)
                {
                    PropertyCategory cat = new PropertyCategory();
                    category.PropertyName = "Component Properties";
                    category.PropertySet = comp.Name;

                    foreach (KeyValuePair<string, PropertyPair> prop in comp.Properties)
                    {
                        if (prop.Value.PropertyType.Name == "Number")
                        {
                            GlobalProperty gp = new GlobalProperty();
                            gp.PropertyType = GlobalProperty.Types.String;
                            gp.PropertyName = prop.Key;
                            gp.String = "";
                            gp.OnItemChanged += () =>
                            {
                                try
                                {
                                    prop.Value.PropertyObject = float.Parse(gp.String);
                                }catch
                                {

                                }
                                
                            };
                            category.Properties.Add(gp);
                        }
                        if (prop.Value.PropertyType.Name == "String")
                        {
                            GlobalProperty gp = new GlobalProperty();
                            gp.PropertyType = GlobalProperty.Types.String;
                            gp.PropertyName = prop.Key;
                            gp.String = "";
                            gp.OnItemChanged += () =>
                            {
                                prop.Value.PropertyObject = gp.String;
                            };
                            category.Properties.Add(gp);
                        }
                        if (prop.Value.PropertyType.Name == "Vector3")
                        {
                            GlobalProperty gp = new GlobalProperty();
                            gp.PropertyName = prop.Key;
                            gp.PropertyType = GlobalProperty.Types.Vector3;
                            gp.Vector = new Shared.Types.Vector3() { X = "0", Y = "0", Z = "0" };
                            gp.Vector.OnChanged += () =>
                            {
                                try
                                {
                                    var x = float.Parse(gp.Vector.X);
                                    var y = float.Parse(gp.Vector.Y);
                                    var z = float.Parse(gp.Vector.Z);
                                    prop.Value.PropertyObject = new Vector3(x, y, z);
                                }
                                catch
                                {

                                }
                                
                            };
                            category.Properties.Add(gp);
                        }
                        
                    }

                   // foreach (KeyValuePair<string, object> prop in comp.Properties)
                   // {
                   //     Property p = new Property(prop.Key, comp.name, Property.PropertyType.String);
                   //     if (prop.Value is string)
                    ///        p = new Property(prop.Key, comp.name, Property.PropertyType.String);
                    //    if (prop.Value is Vector3)
                    //        p = new Property(prop.Key, comp.name, Property.PropertyType.Vector3);
                     //   p.LinkedObject = prop.Value;
                     //   p.LinkedName = prop.Key;
                     //   category.Properties.Add(Resolve(p));
                    //}
                }
            }
            DefaultCateogry = category;
            
            SelectedObject = obj;
            //Properties.Add(category);
            
        }
        public PropertiesViewModel(object a)
        {
            DisplayProperty(a);
            
        }
        private ViewModelBase pcm;
        public ViewModelBase PropertyCatModel { get => pcm; set { this.RaiseAndSetIfChanged(ref pcm, value); } }

        public string Name => "Properties";
    }
}
