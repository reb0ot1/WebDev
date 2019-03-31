using System;
using CakesWebApp.Controllers;
using MvcFramework;
using MvcFramework.Logger;
using MvcFramework.Services;
using SIS.HTTP.Enums;
using SIS.WebServer.Routing;

namespace CakesWebApp
{
    public class StartUp : IMvcApplication
    {

        public void Configure()
        {
        }

        public void ConfigureServices(IServiceCollection collection)
        {
            // TODO: Implement IoC/DI container

            collection.AddService<IHashService, HashService>();
            collection.AddService<IUserCookieService, UserCookieService>();
            collection.AddService<ILogger, ConsoleLogger>();

            //Registraciq koqto vzima tip i vzima funkciq kato parametyr, koqto kogato q izvikame shte vryshta obekt ot tozi tip
            //collection.AddService<ILogger>(() => new FileLogger($"log_{DateTime.Now.Date.ToString()}.txt"));
            collection.AddService<ILogger>(() => new FileLogger($"log_.txt"));

        }
    }

}
