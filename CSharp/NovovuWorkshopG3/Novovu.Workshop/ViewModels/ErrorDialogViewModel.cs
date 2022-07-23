using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.ViewModels
{
    public class ErrorDialogViewModel : ViewModelBase
    {
        public string ExceptionTitle { get; }
        public string ExcpetionMessage { get; }
        public ErrorDialogViewModel(Exception ex)
        {
            ExceptionTitle = ex.GetType().Name;
            ExcpetionMessage = ex.Message;
        }
    }
}
