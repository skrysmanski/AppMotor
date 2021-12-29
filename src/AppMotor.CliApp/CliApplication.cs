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
using System.Threading;
using System.Threading.Tasks;

using AppMotor.CliApp.CommandLine;
using AppMotor.CliApp.CommandLine.Utils;
using AppMotor.CliApp.Properties;
using AppMotor.CliApp.Terminals;
using AppMotor.Core.Exceptions;
using AppMotor.Core.Logging;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.CliApp;

/// <summary>
/// Base class for .NET console applications. Use <see cref="Run{TApp}"/> or <see cref="RunAsync{TApp}"/> as entry point.
///
/// <para>You may consider using <see cref="CliApplicationWithVerbs"/>, <see cref="CliApplicationWithParams"/>, or
/// <see cref="CliApplicationWithCommand"/> instead - for convenience reasons.</para>
/// </summary>
public abstract class CliApplication
{
    private static bool s_tlsSettingsApplied;

    /// <summary>
    /// The exit code to use when an unhandled exception led to the termination
    /// of the process.
    /// </summary>
    [PublicAPI]
    protected virtual int ExitCodeOnException => -1;

    /// <summary>
    /// Whether to display a "Press any key to exit..." message when the process
    /// terminates.
    /// </summary>
    [PublicAPI]
    protected virtual bool WaitForKeyPressOnExit => false;

    /// <summary>
    /// The terminal to use within this application. Defaults to <see cref="Terminals.Terminal"/>.
    /// </summary>
    /// <remarks>
    /// This property mainly exists for unit testing purposes where you need to obtain
    /// everything written to the terminal.
    /// </remarks>
    [PublicAPI]
    public ITerminal Terminal { get; set; } = Terminals.Terminal.Instance;

