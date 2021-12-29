#region License
// Copyright 2021 AppMotor Framework (https://github.com/skrysmanski/AppMotor)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.Text;

using AppMotor.Core.Extensions;

using JetBrains.Annotations;

namespace AppMotor.CliApp.ExecutorGenerator;

internal abstract class SourceCodeGeneratorBase
{
    protected static readonly string INDENTATION_LEVEL = new(' ', 4);

    private static readonly string BASE_INDENTATION = INDENTATION_LEVEL + INDENTATION_LEVEL;

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