// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.Logging.Formatters;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace AppMotor.CliApp.Logging;

/// <summary>
/// Appplication build extensions for <see cref="TerminalLogger"/>.
/// </summary>
public static class TerminalLoggerExtensions
{
    // Implementation based on: https://github.com/dotnet/runtime/blob/v6.0.9/src/libraries/Microsoft.Extensions.Logging.Console/src/ConsoleLoggerExtensions.cs

    private const string FORMATTER_OPTIONS_SECTION_NAME = "FormatterOptions";

    /// <summary>
    /// Adds a terminal logger named 'Terminal' to the factory.
    /// </summary>
    [PublicAPI]
    public static ILoggingBuilder AddTerminalLogger(this ILoggingBuilder loggingBuilder)
    {
        // Register basic logging configuration.
        loggingBuilder.AddConfiguration();

        // Register default formatters.
        loggingBuilder.AddTerminalLogEntryFormatter<JsonTerminalFormatter, JsonConsoleFormatterOptions>();
        loggingBuilder.AddTerminalLogEntryFormatter<SystemdTerminalFormatter, ConsoleFormatterOptions>();
        loggingBuilder.AddTerminalLogEntryFormatter<SimpleTerminalFormatter, SimpleConsoleFormatterOptions>();

        // Register TerminalLoggerProvider as logger provider.
        loggingBuilder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, TerminalLoggerProvider>());

        // Make logging options available.
        LoggerProviderOptions.RegisterProviderOptions<ConsoleLoggerOptions, TerminalLoggerProvider>(loggingBuilder.Services);

        return loggingBuilder;
    }

    /// <summary>
    /// Registers a <see cref="ITerminalLogEntryFormatter"/> and specifies that it's to be configured with <typeparamref name="TOptions"/> as
    /// options.
    /// </summary>
    [PublicAPI]
    public static ILoggingBuilder AddTerminalLogEntryFormatter<TFormatter, TOptions>(this ILoggingBuilder loggingBuilder)
        where TOptions : ConsoleFormatterOptions
        where TFormatter : class, ITerminalLogEntryFormatter
    {
        // Register basic logging configuration.
        loggingBuilder.AddConfiguration();

        // Register "TFormatter" as an additional "ITerminalLogEntryFormatter"
        loggingBuilder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ITerminalLogEntryFormatter, TFormatter>());

        // Make formatter options available for the formatter.
        loggingBuilder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<TOptions>, FormatterOptionsProvider<TOptions>>());
        loggingBuilder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IOptionsChangeTokenSource<TOptions>, FormatterOptionsChangeTokenSource<TOptions>>());

        return loggingBuilder;
    }

    /// <summary>
    /// Makes <typeparamref name="TOptions"/> available to .NET's options system and thus to the dependency injection system. The options
    /// are expected under "Logging::Console::FormatterOptions" (as defined by the <see cref="ProviderAliasAttribute"/> on
    /// <see cref="TerminalLoggerProvider"/> and <see cref="FORMATTER_OPTIONS_SECTION_NAME"/>).
    /// </summary>
    /// <remarks>
    /// The ability to reload/reread the options is provided through <see cref="FormatterOptionsChangeTokenSource{TOptions}"/>.
    /// </remarks>
    private sealed class FormatterOptionsProvider<TOptions> : ConfigureFromConfigurationOptions<TOptions>
        where TOptions : ConsoleFormatterOptions
    {
        public FormatterOptionsProvider(ILoggerProviderConfiguration<TerminalLoggerProvider> providerConfiguration)
            : base(providerConfiguration.Configuration.GetSection(FORMATTER_OPTIONS_SECTION_NAME))
        {
        }
    }

    /// <summary>
    /// Makes <typeparamref name="TOptions"/> instances reloadable/rereadable (i.e. via injecting <see cref="IOptionsMonitor{TOptions}"/>).
    /// </summary>
    /// <remarks>
    /// This class only works if <typeparamref name="TOptions"/> is registered with .NET's options system via <see cref="FormatterOptionsProvider{TOptions}"/>.
    /// </remarks>
    private sealed class FormatterOptionsChangeTokenSource<TOptions> : ConfigurationChangeTokenSource<TOptions>
        where TOptions : ConsoleFormatterOptions
    {
        public FormatterOptionsChangeTokenSource(ILoggerProviderConfiguration<TerminalLoggerProvider> providerConfiguration)
            : base(providerConfiguration.Configuration.GetSection(FORMATTER_OPTIONS_SECTION_NAME))
        {
        }
    }
}
