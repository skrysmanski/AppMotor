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

using JetBrains.Annotations;

namespace AppMotor.CliApp.Terminals
{
    /// <summary>
    /// Represents the error output (i.e. analog to <see cref="Console.Error"/>) of a terminal.
    /// </summary>
    /// <seealso cref="ITerminal"/>
    /// <seealso cref="IOutputTerminal"/>
    /// <seealso cref="IStdOutTerminal"/>
    public interface IErrorOutTerminal
    {
        /// <summary>
        /// The standard error output stream.
        /// </summary>
        [PublicAPI]
        TextWriter Error { get; }

        /// <summary>
        /// Whether <see cref="Error"/> is redirected (to a file or the input
        /// of another process).
        /// </summary>
        [PublicAPI]
        bool IsErrorRedirected { get; }
    }
}
