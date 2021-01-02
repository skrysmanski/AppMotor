# Command Line Parsing

The base classes `CliApplicationWithCommands` and `CliApplicationWithoutCommands` give you access to typed and named command line parameters. You no longer get access to `string[] args` in your main method - instead you create instances of `CliCommand` and `CliParam<T>` to define your command line interface. This also allows for automatic help page generation.

A command line interface definition contains of parameters, commands, and verb groups.

*Side note:* For additional details, see [DESIGN-NOTES.md](DESIGN-NOTES.md).

## Quick Start

In its simplest form, an application can look like this:

```c#
internal sealed class Program : CliApplicationWithParams
{
    // Defines main method
    protected override CliCommandExecutor Executor => new(Execute);

    // A positional parameter
    private CliParam<string> ServerNameParam { get; } = new("server-name", positionIndex: 0)
    {
        HelpText = "The hostname or ip address of the metal-init server.",
    };

    // An optional named parameter
    private CliParam<FileInfo?> CertFileParam { get; } = new("--server-cert")
    {
        DefaultValue = null,
        HelpText = "The server certificate (.cer file) to use.",
    };

    private static int Main(string[] args)
    {
        // Creates an instance of "Program" and runs it.
        return Run<Program>(args);
    }

    // The main method
    private void Execute()
    {
        // Access to a parameter
        var baseUri = new Uri($"http://{this.ServerNameParam.Value}");

        // Main code here
    }
}
```

This program takes a required server name parameter and an optional parameter `--server-cert`.

If you need commands/verbs (like the `git` command), an application can look like this:

```c#
internal static class Program
{
    private static int Main(string[] args)
    {
        var app = new CliApplicationWithVerbs()
        {
            Verbs = new[]
            {
                new CliVerb("add", new AddCommand()),
                new CliVerb("remove", new RemoveCommand()),
            },
        };

        return app.Run(args);
    }
}
```

The `AddCommand` can look like this:

```c#
internal sealed class AddCommand : CliCommand
{
    public override string? HelpText => "Adds an item";

    protected override CliCommandExecutor Executor => new(Execute);

    private CliParam<FileInfo> ItemParam { get; } = new("item", positionIndex: 0)
    {
        HelpText = "The item to add.",
    };

    private void Execute()
    {
        // Command code here
    }
}
```

## Parameters

Parameters come in two variants: named and positional.

Named parameters are something like `--no-verify` or `-m` in `git commit`. Named parameters can either have a value (like with `-m`) or be standalone flags (like `--no-verify`).

Positional parameters are defined by the order in which they are specified. For example, in `git mv` the first parameter is the source and the second is the destination; both are positional parameters.

Parameters are represented by the `CliParam<T>` class. The following C# types are supported:

* number types (`int`, `double`, ...)
* `bool`
* `string`
* anything that has a constructor that takes a single `string` argument

Parameters must be defined in a "container" - either within a `CliCommand` or `CliApplicationWithoutCommands`. Parameters are (usually) defined either as instance (i.e. non-`static`) property or field. These properties or fields can have any visibility (including `private`).

You can define a named parameter like this:

```c#
private readonly CliParam<int> m_valueParam = new("--value");
```

This creates a named parameter with the name `--value` that takes an `int` as value.

To create a positional parameter, define it like this:

```c#
private readonly CliParam<string> m_fromParam = new("from", positionIndex: 0);
```

Note that the name `from` is just used for generating the help page for this parameter - it's not specified on the command line by the end user.

You can also define alias names for named parameters, default values (which makes the parameter optional) and help texts:

```c#
private readonly CliParam<int> m_valueParam = new("--value", "--val")
{
    DefaultValue = 42,
    HelpText = "The value to work on.",
};
```

To access the value of a parameter, simply use the `Value` property:

```c#
private void Execute()
{
    int theValue = this.m_valueParam.Value;
}
```

Note that the value is only available from within an "executor" method (see below).

### Commands

Commands (or verbs) let you have multiple functions within your application. For example, in `git add myfile.cs` the word `add` is a command.

Commands are implemented as classes that inherit from `CliCommand`. Commands have a name, an executor (think: main method) and usually parameters:

