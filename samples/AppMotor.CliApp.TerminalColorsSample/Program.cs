using AppMotor.CliApp.Terminals;
using AppMotor.Core.Extensions;
using AppMotor.Core.Utils;

int maxColorNameLength = EnumUtils.GetValues<ConsoleColor>().Max(consoleColor => consoleColor.ToString().Length);
var colorNameFormatString = $"{{0,-{maxColorNameLength + 2}}}";

foreach (var consoleColor in EnumUtils.GetValues<ConsoleColor>())
{
    Terminal.WriteLine(colorNameFormatString.WithIC($"{consoleColor}:") + new ColoredSubstring(consoleColor, consoleColor.ToString()));
}
