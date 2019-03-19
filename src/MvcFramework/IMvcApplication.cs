using SIS.WebServer.Routing;

namespace MvcFramework
{
    public interface IMvcApplication
    {
        void Configure();

        void ConfigureServices();
    }
}
