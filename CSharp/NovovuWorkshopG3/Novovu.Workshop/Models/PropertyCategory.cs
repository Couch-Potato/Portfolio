using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.Models
{
    public class PropertyCategory
    {
        public PropertyCategory()
        {
            Properties = new ObservableCollection<GlobalProperty>();
        }
        public string PropertyName { get; set; }
        public string PropertySet { get; set; }
        public ObservableCollection<GlobalProperty> Properties { get; set; }
    }
}
