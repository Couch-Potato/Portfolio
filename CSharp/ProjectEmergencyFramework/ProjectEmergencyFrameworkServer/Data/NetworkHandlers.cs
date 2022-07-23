using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
namespace ProjectEmergencyFrameworkServer.Data
{
    public static class NetworkHandlers
    {
        public static List<RoutingBucket> RoutingBuckets = new List<RoutingBucket>() { new RoutingBucket() { IsMainBucket=true, BucketId=1, Name="MAIN"} };
        [Queryable("ROUTING_BUCKET_PRIVATE")]
        public static void RoutingBucketPrivate(Query q, object i, Player px)
        {
            foreach (var bucket in RoutingBuckets)
            {
                if (!bucket.IsMainBucket && !bucket.IsOccupied)
                {
                    bucket.IsOccupied = true;
                    SetPlayerRoutingBucket(px.Handle, bucket.BucketId);
                    q.Reply(true);
                    return;
                }
            }
            RoutingBuckets.Add(new RoutingBucket()
            {
                IsMainBucket = false,
                IsOccupied = true,
                BucketId = RoutingBuckets.Count + 1,
                Name="PRIVATE"
            });
            SetPlayerRoutingBucket(px.Handle, RoutingBuckets.Count);
            q.Reply(true);
        }
        [Queryable("ROUTING_BUCKET_MAIN")]
        public static void RoutingBucketMain(Query q, object i, Player px)
        {
            var currentBucket = GetPlayerRoutingBucket(px.Handle);
            if (currentBucket != 1 && RoutingBuckets[currentBucket - 1].Name =="PRIVATE")
            {
                RoutingBuckets[currentBucket - 1].IsOccupied = false;
            }
            SetPlayerRoutingBucket(px.Handle, 1);
            q.Reply(true);
        }
        [Queryable("ROUTING_BUCKET_CUSTOM")]
        public static void RoutingBucketCustom(Query q, object i, Player px)
        {
            string bucketName = (string)i;
            foreach (var bucket in RoutingBuckets)
            {
                if (bucket.Name == bucketName)
                {
                    SetPlayerRoutingBucket(px.Handle, bucket.BucketId);
                    q.Reply(true);
                    return;
                }
            }
            RoutingBuckets.Add(new RoutingBucket()
            {
                IsMainBucket = false,
                IsOccupied = true,
                BucketId = RoutingBuckets.Count + 1,
                Name = bucketName
            });
            SetPlayerRoutingBucket(px.Handle, RoutingBuckets.Count);
            q.Reply(true);
        }

    }
    public class RoutingBucket
    {
        public int BucketId { get; set; }
        public bool IsMainBucket { get; set; }
        public bool IsOccupied { get; set; }
        public string Name { get; set; }
    }
}
