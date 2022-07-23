using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProjectEmergencyFrameworkShared.Configuration.Schema
{
    public class PedBarterRegion
    {
        public string Name { get; set; }
        public List<PurchaseOrder> PurchaseOrders { get; set; }
        public Location Location { get; set; }
        public float Radius { get; set; }
        public TimeRange AllowedBarterTimeRange { get; set; }
        public List<PedConfig> BarterPeds { get; set; }

    }
    public class PurchaseOrder
    {
        public string ItemName { get; set; }
        public float BasePrice { get; set; }
        public float PriceFlexibility { get; set; }
        public float PurchaseChance { get; set; }
    }
    public class TimeRange
    {
        [XmlAttribute]
        public bool AllDay { get; set; }
        public float To { get; set; }
        public float From { get; set; }

    }
    public class PedConfig
    {
        [XmlAttribute]
        public string ModelName { get; set; }
    }
}
