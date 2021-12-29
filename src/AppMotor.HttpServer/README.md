---
icon: server
---
# APIs for running a HTTP(S) Server

This package provides APIs to host an HTTP(S) (ASP.NET Core) server (in a console application).

## How to use

First, you need to have an ASP.NET Core project.

The simplest form to use this package is to create a `Program.cs` with the following content:

```c#
using AppMotor.HttpServer;

return HttpServerApplication.Run(port: 1234);
```

This will enable ASP.NET Core controllers and Razor views via HTTP on port 1234 on localhost.

To make the server reachable from another machine, change the code to this:

```c#
using AppMotor.HttpServer;

return HttpServerApplication.Run(port: 1234, bindToLoopbackOnly: false);
```

You can also pass a `CancellationToken` to be able to stop the server at any given time (by default it's stopped by pressing `Ctrl+C`):

```c#
using var cts = new CancellationTokenSource();

var serverTask = HttpServerApplication.RunAsync(port: 1234, cts.Token);

cts.Cancel(); // <-- Stops the server
```

To enable HTTPS, you need to obtain a certificate and then use some code like this:

```c#
internal static class Program
{
    public static int Main(string[] args)
    {
        var app = new HttpServerApplication(new MyServerCommand());
        return app.Run(args);
    }

    private sealed class MyServerCommand : HttpServerCommandBase
    {
        protected override IEnumerable<HttpServerPort> GetServerPorts(IServiceProvider serviceProvider)
        {
            yield return new HttpsServerPort(
                SocketListenAddresses.Loopback,
                port: 1234,
                certificateProvider: GetHttpsCertificate
            );
        }

        private TlsCertificate GetHttpsCertificate()
        {
            // Or get certificate somehow else
            return TlsCertificate.CreateSelfSigned(Environment.MachineName, TimeSpan.FromDays(90));
        }
    }
}
```

Note: You can extend `MyServerCommand` with `CliParam`s. See the [CommandLine parsing documentation](../AppMotor.CliApp/CommandLine/README.md).

This will use the class `AppMotor.HttpServer.MvcStartup` as [ASP.NET Startup class](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/startup). If you want to use your own Startup class, simply override `CreateStartupClass`:

```c#
private sealed class MyServerCommand : HttpServerCommandBase
{
    protected override object CreateStartupClass(WebHostBuilderContext context)
    {
        return new MyOwnStartup();
    }
}
```
