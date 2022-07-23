using ProjectEmergencyFrameworkClient.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Services
{
    public delegate Task<bool> PETask();
    public static class TaskService
    {
        private static List<PETask> TaskList = new List<PETask>();
        private static Dictionary<PETask, string> AnonymousTaskNames = new Dictionary<PETask, string>();
        private static List<PETask> toRemove = new List<PETask>();
        /// <summary>
        /// Adds a method that will be called every tick until the method indicates that it is ready to expire
        /// </summary>
        /// <param name="task"></param>
        public static PETask InvokeUntilExpire(PETask task, string setTaskName = null)
        {
            TaskList.Add(task);
            if (setTaskName!= null)
            {
                AnonymousTaskNames.Add(task, setTaskName);
            }else
            {
                AnonymousTaskNames.Add(task, $"ANON_{new StackFrame(1).GetMethod().DeclaringType.Name}_" + new StackFrame(1).GetMethod().Name);
            }
            DebugService.DebugCall("TASK_ADD", AnonymousTaskNames[task]);
            return task;
        }
        /// <summary>
        /// Safely invoke a task to be ran only once
        /// </summary>
        /// <param name="task">The task to be ran</param>
        public static void InvokeOnce(Action task, string setTaskName = null)
        {
            InvokeUntilExpire(async () =>
            {
                task();
                return true;
            }, setTaskName != null ? setTaskName : $"ANON_{new StackFrame(1).GetMethod().DeclaringType.Name}_" + new StackFrame(1).GetMethod().Name);
        }
        /// <summary>
        /// Safely clear a task from the thread queue
        /// </summary>
        /// <param name="taskItem">The task to be cleared</param>
        public static void ForceExpireTask(PETask taskItem)
        {
            InvokeOnce(() =>
            {
                toRemove.Add(taskItem);
            },"FORCE_EXPIRE_" + AnonymousTaskNames[taskItem]);
        }

        [ExecuteAt(ExecutionStage.Tick)]
        public static async void Tick()
        {
            DebugService.SetDebugOwner("TASKS");
            foreach (var task in TaskList)
            {
                DebugService.SetDebugHandler(AnonymousTaskNames[task]);
                var result = await task();
                if (result)
                    toRemove.Add(task);
                DebugService.ClearDebugHandler();

            }
            DebugService.ClearDebugOwner();
            foreach (var item in toRemove)
            {
                TaskList.Remove(item);
                DebugService.DebugCall("TASK_REMOVE", AnonymousTaskNames[item]);
                AnonymousTaskNames.Remove(item);
            }
            toRemove.Clear();
        }
    }
}
