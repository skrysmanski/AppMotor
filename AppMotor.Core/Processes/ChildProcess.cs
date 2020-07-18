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
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.Core.Processes
{
    /// <summary>
    /// Convenience class to execute child processes. This class especially takes care of potential
    /// deadlocks the may occur when not properly reading the output of the process.
    ///
    /// <para>Note: This class is designed to be used with short running processes (as it stores
    /// all output in an internal buffer). Using it for processes that create huge amounts of
    /// output may lead to <see cref="OutOfMemoryException"/>s.</para>
    /// </summary>
    public static class ChildProcess
    {
        /// <summary>
        /// Executes the child process synchronously.
        /// </summary>
        ///
        /// <param name="processFileName">The program to execute.</param>
        /// <param name="arguments">The arguments to pass to the program.</param>
        /// <param name="workingDirectory">The working directory. If not specified, the current working
        /// directory will be used.</param>
        /// <param name="successExitCode">The exit code that represent success. If <c>null</c>, the
        /// exit code won't be checked.</param>
        ///
        /// <returns>The "result" of the process.</returns>
        ///
        /// <exception cref="ChildProcessErrorException">Thrown if the exit code signals failure or if anything
        /// is written to stderr by the child process.</exception>
        /// <exception cref="TimeoutException">Thrown when the process runs longer than 30 seconds.</exception>
        [PublicAPI, NotNull]
        public static ChildProcessResult Exec(
                [NotNull] string processFileName,
                [CanBeNull] ProcessArguments arguments = null,
                [CanBeNull] string workingDirectory = null,
                int? successExitCode = 0
            )
        {
            var startInfo = CreateChildProcessStartInfo(
                processFileName: processFileName,
                arguments: arguments,
                workingDirectory: workingDirectory,
                successExitCode: successExitCode
            );

            return Exec(startInfo);
        }

        /// <summary>
        /// Executes the child process synchronously.
        /// </summary>
        ///
        /// <returns>The "result" of the process.</returns>
        ///
        /// <exception cref="ChildProcessErrorException">Thrown if the exit code signals failure or if anything
        /// is written to stderr by the child process and <see cref="ChildProcessStartInfo.TreatErrorOutputAsFailure"/>
        /// is <c>true</c>.</exception>
        /// <exception cref="TimeoutException">Thrown when the process timeout is exceeded
        /// (see <see cref="ChildProcessStartInfo.ProcessTimeout"/>).</exception>
        [PublicAPI, NotNull]
        public static ChildProcessResult Exec([NotNull] ChildProcessStartInfo startInfo)
        {
            Verify.Argument.NotNull(startInfo, nameof(startInfo));

            var runningProcess = StartProcess(startInfo);

            return runningProcess.Execute();
        }

        /// <summary>
        /// Executes the child process asynchronously.
        /// </summary>
        ///
        /// <param name="processFileName">The program to execute.</param>
        /// <param name="arguments">The arguments to pass to the program.</param>
        /// <param name="workingDirectory">The working directory. If not specified, the current working
        /// directory will be used.</param>
        /// <param name="successExitCode">The exit code that represent success. If <c>null</c>, the
        /// exit code won't be checked.</param>
        ///
        /// <returns>The "result" of the process.</returns>
        ///
        /// <exception cref="ChildProcessErrorException">Thrown if the exit code signals failure or if anything
        /// is written to stderr by the child process.</exception>
        /// <exception cref="TimeoutException">Thrown when the process runs longer than 30 seconds.</exception>
        [PublicAPI, ItemNotNull]
        public static async Task<ChildProcessResult> ExecAsync(
                [NotNull] string processFileName,
                [CanBeNull] ProcessArguments arguments = null,
                [CanBeNull] string workingDirectory = null,
                int? successExitCode = 0
            )
        {
            var startInfo = CreateChildProcessStartInfo(
                processFileName: processFileName,
                arguments: arguments,
                workingDirectory: workingDirectory,
                successExitCode: successExitCode
            );

            return await ExecAsync(startInfo);
        }


        /// <summary>
        /// Executes the child process asynchronously.
        /// </summary>
        ///
        /// <returns>The "result" of the process.</returns>
        ///
        /// <exception cref="ChildProcessErrorException">Thrown if the exit code signals failure or if anything
        /// is written to stderr by the child process and <see cref="ChildProcessStartInfo.TreatErrorOutputAsFailure"/>
        /// is <c>true</c>.</exception>
        /// <exception cref="TimeoutException">Thrown when the process timeout is exceeded
        /// (see <see cref="ChildProcessStartInfo.ProcessTimeout"/>).</exception>
        [PublicAPI, ItemNotNull]
        public static async Task<ChildProcessResult> ExecAsync([NotNull] ChildProcessStartInfo startInfo)
        {
            var runningProcess = StartProcess(startInfo);

            return await runningProcess.ExecuteAsync();
        }

        [NotNull, Pure]
        private static ChildProcessStartInfo CreateChildProcessStartInfo(
                [NotNull] string processFileName,
                [CanBeNull] ProcessArguments arguments,
                [CanBeNull] string workingDirectory,
                int? successExitCode
            )
        {
            var startInfo = new ChildProcessStartInfo(processFileName)
            {
                WorkingDirectory = workingDirectory,
                SuccessExitCode = successExitCode,
            };

            if (arguments != null)
            {
                startInfo.Arguments.AddRange(arguments);
            }

            return startInfo;
        }

        [NotNull]
        private static PreparedProcess StartProcess([NotNull] ChildProcessStartInfo childProcessStartInfo)
        {
            var processStartInfo = new ProcessStartInfo(childProcessStartInfo.ProcessFileName);

            childProcessStartInfo.Arguments.CopyTo(processStartInfo);

            if (childProcessStartInfo.WorkingDirectory != null)
            {
                processStartInfo.WorkingDirectory = childProcessStartInfo.WorkingDirectory;
            }

            // necessary for redirecting streams
            processStartInfo.UseShellExecute = false;
            // don't pop up a new console window when running the process
            processStartInfo.CreateNoWindow = true;

            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;

            var process = new Process
            {
                StartInfo = processStartInfo,
            };

            return new PreparedProcess(process, childProcessStartInfo);
        }

        private sealed class PreparedProcess
        {
            [NotNull]
            private readonly Process m_process;

            [NotNull]
            private readonly ChildProcessStartInfo m_startInfo;

            public PreparedProcess([NotNull] Process process, [NotNull] ChildProcessStartInfo startInfo)
            {
                Verify.Argument.NotNull(process, nameof(process));
                Verify.Argument.NotNull(startInfo, nameof(startInfo));

                this.m_process = process;
                this.m_startInfo = startInfo;
            }

            [NotNull]
            public ChildProcessResult Execute()
            {
                StartProcess(out var stdOutReadTask, out var stdErrReadTask);

                int timeoutAsMilliseconds;

                if (this.m_startInfo.ProcessTimeout == null || this.m_startInfo.ProcessTimeout == Timeout.InfiniteTimeSpan)
                {
                    timeoutAsMilliseconds = Timeout.Infinite;
                }
                else
                {
                    timeoutAsMilliseconds = (int)this.m_startInfo.ProcessTimeout.Value.TotalMilliseconds;
                }

                if (!this.m_process.WaitForExit(timeoutAsMilliseconds))
                {
                    throw new TimeoutException("The process has not finished within the specified timeout.");
                }

                Task.WaitAll(stdOutReadTask, stdErrReadTask);

                return PostProcessAfterExit(stdOutReadTask, stdErrReadTask);
            }

            [ItemNotNull]
            public async Task<ChildProcessResult> ExecuteAsync()
            {
                CancellationTokenSource cts;

                if (this.m_startInfo.ProcessTimeout == null || this.m_startInfo.ProcessTimeout == Timeout.InfiniteTimeSpan)
                {
                    cts = new CancellationTokenSource();
                }
                else
                {
                    cts = new CancellationTokenSource(this.m_startInfo.ProcessTimeout.Value);
                }

                using (cts)
                {
                    var tcs = new TaskCompletionSource<object>();
                    this.m_process.EnableRaisingEvents = true;
                    this.m_process.Exited += (s, e) => tcs.TrySetResult(null);

                    cts.Token.Register(tcs.SetCanceled);

                    StartProcess(out var stdOutReadTask, out var stdErrReadTask);

                    try
                    {
                        await tcs.Task;
                    }
                    catch (OperationCanceledException)
                    {
                        throw new TimeoutException("The process has not finished within the specified timeout.");
                    }

                    await Task.WhenAll(stdOutReadTask, stdErrReadTask);

                    return PostProcessAfterExit(stdOutReadTask, stdErrReadTask);
                }
            }

            private void StartProcess([NotNull] out Task<string> stdOutReadTask, [NotNull] out Task<string> stdErrReadTask)
            {
                bool startSuccessful;

                try
                {
                    startSuccessful = this.m_process.Start();
                }
                catch (Win32Exception ex) when (ex.NativeErrorCode == 2) // Could not find file.
                {
                    throw new Exception($"Could not locate application file at: {this.m_process.StartInfo.FileName}");
                }

                if (!startSuccessful)
                {
                    throw new Exception("Process could not be started for unknown reasons.");
                }

                stdOutReadTask = Task.Run(() => this.m_process.StandardOutput.ReadToEndAsync());
                stdErrReadTask = Task.Run(() => this.m_process.StandardError.ReadToEndAsync());
            }

            [NotNull]
            private ChildProcessResult PostProcessAfterExit(
                    [NotNull] Task<string> stdOutReadTask,
                    [NotNull] Task<string> stdErrorReadTask
                )
            {
                var errorOutput = stdErrorReadTask.Result;

                if (string.IsNullOrWhiteSpace(errorOutput))
                {
                    errorOutput = null;
                }

                if (this.m_startInfo.TreatErrorOutputAsFailure && errorOutput != null)
                {
                    throw new ChildProcessErrorException($"The process has failed with (exit code: {this.m_process.ExitCode}):\n{errorOutput}");
                }

                if (this.m_startInfo.SuccessExitCode != null && this.m_process.ExitCode != this.m_startInfo.SuccessExitCode)
                {
                    throw new ChildProcessErrorException($"The process has failed with exit code {this.m_process.ExitCode}.");
                }

                return new ChildProcessResult(this.m_process.ExitCode, stdOutReadTask.Result ?? "", errorOutput);
            }
        }
    }
}
