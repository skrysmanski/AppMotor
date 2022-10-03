# Command Line Parsing

The base classes `CliCommand` and `CliApplicationWithParams` give you access to **typed and named command line parameters**. You no longer get access to `string[] args` in your main method - instead you create instances `CliParam<T>` (and, optionally `CliVerb`) to define your command line interface. This also allows for **automatic help page generation**.

A command line interface definition consists of parameters and verbs (which are either named command or groups of verbs).

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

or:

```c#
return CliApplication.Run(
    args,
    new CliVerb("add", new AddCommand()),
    new CliVerb("remove", new RemoveCommand())
);
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

Positional parameters are defined by the order in which they are specified on the command line. For example, in `git mv` the first parameter is always the source and the second is always the destination; both are positional parameters.

Parameters are represented by the `CliParam<T>` class. The following C# types are supported:

* number types (`int`, `double`, ...)
* `bool`
* `string`
* anything that has a constructor that takes a single `string` argument

Parameters must be defined in a "container" - either within a `CliCommand` or `CliApplicationWithParams`. Parameters are (usually) defined as instance (i.e. non-`static`) property or field. These properties or fields can have any visibility (including `private`).

You can define a named parameter like this:

```c#
private CliParam<int> ValueParam { get; } = new("--value");
```

This creates a named parameter with the name `--value` that takes an `int` as value.

To create a positional parameter, define it like this:

```c#
private CliParam<string> FromParam { get; } = new("from", positionIndex: 0);
```

Note that the name `from` is just used for generating the help page for this parameter - it's not specified on the command line by the end user.

You can also define alias names for named parameters, default values (which makes the parameter optional) and help texts:

```c#
private CliParam<int> ValueParam { get; } = new("--value", "--val")
{
    DefaultValue = 42,
    HelpText = "The value to work on.",
};
```

To access the value of a parameter, simply use the `Value` property:

```c#
private void Execute()
{
    int theValue = this.ValueParam.Value;
}
```

Note that the value is only available from within an "executor" method (see below).

### Optional Parameters

By default, parameters are considered "required". Parameters become "optional" if the `DefaultValue` property is set.

For example, this parameter is required:

```c#
private CliParam<int> ValueParam { get; } = new("--value");
```

While this parameter is optional:

```c#
private CliParam<int> ValueParam { get; } = new("--value")
{
    DefaultValue = 42,
};
```

To make a reference type parameter optional, set its default value to `null`:

```c#
private CliParam<FileInfo?> ValueParam { get; } = new("--value")
{
    DefaultValue = null,
};
```

Note that the type of the parameter needs to be nullable (e.g. `FileInfo?` - not `FileInfo`) for this to work.

There are a few parameter types that are **optional by default**:

* Named parameters of type `CliParam<bool>`: the default value is set to `false`; these parameters usually represent "flags" (e.g. `--verbose`)
* Parameters with a nullable *value* type (e.g. `CliParam<int?>`): the default value is set to `null`

## Verbs and Commands

Verbs let you have multiple functions within your application. For example, in `git add myfile.cs` the word `add` is such a verb.

Verbs - represented by the `CliVerb` class - always have a name and usually a command:

```c#
var verb = new CliVerb("benchmark", new BenchmarkCommand());
```

Commands are implemented as classes that inherit from `CliCommand`. Commands have an executor (think: main method) and usually parameters:

```c#
public class BenchmarkCommand : CliCommand
{
    protected override CliCommandExecutor Executor => new(Execute);

    private CliParam<int> DurationParam { get; } = new("--duration", "-d")
    {
        DefaultValue = 10,
        HelpText = "How long to run this benchmark (in seconds).",
    };

    private void Execute()
    {
        TimeSpan duration = TimeSpan.FromSeconds(this.DurationParam.Value);
        ...
    }
}
```

The user would execute this command via something like this:

    myapp benchmark --duration 20

Verbs can also have alias names (like named parameters), a help text, and sub/child verbs.

### Verb Groups (Verbs without Command)

Verb groups are simply verbs that just group other verbs under a name.

For example, if you have a CLI api like this:

```
ssh-env keys create
ssh-env keys install
ssh-env keys delete
```

... then `keys` would be a verb group (i.e. it doesn't do anything on its own)

Verb groups are simply **verbs without a command**:

```c#
var verbGroup = new CliVerb("keys")
{
    SubVerbs = new[]
    {
        new CliVerb("create", CreateKeysCommand()),
        new CliVerb("install", new InstallKeysCommand()),
        new CliVerb("delete", new DeleteKeysCommand()),
    },
};
```

## Putting Everything Together

To make your commands, verbs, and parameters accessible to the end user, you must create an instance of one of the following classes:

* An application with *multiple* functions:
  * `CliApplicationWithVerbs`
* An application with a *single* function:
  * `CliApplicationWithParams`
  * `CliApplicationWithCommand`

### Application with Multiple Functions

As the name suggests, `CliApplicationWithVerbs` provides access to verbs. It provides *multiple* functions under *one* application. It cannot have parameters on its own. Examples for this application type are `git` or `dotnet`.

```c#
var gitApplication = new CliApplicationWithVerbs()
{
    Verbs = new[]
    {
        new CliVerb("init", new GitInitCommand()),
        new CliVerb("commit", new GitCommitCommand()),
        new CliVerb("add", new GitAddCommand()),
    },
};
```

### Application with a Single Function

`CliApplicationWithParams` on the other hand cannot have verbs but only parameters. These applications only provide *one* function. Examples for this application type are `cd`, `rm`, or `dir/`ls`. As such, they require an "executor" method and usually have parameters (you can think of them as single-command applications).

```c#
internal sealed class MoveApplication : CliApplicationWithParams
{
    protected override CliCommandExecutor Executor => new(Execute);

    private CliParam<string> SourceParam { get; } = new("from", positionIndex: 0);

    private CliParam<string> DestParam { get; } = new("dest", positionIndex: 1);

    private void Execute()
    {
        File.Move(this.SourceParam.Value, this.DestParam.Value);
    }
}
```

The same application can be written as an application with a command (with `CliApplicationWithCommand`):

```c#
internal sealed class MoveCommand : CliCommand
{
    protected override CliCommandExecutor Executor => new(Execute);

    private CliParam<string> SourceParam { get; } = new("from", positionIndex: 0);

    private CliParam<string> DestParam { get; } = new("dest", positionIndex: 1);

    private void Execute()
    {
        File.Move(this.SourceParam.Value, this.DestParam.Value);
    }
}

var app = new CliApplicationWithCommand(new MoveCommand());
```

This form has the advantage that the same `MoveCommand` can be used with both `CliApplicationWithCommand` (single function) and `CliApplicationWithVerbs` (multiple functions).

### Running the Application

To run either application type, simply invoke `Run()` or `RunAsync()`:

```c#
internal static class Program
{
    public int Main(string[] args)
    {
        var app = new MoveApplication();
        return app.Run(args);
    }
}
```

Or:

```c#
internal static class Program : CliApplicationWithParams
{
    // Other code here

    public int Main(string[] args)
    {
        return Run<Program>(args);
    }
}
```

Or use one of the static `Run()`/`RunAsync()` convenience methods:

```c#
return CliApplication.Run(args, new MyCliCommand());
```
