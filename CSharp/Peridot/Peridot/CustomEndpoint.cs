using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peridot
{
    public class CustomEndpoint
    {
        public string Directory;
        public string Hostname;
        public List<string> IPBlacklist = new List<string>();
        public List<string> IPWhitelist = new List<string>();
        public List<AuthCredentials> UserCredentials = new List<AuthCredentials>();
        public int port;
        public CustomEndpoint(string dir, string hostname, int port = 80)
        {
            Directory = dir;
            Hostname = hostname;
            this.port = port;
        }
    }
    public class AuthCredentials
    {
        public string Username;
        public string Password;
        public string Token;
        public AuthCredentials(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
