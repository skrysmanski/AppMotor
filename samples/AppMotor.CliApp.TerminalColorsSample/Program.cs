using System.Drawing;
using System.Text;

using AppMotor.CliApp.Terminals;
using AppMotor.Core.Colors;

using static AppMotor.CliApp.Terminals.Formatting.TermText;

namespace AppMotor.CliApp.TerminalColorsSample;

public sealed class Program : CliApplication
{
    /// <inheritdoc />
    protected override CliApplicationExecutor MainExecutor => new(Execute);

    private static int Main(string[] args) => Run<Program>(args);

    // NOTE: This example is adapted from: https://github.com/chalk/chalk/blob/main/readme.md
    private void Execute()
    {
        // Combine styled and normal strings
        this.Terminal.WriteLine(Blue("Hello") + " World" + Red("!"));

        // Compose multiple styles using the chainable API
        this.Terminal.WriteLine(BgRed().Black().Underline("Hello world!"));

        // Nest styles of the same type even (color, underline, background)
        this.Terminal.WriteLine(Green(
            "I am a green line " +
            Blue().Underline("with a blue substring") +
            " that becomes green again!"
        ));

        this.Terminal.WriteLine(@$"
        CPU: {Red("90%")}
        RAM: {Green("40%")}
        DISK: {Yellow("70%")}
        ");

        // 24 bit (RGB) support
        this.Terminal.WriteLine(BgRgb(Color.CadetBlue).Rgb(Color.Crimson).Text("Hello") + " World");

        // Hex color support
        this.Terminal.WriteLine(Hex("#ff0000").Text("Text in HTML red (#ff0000)"));

        // Various formatting options
        this.Terminal.WriteLine();
        this.Terminal.WriteLine(Inverse("Inverse") + " - not Inverse");

        this.Terminal.WriteLine();

        AnimateString();

        // Make sure the app's output ends with a line break.
        this.Terminal.WriteLine();
    }

    private void AnimateString()
    {
        if (!this.Terminal.IsOutputRedirected && this.Terminal is ITerminalWindow terminalWindow)
        {
            const int MAX_SECONDS = 10;

            // NOTE: Each loop takes about 16 ms.
            for (int i = 0; i < MAX_SECONDS * 1000 / 16; i++)
            {
                var rainbowString = Rainbow($"This is a rainbow string (for about {MAX_SECONDS} seconds).", i);

                terminalWindow.CursorLeft = 0;
                terminalWindow.Write(rainbowString);

                Thread.Sleep(10);
            }
        }
        else
        {
            var rainbowString = Rainbow("This is a rainbow string.", offset: 0);
            this.Terminal.Write(rainbowString);
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
