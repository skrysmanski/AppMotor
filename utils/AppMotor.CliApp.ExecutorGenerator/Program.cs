﻿#region License
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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using AppMotor.Core.Extensions;
using AppMotor.Core.IO;

using JetBrains.Annotations;

namespace AppMotor.CliApp.ExecutorGenerator
{
    /// <summary>
    /// Generates the code for the executor classes in AppMotor.CliApp.
    /// </summary>
    internal static class Program
    {
        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters")]
        public static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Path to CliApp project directory is not specified.");
                return 1;
            }

            DirectoryPath cliAppProjectDirectoryPath = args[0];

            if (cliAppProjectDirectoryPath.Name != "AppMotor.CliApp")
            {
                Console.WriteLine($"The path '{cliAppProjectDirectoryPath}' does not point to the correct folder.");
                return 1;
            }

            if (!cliAppProjectDirectoryPath.Exists())
            {
                Console.WriteLine($"The path '{cliAppProjectDirectoryPath}' does not exist (resolves to: {cliAppProjectDirectoryPath.AsAbsolutePath()}).");
                return 1;
            }

            ProcessCliApplicationExecutor(cliAppProjectDirectoryPath);
            ProcessCliCommandExecutor(cliAppProjectDirectoryPath);

            return 0;
        }

        private static void ProcessCliApplicationExecutor(DirectoryPath cliAppProjectDirectoryPath)
        {
            var path = new FilePath(cliAppProjectDirectoryPath, "CliApplicationExecutor.cs");

            var generator = new CliAppExecutorGenerator(
                "CliApplicationExecutor",
                new ExecutorParameterDescriptor("string[]", "args", "the command line params")
            );

            ProcessExecutorCodeFile(path, generator);
        }

        private static void ProcessCliCommandExecutor(DirectoryPath cliAppProjectDirectoryPath)
        {
            var path = new FilePath(cliAppProjectDirectoryPath, "CommandLine/CliCommandExecutor.cs");

            var generator = new CliAppExecutorGenerator(
                "CliCommandExecutor"
            );

            ProcessExecutorCodeFile(path, generator);
        }

        private static void ProcessExecutorCodeFile(FilePath filePath, CliAppExecutorGenerator codeGenerator)
        {
            var codeText = filePath.ReadAllText(Encoding.UTF8);

            var codeFile = new CodeFile(codeText);

            var newCodeText = codeFile.InsertNewGeneratedCode(codeGenerator.GenerateClassContent());

            filePath.WriteAllText(newCodeText, Encoding.UTF8);
        }

        private sealed class CodeFile
        {
            private const string START_MARKER = "// START MARKER: Generated code";

            private const string END_MARKER = "// END MARKER: Generated code";

            private readonly string _codeAboveGeneratedCode;

            private readonly string _codeBelowGeneratedCode;

            public CodeFile(string allCode)
            {
                bool foundStartMarker = false;
                bool foundEndMarker = false;
                var codeLinesAboveGeneratedCode = new List<string>();
                var codeLinesBelowGeneratedCode = new List<string>();

                foreach (var line in allCode.SplitLines())
                {
                    if (!foundStartMarker)
                    {
                        codeLinesAboveGeneratedCode.Add(line);
                    }
                    else if (foundEndMarker)
                    {
                        codeLinesBelowGeneratedCode.Add(line);
                    }

                    if (!foundStartMarker)
                    {
                        if (line.Contains(START_MARKER, StringComparison.OrdinalIgnoreCase))
                        {
                            foundStartMarker = true;
                        }

                    }
                    else if (!foundEndMarker)
                    {
                        if (line.Contains(END_MARKER, StringComparison.OrdinalIgnoreCase))
                        {
                            foundEndMarker = true;
                            codeLinesBelowGeneratedCode.Add(line);
                        }
                    }
                }

                if (!foundStartMarker)
                {
                    throw new InvalidOperationException($"Code does not contain the start marker: {START_MARKER}");
                }

                if (!foundEndMarker)
                {
                    throw new InvalidOperationException($"Code does not contain the end marker: {START_MARKER}");
                }

                this._codeAboveGeneratedCode = string.Join(CliAppExecutorGenerator.LINE_BREAK, codeLinesAboveGeneratedCode).TrimEnd() + CliAppExecutorGenerator.LINE_BREAK;
                this._codeBelowGeneratedCode = string.Join(CliAppExecutorGenerator.LINE_BREAK, codeLinesBelowGeneratedCode).TrimEnd() + CliAppExecutorGenerator.LINE_BREAK;
            }

            [MustUseReturnValue]
            public string InsertNewGeneratedCode(string generatedCode)
            {
                return this._codeAboveGeneratedCode
                     + generatedCode + CliAppExecutorGenerator.LINE_BREAK
                     + this._codeBelowGeneratedCode;
            }
        }
    }
}
