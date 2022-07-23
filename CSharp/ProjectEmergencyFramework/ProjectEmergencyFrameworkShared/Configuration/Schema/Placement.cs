using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProjectEmergencyFrameworkShared.Configuration.Schema
{
    public class Placement
    {
        public string ModelHash { get; set; }
        public int Type { get; set; }
        public bool Dynamic { get; set; }
        public bool FrozenPos { get; set; }
        public string HashName { get; set; }
        public string InitialHandle { get; set; }
        public List<object> ObjectProperties { get; set; }
        public int OpacityLevel { get; set; }
        public int LodDistance { get; set; }
        public bool IsVisible { get; set; }
        public int MaxHealth { get; set; }
        public int Health { get; set; }
        public bool HasGravity { get; set; }
        public bool IsOnFire { get; set; }
        public bool IsInvincible { get; set; }
        public bool IsExplosionProof { get; set; }
        public bool IsFireProof { get; set; }
        public bool IsMeleeProof { get; set; }
        public bool IsOnlyDamagedByPlayer { get; set; }

        public PositionRotation PositionRotation { get; set; }
        public Attachment Attachment { get; set; }
    }
    public class PositionRotation
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Pitch { get; set; }
        public float Roll { get; set; }
        public float Yaw { get; set; }
    }
    public class Attachment
    {
        [XmlAttribute]
        public bool isAttached { get; set; }
    }
}
