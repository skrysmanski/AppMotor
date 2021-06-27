# GenericHost Integration

The code in the `AppMotor.CliApp.CommandLine.Hosting` namespace integrates `AppMotor.CliApp` with .NET's Generic Host functionality.

## What is the Generic Host and why would I use it?

You can think of the [Generic Host](https://docs.microsoft.com/en-us/dotnet/core/extensions/generic-host) as .NET's new way to define an application:

* It defines the Dependency Injection (DI) framework.
* It provides a [logging framework](https://docs.microsoft.com/en-us/dotnet/core/extensions/logging) (via DI and `ILogger<T>`)
* It registers all necessary services with the DI framework.
* It manages the lifetime of "hosted services" - for example, an HTTP server.

If you need any of these properties for your application, you may want to use the Generic Host.

*Side note:* Historically, a version of the Generic Host was used to run standalone ASP.NET Core applications (i.e. web applications). At one time, Microsoft extracted a non-ASP.NET-Core version, which now is the Generic Host.

## How to use the GenericHost

In its simplest form, you create a child class of `GenericHostCliCommand`, create an instance of it and pass it to `CliApplicationWithCommand`:

```c#
class MyGenericHostCliCommand : GenericHostCliCommand
{
    protected override void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        // Register your services here; via "services.Add...()
        // See: https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection
        //services.AddSingleton<ITestService, TestService>();
    }
}

var command = new MyGenericHostCliCommand();
var app = new CliApplicationWithCommand(command);
app.Run();
```

This will run an application that doesn't do anything (and that will never stop) - so it's *not very useful*.

To make it useful, we need to explore the two ways of running a `GenericHostCliCommand`:

1. **Server configuration:** The command runs indefinitely (until explicitly stopped) and provides some hosted services (e.g. a web server).
1. **Application configuration:** The command executes a function and then terminates.

*Notes:*

* Both ways configure their services by overriding `ConfigureServices` (like in the example above).
* Unlike `Host.CreateDefaultBuilder()` ([details](https://docs.microsoft.com/en-us/dotnet/core/extensions/generic-host#default-builder-settings)), `GenericHostCliCommand` only gives you a minimum set of services (DI and a Console logger). Any additional service you need to register yourself.

### Server configuration

This configuration is used if you need to run any kind of server (HTTP server, file server, ...). In this configuration, the `GenericHostCliCommand` runs indefinitely - until it's stopped explicitly (see below).

Each server must implemented [`IHostedService`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.ihostedservice) and then be registered via `AddHostedService()`:

```c#
protected override void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    service.AddHostedService<MyServer>();
}
```

And that's it.

*Note:* If you want to host an HTTP(S) server, it's recommended to use the `AppMotor.HttpServer` package. See its [README](../../../AppMotor.HttpServer/README.md) for more details.

### Application configuration

This configuration is used when you don't really want to host a server but just make use of the DI functionality in a regular "main method" application. In this case, you specify an executor method and the `GenericHostCliCommand` only runs as long as this method runs:

```c#
class MyGenericHostCliCommand : GenericHostCliCommand
{
    protected override CliCommandExecutor ExplicitExecutor => new(Execute);

    private void Execute()
    {
        // Your code here.
        // You have access to the DI system via "this.Services.GetRequiredService<T>()".
    }
}
```

## How to stop a GenericHostCliCommand

If you're running a `GenericHostCliCommand` in the server configuration, you need to stop the command explicitly - or it will run forever.

There are various ways to stop a `GenericHostCliCommand`. All of them will shutdown the application *gracefully*, i.e. by executing any existing shutdown code before terminating:

1. You hit `Ctrl+C`.
1. You can call `GenericHostCliCommand.Stop()`.
1. You can obtain (via DI) the `IHostApplicationLifetime` instance and call `StopApplication()`.
1. You can pass a `CancellationToken` to the application's `Run()` method and then cancel it.

## Lifetime events

The property `GenericHostCliCommand.LifetimeEvents` gives you access to various events regarding the lifetime of the command.

```c#
public sealed class GenericHostCliCommandLifetimeEvents
{
    // Triggered when the GenericHostCliCommand has fully started.
    public event EventHandler? Started;

    // Triggered when the GenericHostCliCommand is starting a graceful shutdown.
    // Shutdown will block until all event handlers registered on this event have completed.
    public event EventHandler? Stopping;

    // Triggered when the GenericHostCliCommand has completed a graceful shutdown.
    // The application will not exit until all event handlers registered on this event have completed.
    public event EventHandler? Stopped;
}
```

## Customize the Host Builder

By default, `GenericHostCliCommand` provides you with:

* .NET's built-in dependency injection framework
* a Console logger that's used by `ILogger<T>`

Both (and more) can be customized by providing your own `IHostBuilderFactory`. AppMotor comes with two factories built-in:

1. `MethodHostBuilderFactory`: let's you define a factory method (i.e. `Func<IHostBuilder>`)
1. `DefaultHostBuilderFactory`: let's you customize the host builder by overriding methods

`GenericHostCliCommand` uses `DefaultHostBuilderFactory` by default but you can provide your own by overriding the `GenericHostCliCommand.HostBuilderFactory` property.

The following sections assume you're customizing `DefaultHostBuilderFactory`:

```c#
class MyHostBuilderFactory : DefaultHostBuilderFactory
{
    // Customizations here
}

class MyGenericHostCliCommand : GenericHostCliCommand
{
    protected override IHostBuilderFactory HostBuilderFactory { get; }
        = new MyHostBuilderFactory();
}
```

### How to use another DI framework

To use your own DI framework, it needs to provide an implementation of `IServiceProviderFactory<IServiceCollection>`.

Then simply override `DefaultHostBuilderFactory.CreateServiceProviderFactory()`:

```c#
class MyHostBuilderFactory : DefaultHostBuilderFactory
{
    protected override IServiceProviderFactory<IServiceCollection> CreateServiceProviderFactory(HostBuilderContext context)
    {
        // Return your DI framework
        return ...;
    }
}
```

### How to configure the logging framework

To configure logging, you need to override `DefaultHostBuilderFactory.ConfigureLogging()`:

```c#
class MyHostBuilderFactory : DefaultHostBuilderFactory
{
    protected override void ConfigureLogging(HostBuilderContext context, ILoggingBuilder loggingBuilder)
    {
        // Your logging configuration here
        loggingBuilder.AddConsole();
        loggingBuilder.AddEventLog();
    }
}
```

For more details, see the [official documentation](https://docs.microsoft.com/en-us/dotnet/core/extensions/logging).
