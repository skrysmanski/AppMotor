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
using System.IO;
using System.Text;

using AppMotor.CliApp.Terminals;

namespace AppMotor.CliApp.TestUtils
{
    internal sealed class TestTerminal : ITerminal
    {
        /// <inheritdoc />
        public TextReader Input => throw new NotSupportedException();

        /// <inheritdoc />
        public bool IsInputRedirected => true; // tests always run "non-interactive"

        /// <inheritdoc />
        public bool IsKeyAvailable => throw new NotSupportedException();

        /// <inheritdoc />
        public TextWriter Error { get; }

        /// <inheritdoc />
        public bool IsErrorRedirected => throw new NotSupportedException();

        /// <inheritdoc />
        public TextWriter Out { get; }

        private readonly StringBuilder _outWriter = new();

        public string CurrentOutput => this._outWriter.ToString();

        /// <inheritdoc />
        public bool IsOutputRedirected => false;

        /// <inheritdoc />
        public ConsoleColor BackgroundColor
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public TestTerminal()
        {
            var threadSafeWriter = TextWriter.Synchronized(new StringWriter(this._outWriter));
            this.Out = threadSafeWriter;
            this.Error = threadSafeWriter;
        }

        public void ResetOutput()
        {
            this._outWriter.Clear();
        }

        /// <inheritdoc />
        public ConsoleKeyInfo ReadKey(bool displayPressedKey = true) => throw new NotSupportedException();

        /// <inheritdoc />
        public string ReadLine() => throw new NotSupportedException();

        /// <inheritdoc />
        public void Write(ColoredString? coloredString)
        {
            this.Out.Write(coloredString);
        }
    }
}
