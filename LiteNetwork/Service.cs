using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Text;

namespace LiteNetwork
{
    public class Service
    {
        LiteTcpServer server;
        public Service()
        {
            int port = int.Parse(ConfigurationManager.AppSettings["port"]);
            server = new LiteTcpServer(new Parser(),IPAddress.Any, port);
        }
        public void Start() {
            server.Start();

            while (true)
            {
                var key = Console.ReadKey().Key;
                if (key == ConsoleKey.Escape) break;
            }
        }
    }
}
