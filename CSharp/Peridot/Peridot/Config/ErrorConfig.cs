using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace Peridot.Config
{
    public class ErrorConfig
    {
        public static string Error404 = "";
        public static string Error500 = "";
        public static string Error403 = "";
        public static string Login = "";
        public static string Error401= "";
        private static string loadInternalFile(string url)
        {
            PDLogger.Log($"LOADING ERROR PAGE: {url}", 5);
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = url;
            try
            {
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    PDLogger.Log($"LOADED {url}", 5);
                    return result;
                }

            }
            catch(Exception ex) {
                PDLogger.Log($"Failed to load error page '{url}' {ex.Message}", 1, ex.StackTrace);
                return "";
            }


        }
        public static void loadDefaultErrorPages()
        {
            Error401 = loadInternalFile("Peridot.Error_Pages.401.html");
            Login = loadInternalFile("Peridot.Error_Pages.login.html");
            Error403 = loadInternalFile("Peridot.Error_Pages.403.html");
            Error404 = loadInternalFile("Peridot.Error_Pages.404.html");
            Error500 = loadInternalFile("Peridot.Error_Pages.500.html");
        }
        public ErrorConfig()
        {
            
        }
    }
}
