using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Placeables
{
    public abstract class Placeable : IPlaceable
    {

        public abstract void OnCleanup();
        public abstract void OnDestroyed();
        public abstract void OnPlaced();
        public abstract void OnInstanced();
        public void Cleanup()
        {
            OnCleanup();
        }

        public string OwnerId { get; set; }
        public dynamic Properties { get; set; }

        private Vector3 _position = Vector3.Zero;
        private Vector3 _rotation = Vector3.Zero;
        public Vector3 Position { get => _position; set { 
                _position = value;
                if (this.Prop != null)
                {
                    this.Prop.Position = value;
                    this.Prop.Update();
                }
            } 
        }
        public Vector3 Rotation { get =>_rotation; set {
                _rotation = value;
                if (this.Prop != null)
                {
                    this.Prop.Rotation = value;
                    this.Prop.Update();
                }
            } 
        }
        public InstancedProp Prop { get; set; }

        public string PersistentId { get; set; }
        public string Universe { get; set; }
        public void Destroyed()
        {
            OnDestroyed();
        }

        public void Instanced()
        {
            OnInstanced();
        }

        public void Place()
        {
            OnPlaced();
        }
    }
}
