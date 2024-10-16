using System.Diagnostics;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace openThisFolder;

public class FolderHubClient : IHostedService
{
    private readonly ILogger<FolderHubClient> logger;
    private readonly HubConnection connection;

    public FolderHubClient(ILogger<FolderHubClient> logger)
    {
        this.logger = logger;
        IConfigurationRoot config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
            
        Settings settings = config.GetRequiredSection("Settings").Get<Settings>();
        connection = new HubConnectionBuilder()
            .WithUrl(settings.connectionIP)
            .Build();

        connection.On<string>(settings.eventName, OpenFolder);
    }

    public async Task OpenFolder(string folder)
    {        
        if (!string.IsNullOrEmpty(folder))
        {     
            await Task.Run(() => {
                logger.LogInformation("Received '{Folder}' folder to open", folder);
                ProcessStartInfo startInfo = new()
                {
                    Arguments = folder,
                    FileName = "explorer.exe"
                };
                Process.Start(startInfo);
            });
        }        
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            try
            {
                await connection
                    .StartAsync(cancellationToken)
                    .ContinueWith(task => logger.LogInformation("Started connection to open STL folders"));
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error happened trying to start the connection: {Error}", ex.Message);
                await Task.Delay(1000, cancellationToken);
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopped connection to open STL folders");
        await connection.DisposeAsync();
    }
}