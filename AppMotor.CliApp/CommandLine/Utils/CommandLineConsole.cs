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

using System.CommandLine;
using System.CommandLine.IO;
using System.Diagnostics.CodeAnalysis;

using AppMotor.CliApp.Terminals;

namespace AppMotor.CliApp.CommandLine.Utils
{
    internal sealed class CommandLineConsole : IConsole
    {
        private readonly ITerminal _terminal;

        /// <inheritdoc />
        public IStandardStreamWriter Out { get; }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public bool IsOutputRedirected => this._terminal.IsOutputRedirected;

        /// <inheritdoc />
        public IStandardStreamWriter Error { get; }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public bool IsErrorRedirected => this._terminal.IsErrorRedirected;

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public bool IsInputRedirected => this._terminal.IsInputRedirected;

        public CommandLineConsole(ITerminal terminal)
        {
            this._terminal = terminal;

            this.Out = StandardStreamWriter.Create(terminal.Out);
            this.Error = StandardStreamWriter.Create(terminal.Error);
        }
    }
}
