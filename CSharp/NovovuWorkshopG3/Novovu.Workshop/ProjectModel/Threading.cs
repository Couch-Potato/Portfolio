using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.ProjectModel
{
    public class Threading
    {
        /// <summary>
        /// Posts a task to be done on the UI thread.
        /// </summary>
        /// <typeparam name="T">The return value</typeparam>
        /// <param name="action">The function to be posted</param>
        /// <returns>The result of the task.</returns>
        public static async Task<T> PostUITask<T>(Func<T> action)
        {
            return await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync<T>(action, Avalonia.Threading.DispatcherPriority.Background);
        }

        public static async Task PostUITask(Action action)
        {
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(action, Avalonia.Threading.DispatcherPriority.Background);
        }
    }
}
