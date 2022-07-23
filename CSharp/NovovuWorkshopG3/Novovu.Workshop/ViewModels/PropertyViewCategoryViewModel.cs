using Novovu.Workshop.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.ViewModels
{
    public class PropertyViewCategoryViewModel : ViewModelBase
    {
        public PropertyViewCategoryViewModel(ObservableCollection<PropertyCategory> props)
        {
            _props = props;
        }
        private ObservableCollection<PropertyCategory> _props;
        public ObservableCollection<PropertyCategory> Properties { get => _props; }
    }
}
