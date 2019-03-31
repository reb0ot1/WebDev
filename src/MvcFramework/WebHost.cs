using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

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

            var controllerInstance = serviceCollection.CreateInstance(controller) as Controller;

            if (controllerInstance == null)
            {
                return new TextResult("Controller NOT found", SIS.HTTP.Enums.HttpResponseStatusCode.InternalServerErorr);
            }

            controllerInstance.Request = request;
            controllerInstance.ViewEngine = new ViewEngine.ViewEngine(); // TODO: Use service collection 
            controllerInstance.UserCookieService = serviceCollection.CreateInstance<IUserCookieService>();

            var actionParameters = GetActionParameterObjects(request, action, serviceCollection);
            var actionResult = action.Invoke(controllerInstance, actionParameters.ToArray()) as IHttpResponse;

            return actionResult;

        }

        private static List<object> GetActionParameterObjects(IHttpRequest request, MethodInfo action, IServiceCollection serviceCollection)
        {
            var actionParameters = action.GetParameters();
            var actionParameterObjects = new List<object>();

            foreach (var actionParameter in actionParameters)
            {
                if (actionParameter.ParameterType.IsPrimitive)
                {
                    var stringValue = GetRequestData(request, actionParameter.Name);
                    object value = TryParse(stringValue, actionParameter.ParameterType);
                    actionParameterObjects.Add(value);
                }
                else
                {
                    var instance = serviceCollection.CreateInstance(actionParameter.ParameterType);

                    var properties = actionParameter.ParameterType.GetProperties();
                    foreach (var property in properties)
                    {
                        // TODO: Support IEnumerable 
                        string stringValue = GetRequestData(request, property.Name);
                        // Convert.ChangeType()    
                        object value = TryParse(stringValue, property.PropertyType);

                        property.SetMethod.Invoke(instance, new object[] { value });
                    }

                    actionParameterObjects.Add(instance);
                }
                
            }

            return actionParameterObjects;
        }

        private static object TryParse(string stringValue, Type type)
        {
            var typeCode = Type.GetTypeCode(type);
            object value = null;

            switch (typeCode)
            {
                //case TypeCode.Boolean:
                //    break;
                //case TypeCode.Byte:
                //    break;
                case TypeCode.Char:
                    if (char.TryParse(stringValue, out var charValue)) value = charValue;
                    break;
                case TypeCode.DateTime:
                    if (DateTime.TryParse(stringValue, out var dateTimeValue)) value = dateTimeValue;
                    break;
                //case TypeCode.DBNull:
                //    break;
                case TypeCode.Decimal:
                    if (decimal.TryParse(stringValue, out var decimalValue)) value = decimalValue;
                    break;
                case TypeCode.Double:
                    if (double.TryParse(stringValue, out var doubleValue)) value = doubleValue;
                    break;
                //case TypeCode.Empty:
                //    break;
                //case TypeCode.Int16:
                //    break;
                case TypeCode.Int32:
                    if (int.TryParse(stringValue, out var intValue)) value = intValue;
                    break;
                case TypeCode.Int64:
                    if (long.TryParse(stringValue, out var longValue)) value = longValue;
                    break;
                //case TypeCode.Object:
                //    break;
                //case TypeCode.SByte:
                //    break;
                //case TypeCode.Single:
                //    break;
                case TypeCode.String:
                    value = stringValue;
                    break;
                //case TypeCode.UInt16:
                //    break;
                //case TypeCode.UInt32:
                //    break;
                //case TypeCode.UInt64:
                //    break;
                default:
                    break;
            }

            return value;
        }

        private static string GetRequestData(IHttpRequest request, string key)
        {
            key = key.ToLower();
            string stringValue = null;
            if (request.FormData.Any(x => x.Key.ToLower() == key))
            {
                stringValue = request.FormData.First(x => x.Key.ToLower() == key).Value.ToString();
            }
            else if (request.QueryData.Any(x => x.Key.ToLower() == key))
            {
                stringValue = request.QueryData.First(x => x.Key.ToLower() == key).Value.ToString();
            }

            return stringValue;
        }
    }
}
