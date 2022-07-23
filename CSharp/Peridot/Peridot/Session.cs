using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace Peridot
{
    public class Session
    {
        public HttpListenerContext PageContext = null;
        public Session(HttpListenerContext cx)
        {
            PageContext = cx;
        }
        public string URLDecode(string s)
        {
            return WebUtility.UrlDecode(s);
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
        public List<UrlParameter> GetUrlParameters()
        {
            var cx = PageContext;
            try
            {
                string path = cx.Request.Url.Query.Split('?')[1];
                List<UrlParameter> p = new List<UrlParameter>();
                foreach (string x in URLDecode(path).Split('&'))
                {
                    string n = x.Split('=')[0];
                    string v = x.Split('=')[1];
                    UrlParameter a = new UrlParameter();
                    a.Name = n;
                    a.Value = v;
                    p.Add(a);
                }
                return p;
            }
            catch (Exception ex)

            {
                cx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                cx.Response.OutputStream.Flush();
                cx.Response.OutputStream.Close();
                Console.WriteLine($"[WARNING] Generate Parameter Error {ex.Message} on url {cx.Request.Url.AbsolutePath} Query {cx.Request.Url.Query}");
                return null;
            }

        }
        public string getParameter(string param)
        {
            List<UrlParameter> pl = GetUrlParameters();
            foreach (UrlParameter p in pl)
            {
                if (param == p.Name)
                {
                    return p.Value;
                }
            }
            return null;
        }
        public void ChangeLocation(string location)
        {
            Stream input = GenerateStreamFromString($"<script>document.location.replace('{location}')</script>");

            //Adding permanent http response headers
            string mime;
            HttpListenerContext context = PageContext;
            context.Response.ContentType = "text/html";
            context.Response.ContentLength64 = input.Length;
            context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
            context.Response.AddHeader("Last-Modified", System.IO.File.GetLastWriteTime("src/index.html").ToString("r"));

            byte[] buffer = new byte[1024 * 16];
            int nbytes;
            while ((nbytes = input.Read(buffer, 0, buffer.Length)) > 0)
                context.Response.OutputStream.Write(buffer, 0, nbytes);
            input.Close();

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.OutputStream.Flush();
            context.Response.OutputStream.Close();
        }
        public void GenerateTextOutput(string s)
        {
            Stream input = GenerateStreamFromString(s);
            var context = PageContext;
            //Adding permanent http response headers
            string mime;
            context.Response.ContentType = "text/plain";
            context.Response.ContentLength64 = input.Length;
            context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
            context.Response.AddHeader("Last-Modified", System.IO.File.GetLastWriteTime("src/index.html").ToString("r"));

            byte[] buffer = new byte[1024 * 16];
            int nbytes;
            while ((nbytes = input.Read(buffer, 0, buffer.Length)) > 0)
                context.Response.OutputStream.Write(buffer, 0, nbytes);
            input.Close();

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.OutputStream.Flush();
            context.Response.OutputStream.Close();
        }
        public void GenerateHTMLOutput(string s)
        {
            Stream input = GenerateStreamFromString(s);
            var context = PageContext;
            //Adding permanent http response headers
            string mime;
            context.Response.ContentType = "text/html";
            context.Response.ContentLength64 = input.Length;
            context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
            context.Response.AddHeader("Last-Modified", System.IO.File.GetLastWriteTime("src/index.html").ToString("r"));

            byte[] buffer = new byte[1024 * 16];
            int nbytes;
            while ((nbytes = input.Read(buffer, 0, buffer.Length)) > 0)
                context.Response.OutputStream.Write(buffer, 0, nbytes);
            input.Close();

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.OutputStream.Flush();
            context.Response.OutputStream.Close();
        }
        public void setStatusCode(HttpStatusCode code)
        {
            var context = PageContext;
            context.Response.StatusCode = (int)code;
            context.Response.OutputStream.Flush();
            context.Response.OutputStream.Close();
            
        }
        public void CreateCookie(string key, string value)
        {
            var cx = PageContext;
            cx.Response.AppendCookie(new Cookie(key, value, "/"));
        }
        public string GetCookie(string key)
        {
            var cx = PageContext;
            foreach (Cookie c in cx.Request.Cookies)
            {
                if (c.Name == key)
                {
                    return c.Value;
                }
            }
            return null;
        }
    }
    public class UrlParameter
    {
        public string Name;
        public string Value;
    }
}
