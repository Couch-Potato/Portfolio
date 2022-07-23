using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interfaces
{
    public interface IUserInterface
    {
        /// <summary>
        /// Shows an interface
        /// </summary>
        Task Show();

        /// <summary>
        /// Hides an interface
        /// </summary>
        void Hide();
        void Destroy();
    }
}