```c#
public class BenchmarkCommand : CliCommand
{
    protected override CliCommandExecutor Executor => new(Execute);

    private readonly CliParam<int> m_durationParam = new("--duration", "-d")
    {
        DefaultValue = 10,
        HelpText = "How long to run this benchmark (in seconds).",
    };

    public BenchmarkCommand() : base("benchmark")
    {
    }

    private void Execute()
    {
        TimeSpan duration = TimeSpan.FromSeconds(this.m_durationParam.Value);
        ...
    }
}
```

The user would execute this command via something like this:

    myapp benchmark --duration 20

Commands can also have alias names (like named parameters), a help text (via the `HelpText` properties), and sub commands and sub verb groups.

## Verb Groups

Verb groups are used to group commands and other verb groups under a name.

For example, if you have an CLI api like this:

```
ssh-env keys create
ssh-env keys install
ssh-env keys delete
```

... then `keys` would be a verb group.

Verb groups are like commands but can't be executed and don't have parameters.

To define a verb group, create a class that inherits from `CliVerbGroup`:

```c#
public class KeysVerbGroup : CliVerbGroup
{
    public KeysVerbGroup() : base("keys")
    {
    }

    protected override IEnumerable<CliVerb> GetSubVerbs()
    {
        yield return new CreateKeysCommand();
        yield return new InstallKeysCommand();
        yield return new DeleteKeysCommand();
    }
}
```

Like commands, verb groups can have alias names, a help text, and sub commands and sub verb groups.

*Note:* Both `CliCommand` and `CliVerbGroup` inherit from `CliVerb`. In this library, both are called "verbs" - with commands being "executable verbs" and verb groups being "non-executable verbs".

## Plugging Everything Together

To make your commands, verb groups and parameters accessible to the end user, you must implement a class that either inherits from `CliApplicationWithCommands` or `CliApplicationWithoutCommands`.

As the name suggests, `CliApplicationWithCommands` provides access to commands (or "verbs", to be more precise). It provides *many* functions under *one* application. It cannot have parameters on its own. Examples for this application type are `git` or `dotnet`.

```c#
public class GitApplication : CliApplicationWithCommands
{
    protected override IEnumerable<CliVerb> GetVerbs()
    {
        yield return new GitInitCommand();
        yield return new GitCommitCommand();
        yield return new GitAddCommand();
        ...
    }
}
```

`CliApplicationWithoutCommands` on the other hand cannot have commands (or verbs) but only parameters. These applications only provide *one* function. Examples for this application type are `cd`, `rm`, or `dir`. As such, they require an "executor" method and usually have parameters (you can think of them as single-command applications).

```c#
public class MoveApplication : CliApplicationWithoutCommands
{
    protected override CliCommandExecutor Executor => new(Execute);

    private readonly CliParam<string> m_sourceParam = new("from", positionIndex: 0);

    private readonly CliParam<string> m_destParam = new("dest", positionIndex: 1);

    private void Execute()
    {
        File.Move(this.m_sourceParam.Value, this.m_destParam.Value);
    }
}
```

To run either application type, simply invoke `Run()` or `RunAsync()`:

```c#
internal class Program
{
    public int Main(string[] args)
    {
        var app = new MoveApplication();
        return app.Run(args);
    }
}
```

## Executors

Executors exist to give you the freedom to implement your command or application main method however you like: synchronous or `async`, with or without return value.

To create an instance of an executor, you simply pass a fitting delegate to one of its constructors.

There are two types of executors: `CliApplicationExecutor` and `CliCommandExecutor`

Both support (parameter-less) delegates with the following return types: `void`, `Task`, `bool`, `Task<bool>`, `int`, `Task<int>`

The `CliApplicationExecutor` also supports delegates that take a single `string[]` parameter.

The class `CliCommandExecutor` is used by `CliCommand` and `CliApplicationWithoutCommands`. This is the only executor you need if you want to work with the command line parsing functionality of this library.

The class `CliApplicationExecutor` is used by `CliApplication` (which is an application base class that does not do any command line argument parsing).
