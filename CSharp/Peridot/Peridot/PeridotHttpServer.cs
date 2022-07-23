using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Peridot
{
    public class PeridotHttpServer
    {
        public Config.Config peridotConfiguration = new Config.Config();
        PDServer p;
        AppEngine.App currentApp = new AppEngine.App();
        public PeridotHttpServer(string htmlFolder, int port = 80)
        {
            Config.ErrorConfig.loadDefaultErrorPages();
            File.WriteAllText(".time", "");
            p = new PDServer(htmlFolder, port);
        }
        public void createApplicationEndpoint(string endpoint, string folder)
        {
            currentApp.appFolder = folder;
            p.Webpages.Add(new CSharpWebpage(endpoint + "/run", currentApp.runScript));
            PDLogger.Log($"CREATED APPLICATION ENDPOINT {endpoint + "/run"}", 5);
            p.Webpages.Add(new CSharpWebpage(endpoint + "/assets", currentApp.assets));
            PDLogger.Log($"CREATED APPLICATION ENDPOINT {endpoint + "/assets"}", 5);
            p.Webpages.Add(new CSharpWebpage(endpoint, currentApp.cback));
            PDLogger.Log($"CREATED APPLICATION ENDPOINT {endpoint}", 5);
            
            
        }
        public void createCSharpEndpoint(string endpoint, CSharpWebpage.EPL callback)
        {
            p.Webpages.Add(new CSharpWebpage(endpoint, callback));
        }
        public void createCustomEndpoint(CustomEndpoint c)
        {
            p.addCustomEndpoint(c);
        }
    }
}
