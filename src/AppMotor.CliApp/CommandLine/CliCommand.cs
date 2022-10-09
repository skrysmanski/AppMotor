// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Collections.Immutable;
using System.CommandLine.Invocation;
using System.Diagnostics.CodeAnalysis;

using AppMotor.CliApp.AppBuilding;
using AppMotor.CliApp.CommandLine.Utils;
using AppMotor.CliApp.Properties;
using AppMotor.CliApp.Terminals;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AppMotor.CliApp.CommandLine;

/// <summary>
/// <para>Represents a command in a command line call; e.g. in <c>git add file.txt</c> the word "add" is the command.
/// The command's name is defined by wrapping the command instance in a <see cref="CliVerb"/> instance.</para>
///
/// <para>This class also provides access to .NET's Generic Host functionality (for more details, see
/// https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host) - via
/// <see cref="ConfigureServices"/>, <see cref="ConfigureApplication"/>, <see cref="CreateHostBuilder"/>,
/// and <see cref="Services"/>.</para>
///
/// <para>If you need a command that *just* runs one or more services but doesn't need a "main" method
/// (i.e. <see cref="Executor"/>), use <see cref="ServiceHostCliCommand"/> instead.</para>
/// </summary>
/// <remarks>
/// You can use this class as root command with <see cref="CliApplicationWithCommand"/> or as a verb with
/// <see cref="CliApplicationWithVerbs"/>.
/// </remarks>
public abstract class CliCommand
{
    /// <summary>
    /// The help text for this command.
    /// </summary>
    public virtual string? HelpText => null;

    /// <summary>
    /// The main/execute method for this command. Implementations can access all command line parameters though
    /// the <see cref="CliParam{T}.Value"/> properties of this <see cref="CliParam{T}"/> instance.
    ///
    /// <para>Recommendation: For ease of use, use the array syntax (<c>=&gt;</c>) when implementing
    /// this property.</para>
    /// </summary>
    protected abstract CliCommandExecutor Executor { get; }

    /// <summary>
    /// The terminal to use within this application. "Inherited" from the <see cref="CliApplication"/> this
    /// command runs in. Only available from within <see cref="Executor"/>. (This is also why this property is <c>protected</c>
    /// rather than <c>public</c>.)
    /// </summary>
    /// <remarks>
    /// This property mainly exists for unit testing purposes where you need to obtain
    /// everything written to the terminal.
    /// </remarks>
    protected ITerminal Terminal => this._terminal ?? throw new InvalidOperationException("The terminal is not available in this command state.");

    private ITerminal? _terminal;

    /// <summary>
    /// The registered services (i.e. dependency injection). Only available from within <see cref="Executor"/>.
    /// (This is also why this property is <c>protected</c> rather than <c>public</c>.)
    /// </summary>
    protected IServiceProvider Services => this._serviceProvider ?? throw new InvalidOperationException("This ServiceProvider is not available in this command state.");

    private IServiceProvider? _serviceProvider;

    /// <summary>
    /// Runs this command.
    /// </summary>
    /// <returns>The exit code for the running program.</returns>
    private async Task<int> Execute(ITerminal terminal, CancellationToken cancellationToken)
    {
        IHostBuilder hostBuilder = CreateHostBuilder();

        hostBuilder.ConfigureServices(services =>
        {
            services.AddSingleton<IHostLifetime, NullLifetime>();

            // Required for the terminal logger
            services.AddSingleton<ITerminalOutput>(terminal);
        });

        hostBuilder.ConfigureServices(ConfigureServices);

        ConfigureApplication(hostBuilder);

        IHost host = hostBuilder.Build();

        this._terminal = terminal;
        this._serviceProvider = host.Services;

        try
        {
            await host.StartAsync(cancellationToken).ConfigureAwait(false);

            var exitCode = await this.Executor.Execute(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);

            // IMPORTANT: Don't pass "cancellationToken" here because it may have already been canceled and we don't
            //   want to cancel "StopAsync" in this case.
            await host.StopAsync(CancellationToken.None).ConfigureAwait(false);

            return exitCode;
        }
        finally
        {
            await DisposeHelper.DisposeWithAsyncSupport(host).ConfigureAwait(false);
            this._terminal = null;
            this._serviceProvider = null;
        }
    }

    /// <summary>
    /// Returns all parameters defined for this command. The default implementation uses reflection to find all properties
    /// and fields of type <see cref="CliParamBase"/>. Inheritors may override this method either to filter its result or provide
    /// their own list.
    /// </summary>
    protected virtual IEnumerable<CliParamBase> GetAllParams()
    {
        return CliParamUtils.GetAllParamsFor(this);
    }

