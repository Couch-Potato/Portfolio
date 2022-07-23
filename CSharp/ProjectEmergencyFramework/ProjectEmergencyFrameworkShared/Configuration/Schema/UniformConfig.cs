using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProjectEmergencyFrameworkShared.Configuration.Schema
{
    public class UniformConfig
    {
        public string OrganizationId { get; set; }
        public List<UniformLoadout> Uniform { get; set; }
    }
    public class UniformLoadout
    {
        public string Name { get; set; }
        public int Torso { get; set; } = -1;
        public UniformItem Pants { get; set; } = new UniformItem();
        public UniformItem Shirt { get; set; } = new UniformItem();
        public UniformItem Shoes { get; set; } = new UniformItem();
        public UniformItem Hat { get; set; } = new UniformItem();
        public UniformItem Undershirt { get; set; } = new UniformItem();
        public UniformItem Accessory { get; set; } = new UniformItem();
        public UniformItem Badge { get; set; } = new UniformItem();
        public UniformItem BodyArmor { get; set; } = new UniformItem();
        public UniformItem BagsAndParachutes { get; set; } = new UniformItem();
        public UniformItem Mask { get; set; } = new UniformItem();

    }
    public class UniformItem
    {
        [XmlAttribute]
        public int Drawable { get; set; } = -1;
        [XmlAttribute]
        public int Texture { get; set; } = -1;
    }
}
