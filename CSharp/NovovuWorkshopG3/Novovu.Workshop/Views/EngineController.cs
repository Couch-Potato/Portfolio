using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Media.TextFormatting;
using Avalonia.Platform;
using Avalonia.Threading;
using DynamicData.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Novovu.Workshop.Models;
using Novovu.Workshop.ProjectModel;
using Novovu.Xenon.AvaloniaControl;
using Novovu.Xenon.Engine;
using Novovu.Xenon.Logging;
using System;
using System.Collections.Generic;
using Novovu.Controls.ContextMenu;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using ContextMenu = Novovu.Controls.ContextMenu.ContextMenu;

namespace Novovu.Workshop.Views
{
    public class EngineController : EngineControlPlane
    {
        IAssetLoader assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
        Bitmap leftItem;
        Bitmap rightItem;
        Bitmap centerItem;

        public delegate void KeyEvent(Keys key);
        public event KeyEvent EngineKeyDown;
        public event KeyEvent EngineKeyUp;


        ContextMenu NovovuContextMenu = new ContextMenu();
        public List<ContextMenuItem> ContextMenuItems
        {
            get => NovovuContextMenu.Items;
            set {
                NovovuContextMenu.Items = value;
                NovovuContextMenu.Hide();
            }
        }



        public List<ToolItem> Tools = new List<ToolItem>();
        public List<ToolItem> QuickButtons = new List<ToolItem>();

        public bool MovementEnabled = true;

        private ToolItem Selected;
        public ToolItem SelectedTool
        {
            get => Selected;
            set
            {
                if (Selected != null)
                {
                    Selected.Selected = false;
                    Selected.SelectionLost?.Invoke();
                }
                value.Selected = true;
                Selected = value;
                value.SelectionGained?.Invoke();
            }
        }

        public bool DrawConsole = false;
        public string[] ConsoleLines = new string[5];

        public EngineController()
        {
            ProjectStatic.MainWindow.KeyUp += MainWindow_KeyUp;
            ProjectStatic.MainWindow.KeyDown += MainWindow_KeyDown;
            Width = 300;
            Height = 300;
            Game = new Novovu.Xenon.Engine.Game();
            Stopwatch sw = new Stopwatch();
            //UpdateLoop();
            var updateThread = new Thread(() =>
            {
                while (true)
                {
                    if (sw.ElapsedMilliseconds < 20)
                        Thread.Sleep(20 - (int)sw.ElapsedMilliseconds);
                    sw.Reset();
                    sw.Start();
                    UpdateLoop();
                    sw.Stop();
                    //Debug.WriteLine("UDO Took " + sw.ElapsedMilliseconds + "ms");
                }
            });
            updateThread.IsBackground = true;
            updateThread.Priority = ThreadPriority.AboveNormal;
            updateThread.Start();

            Logger.LogHandler = (Logger.LogTypes type, string time, string from, string message) =>
            {
                if (type == Logger.LogTypes.Script)
                {
                    ConsoleLines[4] = ConsoleLines[3];
                    ConsoleLines[3] = ConsoleLines[2];
                    ConsoleLines[2] = ConsoleLines[1];
                    ConsoleLines[1] = ConsoleLines[0];
                    ConsoleLines[0] = $"[{from}] {message}";
                }
                Debug.WriteLine($"{time} [{from}] ~{type}~ {message}");
            };

            centerItem = new Bitmap(assets.Open(new Uri("avares://Novovu.Workshop/Assets/t_default_center.png")));
            rightItem = new Bitmap(assets.Open(new Uri("avares://Novovu.Workshop/Assets/t_default_rightround.png")));
            leftItem = new Bitmap(assets.Open(new Uri("avares://Novovu.Workshop/Assets/t_default_leftround.png")));
            ft.Typeface = tf;
            ft.FontSize = 12;
            this.OnGDeviceResize += EngineController_OnGDeviceResize;

            //Runtime = new Xenon.Runtime.RenderRuntime(0, 0);
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            var k = KeyConvert(e.Key);
            if (k == Keys.LeftShift)
                ShiftDown = true;
            EngineKeyDown?.Invoke(k);
            Xenon.Engine.Engine.Input.SetKey(k, true);
            base.OnKeyDown(e);
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            var k = KeyConvert(e.Key);
            if (k == Keys.LeftShift)
                ShiftDown = false;
            EngineKeyUp?.Invoke(k);
            Xenon.Engine.Engine.Input.SetKey(k, false);
            base.OnKeyUp(e);
        }

