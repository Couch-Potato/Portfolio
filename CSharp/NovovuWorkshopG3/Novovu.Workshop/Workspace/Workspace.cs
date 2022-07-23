using Dock.Model;
using Dock.Model.Controls;
using Novovu.Workshop.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.Workspace
{
    public abstract class Workspace:ViewModelBase
    {
        public string Name { get; }
        public string Icon { get; }

        public bool Active { get; private set; }

        public Tool leftHost;
        public Tool rightHost;
        public Tool bottomHost;
        public Tool centerHost;
        public bool IsMainDocument = false;
        public void Activate()
        {
            Active = true;
            WorkspaceRegistry.SetActive(this);
            
            //Engine.ControlPlane = new Views.EngineController();
        }
        public void Deactivate()
        {
        
            
            Active = false;
        }
        MainWindowViewModel main;
        public void Link(MainWindowViewModel mmvm)
        {
            main = mmvm;
        }
        public void SetActiveDocument(Document dx)
        {
            if (main != null)
            {
                if (main.Layout?.ActiveDockable is IDock active)
                {
                    if (active.Factory?.FindDockable(active, (d) => d.Id == "DocumentsView") is IDock files)
                    {
                        main.Factory?.AddDockable(files, dx);
                        main.Factory?.SetActiveDockable(dx);
                        main.Factory?.SetFocusedDockable(main.Layout, dx);
                    }
                }
            }
            else
            {
                throw new Exception("Cannot set the active document when a workspace is not linked.");
            }
        }
        public Tool LeftHost
        {
            get => leftHost;
            private set => this.RaiseAndSetIfChanged(ref leftHost, value);
        }

        public Tool RightHost
        {
            get => rightHost;
            private set => this.RaiseAndSetIfChanged(ref rightHost, value);
        }
        public Tool BottomHost
        {
            get => bottomHost;
            private set => this.RaiseAndSetIfChanged(ref bottomHost, value);
        }
        public Tool CenterHost
        {
            get => centerHost;
            private set => this.RaiseAndSetIfChanged(ref centerHost, value);
        }
        public Workspace(string Name, string Icon, Tool Left, Tool Right, Tool Bottom, Tool Center)
        {
            this.Name = Name;
            this.Icon = Icon;
            this.LeftHost = Left;
            RightHost = Right;
            BottomHost = Bottom;
            CenterHost = Center;
            WorkspaceRegistry.ActivationChanged += WorkspaceRegistry_ActivationChanged;
        }

        private void WorkspaceRegistry_ActivationChanged(Workspace w)
        {
            if (w != this)
            {
                Deactivate();
            }
        }
    }
    public class WorkspaceInterface
    {
        public void Raise(string signal, params object[] parameters)
        {
            if (Signals.ContainsKey(signal))
            {
                foreach (var sig in Signals[signal])
                {
                    sig(parameters);
                }
            }
        }
        public delegate void Signaled(params object[] parameters);
        public Dictionary<string, List<Signaled>> Signals = new Dictionary<string, List<Signaled>>();
        public void OnSignal(string signalType, Signaled signal)
        {
            if (Signals.ContainsKey(signalType))
            {
                Signals[signalType].Add(signal);
                return;
            }
            Signals.Add(signalType, new List<Signaled>()
            {
                signal
            });

        }
    }
}
