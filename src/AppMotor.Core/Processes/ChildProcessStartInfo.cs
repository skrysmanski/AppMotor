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

using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.Core.Processes
{
    /// <summary>
    /// Allows some more advanced options for <see cref="ChildProcess"/> to be specified.
    /// </summary>
    [PublicAPI]
    public sealed class ChildProcessStartInfo
    {
        /// <summary>
        /// The default value for <see cref="ProcessTimeout"/>.
        /// </summary>
        public static readonly TimeSpan DEFAULT_PROCESS_TIMEOUT = TimeSpan.FromSeconds(30);

        /// <summary>
        /// The path to the file to execute.
        /// </summary>
        public string ProcessFileName { get; }

        /// <summary>
        /// The arguments to pass to <see cref="ProcessFileName"/>.
        /// </summary>
        public ProcessArguments Arguments { get; } = new();

        /// <summary>
        /// The working directory. Can be <c>null</c> in which case the runtime picks
        /// a working directory.
        /// </summary>
        public string? WorkingDirectory { get; set; }

        /// <summary>
        /// Determines for how long the process can run before triggering a <see cref="TimeoutException"/>.
        /// Can be <c>null</c> in which case <see cref="ChildProcess"/> will wait indefinitely for the process
        /// to finish (not recommended).
        ///
        /// <para>Defaults to 30 seconds (see <see cref="DEFAULT_PROCESS_TIMEOUT"/>).</para>
        /// </summary>
        public TimeSpan? ProcessTimeout { get; set; } = DEFAULT_PROCESS_TIMEOUT;

        /// <summary>
        /// The exit code that represents success. If the child process reports any other exit code,
        /// <see cref="ChildProcess"/> will throw a <see cref="ChildProcessErrorException"/> exception.
        /// If set to <c>null</c>, the exit code won't be checked (and thus no exception will occur).
        ///
        /// <para>Defaults to 0.</para>
        /// </summary>
        public int? SuccessExitCode { get; set; } = 0;

        /// <summary>
        /// Whether any (non-whitespace) output to stderr should be considered an error. If <c>true</c>,
        /// <see cref="ChildProcess"/> will throw a <see cref="ChildProcessErrorException"/> in this case.
        /// If <c>false</c>, the error output will be returned via <see cref="ChildProcessResult.ErrorOutput"/>.
        ///
        /// <para>Defaults to <c>true</c>.</para>
        /// </summary>
        public bool TreatErrorOutputAsFailure { get; set; } = true;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="processFileName">The path to the file to execute.</param>
        public ChildProcessStartInfo(string processFileName)
        {
            Validate.ArgumentWithName(nameof(processFileName)).IsNotNullOrWhiteSpace(processFileName);

            this.ProcessFileName = processFileName;
        }
    }
}
