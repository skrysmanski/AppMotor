# Generic Host Integration

All `CliCommand`s provide integration with .NET's [Generic Host](https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host) functionality. Through this integration you get:

* A [dependency injection (DI) framework](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)
* A [logging framework](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging?tabs=command-line) (via DI and `ILogger<T>`)
* A [configuration system](https://github.com/skrysmanski/dotnet-docs/blob/main/README.Configuration.md)
* A lifetime of "hosted services" - that automatically starts and stops all registered server classes.

*Side note:* To better understand how `IHostBuilder` work, see [Generic Host - demystified](https://github.com/skrysmanski/dotnet-docs/blob/main/README.GenericHost.md).

## How to use the Generic Host

To interact with the Generic Host (via its `IHostBuilder` interface), you need to override one or both of the following methods in your `CliCommand` sub class:

* `ConfigureServices`
* `ConfigureApplication`

Like so:

```c#
class MyCliCommand : CliCommand
{
    protected override void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        // Register your services here; via "services.Add...()
        //services.AddSingleton<ITestService, TestService>();
    }

    protected override void ConfigureApplication(IHostBuilder hostBuilder)
    {
        // Add configuration sources, ...
    }
}
```

While the command is running, you get access to the DI system via the `CliCommand.Services` property.

*Note:* `CliCommand.CreateHostBuilder()` creates a host builder through the `DefaultHostBuilderFactory` class. This class creates a host builder with a reduced set of services (compared to [`Host.CreateDefaultBuilder()`](https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host#default-builder-settings)).

## Running a Server

There's a `CliCommand` sub class called `ServiceHostCliCommand`: It's main purpose is to *just* run one or more servers (HTTP server, file server, ...).

This command runs indefinitely - until explicitly stopped (see below). It's (more or less) the equivalent of .NET's Generic Host wrapped in a `CliCommand`.

Each server must implemented [`IHostedService`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.ihostedservice) and then be registered via `AddHostedService()`:

```c#
protected override void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    service.AddHostedService<MyServer>();
}
```

*Note:* If you want to host an HTTP(S) server, it's recommended to use the `AppMotor.HttpServer` package. See its [README](../../../AppMotor.HttpServer/README.md) for more details.

### How to stop a ServiceHostCliCommand

If you're running a `ServiceHostCliCommand`, you need to stop the command explicitly - or it will run forever.

There are various ways to stop a `ServiceHostCliCommand`. All of them will shutdown the application *gracefully*, i.e. by executing any existing shutdown code before terminating:

* You hit `Ctrl+C`.
* You can call `ServiceHostCliCommand.Stop()`.
* You can obtain (via DI) the `IHostApplicationLifetime` instance and call `StopApplication()`.
* You can pass a `CancellationToken` to the application's `Run()` method and then cancel it.

### Lifetime events

This `ServiceHostCliCommand` provides access to various events regarding the lifetime of the command via the `IServiceHostLifetimeEvents` interface.

```c#
public interface IServiceHostLifetimeEvents
{
    // Triggered when the ServiceHostCliCommand has fully started.
    public OneTimeEvent Started { get; }

    // Triggered when the ServiceHostCliCommand has completed a graceful shutdown.
    // The application will not exit until all event handlers registered on this event have completed.
    public OneTimeEvent Stopped { get; }

    // This token is canceled just before the "Stopped" event is triggered.
    public CancellationToken CancellationToken { get; }
}
```

You get the instance of `IServiceHostLifetimeEvents` either via the `ServiceHostCliCommand.LifetimeEvents` property or via dependency injection.
