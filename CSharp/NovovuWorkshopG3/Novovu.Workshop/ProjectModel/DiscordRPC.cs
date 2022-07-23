using DiscordRPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.ProjectModel
{
    public class DiscordRPC
    {
        private static DiscordRpcClient client;
        public static void Initialize()
        {
			client = new DiscordRpcClient("556979532977012745");
			//Connect to the RPC
			client.Initialize();
		}
        public static void SetPresence(string status)
        {
            var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            client.SetPresence(new RichPresence()
            {
                State=status,
                Details="Novovu Workshop",
                Timestamps = new Timestamps()
                {
                    Start = DateTime.UtcNow
                },
                Assets= new Assets()
                {
                    LargeImageKey="2",
                    SmallImageKey="2"
                }
            });
        }
    }
}
