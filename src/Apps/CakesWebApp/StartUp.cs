using CakesWebApp.Controllers;
using MvcFramework;
using SIS.HTTP.Enums;
using SIS.WebServer.Routing;

namespace CakesWebApp
{
    public class StartUp : IMvcApplication
    {

        public void Configure()
        {
            //routing.Routes[HttpRequestMethod.Get]["/"] = request => new HomeController { Request = request }.Index();
            //routing.Routes[HttpRequestMethod.Get]["/login"] = request => new AccountController { Request = request }.Login();
            //routing.Routes[HttpRequestMethod.Post]["/login"] = request => new AccountController { Request = request }.DoLogin();
            //routing.Routes[HttpRequestMethod.Get]["/register"] = request => new AccountController { Request = request }.Register();
            //routing.Routes[HttpRequestMethod.Post]["/register"] = request => new AccountController { Request = request }.DoRegister();
            //routing.Routes[HttpRequestMethod.Get]["/hello"] = request => new HomeController { Request = request }.HelloUSer();
            //routing.Routes[HttpRequestMethod.Get]["/logout"] = request => new AccountController { Request = request }.Logout();
        }

        public void ConfigureServices()
        {
            // TODO: Implement IoC/DI container
        }
    }
}
