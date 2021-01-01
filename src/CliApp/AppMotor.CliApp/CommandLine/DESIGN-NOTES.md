# Design Notes on the Command Line API

This document contains notes and backgrounds on the design of the Command Line API.

## Why another Command Line Parser?

This library actually doesn't implement its own command line parser. Instead it relies on the [`System.CommandLine` NuGet package](https://www.nuget.org/packages/System.CommandLine/) for parsing (and for help text generation as well).

As a nice side effect, applications using `AppMotor.CliApp` will automatically get [command line auto completion](https://github.com/dotnet/command-line-api/blob/main/docs/Features-overview.md#Suggestions).

The command line API of `AppMotor.CliApp` just provides the "opinionated" frontend for the command line parser.

## Why Another Frontend, Then?

At time of writing there were two major command line parsing frontends for C#: System.CommandLine and [CommandLineParser](https://www.nuget.org/packages/CommandLineParser/).

Each has its own approach on how to give you access to the parameters the end user entered; but both also had things I did not like.

*Note:* Some of the following is highly opinionated - but these opinions were still the basis on why I wrote this library (instead of using an existing one).

### System.CommandLine

You explicitly define each parameter upfront and then provide a method whose parameter names that must match the parameter definitions you provided earlier. It's big "magic".

What I don't like about this approach is that the names of the parameters of your "main method" are disconnected from the parameter definition. Rename a parameter (of a `private` method) and suddenly you command line parsing doesn't work anymore. It's a little bit too fragile for my taste.

Also, at the time of writing, there was almost no source code documentation - making it hard to understand what all the types and methods mean. To be fair, the library was still in beta at this time, but I needed something at that time (and not a couple of months later).

### CommandLineParser

With the CommandLineParser library, you define your commands (or "verbs" as they call them) as classes with parameters being properties or fields. To mark fields as cli parameters, you add attributes to them.

I personally like this approach very much (as - from a design standpoint - it doesn't get much cleaner than this). Unfortunately, it has the downside that values for attributes can only be constants. If you need a dynamic default value based, for example, on the operating system, then you can't do this with attributes. You can also not use .NET's resources mechanism to provide translated help texts (e.g. `LocalizableResources.HelpText`) because the texts are not constants (in the .NET sense).

Sure, the CommandLineParser library gives you ways to do this by some convention - but conventions are not easily discoverable (and you now need to *mix* clean attributed classes with other mechanisms).

I also found that way the whole thing is plugged together rather unintuitively (although this was not *that* important).

Aside from all of this there was also the fact that Microsoft now had its own command line parser (System.CommandLine above). I felt more comfortable betting on Microsoft's implementation (might be totally unjustified but no one can predict the future).

## Mixing of Parameter Definition and Value

The design of the `CliParam<T>` class mixes the *definition* of the parameter (e.g. its name) with the *value* of the parameter (via the `Value` property).

From a design standpoint, this is a bad design. I'd personally rather have something like attributes on fields/properties. But, as stated above, this approach also has some severe limitations.

In the end, I decided that the ease of use of this design outweighs its not-so-clean design.

## I can't do X with this library

As state above, this library is "opinionated". There are probably lots of CLIs that can't be expressed with this library. But that's okay. This library wants to be "easy to use" rather than "I can do everything". It's a 90% solution. When using this library, you're required to adopt your CLI to the possibilities/structure of this library.

## Why no Argument Validation?

This library does not contain any argument validation (other than making sure that the text the end user entered can be converted to the desired .NET type). This was done on purpose.

The problem with a built-in argument validation is that you need to provide a proper error text to the end user in case the validation failed. This text can be/is highly situational. We could provide a generic text and allow the developer to customize it somehow - but then the developer could just as easily validate arguments in their executor method and return a non-zero exit code in case of any error.

And so this is the way this library works. It *just* provides the application with the values that end user entered - everything beyond that is up to the application itself.
