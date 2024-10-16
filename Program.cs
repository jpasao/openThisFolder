using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace openThisFolder;

class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging => 
            {
                logging.AddEventLog(settings =>
                {
                    settings.SourceName = "OpenThisFolderLogs";
                });
            })
            .ConfigureServices(services => 
            {
                services.AddHostedService<FolderHubClient>();
            })
            .Build();

        host.Run();
    }
}
