using ProjectEmergencyFrameworkClient.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Services
{
    public static class CameraService
    {
        public static CameraOperator currentOperator;
        public static void SetCamera<T>(bool interop= false, int type=0, dynamic mods = null) where T : CameraOperator
        {
            currentOperator = Activator.CreateInstance<T>();
            currentOperator.Interop = interop;
            currentOperator.CameraType = type;
            currentOperator.Modifiers = mods;
            currentOperator.Start();
        }
        public static void Terminate()
        {
            if (currentOperator != null)
            {
                currentOperator.Stop();
            }
            currentOperator = null;
        }
        public static T GetCameraOperator<T>() where T : CameraOperator
        {
            return (T)currentOperator;
        }
        [ExecuteAt(ExecutionStage.Tick)]
        public static void Tick()
        {
            if (currentOperator!= null)
            {
                currentOperator.Tick();
            }
        }
    }
    public abstract class CameraOperator
    {
        protected abstract void OnTick();
        protected virtual void OnStart() { }
        protected virtual void OnStop() { }
        public int CameraType { get; set; } = 0;
        public dynamic Modifiers { get; set; } = new { };
        public bool Interop { get; set; } = false;
        public void Tick()
        {
            OnTick();
        }
        public void Stop()
        {
            OnStop();
        }
        public void Start()
        {
            OnStart();
        }
    }

    
}
