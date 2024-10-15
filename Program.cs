using System.Diagnostics;
using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace openThisFolder;

class Program
{
    public static void Main()
    {
        Run().Wait();
    }


    private static async Task Run() 
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();
        Settings settings = config.GetRequiredSection("Settings").Get<Settings>();

        await SetConnection(settings);
    }

    private static async Task SetConnection(Settings settings) 
    {
        string connectionIP = settings.connectionIP;
        string eventName = settings.eventName;
    
        HubConnection connection = new HubConnectionBuilder()
            .WithUrl(connectionIP)
            .WithAutomaticReconnect()
            .Build();

        var events = Channel.CreateUnbounded<bool>();
        events.Writer.TryWrite(true);

        connection.Closed += async (error) =>
        {
            await Task.Delay(new Random().Next(0,5) * 1000);
            await connection.StartAsync().ContinueWith(task => {
                Console.WriteLine($"Reconnected with connectionIP {connectionIP}, eventName {eventName}");
            });
        };
        connection.On<string>(eventName, OpenFolder);

        await connection.StartAsync();
        
        await foreach (var item in events.Reader.ReadAllAsync())
        {
            if (item && Console.ReadLine() is null)
            {
                break;
            }
        }
    }

    private static void OpenFolder(string folder)
    {
        if (!string.IsNullOrEmpty(folder))
        {
            Console.WriteLine($"Received {folder} to open");
            ProcessStartInfo startInfo = new()
            {
                Arguments = folder,
                FileName = "explorer.exe"
            };
            Process.Start(startInfo);      
        }
    }
}
