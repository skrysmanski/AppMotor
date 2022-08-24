// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.CommandLine.Utils;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.CliApp.CommandLine;

/// <summary>
/// Represents a command line application with automatic command line argument parsing that only does
/// one function - like the <c>mv</c>, <c>rm</c>, or <c>dir</c> commands.
///
/// <para>The main command is specified either via constructor or via the <see cref="Command"/> property.</para>
///
/// <para>If you need an application that bundles various functions, use <see cref="CliApplicationWithVerbs"/> instead.
/// If you want to specify the <see cref="CliParam{T}"/>s directly in a sub class of this class, use
/// <see cref="CliApplicationWithParams"/> instead.</para>
/// </summary>
public class CliApplicationWithCommand : CliApplication
{
    /// <summary>
    /// The description of this application. Used for generating the help text.
    /// </summary>
    [PublicAPI]
    public string? AppDescription { get; init; }

    /// <summary>
    /// Whether to automatically add a debug parameter (<c>--debug</c>/<c>-d</c>) to this application.
    /// Will only be added if at least one of the parameter names is not in use.
    /// </summary>
    [PublicAPI]
    public bool EnableGlobalDebugParam { get; set; } = true;

    /// <inheritdoc />
    protected sealed override CliApplicationExecutor MainExecutor => new(Execute);

    /// <summary>
    /// The main <see cref="CliCommand"/> for this application.
    /// </summary>
    public CliCommand Command
    {
        get => this._command ?? throw new InvalidOperationException($"The property '{nameof(this.Command)}' has never been set.");
        init => this._command = value;
    }

    private readonly CliCommand? _command;

    /// <summary>
    /// Constructor without setting <see cref="Command"/>. You need to set this property manually.
    /// </summary>
    public CliApplicationWithCommand()
    {
    }

    /// <summary>
    /// Constructor that sets <see cref="Command"/>.
    /// </summary>
    /// <param name="command">The value for <see cref="Command"/></param>
    public CliApplicationWithCommand(CliCommand command)
    {
        Validate.ArgumentWithName(nameof(command)).IsNotNull(command);

        this.Command = command;
    }

    private async Task<int> Execute(string[] args, CancellationToken cancellationToken)
    {
        var commandHandler = new CliCommand.CliCommandHandler(this.Command, this.EnableGlobalDebugParam, this.Terminal, cancellationToken);

        return await RootCommandInvoker.InvokeRootCommand(
            this.AppDescription,
            commandHandler.AllParams.Select(p => p.UnderlyingImplementation),
            commandHandler,
            this.Terminal,
            args,
            ProcessUnhandledException
        )
            .ConfigureAwait(continueOnCapturedContext: false);
    }
}
