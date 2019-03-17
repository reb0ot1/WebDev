using SIS.WebServer.Routing;

namespace MvcFramework
{
    public interface IMvcApplication
    {
        void Configure(ServerRoutingTable routingTable);

        void ConfigureServices();
    }
}
