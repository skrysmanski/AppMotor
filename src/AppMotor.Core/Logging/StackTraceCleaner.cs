// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Globalization;
using System.Text.RegularExpressions;

using JetBrains.Annotations;

namespace AppMotor.Core.Logging;

/// <summary>
/// Cleans up stack traces to make them easier to read.
/// </summary>
/// <remarks>
/// Note to implementers: What's easier to read is most likely a matter of experience or personal preference.
/// Also, changing the stack trace syntax may break existing tools (like the ReSharper navigation). So this
/// needs to be considered when introducing changes here.
/// </remarks>
internal static class StackTraceCleaner
{
    private static readonly Regex s_stackTraceLineRegex = new(@"^(\s*)\w+ ([\w.<>]+)\((.*)\)(?:\s+\w+\s+(.+):\w+\s+(\d+))?$", RegexOptions.Compiled);

    [MustUseReturnValue]
    public static IEnumerable<string> CleanupStackTraceLines(IEnumerable<string> stackTraceLines)
    {
        foreach (var stackTraceLineAsString in stackTraceLines)
        {
            var match = s_stackTraceLineRegex.Match(stackTraceLineAsString);
            if (!match.Success)
            {
                yield return stackTraceLineAsString;
                continue;
            }

            string whiteSpaceAtBeginningOfLine = match.Groups[1].Value;

            StackTraceLine stackTraceLine;

            if (match.Groups[4].Success)
            {
                // With file information and line
                stackTraceLine = new StackTraceLine(
                    namespaceTypeAndMethod: match.Groups[2].Value,
                    parameters: match.Groups[3].Value,
                    filePath: match.Groups[4].Value,
                    line: int.Parse(match.Groups[5].Value, CultureInfo.InvariantCulture)
                );
            }
            else
            {
                // Without file information and line
                stackTraceLine = new StackTraceLine(
                    namespaceTypeAndMethod: match.Groups[2].Value,
                    parameters: match.Groups[3].Value,
                    filePath: null,
                    line: null
                );
            }

            yield return $"{whiteSpaceAtBeginningOfLine}{stackTraceLine}";
        }
    }

    private sealed class StackTraceLine
    {
        // Matches
        // * .<>c.<.ctor>b__1_0
        // * .<>c__DisplayClass1_0.<.ctor>b__3
        private static readonly Regex LAMBDA_REGEX = new(@"\.<>c[^.]*\.<(.+)>b__.+$", RegexOptions.Compiled);

        private readonly string _namespaceTypeAndMethod;

        private readonly string _parameters;

        private readonly string? _filePath;

        private readonly int? _line;

        public StackTraceLine(string namespaceTypeAndMethod, string parameters, string? filePath, int? line)
        {
            this._namespaceTypeAndMethod = ProcessNamespaceTypeAndMethod(namespaceTypeAndMethod);
            this._parameters = ProcessParameters(parameters);
            this._filePath = filePath;
            this._line = line;
        }

        [MustUseReturnValue]
        private static string ProcessNamespaceTypeAndMethod(string namespaceTypeAndMethod)
        {
            //
            // NOTE: There are some signatures I'd like to replace with something easier to identify
            //   (although this is probably a matter of perspective) - like "get_HelpBuilder()" to
            //   "HelpBuilder#get()", or ".ctor" to "#ctor" - but ReSharper can read the unmodified
            //   syntax and jump to the correct place. So I did apply these changes so that ReSharper
            //   navigation would not get broken by this.
            //

            //
            // Replace compiler generated members syntax
            //
            // See: https://stackoverflow.com/a/2509524/614177
            //
            // NOTE:
            // * A < (and >) in a name means "compiler generated member".
            // * The character after "<>" is: https://github.com/dotnet/roslyn/blob/main/src/Compilers/CSharp/Portable/Symbols/Synthesized/GeneratedNameKind.cs
            // * The format is: [CS$]<[middle]>c[__[suffix]] where [CS$] is included for certain
            //   generated names, where [middle] and [__[suffix]] are optional,
            //   and where c is a single character in [1-9a-z].
            //   Source: https://github.com/dotnet/roslyn/blob/main/src/Compilers/CSharp/Portable/Symbols/Synthesized/GeneratedNames.cs

            namespaceTypeAndMethod = LAMBDA_REGEX.Replace(namespaceTypeAndMethod, ".$1.λ");

            return namespaceTypeAndMethod;
        }

        [MustUseReturnValue]
        private static string ProcessParameters(string parameters)
        {
            if (parameters.Length == 0)
            {
                return "";
            }

            return parameters
                   .Replace(nameof(Int16), "short")
                   .Replace(nameof(UInt16), "ushort")
                   .Replace(nameof(Int32), "int")
                   .Replace(nameof(UInt32), "uint")
                   .Replace(nameof(Int64), "long")
                   .Replace(nameof(UInt64), "ulong")
                ;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            if (this._filePath is null)
            {
                return $"at {this._namespaceTypeAndMethod}({this._parameters})";
            }
            else
            {
                return $"at {this._namespaceTypeAndMethod}({this._parameters}) in {this._filePath}:line {this._line}";
            }
        }
    }
}
