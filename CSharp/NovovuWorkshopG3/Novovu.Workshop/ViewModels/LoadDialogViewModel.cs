using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.ViewModels
{
    public class LoadDialogViewModel : ViewModelBase
    {
        public string Title
        {
            get;
        }
        public LoadDialogViewModel(string title)
        {
            Title = title;
        }

        private double _s = 0;
        public double ShowW
        {
            get => _s;
            set => this.RaiseAndSetIfChanged(ref _s, value);
        }

        private string _t = "";
        public string Status
        {
            get => _t;
            set => this.RaiseAndSetIfChanged(ref _t, value);
        }
    }
}
