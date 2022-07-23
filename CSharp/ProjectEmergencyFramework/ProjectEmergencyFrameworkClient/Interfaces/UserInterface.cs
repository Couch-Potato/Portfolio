using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;

namespace ProjectEmergencyFrameworkClient.Interfaces
{
    public abstract class UserInterface : IUserInterface
    {
        private dynamic _properties = null;
        public dynamic Properties { get
            {
                return _properties;
            }
            set
            {
                DebugService.Watchpoint("PROPERTIES_SET", _properties == null);
                _properties = value;
            }
        }
        public delegate void ReactiveHook();
        public UserInterface()
        {
            

        }
        protected virtual bool AllowHiddenConfiguration { get; } = true;
        protected virtual async Task BeforeShow() { }

        public void SetProperties(dynamic props)
        {
            Properties = props;
        }
        bool IsDestroyed = false;
        public void Destroy()
        {
            IsDestroyed = true;
            //Properties = new { };
        }
        protected virtual async Task ConfigureAsync() { }
        public bool IsShowing = false;
        private async Task OkDoConfigure()
        {
            if (IsDestroyed) return;
            if (!IsShowing && !AllowHiddenConfiguration) return;
            try
            {
                await ConfigureAsync();
            }
            catch(Exception ex)
            {
                DebugService.UnhandledException(ex);
            }
            
            Type UIType = GetType();

            foreach (var prop in UIType.GetProperties())
            {
                if (prop.GetCustomAttribute<ConfigurationAttribute>() != null)
                {
                    var attb = prop.GetCustomAttribute<ConfigurationAttribute>();

                    
                    SendConfiguration(attb.ConfigurationName, prop.GetValue(this, null));
                }
            }
        }

        public void Update()
        {
            if (IsDestroyed) return;
            OkDoConfigure().Wait();
        }
        public async Task UpdateAsync()
        {
            if (IsDestroyed) return;
            await OkDoConfigure();
        }

        public async Task Show()
        {
            IsShowing = true;
            if (IsDestroyed) return;
            await OkDoConfigure();
            await BeforeShow();
            
            InterfaceController.I_SHOW(this);
            AfterShow();
            AttachHooks();
           
        }
        /// <summary>
        /// Helper that allows you to set reactive fields in a type
        /// </summary>
        private void AttachHooks()
        {
            Type UIType = GetType();

            var fields = UIType.GetProperties();

            Hook("_FORCEhide", (object trash) =>
            {
                Hide();
            });

            foreach (var method in UIType.GetMethods())
            {
                if (method.GetCustomAttribute<ReactiveAttribute>() != null)
                {
                    var reactive = method.GetCustomAttribute<ReactiveAttribute>();
                    var p = method.GetParameters();
                    //Debug.WriteLine(p.Length.ToString());
                    
                    var deleg = (ReactiveHook)method.CreateDelegate(typeof(ReactiveHook), this);
                    string name = reactive.IsCustomName ? reactive.CustomName : method.Name;

                    Hook(name, (object trash) =>
                    {
                        deleg();
                    });
                }
            }

            foreach (var field in fields)
            {
                if (field.GetCustomAttribute<ReactiveAttribute>() != null)
                {
                    var reactive = field.GetCustomAttribute<ReactiveAttribute>();
                    string name = reactive.IsCustomName ? reactive.CustomName : field.Name;

                    if (field.PropertyType == typeof(int))
                    {
                        Hook(name, (int i)=>
                        {
                            if (IsDestroyed) return;

                            field.SetValue(this, i, null);
                        });
                    }else if (field.PropertyType == typeof(string))
                    {
                        //Needed to not kill itself if you enter a number becuz fivem dumb
                        Hook(name, (object i) =>
                        {
                            if (IsDestroyed) return;

                            field.SetValue(this, i.ToString(), null);
                        });
                    }
                    else if (field.PropertyType == typeof(bool))
                    {
                        Hook(name, (bool i) =>
                        {
                            if (IsDestroyed) return;

                            field.SetValue(this, i, null);
                        });
                    }
                    else if (field.PropertyType == typeof(double))
                    {
                        Hook(name, (double i) =>
                        {
                            if (IsDestroyed) return;

                            field.SetValue(this, i, null);
                        });
                    }
                    else if (field.PropertyType == typeof(float))
                    {
                        Hook(name, (double i) =>
                        {
                            if (IsDestroyed) return;

                            field.SetValue(this, (float)i, null);
                        });
                    }
                    else if (field.PropertyType == typeof(object))
                    {
                        Hook(name, (object i) =>
                        {
                            if (IsDestroyed) return;

                            field.SetValue(this, i, null);
                        });
                    }
                    
                }
            }
        }

        public void ReactUpdate<T>(string item, T data)
        {
            InterfaceController.CONFIGURE(HookName, "update_" + item, data);
        }

        public virtual void AfterShow() { }

        public void Hide()
        {
            IsShowing = false;
            Cleanup();
            InterfaceController.I_HIDE(this);
            InterfaceController.SneakyHide(this);
        }
        protected virtual void Cleanup() { }

        public string HookName { get {
                return this.GetType().GetCustomAttribute<UserInterfaceAttribute>().HOOK;
            }
        }

        protected void Hook<T>(string name, Action<T> HookResponse)
        {
            InterfaceController.HOOK(HookName, name, HookResponse);
        }

        protected void SendConfiguration(string name, object data)
        {
            InterfaceController.CONFIGURE(HookName, name, data);
        }
    }
}