    /// <summary>
    /// The main/execute method for this application.
    ///
    /// <para>Recommendation: For ease of use, use the array syntax (<c>=&gt;</c>) when implementing
    /// this property.</para>
    /// </summary>
    protected abstract CliApplicationExecutor MainExecutor { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    protected CliApplication()
    {
        if (!s_tlsSettingsApplied)
        {
            TlsSettings.ApplyToCurrentProcess();
            s_tlsSettingsApplied = true;
        }
    }

    /// <summary>
    /// Executes the application specified by <typeparamref name="TApp"/>. A new instance of
    /// <typeparamref name="TApp"/> will be created by this method.
    /// </summary>
    /// <returns>The exit code to use.</returns>
    [PublicAPI, MustUseReturnValue]
    public static int Run<TApp>(string[] args, ITerminal? terminal = null) where TApp : CliApplication, new()
    {
        return Task.Run(() => RunAsync<TApp>(args, terminal)).Result;
    }

    /// <summary>
    /// Executes the application specified by <typeparamref name="TApp"/>. A new instance of
    /// <typeparamref name="TApp"/> will be created by this method.
    /// </summary>
    /// <returns>The exit code to use.</returns>
    [PublicAPI, MustUseReturnValue]
    public static async Task<int> RunAsync<TApp>(string[] args, ITerminal? terminal = null) where TApp : CliApplication, new()
    {
        var app = new TApp();

        if (terminal != null)
        {
            app.Terminal = terminal;
        }

        return await app.RunAsync(args).ConfigureAwait(continueOnCapturedContext: false);
    }

    /// <summary>
    /// Runs this application.
    /// </summary>
    /// <returns>The exit code to use.</returns>
    [PublicAPI, MustUseReturnValue]
    public int Run(params string[] args)
    {
        return Task.Run(() => RunAsync(args)).Result;
    }

    /// <summary>
    /// Runs this application.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to be used to cancel/stop long running
    /// applications (e.g. server applications).</param>
    /// <returns>The exit code to use.</returns>
    [PublicAPI, MustUseReturnValue]
    public int Run(CancellationToken cancellationToken)
    {
        return Task.Run(() => RunAsync(cancellationToken), cancellationToken).Result;
    }

    /// <summary>
    /// Runs this application.
    /// </summary>
    /// <param name="args">The args arrays from the main method</param>
    /// <param name="cancellationToken">A cancellation token to be used to cancel/stop long running
    /// applications (e.g. server applications).</param>
    /// <returns>The exit code to use.</returns>
    [PublicAPI, MustUseReturnValue]
    public int Run(string[] args, CancellationToken cancellationToken)
    {
        return Task.Run(() => RunAsync(args, cancellationToken), cancellationToken).Result;
    }

    /// <summary>
    /// Runs this application.
    /// </summary>
    /// <returns>The exit code to use.</returns>
    [PublicAPI, MustUseReturnValue]
    public Task<int> RunAsync(params string[] args)
    {
        return RunAsync(args, CancellationToken.None);
    }

    /// <summary>
    /// Runs this application.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to be used to cancel/stop long running
    /// applications (e.g. server applications).</param>
    /// <returns>The exit code to use.</returns>
    [PublicAPI, MustUseReturnValue]
    public Task<int> RunAsync(CancellationToken cancellationToken)
    {
        return RunAsync(Array.Empty<string>(), cancellationToken);
    }

    /// <summary>
    /// Runs this application.
    /// </summary>
    /// <param name="args">The args arrays from the main method</param>
    /// <param name="cancellationToken">A cancellation token to be used to cancel/stop long running
    /// applications (e.g. server applications).</param>
    /// <returns>The exit code to use.</returns>
    [PublicAPI, MustUseReturnValue]
    public async Task<int> RunAsync(string[] args, CancellationToken cancellationToken)
    {
        int exitCode;

        try
        {
            exitCode = await this.MainExecutor.Execute(args, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
        }
        catch (Exception exception) when (!DebuggerUtils.IsDebuggerAttached)
        {
            exitCode = ProcessUnhandledException(exception);
        }

        if ((DebuggerUtils.IsDebuggerAttached || this.WaitForKeyPressOnExit) && !this.Terminal.IsInputRedirected)
        {
            this.Terminal.WriteLine();
            this.Terminal.WriteLine(LocalizableResources.PressAnyKeyToExit);
            this.Terminal.ReadKey(displayPressedKey: false);
        }

        return exitCode;
    }

    /// <summary>
    /// Does the default processing for unhandled exceptions. Implementers may use this method
    /// in places where there is additional unhandled exception handling (e.g. for frameworks).
    /// </summary>
    /// <param name="exception">The unhandled exception</param>
    /// <returns>The exit code to be used.</returns>
    [PublicAPI]
    protected int ProcessUnhandledException(Exception exception)
    {
        int exitCode;

        if (exception is ErrorMessageException)
        {
            // ErrorMessageExceptions are used to display some message directly to the user.
            // As such, they're not "actual" exceptions and thus should not use "ExitCodeOnException".
            exitCode = 1;
        }
        else
        {
            exitCode = this.ExitCodeOnException;
        }

        OnUnhandledException(exception, ref exitCode);

        return exitCode;
    }

    /// <summary>
    /// Called for any unhandled exception that is thrown by the <see cref="MainExecutor"/>.
    /// </summary>
    /// <param name="exception">The unhandled exception</param>
    /// <param name="exitCode">The exit code to be used; may be modified by implementations
    /// as they see fit. For most exceptions, this will be initialized to <see cref="ExitCodeOnException"/>.</param>
    [PublicAPI]
    protected virtual void OnUnhandledException(Exception exception, ref int exitCode)
    {
        var supportMessage = GetSupportMessage(exception);
        PrintUnhandledException(exception, supportMessage: supportMessage, terminal: this.Terminal);
    }

    /// <summary>
    /// Returns a support message to be printed alongside the exception report for an unhandled exception
    /// (via <see cref="OnUnhandledException"/>). Should contain information about what to do with the
    /// exception (e.g. a link to a bug tracker). If <c>null</c>, no support message will be printed (the
    /// default).
    /// </summary>
    /// <param name="exception">The unhandled exception</param>
    [PublicAPI]
    protected virtual string? GetSupportMessage(Exception exception)
    {
        return null;
    }

    /// <summary>
    /// Writes the specified exception to the Console.
    /// </summary>
    /// <param name="exception">The exception to print</param>
    /// <param name="supportMessage">An optional message with information about what to do
    /// with the exception (e.g. link to bug tracker).</param>
    /// <param name="terminal">The terminal to use; defaults to <see cref="Terminals.Terminal"/>.</param>
    [PublicAPI]
    public static void PrintUnhandledException(Exception exception, string? supportMessage, ITerminal? terminal = null)
    {
        terminal ??= Terminals.Terminal.Instance;

        bool printSupportMessage = PrintUnhandledException(exception, terminal);

        if (supportMessage != null && printSupportMessage)
        {
            terminal.WriteLine();
            terminal.WriteLine((TextInMagenta)supportMessage);
            terminal.WriteLine();
        }
    }

    [MustUseReturnValue]
    private static bool PrintUnhandledException(Exception exception, ITerminal terminal)
    {
        bool printSupportMessage;

        if (exception is AggregateException aggregateException)
        {
            printSupportMessage = false;
            foreach (var innerException in aggregateException.InnerExceptions)
            {
                if (PrintUnhandledException(innerException, terminal))
                {
                    printSupportMessage = true;
                }
            }
        }
        else if (exception is ErrorMessageException)
        {
            terminal.WriteLine((TextInRed)exception.Message);
            printSupportMessage = false;
        }
        else
        {
            terminal.WriteLine((TextInRed)exception.ToStringExtended());
            printSupportMessage = true;
        }

        return printSupportMessage;
    }
}
