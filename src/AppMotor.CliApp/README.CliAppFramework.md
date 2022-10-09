# CLI Application Framework

The AppMotor CLI Application Framework provides you with:

* Exception Handling
* [Command Line Parameter Parsing](CommandLine/README.md) including an automatically generated help page (optional)
* [Generic Host (`IHostBuilder`) Integration](CommandLine/README.GenericHost.md) (optional)

## Application Classes

The following application classes are available:

| Class                         | Exception Handling | CLI Parameter Parsing | Generic Host Integration | Multi Command Support
| ----------------------------- | ------------------ | --------------------- | ------------------------ | ---------------------
| `CliApplication`              | yes                | no                    | no                       | no
| `CliApplicationWithParams`    | yes                | yes                   | yes                      | no
| `CliApplicationWithCommand`   | yes                | yes                   | yes                      | no
| `CliApplicationWithVerbs`     | yes                | yes                   | yes                      | yes

All these class inherit from `CliApplication` and thus inherit all its features.

**CLI Parameter Parsing** means that you use properties (like `string TargetDir { get; set; }`) to get the parameters passed by the user on the command line - instead of simply `string[] args`.

**Generic Host Integration** means that you get access to all the features that `IHostBuilder`/`IHost` provide; e.g. dependency injection, configuration, logging.

**Multi Command Support** means that your application can support multiple commands where each command has its own CLI parameters. For example, the `git` command is a multi command application (with commands being `checkout`, `commit`, `push`, ...) whereas `mv`/`move` are single command applications (and thus would be modeled with `CliApplicationWithParams`).

The class `CliApplicationWithCommand` is basically the same as `CliApplicationWithParams` but you specify the parameters and main method on a command instead of the application itself. This class is useful, if you want to use `GenericHostCliCommand` ([details](CommandLine/Hosting/README.md)).

## Commands

The following command types are available:

| Type                    | Description                                                               | Has `Main()` Method
| ----------------------- | ------------------------------------------------------------------------- | -------------------
| `CliCommand`            | Base class for any command                                                | yes
| `CliVerb`               | A named `CliCommand` or command group; required for multi command support | -
| `GenericHostCliCommand` | Command for hosting a service                                             | no

All commands have support for named command line parameters (i.e. Command Line Parameter Parsing).

Commands *with* a `Main()` only execute for as long as the main method runs. Commands *without* a `Main()` method run indefinitely until stop through some API (`CancellationToken`, `GenericHostCliCommand.Stop()`).

## Examples

The simplest form of using `CliApplication` is like this (in your `Program.cs`):

```c#
return CliApplication.Run(() =>
{
    Terminal.WriteLine("Hello, World!")
});
```

There's also an async version:

```c#
return CliApplication.RunAsync(async () =>
{
    await Task.Delay(10);
    Terminal.WriteLine("Hello, World!")
});
```

Alternatively, you can also inherit from `CliApplication` like so:

```c#
public sealed class Program : CliApplication
{
    protected override CliApplicationExecutor MainExecutor => new(Execute);

    private static int Main(string[] args) => Run<Program>(args);

    private void Execute()
    {
        this.Terminal.WriteLine("Hello, World!")
    }
}
```

The signature of the `Execute` method is "dynamic". It can take `string[] args` and/or `CancellationToken cancellationToken` as parameter, be synchronous or `async`, and return `void`, `int`, or `bool`. For all possible combinations, see the available constructors in `CliApplicationExecutor`.

If you already have a command (i.e. an instance of `CliCommand`), you can run an application like so (in your `Program.cs`):

```c#
return CliApplication.Run(args, new MyCliCommand());
```

If you already have a set of `CliVerb`s (i.e. named `CliCommand`s), you can run an application like so:

```c#
return CliApplication.Run(args, verb1, verb2, verb3);
```

To set the description of you application (for the automatically generated help page), create the appropriate application instance and set the `AppDescription` property:

```c#
var app = new CliApplicationWithVerbs()
{
    AppDescription = ".NET wrapper around Git (for demonstration purposes). The commands are non-functional.",
    Verbs = new[]
    {
        new CliVerb("clone", new CloneCommand()),
        new CliVerb("add", new AddCommand()),
    },
};

return app.Run(args);
```

For more details and examples on **Command Line Parameter Parsing**, see [here](CommandLine/README.md).

For more details and examples on the **Generic Host (`IHostBuilder`) Integration**, see [here](CommandLine/README.GenericHost.md).

## Executors

Executors exist to give you the freedom to implement your command's or application's main method however you like: synchronous or `async`, with or without return value.

To create an instance of an executor, you simply pass a fitting delegate to one of its constructors.

There are two types of executors: `CliApplicationExecutor` and `CliCommandExecutor`

Both support (parameter-less) methods/delegates with the following return types: `void`, `Task`, `bool`, `Task<bool>`, `int`, `Task<int>`

The `CliApplicationExecutor` also supports methods/delegates that take a single `string[]` parameter (the command line args).

The class `CliCommandExecutor` is used by `CliCommand` and `CliApplicationWithParams`. This is the only executor you need if you want to work with the command line parsing functionality of this library.

The class `CliApplicationExecutor` is used by `CliApplication` (which is an application base class that does not do any command line argument parsing).
