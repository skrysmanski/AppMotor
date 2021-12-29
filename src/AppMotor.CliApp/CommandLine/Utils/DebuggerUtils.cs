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

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;

using AppMotor.CliApp.Properties;
using AppMotor.CliApp.Terminals;
using AppMotor.Core.Extensions;

namespace AppMotor.CliApp.CommandLine.Utils;

/// <summary>
/// Utils to interact with <see cref="Debugger"/> - but make it compatible for unit tests.
/// </summary>
[ExcludeFromCodeCoverage]
internal static class DebuggerUtils
{
    public static bool IsTestRun { get; set; }

    public static int DebuggerLaunchCount { get; private set; }

    /// <summary>
    /// Should be used instead of <see cref="Debugger.IsAttached"/>.
    /// </summary>
    public static bool IsDebuggerAttached
    {
        get
        {
            if (IsTestRun)
            {
                return false;
            }
            else
            {
                return Debugger.IsAttached;
            }
        }
    }

    /// <summary>
    /// Should be used instead of <see cref="Debugger.Launch"/>.
    /// </summary>
    public static void LaunchDebugger(IStdOutTerminal terminal)
    {
        if (IsDebuggerAttached)
        {
            return;
        }

        if (IsTestRun)
        {
            DebuggerLaunchCount++;
        }
        else
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // This is only supported on Windows.
                Debugger.Launch();
            }
            else
            {
                var process = Process.GetCurrentProcess();

                terminal.WriteLine(LocalizableResources.WaitForDebuggerAttach.With(process.Id, process.ProcessName));
                terminal.WriteLine();

                while (!Debugger.IsAttached)
                {
                    Thread.Sleep(50);
                }
            }
        }
    }
}