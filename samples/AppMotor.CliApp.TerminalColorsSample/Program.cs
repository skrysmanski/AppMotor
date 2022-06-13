using System.Drawing;
using System.Text;

using AppMotor.CliApp.Terminals.Formatting;
using AppMotor.Core.Colors;

using static AppMotor.CliApp.Terminals.Formatting.TermText;

namespace AppMotor.CliApp.TerminalColorsSample;

public sealed class Program : CliApplication
{
    /// <inheritdoc />
    protected override CliApplicationExecutor MainExecutor => new(Execute);

    private static int Main(string[] args) => Run<Program>(args);

    // NOTE: This example is adapted from: https://github.com/chalk/chalk/blob/main/readme.md
    private static void Execute()
    {
        if (!AnsiSupportOnWindows.Enable())
        {
            Console.WriteLine("ERROR: ANSI is not supported on your system.");
            return;
        }

        // Combine styled and normal strings
        Console.WriteLine(Blue("Hello") + " World" + Red("!"));

        // Compose multiple styles using the chainable API
        Console.WriteLine(BgRed().Black().Underline("Hello world!"));

        // Nest styles of the same type even (color, underline, background)
        Console.WriteLine(Green(
            "I am a green line " +
            Blue().Underline("with a blue substring") +
            " that becomes green again!"
        ));

        Console.WriteLine(@$"
        CPU: {Red("90%")}
        RAM: {Green("40%")}
        DISK: {Yellow("70%")}
        ");

        // 24 bit (RGB) support
        Console.WriteLine(BgRgb(Color.CadetBlue).Rgb(Color.Crimson).Text("Hello") + " World");

        // Hex color support
        Console.WriteLine(Hex("#ff0000").Text("Text in HTML red (#ff0000)"));

        // Various formatting options
        Console.WriteLine();
        Console.WriteLine(Inverse("Inverse") + " - not Inverse");

        Console.WriteLine();

        AnimateString();

        // Make sure the app's output ends with a line break.
        Console.WriteLine();
    }

    private static void AnimateString()
    {
        const int MAX_SECONDS = 10;

        // NOTE: Each loop takes about 16 ms.
        for (int i = 0; i < MAX_SECONDS * 1000 / 16; i++)
        {
            var rainbowString = Rainbow($"This is a rainbow string (for about {MAX_SECONDS} seconds).", i);

            Console.CursorLeft = 0;
            Console.Write(rainbowString);

            Thread.Sleep(10);
        }
    }

    // Based on: https://github.com/chalk/chalk/blob/main/examples/rainbow.js
    private static string Rainbow(string text, int offset)
    {
        if (text.Length == 0)
        {
            return "";
        }

        int hueStep = 360 / text.Length;

        int hue = offset % 360;

        var result = new StringBuilder();

        foreach (char ch in text)
        {
            hue = (hue + hueStep) % 360;

            var color = new HslColor(hue, 100, 50);

            result.Append(Rgb(color).Text(ch));
        }

        return result.ToString();
    }
}
