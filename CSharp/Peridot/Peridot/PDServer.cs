using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Peridot
{
    public class PDServer
    {
        public List<CSharpWebpage> Webpages = new List<CSharpWebpage>();
        public List<CustomEndpoint> customEndpoints = new List<CustomEndpoint>();
        private readonly string[] _indexFiles = {
        "index.html",
        "index.htm",
        "default.html",
        "default.htm"
    };
        public static IDictionary<string, string> _mimeTypeMappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase) {
        #region extension to MIME type list
        {".asf", "video/x-ms-asf"},
        {".asx", "video/x-ms-asf"},
        {".avi", "video/x-msvideo"},
        {".bin", "application/octet-stream"},
        {".cco", "application/x-cocoa"},
        {".crt", "application/x-x509-ca-cert"},
        {".css", "text/css"},
        {".deb", "application/octet-stream"},
        {".der", "application/x-x509-ca-cert"},
        {".dll", "application/octet-stream"},
        {".dmg", "application/octet-stream"},
        {".ear", "application/java-archive"},
        {".eot", "application/octet-stream"},
        {".exe", "application/octet-stream"},
        {".flv", "video/x-flv"},
        {".gif", "image/gif"},
        {".hqx", "application/mac-binhex40"},
        {".htc", "text/x-component"},
        {".htm", "text/html"},
        {".html", "text/html"},
        {".ico", "image/x-icon"},
        {".img", "application/octet-stream"},
        {".iso", "application/octet-stream"},
        {".jar", "application/java-archive"},
        {".jardiff", "application/x-java-archive-diff"},
        {".jng", "image/x-jng"},
        {".jnlp", "application/x-java-jnlp-file"},
        {".jpeg", "image/jpeg"},
        {".jpg", "image/jpeg"},
        {".js", "application/x-javascript"},
        {".mml", "text/mathml"},
        {".mng", "video/x-mng"},
        {".mov", "video/quicktime"},
        {".mp3", "audio/mpeg"},
        {".mpeg", "video/mpeg"},
        {".mpg", "video/mpeg"},
        {".msi", "application/octet-stream"},
        {".msm", "application/octet-stream"},
        {".msp", "application/octet-stream"},
        {".pdb", "application/x-pilot"},
        {".pdf", "application/pdf"},
        {".pem", "application/x-x509-ca-cert"},
        {".pl", "application/x-perl"},
        {".pm", "application/x-perl"},
        {".png", "image/png"},
        {".prc", "application/x-pilot"},
        {".ra", "audio/x-realaudio"},
        {".rar", "application/x-rar-compressed"},
        {".rpm", "application/x-redhat-package-manager"},
        {".rss", "text/xml"},
        {".run", "application/x-makeself"},
        {".sea", "application/x-sea"},
        {".shtml", "text/html"},
        {".sit", "application/x-stuffit"},
        {".swf", "application/x-shockwave-flash"},
        {".tcl", "application/x-tcl"},
        {".tk", "application/x-tcl"},
        {".txt", "text/plain"},
        {".war", "application/java-archive"},
        {".wbmp", "image/vnd.wap.wbmp"},
        {".wmv", "video/x-ms-wmv"},
        {".xml", "text/xml"},
        {".xpi", "application/x-xpinstall"},
        {".zip", "application/zip"},
        #endregion
    };
        private Thread _serverThread;
        private string _rootDirectory;
        private HttpListener _listener = new HttpListener();
        private int _port;
        private Random random = new Random();
        private string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz!@#$%^&*()";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public int Port
        {
            get { return _port; }
            private set { }
        }
        public PDServer(string path, int port)
        {
            this.Initialize(path, port);
        }
        public void addCustomEndpoint(CustomEndpoint e)
        {
            customEndpoints.Add(e);
            _listener.Prefixes.Add($"http://{e.Hostname}:{e.port}/");
            PDLogger.Log($"Started listening from endpoint http://{e.Hostname}:{e.port}/", 3);
        }
        public PDServer(string path)
        {
            //get an empty port
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            this.Initialize(path, port);
        }
        private void Listen()
        {
            
            PDLogger.Log($"Starting HTTP Listener on port {_port.ToString()}", 4);
            _listener.Prefixes.Add("http://*:" + _port.ToString() + "/");
            PDLogger.Log($"Started listening from endpoint http://*:{ _port.ToString()}/", 3);
            try
            {
                _listener.Start();
                while (true)
                {
                    try
                    {
                        HttpListenerContext context = _listener.GetContext();
                        Process(context);
                    }
                    catch (Exception ex)
                    {
                        PDLogger.Log($"Non breaking listener Error:: {ex.Message}", 5);
                    }
                }
            }
            catch(Exception ex)
            {
                PDLogger.Log($"Listener error: {ex.Message} ({ex.HResult.ToString()})", 1, ex.StackTrace);
                return;
            }
            
            
            
        }
        private Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
        private void writeHtml(HttpListenerContext a, string html, HttpStatusCode code = HttpStatusCode.OK)
        {
            Stream input = GenerateStreamFromString(html);
            var context = a;
            //Adding permanent http response headers
            string mime;
            context.Response.ContentType = "text/html";
            context.Response.ContentLength64 = input.Length;
            context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
            
            context.Response.AddHeader("Last-Modified", System.IO.Directory.GetLastWriteTime(_rootDirectory).ToString("r"));

            byte[] buffer = new byte[1024 * 16];
            int nbytes;
            while ((nbytes = input.Read(buffer, 0, buffer.Length)) > 0)
                context.Response.OutputStream.Write(buffer, 0, nbytes);
            input.Close();

            context.Response.StatusCode = (int)code;
            context.Response.OutputStream.Flush();
            context.Response.OutputStream.Close();

        }
        private string getFormEncode(string data, string name)
        {
            foreach (string s in data.Split('&'))
            {
                if (s.Split('=')[0] == name)
                {
                    return s.Split('=')[1];
                }
            }
            return null;
        }
        private void Process(HttpListenerContext context)
        {
            PDLogger.Log($"Traffic on {context.Request.Url} by {context.Request.RemoteEndPoint.Address.ToString()} ({context.Request.UserHostName})", 4);
            string filename = context.Request.Url.AbsolutePath;
            bool found = false;
            foreach (CSharpWebpage pe in Webpages)
            {
                
                PDLogger.Log($"FINDING CANDIDATE: {pe.PageLink} &&  IN ({filename})", 5);
                if (filename.Contains("/" + pe.PageLink))
                {
                    found = true;
                    PDLogger.Log($"Setting Traffic to PDX Endpoint", 4);
                    PDLogger.Log($"Executing PDX Endpoint ID:: {pe.PageLink}", 5);
                 
                    pe.WEBPAGE_Callback(new Session(context));
                  
                    PDLogger.Log($"Finished Processing PDX Endpoint ID:: {pe.PageLink}", 5);
                    return;
                }
            }
            if (!found)
            {
                //Console.WriteLine(filename);


                filename = filename.Substring(1);

                foreach (CustomEndpoint end in customEndpoints)
                {
                    if (end.Hostname + ":" + end.port.ToString() == context.Request.UserHostName || end.Hostname == context.Request.UserHostName)
                    {
                        bool allow = true;
                        string reason = "NONE GIVEN";
                        PDLogger.Log($"Entering custom endpoint {end.Hostname}", 4);
                        if (end.IPWhitelist.Count > 0)
                        {
                            allow = false;
                            foreach (string ip in end.IPWhitelist)
                            {
                                if (ip == context.Request.RemoteEndPoint.Address.ToString())
                                {
                                    reason = "IP_WHITELIST_ENABLED";
                                    allow = true;
                                }
                            }
                        }
                        foreach (string blacklist in end.IPBlacklist)
                        {
                            if (blacklist == context.Request.RemoteEndPoint.Address.ToString())
                            {
                                reason = "IP_BLACKLIST_MATCH";
                                allow = false;
                            }
                        }
                        
                        if (end.UserCredentials.Count > 0)
                        {
                            bool authed = false;
                            Session iSession = new Session(context);
                            string auth = iSession.GetCookie("PERIDOT_USER_BASIC_AUTH");
                            if (auth == null || auth == "")
                            {
                                iSession.CreateCookie("PERIDOT_USER_BASIC_AUTH", "request");
                                writeHtml(context, Config.ErrorConfig.Login, HttpStatusCode.OK);
                                return;
                            }
                            else if (auth == "request")
                            {
                                string text;
                                using (var reader = new StreamReader(context.Request.InputStream,
                                                                     context.Request.ContentEncoding))
                                {
                                    text = reader.ReadToEnd();
                                }
                                if (getFormEncode(text, "username") != null && getFormEncode(text, "password") != null)
                                {
                                    foreach (AuthCredentials authc in end.UserCredentials)
                                    {
                                        if (authc.Username == getFormEncode(text, "username") && getFormEncode(text, "password") == authc.Password)
                                        {
                                            authed = true;
                                            string newtoken = RandomString(40);

                                            iSession.CreateCookie("PERIDOT_USER_BASIC_AUTH", newtoken);
                                            authc.Token = newtoken;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                foreach (AuthCredentials authc in end.UserCredentials)
                                {
                                    if (authc.Token == auth)
                                    {
                                        authed = true;
                                    }
                                }
                            }
                            if (!authed)
                            {
                                PDLogger.Log("ERROR 401 GIVEN - REASON: INVALID CREDENTIALS/AUTH TOKEN. REAUTHORIZATION REQUIRED.",4);
                                iSession.CreateCookie("PERIDOT_USER_BASIC_AUTH", "request");
                                writeHtml(context, Config.ErrorConfig.Error401, HttpStatusCode.Unauthorized);
                                
                                return;
                            }
                        }
                        if (!allow)
                        {
                            PDLogger.Log($"ERROR 404 GIVEN - REASON: {reason}", 4);
                            writeHtml(context, Config.ErrorConfig.Error403, HttpStatusCode.Forbidden);
                            return;
                        }
                        filename = Path.Combine(end.Directory, filename);
                        if (filename.Contains(".") == false)
                        {
                            filename += "/index.html";
                        }
                        if (File.Exists(filename))
                        {
                            try
                            {
                                Stream input = new FileStream(filename, FileMode.Open);

                                //Adding permanent http response headers
                                string mime;
                                context.Response.ContentType = _mimeTypeMappings.TryGetValue(Path.GetExtension(filename), out mime) ? mime : "application/octet-stream";
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
                                writeHtml(context, Config.ErrorConfig.Error500, HttpStatusCode.InternalServerError);
                                // context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            }

                        }
                        else
                        {
                            PDLogger.Log($"Error 404 on request {context.Request.Url}", 4);
                            writeHtml(context, Config.ErrorConfig.Error404, HttpStatusCode.NotFound);
                            // context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        }
                    }
                }

                filename = Path.Combine(_rootDirectory, filename);
                if (filename.Contains(".") == false)
                {
                    PDLogger.Log($"Resolving URL to {filename}/index.html", 5);
                    filename += "/index.html";
                }
                if (File.Exists(filename))
                {
                    try
                    {
                        Stream input = new FileStream(filename, FileMode.Open);

                        //Adding permanent http response headers
                        string mime;
                        context.Response.ContentType = _mimeTypeMappings.TryGetValue(Path.GetExtension(filename), out mime) ? mime : "application/octet-stream";
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
                        writeHtml(context, Config.ErrorConfig.Error500, HttpStatusCode.InternalServerError);
                       // context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    }

                }
                else
                {
                    PDLogger.Log($"Error 404 on request {context.Request.Url}",4);
                    writeHtml(context, Config.ErrorConfig.Error404, HttpStatusCode.NotFound);
                   // context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                }

                context.Response.OutputStream.Close();
            }
        }
        private void Initialize(string path, int port)
        {
            this._rootDirectory = path;
            this._port = port;
            _serverThread = new Thread(this.Listen);
            PDLogger.Log($"CREATING THREAD:: {_serverThread.Name} ", 5);
            _serverThread.Start();
        }

        public void Stop()
        {
            _serverThread.Abort();
            _listener.Stop();
        }
    }
    public class CSharpWebpage
    {
        public string PageLink;
        public delegate void EPL(Session s);
        public EPL WEBPAGE_Callback;
        private string v;
        public CSharpWebpage(string link, EPL callback)
        {
            WEBPAGE_Callback = callback;
            PageLink = link;
        }

    }
}
