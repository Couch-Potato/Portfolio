using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Accord.Video.FFMPEG;
using Microsoft.Xna.Framework;

namespace Novovu.Xenon.Video
{
  
    public class Video
    {
       // VideoFileReader reader = new VideoFileReader();
        public Video(Stream source)
        {
            string temp = Path.GetTempFileName();
            Logging.Logger.Log(Logging.Logger.LogTypes.Message, "Xenon.Video", "Writing video stream to temporary file");
            using (Stream s = File.OpenRead(temp))
            {
                source.CopyTo(s);
            }
            Logging.Logger.Log(Logging.Logger.LogTypes.Message, "Xenon.Video", "Loading video stream from temporary file");
            //reader.Open(temp);
            Logging.Logger.Log(Logging.Logger.LogTypes.Message, "Xenon.Video", "Cleaning temporary file");
            File.Delete(temp);
        }
        public void Play()
        {
            StartTime = LastSetTime;
            Started = true;
        }
        public void Stop()
        {
            StartTime = null;
            Started = false;
        }
        GameTime StartTime;
        GameTime LastSetTime;
        public bool Started = false;
        public void Update(GameTime span)
        {
            LastSetTime = span;
        }
       // public Bitmap Draw()
       // {
       //     int frameId = 0;
        //    if (StartTime == null)
         //   {
         //       return null;
         //   }
         //   float elapsedSinceStart = LastSetTime.TotalGameTime.Milliseconds - StartTime.TotalGameTime.Milliseconds;
         //   float millsPerFrame = (float)(1000 / reader.FrameRate.ToDouble());
         //   frameId = (int)(elapsedSinceStart / millsPerFrame);
         //   return reader.ReadVideoFrame(frameId);
       // }
    }
}
