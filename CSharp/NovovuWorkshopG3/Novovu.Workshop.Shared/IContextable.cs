using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.Shared
{
    public interface IContextable
    {
        ContextOption[] ContextOptions { get; }
    }
    public class ContextOption
    {
        public string Name { get; }
        public delegate void Clicked();
        public event Clicked ContextClick;
        public ContextOption[] ChildOptions;
        public ContextOption(string name)
        {
            Name = name;
        }
        public void Select()
        {
            ContextClick?.Invoke();
        }
    }
}
