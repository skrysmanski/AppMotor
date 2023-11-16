// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.CommandLine;
using AppMotor.CliApp.CommandLine.Utils;
using AppMotor.CliApp.Properties;
using AppMotor.CliApp.Terminals;
using AppMotor.CliApp.Terminals.Formatting;
using AppMotor.Core.Exceptions;
using AppMotor.Core.Logging;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.CliApp;

/// <summary>
/// Base class for .NET console applications. Provides basic exception handling (via <see cref="OnUnhandledException"/>),
/// exit code configuration (<see cref="ExitCodeOnException"/>), access to an <see cref="ITerminal"/> instance, and
/// support for <see cref="WaitForKeyPressOnExit"/>.
///
/// <para>Use <see cref="Run{TApp}"/>, <see cref="RunAsync{TApp}"/>, or any of the other <c>Run</c> methods as entry point.</para>
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

    #region Static Run Methods

    /// <summary>
    /// Executes the application specified by <typeparamref name="TApp"/>. A new instance of
    /// <typeparamref name="TApp"/> will be created by this method.
    /// </summary>
    /// <returns>The exit code to use.</returns>
    [MustUseReturnValue]
    public static int Run<TApp>(string[] args, ITerminal? terminal = null) where TApp : CliApplication, new()
    {
        return Task.Run(() => RunAsync<TApp>(args, terminal)).Result;
    }

    /// <summary>
    /// Executes the application specified by <typeparamref name="TApp"/>. A new instance of
    /// <typeparamref name="TApp"/> will be created by this method.
    /// </summary>
    /// <returns>The exit code to use.</returns>
    [MustUseReturnValue]
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
    /// Executes <paramref name="mainFunc"/> as an <see cref="CliApplication"/>.
    /// </summary>
    /// <returns>The exit code to use.</returns>
    [MustUseReturnValue]
    public static int Run(Action mainFunc, ITerminal? terminal = null)
    {
        var app = new MiniApp(new(mainFunc));

        if (terminal is not null)
        {
            app.Terminal = terminal;
        }

        return app.Run();
    }

    /// <summary>
    /// Executes <paramref name="mainFunc"/> as an <see cref="CliApplication"/>.
    /// </summary>
    /// <returns>The exit code to use.</returns>
    [MustUseReturnValue]
    public static int Run(Func<int> mainFunc, ITerminal? terminal = null)
    {
        var app = new MiniApp(new(mainFunc));

        if (terminal is not null)
        {
            app.Terminal = terminal;
        }

        return app.Run();
    }

    /// <summary>
    /// Executes <paramref name="mainFunc"/> as an <see cref="CliApplication"/>.
    /// </summary>
    /// <returns>The exit code to use.</returns>
    [MustUseReturnValue]
    public static Task<int> RunAsync(Func<Task> mainFunc, ITerminal? terminal = null)
    {
        var app = new MiniApp(new(mainFunc));

        if (terminal is not null)
        {
            app.Terminal = terminal;
        }

        return app.RunAsync();
    }

    /// <summary>
    /// Executes <paramref name="mainFunc"/> as an <see cref="CliApplication"/>.
    /// </summary>
    /// <returns>The exit code to use.</returns>
    [MustUseReturnValue]
    public static Task<int> RunAsync(Func<Task<int>> mainFunc, ITerminal? terminal = null)
    {
        var app = new MiniApp(new(mainFunc));

        if (terminal is not null)
        {
            app.Terminal = terminal;
        }

        return app.RunAsync();
    }

    /// <summary>
    /// Executes <paramref name="mainCommand"/> as an <see cref="CliApplication"/> (via
    /// <see cref="CliApplicationWithCommand"/>).
    /// </summary>
    /// <returns>The exit code to use.</returns>
    [MustUseReturnValue]
    public static int Run(string[] args, CliCommand mainCommand, ITerminal? terminal = null)
    {
        var app = new CliApplicationWithCommand(mainCommand);

        if (terminal is not null)
        {
            app.Terminal = terminal;
        }

        return app.Run(args);
    }

    /// <summary>
    /// Executes <paramref name="mainCommand"/> as an <see cref="CliApplication"/> (via
    /// <see cref="CliApplicationWithCommand"/>).
    /// </summary>
    /// <returns>The exit code to use.</returns>
    [MustUseReturnValue]
    public static Task<int> RunAsync(string[] args, CliCommand mainCommand, ITerminal? terminal = null)
    {
        var app = new CliApplicationWithCommand(mainCommand);

        if (terminal is not null)
        {
            app.Terminal = terminal;
        }

        return app.RunAsync(args);
    }

    /// <summary>
    /// Executes the specified <paramref name="verbs"/> as an <see cref="CliApplication"/> (via
    /// <see cref="CliApplicationWithVerbs"/>).
    /// </summary>
    /// <returns>The exit code to use.</returns>
    [MustUseReturnValue]
    public static int Run(string[] args, params CliVerb[] verbs)
    {
        Validate.ArgumentWithName(nameof(verbs)).IsNotNullOrEmpty(verbs);

        return new CliApplicationWithVerbs(verbs).Run(args);
    }

    /// <summary>
    /// Executes the specified <paramref name="verbs"/> as an <see cref="CliApplication"/> (via
    /// <see cref="CliApplicationWithVerbs"/>).
    /// </summary>
    /// <returns>The exit code to use.</returns>
    [MustUseReturnValue]
    public static Task<int> RunAsync(string[] args, params CliVerb[] verbs)
    {
        Validate.ArgumentWithName(nameof(verbs)).IsNotNullOrEmpty(verbs);

        return new CliApplicationWithVerbs(verbs).RunAsync(args);
    }

    #endregion Static Run Methods

    #region Instance Run Methods

    /// <summary>
    /// Runs this application.
    /// </summary>
    /// <returns>The exit code to use.</returns>
    [MustUseReturnValue]
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
    [MustUseReturnValue]
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
    [MustUseReturnValue]
    public int Run(string[] args, CancellationToken cancellationToken)
    {
        return Task.Run(() => RunAsync(args, cancellationToken), cancellationToken).Result;
    }

    /// <summary>
    /// Runs this application.
    /// </summary>
    /// <returns>The exit code to use.</returns>
    [MustUseReturnValue]
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
    [MustUseReturnValue]
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
    [MustUseReturnValue]
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

        if (this.WaitForKeyPressOnExit && !this.Terminal.IsInputRedirected)
        {
            this.Terminal.WriteLine();
            this.Terminal.WriteLine(LocalizableResources.PressAnyKeyToExit);
            this.Terminal.ReadKey(displayPressedKey: false);
        }

        return exitCode;
    }

    #endregion Instance Run Methods

    /// <summary>
    /// Does the default processing for unhandled exceptions. Implementers may use this method
    /// in places where there is additional unhandled exception handling (e.g. for frameworks).
    /// </summary>
    /// <param name="exception">The unhandled exception</param>
    /// <returns>The exit code to be used.</returns>
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
    /// Called for any unhandled exception that is thrown by the <see cref="MainExecutor"/>. Calls <see cref="GetSupportMessage"/>
    /// to get any custom support message.
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
            terminal.WriteLine(TermText.Magenta(supportMessage));
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
            terminal.WriteLine(TermText.Red(exception.Message));
            printSupportMessage = false;
        }
        else
        {
            terminal.WriteLine(TermText.Red(exception.ToStringExtended()));
            printSupportMessage = true;
        }

        return printSupportMessage;
    }

    private sealed class MiniApp : CliApplication
    {
        /// <inheritdoc />
        protected override CliApplicationExecutor MainExecutor { get; }

        /// <inheritdoc />
        public MiniApp(CliApplicationExecutor mainExecutor)
        {
            this.MainExecutor = mainExecutor;
        }
    }
}
