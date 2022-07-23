using ProjectEmergencyFrameworkClient.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Services
{
    public static class RoutingService
    {
        public static async Task RouterSetPrivateBucket()
        {
            await QueryService.QueryConcrete<bool>("ROUTING_BUCKET_PRIVATE", true);
        }
        public static async Task RouterSetMainBucket()
        {
            await QueryService.QueryConcrete<bool>("ROUTING_BUCKET_MAIN", true);
        }
        public static async Task RouterSetNamedBucket(string name)
        {
            await QueryService.QueryConcrete<bool>("ROUTING_BUCKET_CUSTOM", name);
        }
    }
}
