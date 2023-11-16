// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Collections.Immutable;

using AppMotor.CliApp.CommandLine.Utils;

using JetBrains.Annotations;

namespace AppMotor.CliApp.CommandLine;

/// <summary>
/// Represents a command line application with automatic command line argument parsing that bundles various
/// functions (via <see cref="CliVerb"/>s) - like the <c>git</c> or <c>dotnet</c> commands. If you need an
/// application that only does one thing, use <see cref="CliApplicationWithParams"/> or <see cref="CliApplicationWithCommand"/>
/// instead.
/// </summary>
/// <remarks>
/// Sub classes cannot have parameters (<see cref="CliParam{T}"/>) of their own. Parameter can only exist
/// on the commands.
/// </remarks>
public class CliApplicationWithVerbs : CliApplication
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

    /// <summary>
    /// The verbs of this application.
    /// </summary>
    // NOTE: The type of this property is not ImmutableArray on purpose - because you can't initialize
    //   ImmutableArray with an array initializer (i.e. "new[] { ... }") - which is what we want for ease of use.
    [PublicAPI]
    public IReadOnlyList<CliVerb> Verbs
    {
        get => this._verbs ?? throw new InvalidOperationException($"The property '{nameof(this.Verbs)}' has never been set.");
        init => this._verbs = value.ToImmutableArray();
    }

    private readonly ImmutableArray<CliVerb>? _verbs;

    /// <inheritdoc />
    protected sealed override CliApplicationExecutor MainExecutor => new(Execute);

    /// <inheritdoc />
    public CliApplicationWithVerbs(IReadOnlyCollection<CliVerb>? verbs = null)
    {
        if (verbs?.Count > 0)
        {
            this._verbs = verbs.ToImmutableArray();
        }
    }

    private async Task<int> Execute(string[] args, CancellationToken cancellationToken)
    {
        if (this.Verbs.Count == 0)
        {
            throw new InvalidOperationException($"No verbs have be defined in property '{nameof(this.Verbs)}'.");
        }

        return await RootCommandInvoker.InvokeRootCommandAsync(
            this.AppDescription,
            this.Verbs.Select(verb => verb.ToUnderlyingImplementation(enableDebugParam: this.EnableGlobalDebugParam, this.Terminal, cancellationToken)),
            commandHandler: null,
            this.Terminal,
            args: SortHelpFirst(args),
            ProcessUnhandledException
        )
            .ConfigureAwait(continueOnCapturedContext: false);
    }

    /// <summary>
    /// The "System.Commandline" library only supports the help parameter as first parameter;
    /// e.g. "myapp --help mycommand" is supported but "myapp mycommand --help" is not. Thus, we
    /// simply move the help parameter to the front, if there is one in the args.
    /// </summary>
    [MustUseReturnValue]
    private static string[] SortHelpFirst(string[] args)
    {
        if (args.Length > 0 && HelpParamUtils.IsHelpParamName(args[0]))
        {
            // Help param is already first param. Nothing to do.
            return args;
        }

        if (args.Length > 0 && string.Equals(args[0], HelpParamUtils.HelpCommandName, StringComparison.OrdinalIgnoreCase))
        {
            // Usage of virtual "help" command
            var newArgs = new List<string>(args.Length);
            newArgs.Add(HelpParamUtils.DefaultHelpParamName);
            newArgs.AddRange(args[1..]);
            return newArgs.ToArray();
        }

        bool hasHelpParam = args.Any(HelpParamUtils.IsHelpParamName);
        if (!hasHelpParam)
        {
            // No help param used. Nothing to do.
            return args;
        }
        else
        {
            // Set first arg to the help param and add the others afterwards.
            var newArgs = new List<string>(args.Length);
            newArgs.Add(HelpParamUtils.DefaultHelpParamName);
            newArgs.AddRange(args.Where(arg => !HelpParamUtils.IsHelpParamName(arg)));
            return newArgs.ToArray();
        }
    }
}
