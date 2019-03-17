using System;
using System.Collections.Generic;
using System.Text;
using SIS.WebServer;
using SIS.WebServer.Routing;

namespace MvcFramework
{
    public static class WebHost
    {
        public static void Start(IMvcApplication application)
        {
            application.ConfigureServices();

            ServerRoutingTable serverRoutingTable = new ServerRoutingTable();

            application.Configure(serverRoutingTable);

            var server = new Server(80, serverRoutingTable);
            server.Run();
        }
    }
}
