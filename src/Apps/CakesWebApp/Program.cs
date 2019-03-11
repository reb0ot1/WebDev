using System;
using CakesWebApp.Controllers;
using SIS.HTTP.Enums;
using SIS.WebServer;
using SIS.WebServer.Routing;

namespace CakesWebApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            ServerRoutingTable serverRoutingTable = new ServerRoutingTable();

            serverRoutingTable.Routes[HttpRequestMethod.Get]["/"] = request => new HomeController { Request = request}.Index();
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/login"] = request => new AccountController { Request = request }.Login();
            serverRoutingTable.Routes[HttpRequestMethod.Post]["/login"] = request => new AccountController { Request = request }.DoLogin();
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/register"] = request => new AccountController { Request = request }.Register();
            serverRoutingTable.Routes[HttpRequestMethod.Post]["/register"] = request => new AccountController { Request = request }.DoRegister();
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/hello"] = request => new HomeController { Request = request }.HelloUSer();
            serverRoutingTable.Routes[HttpRequestMethod.Get]["/logout"] = request => new AccountController { Request = request }.Logout();

            Server server = new Server(8000, serverRoutingTable);

            server.Run();
        }
    }
}