        private void EngineController_OnGDeviceResize(float width, float height)
        {
            Xenon.Engine.Engine.AspectRatio = width / height;
        }

        public bool IsGraphicsReady = false;
        protected override void OnInitialized()
        {
            base.OnInitialized();
            Xenon.Engine.Engine.graphicsDevice = GraphicsDevice;
            IsGraphicsReady = true;
            OnGraphicsInstanceReady?.Invoke(this, new EventArgs());
        }

        public event EventHandler OnGraphicsInstanceReady;
        private Vector2 oldMouseCords = new Vector2(0, 0);
        float Sensitivity = 3f;
        public float Speed = 50f;
        public float MovementSpeed = 1f;
        Xenon.Runtime.IRuntime Runtime;
        public bool AllowSpeedSet = true;

        protected override void Update(GameTime gameTime)
        {


            var MoveVector = Vector3.Zero;
            if (Xenon.Engine.Engine.Input.IsKeyDown(Keys.W) && MovementEnabled)
            {
                MoveVector.Z = MovementSpeed*(float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (Xenon.Engine.Engine.Input.IsKeyDown(Keys.S) && MovementEnabled)
            {
                MoveVector.Z = -MovementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (Xenon.Engine.Engine.Input.IsKeyDown(Keys.A) && MovementEnabled)
            {
                MoveVector.X = MovementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (Xenon.Engine.Engine.Input.IsKeyDown(Keys.D) && MovementEnabled)
            {
                MoveVector.X = -MovementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (Xenon.Engine.Engine.Input.IsKeyDown(Keys.E) && MovementEnabled)
            {
                MoveVector.Y = MovementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (Xenon.Engine.Engine.Input.IsKeyDown(Keys.Q) && MovementEnabled)
            {
                MoveVector.Y = -MovementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (Xenon.Engine.Engine.Input.RightButton == ButtonState.Pressed)
            {
                var mstate = Xenon.Engine.Engine.Input.MousePosition;
                if (oldMouseCords.X == 0 && oldMouseCords.Y == 0)
                {
                    oldMouseCords = new Vector2(mstate.X, mstate.Y);
                }
                var dif = new Vector2(mstate.X, mstate.Y) - oldMouseCords;
                var addX = -dif.X / Sensitivity;
                var addY = dif.Y / Sensitivity;
                Game.SelectedLevel.Camera.Orientation += new Vector3(addY, addX, 0);
                oldMouseCords = mstate;
                //Mouse.SetPosition(Engine.graphicsDevice.Viewport.Width / 2, Engine.graphicsDevice.Viewport.Height / 2);
            }
            if (Xenon.Engine.Engine.Input.RightButton == ButtonState.Released)
            {
                oldMouseCords = new Vector2(0, 0);
            }
            if (MoveVector != Vector3.Zero)
            {
                MoveVector *= Speed;
                Game.SelectedLevel.Camera.TranslationVector(MoveVector);
            }
            base.Update(gameTime);
        }
        int pItems = 3;
        
        public override void Render(DrawingContext context)
        {
            base.Render(context);




            if (Tools.Count >= 2)
            {

                int centerPieces = Tools.Count - 2;
                var initPoint = new Avalonia.Point(Width - (centerPieces * 47) - 20, 20);
                context.DrawImage(rightItem, new Rect(initPoint.X, initPoint.Y, 47, 47));
                context.DrawImage(Tools[Tools.Count - 1].ItemMap, new Rect(initPoint.X + 7.5, initPoint.Y + 7.5, 32, 32));

                for (int i = 1; i < Tools.Count - 1; i++)
                {
                    context.DrawImage(centerItem, new Rect(initPoint.X - (i * 47), initPoint.Y, 47, 47));
                    context.DrawImage(Tools[i].ItemMap, new Rect(initPoint.X + 7.5 - (i * 47), initPoint.Y + 7.5, 32, 32));
                }

                context.DrawImage(leftItem, new Rect(initPoint.X - (Tools.Count * 47) + 47, initPoint.Y, 47, 47));
                context.DrawImage(Tools[0].ItemMap, new Rect(initPoint.X + 7.5 - (Tools.Count * 47) + 47, initPoint.Y + 7.5, 32, 32));
            }

            if (QuickButtons.Count >= 2)
            {
                int centerPieces = QuickButtons.Count - 2;
                var initPoint = new Avalonia.Point(60, 20);
                context.DrawImage(rightItem, new Rect(initPoint.X, initPoint.Y, 47, 47));
                context.DrawImage(QuickButtons[QuickButtons.Count - 1].ItemMap, new Rect(initPoint.X + 7.5, initPoint.Y + 7.5, 32, 32));

                for (int i = 1; i < QuickButtons.Count - 1; i++)
                {
                    context.DrawImage(centerItem, new Rect(initPoint.X - (i * 47), initPoint.Y, 47, 47));
                    context.DrawImage(QuickButtons[i].ItemMap, new Rect(initPoint.X + 7.5 - (i * 47), initPoint.Y + 7.5, 32, 32));
                }

                context.DrawImage(leftItem, new Rect(initPoint.X - (QuickButtons.Count * 47) + 47, initPoint.Y, 47, 47));
                context.DrawImage(QuickButtons[0].ItemMap, new Rect(initPoint.X + 7.5 - (QuickButtons.Count * 47) + 47, initPoint.Y + 7.5, 32, 32));
            }

            int iv = 0;
            foreach (string s in ConsoleLines)
            {
                if (!string.IsNullOrWhiteSpace(s) && DrawConsole)
                {
                    iv++;

                    ft.Text = s;

                    context.DrawText(Brushes.White, new Avalonia.Point(10, (iv * -14) + Height - 10), ft);
                }
            }

            NovovuContextMenu.Draw(context);

            /// RUNTIMES ARE NOT BEING USED YET!
            /// THEY WILL BE INCLUDED IN A LATER VERSION
            /// WHEN I FIX THE RENDER PIPELINE
        /*    if (Runtime != null)
            {
                var frame = Runtime.Render();
                Runtime.Width = (float)Width;
                Runtime.Height = (float)Height;
                frame = Xenon.AvaloniaControl.Utility.Bitmapper.FilterOutColor(frame, Colors.Black);
                var bmp = Xenon.AvaloniaControl.Utility.Bitmapper.ToBitmap(frame, RuntimeBitmap, (int)Runtime.Width, (int)Runtime.Height);
                context.DrawImage(bmp, new Rect(0, 0, Runtime.Width, Runtime.Height));
            }
            */
        }
        WriteableBitmap RuntimeBitmap;
        Typeface tf = new Typeface(FontFamily.Default, FontStyle.Normal, FontWeight.Medium);
        FormattedText ft = new FormattedText();

        protected override void OnKeyDown(KeyEventArgs e)
        {
           
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.Key == Key.OemPlus)
            {
                MovementSpeed = MovementSpeed * 2;
            }
            if (e.Key == Key.OemMinus)
            {
                MovementSpeed = MovementSpeed / 2;
            }
        }
        protected override void OnPointerMoved(PointerEventArgs e)
        {
            
            Avalonia.Point Position = e.GetPosition(this);
            Xenon.Engine.Engine.Input.MousePosition = new Vector2((float)Position.X, (float)Position.Y);
            NovovuContextMenu.MouseMove(e.GetPosition(this));
            base.OnPointerMoved(e);

        }
        bool LeftPressed = false;
        bool RightPressed = false;
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            var pt = e.GetCurrentPoint(this);
            if (pt.Properties.IsLeftButtonPressed)
            {
                LeftPressed = true;

            }
            if (pt.Properties.IsRightButtonPressed)
            {
                RightPressed = true;
    
            }
            var mp = e.GetCurrentPoint(this).Properties;

            if (Tools.Count >= 2)
            {

                int centerPieces = Tools.Count - 2;
                var initPoint = new Avalonia.Point(Width - (centerPieces * 47) - 20, 20);
                Rect rightMost = new Rect(Width - (centerPieces * 47) - 20, 20, 47,47);
                if (rightMost.Contains(e.GetPosition(this)))
                {
                    Selected?.SelectionLost?.Invoke();
                    if (Selected != null)
                    {
                        Selected.Selected = false;
                    }
                    Tools[Tools.Count - 1].Selected = true;
                    Tools[Tools.Count - 1].SelectionGained?.Invoke();
                    Selected = Tools[Tools.Count - 1];
                    return;
                }

                for (int i = 1; i < Tools.Count - 1; i++)
                {
                    var hitbox = new Rect(initPoint.X - (i * 47), 20, 47, 47);
                    if (hitbox.Contains(e.GetPosition(this)))
                    {
                        if (Selected != null)
                        {
                            Selected.Selected = false;
                        }
                        Selected?.SelectionLost?.Invoke();
                        Tools[i].Selected = true;
                        Tools[i].SelectionGained?.Invoke();
                        Selected = Tools[i];
                        return;
                    }
                }

                var leftMost = new Rect(initPoint.X - (Tools.Count * 47) + 47, initPoint.Y, 47, 47);
                if (leftMost.Contains(e.GetPosition(this)))
                {
                    if (Selected != null)
                    {
                        Selected.Selected = false;
                    }
                    Selected?.SelectionLost?.Invoke();
                    Tools[0].Selected = true;
                    Tools[0].SelectionGained?.Invoke();
                    Selected = Tools[0];
                    return;
                }
            }


            if (mp.IsLeftButtonPressed)
            {
                Xenon.Engine.Engine.Input.LeftButton = ButtonState.Pressed;
            }else
            {
                Xenon.Engine.Engine.Input.LeftButton = ButtonState.Released;
            }
            if (mp.IsMiddleButtonPressed)
            {
                Xenon.Engine.Engine.Input.MiddleButton = ButtonState.Pressed;
            }
            else
            {
                Xenon.Engine.Engine.Input.MiddleButton = ButtonState.Released;
            }
            if (mp.IsRightButtonPressed)
            {
                Xenon.Engine.Engine.Input.RightButton = ButtonState.Pressed;
            }
            else
            {
                Xenon.Engine.Engine.Input.RightButton = ButtonState.Released;
            }





            base.OnPointerPressed(e);
        }
        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            var pt = e.GetCurrentPoint(this);
            if (!pt.Properties.IsLeftButtonPressed && LeftPressed )
            {
                NovovuContextMenu.Click(e.GetPosition(this));
                LeftPressed = false;
            }
            if (!pt.Properties.IsRightButtonPressed && RightPressed && ShiftDown)
            {
                NovovuContextMenu.invpos = e.GetPosition(this);
                RightPressed = false;
                NovovuContextMenu.Show();
            }
            var mp = e.GetCurrentPoint(this).Properties;
            if (mp.IsLeftButtonPressed)
            {
                Xenon.Engine.Engine.Input.LeftButton = ButtonState.Pressed;
            }
            else
            {
                Xenon.Engine.Engine.Input.LeftButton = ButtonState.Released;
            }
            if (mp.IsMiddleButtonPressed)
            {
                Xenon.Engine.Engine.Input.MiddleButton = ButtonState.Pressed;
            }
            else
            {
                Xenon.Engine.Engine.Input.MiddleButton = ButtonState.Released;
            }
            if (mp.IsRightButtonPressed)
            {
                Xenon.Engine.Engine.Input.RightButton = ButtonState.Pressed;
            }
            else
            {
                Xenon.Engine.Engine.Input.RightButton = ButtonState.Released;
            }

            base.OnPointerReleased(e);
        }
        private bool ShiftDown = false;
        private Keys KeyConvert(Key key)
        {
           
            try
            {
                if (key == Key.LeftCtrl)
                {
                    return Keys.LeftControl;
                }
                if (key == Key.RightCtrl)
                {
                    return Keys.RightControl;
                }
                return (Keys)Enum.Parse(typeof(Keys), key.ToString());
            }
            catch { }
            return Keys.SelectMedia;
        }

    }
}
