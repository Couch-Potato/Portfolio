using Novovu.Workshop.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.Models
{
    public class Component : Novovu.Xenon.Engine.Component, IContextable
    {
        public ContextOption[] ContextOptions => throw new NotImplementedException();
    }
}
