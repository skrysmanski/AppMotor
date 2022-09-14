// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Text;

using AppMotor.Core.Extensions;

using JetBrains.Annotations;

namespace AppMotor.CliApp.ExecutorGenerator;

internal abstract class SourceCodeGeneratorBase
{
    protected static readonly string INDENTATION_LEVEL = new(' ', 4);

    private static readonly string BASE_INDENTATION = INDENTATION_LEVEL;

    public const string LINE_BREAK = "\r\n";

    private const string GENERATED_CODE_NOTE = @"
//
// NOTE: The code of this class has been generated with the 'ExecutorGenerator' tool. Do
//   not make manual changes to this class or they may get lost (by accident) when the code
//   for this class is generated the next time!!!
//
";
    private readonly StringBuilder _contentBuilder = new();

    [MustUseReturnValue]
    public string GenerateClassContent()
    {
        if (this._contentBuilder.Length == 0)
        {
            AppendLines(GENERATED_CODE_NOTE);
            AppendLine();

            GenerateClassContentCore();
        }

        return this._contentBuilder.ToString();
    }

    protected abstract void GenerateClassContentCore();

    protected void AppendLine(string line = "")
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            this._contentBuilder.Append(LINE_BREAK);
        }
        else
        {
            this._contentBuilder.Append($"{BASE_INDENTATION}{line}{LINE_BREAK}");
        }
    }

    protected void AppendLines(string lines)
    {
        AppendLines(lines.SplitLines());
    }

    private void AppendLines(IEnumerable<string> lines)
    {
        foreach (var line in lines)
        {
            AppendLine(line);
        }
    }

    [MustUseReturnValue]
    protected static string FixMultiLineText(string text)
    {
        return text.Trim('\r', '\n');
    }
}
