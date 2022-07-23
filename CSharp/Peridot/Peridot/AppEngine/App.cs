using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
namespace Peridot.AppEngine
{
    public class App
    {
        private Random random = new Random();
        private string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz!@#$%^&*()";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        List<AppUser> users = new List<AppUser>();
        public string appFolder = "";
        private static string loadInternalFile(string url)
        {
            PDLogger.Log($"LOADING INTERNAL FILE {url}", 5);
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = url;
            try
            {
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    PDLogger.Log($"LOADED INTERNAL FILE {url}", 5);
                    string result = reader.ReadToEnd();
                    return result;
                }

            }
            catch (Exception ex)
            {
                PDLogger.Log($"Error on loading internal file {url}", 2);
                return "";
            }


        }
        public void assets(Session s)
        {
            HttpListenerContext context = s.PageContext;
            string _rootDirectory = appFolder + "/assets";
            string filename = context.Request.Url.AbsolutePath;
            filename = filename.Substring(1);
            //filename = Path.Combine(_rootDirectory, filename);
            if (filename.Contains(".") == false)
            {
                PDLogger.Log($"Resolving URL to {filename}/index.html", 4);
                filename += "/index.html";
            }
            PDLogger.Log($"LOADING APP ASSET {filename}", 5);
            if (File.Exists(filename))
            {
                try
                {
                    Stream input = new FileStream(filename, FileMode.Open);

                    //Adding permanent http response headers
                    string mime;
                    context.Response.ContentType = PDServer._mimeTypeMappings.TryGetValue(Path.GetExtension(filename), out mime) ? mime : "application/octet-stream";
                    context.Response.ContentLength64 = input.Length;
                    context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
                    context.Response.AddHeader("Last-Modified", System.IO.File.GetLastWriteTime(filename).ToString("r"));
                    context.Response.AddHeader("Cache-Control", "no-cache");
                    byte[] buffer = new byte[1024 * 16];
                    int nbytes;
                    while ((nbytes = input.Read(buffer, 0, buffer.Length)) > 0)
                        context.Response.OutputStream.Write(buffer, 0, nbytes);
                    input.Close();

                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    context.Response.OutputStream.Flush();
                }
                catch (Exception ex)
                {
                    PDLogger.Log($"Internal Server Error:: {ex.Message} in PERIDOT_HTTP_FEEDBACK_SENDER", 1);
                   
                    // context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }

            }
            else
            {
                PDLogger.Log($"Error 404 on request {context.Request.Url}", 4);
                s.setStatusCode(HttpStatusCode.NotFound);
                // context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
        }
        public void cback(Session s)
        {
            string token = s.GetCookie("PERIDOT_APP_TOKEN");
            foreach (AppUser user in users)
            {
                if (token == user.Token)
                {
                    //I need toadd the reader rq
                    s.GenerateHTMLOutput($"<script>{loadInternalFile("Peridot.AppEngine.app_api.js")}</script>" + System.IO.File.ReadAllText(appFolder + "/" + user.CurrentPage));
                    return;
                }
            }
            string newToken = RandomString(40);
            s.CreateCookie("PERIDOT_APP_TOKEN", newToken);
            AppUser a = new AppUser();
            a.Token = newToken;
            users.Add(a);
            s.GenerateHTMLOutput($"<script>{loadInternalFile("Peridot.AppEngine.app_api.js")}</script>" + System.IO.File.ReadAllText(appFolder + "/" + a.CurrentPage));
        }
        public void runScript(Session s)
        {
            PDLogger.Log($"STARTING SCRIPT RUN:: {s.getParameter("script")}.cs", 5);
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters param = new CompilerParameters { GenerateExecutable = false, GenerateInMemory = true };
            param.ReferencedAssemblies.Add("Peridot.dll");
            param.ReferencedAssemblies.Add("System.dll");
            foreach (string file in Directory.GetFiles(appFolder))
            {
                FileInfo f = new FileInfo(file);
                if (f.Name.Contains(".dll"))
                {
                    param.ReferencedAssemblies.Add(f.Name);
                }
            }
            PDLogger.Log($"Loaded internal librarys", 5);
            string scriptToExecute = s.getParameter("script") + ".cs";
            string src = File.ReadAllText(appFolder+"/"+scriptToExecute);
            CompilerResults results = provider.CompileAssemblyFromSource(param, src);
            if (results.Errors.Count != 0)
            {
                PDLogger.Log($"Failed to load script: {scriptToExecute} (Compiler errors)", 1);
                foreach (CompilerError error in results.Errors)
                {
                    PDLogger.Log($"{error.ErrorText} in line {error.Line} ({error.ErrorNumber})", 1);
                }
                return;
            }
            PDLogger.Log($"COMPILED SCRIPT:: {s.getParameter("script")}.cs", 5);
            try
            {
                object o = results.CompiledAssembly.CreateInstance("PeridotApp." + s.getParameter("script"));
                MethodInfo mi = o.GetType().GetMethod("main");
                AppSession asa = new AppSession();
                asa.session = s;
                asa.appUser = getAppUser(s.GetCookie("PERIDOT_APP_TOKEN"));
                asa.appAPI = this;
                if (asa.appUser != null)
                {
                    string returns =(string)mi.Invoke(o, new object[] { asa });
                    s.GenerateHTMLOutput(returns);
                    PDLogger.Log($"EXECUTED SCRIPT:: {s.getParameter("script")}.cs", 5);
                }
                else
                {
                    PDLogger.Log($"Got a null APPUSER request which violates app policy. (Internal Sources Only)", 4);
                }
                
            }
            catch (Exception ex)    
            {
                PDLogger.Log($"Failed to run script: {scriptToExecute} ({ex.Message})", 1);
            }
            
        }
        public void setCurrentPageForUser(string token, string page)
        {
            foreach (AppUser user in users)
            {
                if (user.Token == token)
                {
                    user.CurrentPage = page;
                    return;
                }
            }
        }
        public string getCurrentUserAppToken(Session s)
        {
            return s.GetCookie("PERIDOT_APP_TOKEN");
        }
        public List<UserData> getUserData(string token)
        {
            foreach (AppUser user in users)
            {
                if (user.Token == token)
                {
                    return user.userData;
                }
            }
            return null;
        }
        public string getUserData(string token, string name)
        {
            foreach (UserData data in getUserData(token))
            {
                if (data.Name == name)
                {
                    return data.Data;
                }
            }
            return null;
        }
        public AppUser getAppUser(string token)
        {
            foreach (AppUser user in users)
            {
                if (user.Token == token)
                {
                    return user;
                }
            }
            return null;
        }
        public void addUserData(UserData data, string token)
        {
            foreach (AppUser user in users)
            {
                if (user.Token == token)
                {
                    user.userData.Add(data);
                }
            }
        }
        public void editUserData(string name, string newV, string token)
        {
            List<UserData> d = getUserData(token);
            foreach (UserData dd in d)
            {
                if (dd.Name == name)
                {
                    dd.Data = newV;
                    return;
                }
            }
        }
    }
    public class AppSession
    {
        public Session session;
        public AppUser appUser;
        public App appAPI;
    }
    public class AppUser
    {
        public string Token;
        public string CurrentPage = "index.html";
        public List<UserData> userData = new List<UserData>();
    }
    public class UserData
    {
        public string Name;
        public string Data;
    }
}
