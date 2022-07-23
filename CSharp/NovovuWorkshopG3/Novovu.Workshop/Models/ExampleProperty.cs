using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Novovu.Workshop.Shared;
namespace Novovu.Workshop.Models
{
    public class ExampleProperty
    {
        [Property("Test","", Property.PropertyType.String)]
        public string Test
        {
            get { return "123"; }
            set
            {
                Debug.WriteLine(value);
            }
        }
        [Property("Vector", "", Property.PropertyType.Vector3)]
        public Vector3 Vector
        {
            get => Vector3.Zero;
            set
            {
                Debug.WriteLine(value);
            }
        }

    }
}
