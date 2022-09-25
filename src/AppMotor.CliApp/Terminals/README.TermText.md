# TermText

The `TermText` API provides an easy way of creating colored console output.

## Examples

```c#
using static AppMotor.CliApp.Terminals.Formatting.TermText;

// Combine styled and normal strings
Terminal.WriteLine(Blue("Hello") + " World" + Red("!"));

// Compose multiple styles using the chainable API
Terminal.WriteLine(BgRed().Black().Underline("Hello world!"));

// Nest styles of the same type even (color, underline)
Terminal.WriteLine(Green(
    "I am a green line " +
    Blue().Underline("with a blue substring") +
    " that becomes green again!"
));

Terminal.WriteLine(@$"
    CPU: {Red("90%")}
    RAM: {Green("40%")}
    DISK: {Yellow("70%")}
");

// 24 bit (RGB) support
Terminal.WriteLine(BgRgb(Color.CadetBlue).Rgb(Color.Crimson).Text("Hello") + " World");

// Hex color support
Terminal.WriteLine(Hex("#ff0000").Text("Text in HTML red (#ff0000)"));

// Various formatting options
Terminal.WriteLine(Inverse("Inverse") + " - not Inverse");
```

For a more complete example, see the **AppMotor.CliApp.TerminalColorsSample** in this repository.

## Under the Hood

Under the hood, `TermText` uses [ANSI Escape Sequences](https://en.wikipedia.org/wiki/ANSI_escape_code#SGR_(Select_Graphic_Rendition)_parameters) for encode colors and styles. These escape sequences are supported out-of-the-box on Linux/Unix and macOS. On Windows, they're supported with Windows 10 or higher and Windows Server 2016 or higher.

If you want to use `TermText` on Windows without `Terminal`, you must call `AnsiSupportOnWindows.Enable()` before the first use.

## Credits

TermText's API design was mainly influence by [Crayon](https://github.com/riezebosch/crayon) and [Chalk](https://github.com/chalk/chalk).
