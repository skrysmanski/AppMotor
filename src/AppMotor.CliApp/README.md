---
icon: terminal
---
# APIs for Command Line Apps

This library contains various building blocks for command line applications.

## Terminal/Console APIs

The `Terminal` class is an alternative to `System.Console` that only supports cross-platform APIs. It also provides support for `TermText` (or any "raw" string ANSI escape sequences) by making sure they work on Windows automatically.

`TermText` is a `string` for the Console that supports colors. See [README.TermText.md](Terminals/README.TermText.md) for more details.

## Command Line Parsing

For details, [README.md](CommandLine/README.md).

## Generic Host

If you want to use .NET's Generic Host, see [AppMotor.CliApp.Hosting](CommandLine/Hosting/README.md).

## Code Documentation

This library has full code documentation. You can find more details about each type there.
