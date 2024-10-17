# openThisFolder
This is a console .NET CORE app that solely listens to a SignalR hub to open a file explorer with the received path.
### Some explanation

It has an `appsettings.json` file backed by a strongly typed class, mainly to configure the hub IP and the name of the event. The logger class, and the `FolderHubClient`, in which all the magic happens:

1. The constructor reads the settings,  initializes the connection and sets the handler for the incoming events
2. As the class inherits from `IHostedService`, the methods `StartAsync` and `StopAsync` have been overrided.
3. We try to start the connection and, once connected, exit the loop.

As this was created to run in a Windows machine, the logs are written in  the Windows event log, under the `OpenThisFolderLogs` entry

The first attempt was to run this as a service, to avoid the need of an open window console but, even though the event arrived, it was impossible to me to achieve the same that I was getting with a console project . Process.Start() did not work under those circumstances. So the solution came setting the `OutputType` to _Winexe_ in the project file, _et voilà_, once the application was executed from a console window, the control was returned to that window, the window can be closed and the process is still running in the background listening to the hub server and opening the file explorer folders as desired. Yay!
