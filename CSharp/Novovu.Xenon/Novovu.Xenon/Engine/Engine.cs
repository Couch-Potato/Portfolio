using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Novovu.Xenon.Engine
{
    public class Engine
    {
        public static IServiceProvider ContentProvider;
        public static GraphicsDevice graphicsDevice;
        public static ContentManager CoreContentManager;
        public static GraphicsDeviceManager GraphicsDeviceManager;
        public static SpriteBatch SpriteBatch;
        public static IntPtr Handle;

        public static Assets.AssetLoader AssetLoader;

        public static EventModel EventModel = new EventModel();
   

        private static bool aa = true;

        private static int mscDefault = 0;
        private static bool preferMSC = false;
        private static GraphicsProfile profile = GraphicsProfile.HiDef;
        public static  object GetTexture()
        {
            //int[] imgData = new int[bmp.Width * bmp.Height];
          //  GraphicsDevice dev = graphicsDevice;
           // Texture2D texture = new Texture2D(dev, bmp.Width, bmp.Height);
           
          //  unsafe
          //  {
                // lock bitmap
                //System.Drawing.Imaging.BitmapData origdata =
                   // bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);

                //uint* byteData = (uint*)origdata.Scan0;

                // Switch bgra -> rgba
                //for (int i = 0; i < imgData.Length; i++)
               // {
                    //byteData[i] = (byteData[i] & 0x000000ff) << 16 | (byteData[i] & 0x0000FF00) | (byteData[i] & 0x00FF0000) >> 16 | (byteData[i] & 0xFF000000);
               // }

                // copy data
               // System.Runtime.InteropServices.Marshal.Copy(origdata.Scan0, imgData, 0, bmp.Width * bmp.Height);

               // byteData = null;

                // unlock bitmap
               // bmp.UnlockBits(origdata);
            //}

           // texture.SetData(imgData);

            return null;
        }

        

        public static bool AntiAliasing
        {
            get
            {
                return aa;
            }
            set
            {
                aa = value;
                if (aa)
                {

                    GraphicsDeviceManager.GraphicsProfile = GraphicsProfile.HiDef;
                    GraphicsDeviceManager.PreferMultiSampling = true;
                    graphicsDevice.PresentationParameters.MultiSampleCount = 8;
                }
                else
                {
                    GraphicsDeviceManager.GraphicsProfile = profile;
                    GraphicsDeviceManager.PreferMultiSampling = preferMSC;
                    graphicsDevice.PresentationParameters.MultiSampleCount = mscDefault;
                }
            }
        }
        public static Vector3 GetVectorDirection(Vector3 x1, Vector3 x2)
        {
            Vector3 rtf = x1 - x2;
            rtf.Normalize();
            return rtf;
        }
        public static float GetDistanceOfPoints(Vector3 n1, Vector3 n2)
        {
            float X = n2.X - n1.X;
            float Y = n2.Y - n1.Y;
            float Z = n2.Z - n1.Z;
            X = (float)Math.Pow(X, 2);
            Y = (float)Math.Pow(Y, 2);
            Z = (float)Math.Pow(Z, 2);
            return (float)Math.Sqrt(X + Y + Z);
        }
        public static void Init()
        {
            mscDefault = graphicsDevice.PresentationParameters.MultiSampleCount;
            preferMSC = GraphicsDeviceManager.PreferMultiSampling;
            profile = GraphicsProfile.HiDef;
            AntiAliasing = true;
            AssetLoader = new Assets.AssetLoader(ContentProvider, "");
        }
    }
}
