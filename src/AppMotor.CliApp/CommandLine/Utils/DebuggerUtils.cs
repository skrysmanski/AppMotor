// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

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
    public static void LaunchDebugger(ITerminalOutput terminal)
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
