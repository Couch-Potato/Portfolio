using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Argon
{
    public class ARTComposition
    {
        private ChromiumWebBrowser handle;
        public bool CWB_Run = false;
        public void SetHandle(object ha)
        {
            
            handle = (ChromiumWebBrowser)ha;
            handle.LoadingStateChanged += Handle_LoadingStateChanged;
            handle.ConsoleMessage += Handle_ConsoleMessage;
            
            handle.JavascriptObjectRepository.ResolveObject += (sender, e) =>
             {
                 
                 if (LightVarObjectRepo.ContainsKey(e.ObjectName))
                 {
                
                     e.ObjectRepository.Register(e.ObjectName, LightVarObjectRepo[e.ObjectName]);
                 }
                 else
                 {
                 
                 }
             };
        }
        public delegate void ConsoleEventDelegate(string src, string message, string severity);
        public event ConsoleEventDelegate ConsoleOutput;
        private void Handle_ConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
          
            string severity = "MESSAGE";
            if (e.Level == LogSeverity.Warning)
            {
                severity = "WARNING";
            }
            if (e.Level == LogSeverity.Error)
            {
                severity = "ERROR";
            }
            if (e.Level == LogSeverity.Default)
            {
                severity = "MESSAGE";
            }
            if (ConsoleOutput == null)
            {
                return;
            }
            ConsoleOutput.Invoke(e.Source, e.Message, severity);

                
        }

        private void Handle_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (!e.IsLoading)
            {
                if (OnLoadComplete != null)
                {
                    OnLoadComplete.Invoke(this, null);
                }
                
            }
        }

        private async Task<T> JSEvalAsync<T>(string scr)
        {
            JavascriptResponse response = await handle.GetBrowser().MainFrame.EvaluateScriptAsync(scr);
            return (T)response.Result;
        }
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        private Dictionary<string, object> LightVarObjectRepo = new Dictionary<string, object>();
        public async Task<T> RunJSMethod<T>(string method, params object[] methodPs)
        {
            
            string query = method + "(";
            foreach (object ax in methodPs)
            {
                string handleId = (method + RandomString(6));

                
                //Cannot use their object storage registry because they are ghetto.
                if (ax.GetType().Namespace.StartsWith("System"))
                {
                    if (ax.GetType() == typeof(string))
                    {
                        query += '"' + ax.ToString() + '"' + ",";
                    }
                    else {
                        query += ax.ToString() + ",";
                    }

                }else
                {

                    LightVarObjectRepo.Add(handleId, ax);
                    JSEvalAsync<bool>("CefSharp.BindObjectAsync('"+handleId+"');");
                    await Task.Delay(1000);
                    
                    
                    //handle.JavascriptObjectRepository.Register(handleId, ax);

                    query += handleId + ",";
                }

                
            }
            query = query.TrimEnd(',');
            query += ")";
            
            return await JSEvalAsync<T>(query);
        }
        public void Build(string src)
        {
            handle.GetBrowser().MainFrame.LoadHtml(src);
            
        }
        public event EventHandler OnLoadComplete;
        public void Bind(string name, object binder)
        {
            handle.JavascriptObjectRepository.Register(name, binder, true);
        }
    }
}
