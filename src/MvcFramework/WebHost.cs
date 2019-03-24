using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MvcFramework.Services;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;
using SIS.WebServer;
using SIS.WebServer.Results;
using SIS.WebServer.Routing;

namespace MvcFramework
{
    public static class WebHost
    {

        public static void Start(IMvcApplication application)
        {
            IServiceCollection dependencyContainer = new ServiceCollection();
            application.ConfigureServices(dependencyContainer);

            ServerRoutingTable serverRoutingTable = new ServerRoutingTable();
            AutoRegisterRoutes(serverRoutingTable, application, dependencyContainer);

            application.Configure();


            var server = new Server(80, serverRoutingTable);
            server.Run();
        }

        private static void AutoRegisterRoutes(ServerRoutingTable routing, IMvcApplication application, IServiceCollection serviceCollection)
        {
            var controllers = application.GetType().Assembly
                //.GetAssembly(typeof(T).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Controller))));
                .GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Controller)));

            foreach (var controller in controllers)
            {
                //Take methods which has custom attributes which are subclass of HttpAttribute
                var getMethods = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(method => method.CustomAttributes.Any(
                        ca => ca.AttributeType.IsSubclassOf(typeof(HttpAttribute))));

                foreach (var methodInfo in getMethods)
                {
                    Console.WriteLine($"{controller.FullName} => { methodInfo.Name}");

                    var httpAttribute = (HttpAttribute)methodInfo.GetCustomAttributes(true)
                        .FirstOrDefault(ca => 
                                        ca.GetType().IsSubclassOf(typeof(HttpAttribute)));

                    if (httpAttribute == null)
                    {
                        continue;
                    }

                    

                    routing.Add(httpAttribute.Method, httpAttribute.Path, (request) => ExecuteAction(controller, methodInfo, request, serviceCollection));
                        
                }
            }
        }

        private static IHttpResponse ExecuteAction(Type controller, MethodInfo action, IHttpRequest request, IServiceCollection serviceCollection)
        {
            //1. Create Instance of controller name
            //2. Set request
            //3. Invoke action name
            //4. Return action result

            var controllerInstance = serviceCollection.CreateInstance(controller) as Controller;

            if (controllerInstance == null)
            {
                return new TextResult("Controller NOT found", SIS.HTTP.Enums.HttpResponseStatusCode.InternalServerErorr);
            }

            controllerInstance.Request = request;

            controllerInstance.UserCookieService = serviceCollection.CreateInstance<IUserCookieService>();

            var actionResult = action.Invoke(controllerInstance, new object[] { }) as IHttpResponse;

            return actionResult;

        }
    }
}
