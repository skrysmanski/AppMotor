#region License
// Copyright 2020 AppMotor Framework (https://github.com/skrysmanski/AppMotor)
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
using System.CommandLine;
using System.CommandLine.Builder;

using JetBrains.Annotations;

namespace AppMotor.CliApp.CommandLine.Utils
{
    internal static class RootCommandFactory
    {
        [MustUseReturnValue]
        public static RootCommand CreateRootCommand(string? appDescription, Func<Exception, int> exceptionHandlerFunc)
        {
            var rootCommand = new RootCommand();

            if (!string.IsNullOrWhiteSpace(appDescription))
            {
                rootCommand.Description = appDescription;
            }

            CreatePipelineFor(rootCommand, exceptionHandlerFunc);

            return rootCommand;
        }

        private static void CreatePipelineFor(RootCommand rootCommand, Func<Exception, int> exceptionHandlerFunc)
        {
            var builder = new CommandLineBuilder(rootCommand);

            builder.UseDefaults();

            builder.UseExceptionHandler((exception, context) => {
                int exitCode = exceptionHandlerFunc(exception);
                context.ResultCode = exitCode;
            });

            builder.Build();
        }
    }
}
