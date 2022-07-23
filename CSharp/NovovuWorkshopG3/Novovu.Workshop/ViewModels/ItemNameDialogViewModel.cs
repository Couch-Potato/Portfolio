using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.ViewModels
{
    public class ItemNameDialogViewModel : ViewModelBase
    {
        public string Title { get; }
        private string _s = "";

        public LoadDialogResponse.Result Result = LoadDialogResponse.Result.Showed;
        
        public string Text { get => _s; set => this.RaiseAndSetIfChanged(ref _s, value); }

        public delegate void RequestCloseHandler(ItemNameDialogViewModel vm);
        public event RequestCloseHandler RequestClose;

        public void OK()
        {
            Result = LoadDialogResponse.Result.Confirmed;
            RequestClose?.Invoke(this);
        }
        public void Cancel()
        {
            Result = LoadDialogResponse.Result.Canceled;
            RequestClose?.Invoke(this);
        }

        public ItemNameDialogViewModel(string title)
        {
            Title = title;
        }
    }
    public class LoadDialogResponse
    {
        public enum Result
        {
            Showed,
            Confirmed,
            Canceled
        }
        public string Value;
        public Result DialogResult;
    }
}
