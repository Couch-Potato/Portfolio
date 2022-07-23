using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Services
{
    public static class HeadshotService
    {
        public static Task<string> GetHeadshotOfPed(int ped)
        {
            var tcs = new TaskCompletionSource<string>();
            Framework.FrameworkController.EXP["mugshot"].getMugshotUrl(ped, new Action<string>((string url) =>
            {
                tcs.TrySetResult(url);
            }));
            return tcs.Task;
        }
    }
}