    /// <summary>
    /// Creates the <see cref="IHostBuilder"/> to be used by this command. The default implementation
    /// uses <see cref="DefaultHostBuilderFactory.CreateHostBuilder"/>.
    /// </summary>
    /// <remarks>
    /// Note to implementers: DO NOT configure any required services/features here (use <see cref="ConfigureApplication"/> for that).
    /// Reason: If this method is overridden, the override often does NOT call the base implementation but creates its own
    /// <see cref="IHostBuilder"/>.
    /// </remarks>
    /// <remarks>
    /// Since <see cref="CliCommand"/>s have their own command line parameter parsing, this method doesn't
    /// get the command line parameters as parameter (for example to pass to <see cref="Host.CreateDefaultBuilder(string[])"/>).
    /// You can still get them via <see cref="Environment.GetCommandLineArgs"/> if you really want.
    /// </remarks>
    [MustUseReturnValue]
    protected virtual IHostBuilder CreateHostBuilder()
    {
        return DefaultHostBuilderFactory.Instance.CreateHostBuilder();
    }

    /// <summary>
    /// Registers all services with the dependency injection framework.
    ///
    /// <para>Note: Use <see cref="ServiceCollectionHostedServiceExtensions.AddHostedService{THostedService}(IServiceCollection)"/>
    /// to register server class (that implement <see cref="IHostedService"/>).</para>
    /// </summary>
    /// <remarks>
    /// If you need to configure the application itself, you can use <see cref="ConfigureApplication"/>.
    /// </remarks>
    /// <remarks>
    /// For more details, see: https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection
    /// </remarks>
    [PublicAPI]
    protected virtual void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        // Does nothing by default.
    }

    /// <summary>
    /// Adds additional configuration to the application itself (other than registering services with is done
    /// by <see cref="ConfigureServices"/> instead) - via extension methods to <paramref name="hostBuilder"/>.
    /// </summary>
    /// <remarks>
    /// Use this method to register additional configuration providers. For more details, see:
    /// https://github.com/skrysmanski/dotnet-docs/blob/main/README.Configuration.md
    /// </remarks>
    /// <remarks>
    /// Use this method to configure logging for your application. For more details, see:
    /// https://learn.microsoft.com/en-us/dotnet/core/extensions/logging
    /// </remarks>
    /// <remarks>
    /// Note to implementers: Use this method instead of <see cref="CreateHostBuilder"/> to configure required
    /// features. See remarks <see cref="CreateHostBuilder"/> for more details.
    /// </remarks>
    [PublicAPI]
    protected virtual void ConfigureApplication(IHostBuilder hostBuilder)
    {
        // Does nothing by default.
    }

    internal sealed class CliCommandHandler : ICommandHandler
    {
        private readonly CliCommand _command;

        private readonly ITerminal _terminal;

        private readonly CancellationToken _cancellationToken;

        public ImmutableArray<CliParamBase> AllParams { get; }

        private CliParam<bool>? DebugParam { get; }

        public CliCommandHandler(CliCommand command, bool enableDebugParam, ITerminal terminal, CancellationToken cancellationToken)
        {
            this._command = command;
            this._terminal = terminal;
            this._cancellationToken = cancellationToken;

            var paramsCollectionBuilder = new ParamsCollectionBuilder();

            foreach (var cliParam in command.GetAllParams())
            {
                paramsCollectionBuilder.AddParam(cliParam);
            }

            if (enableDebugParam)
            {
                var availableAliases = new List<string>();

                if (!paramsCollectionBuilder.RegisteredAliases.Contains("--debug"))
                {
                    availableAliases.Add("--debug");
                }

                if (!paramsCollectionBuilder.RegisteredAliases.Contains("-d"))
                {
                    availableAliases.Add("-d");
                }

                if (availableAliases.Count > 0)
                {
                    this.DebugParam = new(availableAliases)
                    {
                        HelpText = LocalizableResources.DebugParamHelpText,
                    };

                    paramsCollectionBuilder.AddParam(this.DebugParam);
                }
            }

            this.AllParams = paramsCollectionBuilder.Build();
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public int Invoke(InvocationContext context)
        {
            throw new NotSupportedException("This call is not implemented as it's not needed.");
        }

        /// <inheritdoc />
        public async Task<int> InvokeAsync(InvocationContext context)
        {
            foreach (var cliParam in this.AllParams)
            {
                cliParam.SetValueFromParseResult(context.ParseResult);
            }

            if (this.DebugParam?.Value == true && !DebuggerUtils.IsDebuggerAttached)
            {
                DebuggerUtils.LaunchDebugger(this._terminal);
            }

            return await this._command.Execute(this._terminal, this._cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
        }
    }

    /// <summary>
    /// This implementation replaces <see cref="Microsoft.Extensions.Hosting.Internal.ConsoleLifetime"/>
    /// which is usually used by <see cref="HostBuilder"/>. This lifetime prints the various log messages and
    /// registers the Ctrl+C handler. This NullLifetime doesn't do anything of this. Note
    /// that <see cref="ServiceHostCliCommand"/> still uses <c>ConsoleLifetime</c>.
    /// </summary>
    private sealed class NullLifetime : IHostLifetime
    {
        /// <inheritdoc />
        public Task WaitForStartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
