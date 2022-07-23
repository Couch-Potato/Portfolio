using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dock.Avalonia.Controls;
using Dock.Model;
using Dock.Model.Controls;
using Novovu.Workshop.Converters;
using Novovu.Workshop.ViewModels;

namespace Novovu.Workshop.DockSystem
{
    public class MainWindowDockFactory : Factory
    {
        Workspace.Workspace ws;
        public MainWindowDockFactory(Workspace.Workspace w)
        {
            ws = w;
        }
        private ProportionalDock GetLeftOrRight(Tool Left)
        {
            if (Left != null)
            {
                return new ProportionalDock
                {
                    Proportion = 0.2,
                    Orientation = Orientation.Vertical,
                    VisibleDockables = CreateList<IDockable>(
                        new ToolDock
                        {
                            Proportion = double.NaN,
                            ActiveDockable = Left,
                            VisibleDockables = CreateList<IDockable>(
                                Left
                                )
                        }
                        )
                };
            }
            return new ProportionalDock
            {
                Orientation = Orientation.Vertical,
                Proportion = 0
            };
        }
        private ProportionalDock GetCenter(Tool C1, Tool C2, bool IsLeft, bool IsRight)
        {
            double proportion = 0.6;
            if (!IsLeft) proportion += .2;
            if (!IsRight) proportion += .2;

            if (C2 != null)
            {
                return new ProportionalDock
                {
                    Proportion = proportion,
                    Orientation = Orientation.Vertical,
                    VisibleDockables = new List<IDockable>
                    {
                        new ToolDock
                        {
                            Proportion = 0.7,
                            ActiveDockable = C1,
                            VisibleDockables = CreateList<IDockable>(C1)
                        },
                        new SplitterDock(),
                        new ToolDock
                        {
                            Proportion = 0.3,
                            ActiveDockable = C2,
                            VisibleDockables = CreateList<IDockable>(C2)
                        }
                    }
                };
            }
            if (C1 == null && ws.IsMainDocument)
            {
                return new ProportionalDock
                {
                    Proportion = proportion,
                    Orientation = Orientation.Vertical,
                    VisibleDockables = new List<IDockable>
                    {
                        new DocumentDock
                        {
                            Id="DocumentsView",
                            Title="Documents",
                            IsCollapsable = false,
                            Proportion = double.NaN,
                            
                        },

                    }
                };
            }
            return new ProportionalDock
            {
                Proportion = proportion,
                Orientation = Orientation.Vertical,
                VisibleDockables = new List<IDockable>
                    {
                        new ToolDock
                        {
                            Proportion = double.NaN,
                            ActiveDockable = C1,
                            VisibleDockables = CreateList<IDockable>(C1)
                        },

                    }
            };

        }

        public override IDock CreateLayout()
        {
            var hierarch = ws.LeftHost;
            if (hierarch!= null && hierarch is IDockNameable)
                hierarch.Title = ((IDockNameable)hierarch).Name;
            var properties = ws.RightHost;
            if (properties != null && properties is IDockNameable)
                properties.Title = ((IDockNameable)properties).Name;
            var hierarch2 = ws.BottomHost;
            if (hierarch2 != null && hierarch2 is IDockNameable)
                hierarch2.Title = ((IDockNameable)hierarch2).Name;
            var engine = ws.CenterHost;
            if (engine != null && engine is IDockNameable)
                engine.Title = ((IDockNameable)engine).Name;

            var LEFT = GetLeftOrRight(hierarch);
            var RIGHT = GetLeftOrRight(properties);
            var CENTER = GetCenter(engine, hierarch2, hierarch != null, properties != null);
            var windowLayout = CreateRootDock();
            windowLayout.Title = "Default";
            var windowLayoutContent = new ProportionalDock
            {
                Proportion = double.NaN,
                Orientation = Orientation.Horizontal,
                IsCollapsable = false,
                VisibleDockables = CreateList<IDockable>
                (
                    LEFT,
                    new SplitterDock(),
                    CENTER,
                    new SplitterDock(),
                    RIGHT
                )
            };
            windowLayout.IsCollapsable = false;
            windowLayout.VisibleDockables = CreateList<IDockable>(windowLayoutContent);
            windowLayout.ActiveDockable = windowLayoutContent;

            var root = CreateRootDock();

            root.IsCollapsable = false;
            root.VisibleDockables = CreateList<IDockable>(windowLayout);
            root.ActiveDockable = windowLayout;
            root.DefaultDockable = windowLayout;
     /*       root.Top = CreatePinDock();
            root.Top.Alignment = Alignment.Top;
            root.Bottom = CreatePinDock();
            root.Bottom.Alignment = Alignment.Bottom;
            root.Left = CreatePinDock();
            root.Left.Alignment = Alignment.Left;
            root.Right = CreatePinDock();
            root.Right.Alignment = Alignment.Right;
*/
            return root;
        }
    }
}
