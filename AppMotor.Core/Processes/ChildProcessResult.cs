#region License
// Copyright  - 2020 AppWeave.Core (https://github.com/skrysmanski/AppWeave.Core)
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

using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.Core.Processes
{
    /// <summary>
    /// The result of executing a child process through <see cref="ChildProcess"/>.
    /// </summary>
    public sealed class ChildProcessResult
    {
        /// <summary>
        /// The exit code of the child process.
        /// </summary>
        [PublicAPI]
        public int ExitCode { get; }

        /// <summary>
        /// The output (stdout) of the child process. Never <c>null</c> but may be empty.
        /// </summary>
        [PublicAPI, NotNull]
        public string Output { get; }

        /// <summary>
        /// The error output (stderr) of the child process. Is <c>null</c> if no
        /// error output was produced by the child process.
        /// </summary>
        [PublicAPI, CanBeNull]
        public string ErrorOutput { get; }

        public ChildProcessResult(int exitCode, [NotNull] string output, [CanBeNull] string errorOutput)
        {
            Verify.ParamNotNull(output, nameof(output));

            this.ExitCode = exitCode;
            this.Output = output;
            this.ErrorOutput = errorOutput;
        }
    }
}
