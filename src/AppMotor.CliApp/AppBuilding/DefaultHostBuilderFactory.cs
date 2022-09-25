// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.CommandLine;
using AppMotor.CliApp.Logging;
using AppMotor.Core.IO;

using JetBrains.Annotations;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AppMotor.CliApp.AppBuilding;

/// <summary>
/// <para>A default <see cref="IHostBuilder"/> factory implementation for AppMotor applications. Lets you customize the
/// host by setting the various properties in this class (or even by overriding <see cref="CreateHostBuilder"/>).</para>
///
/// <para>Usually used for the <see cref="CliCommand.CreateHostBuilder"/> property.</para>
///
/// <para>By default, this factory creates hosts with the following features enabled:</para>
///
/// <list type="bullet">
///     <item><description>Dependency injection (via <see cref="CreateServiceProviderFactory"/>)</description></item>
///     <item><description>Configuration values loaded from "appsettings.json", "appsettings.{Env}.json" and the environment variables (via <see cref="AppConfigurationProvider"/>)</description></item>
///     <item><description>Logging to the Console (via <see cref="LoggingConfigurationProvider"/>)</description></item>
///     <item><description>Logging configuration via the "Logging" section (via <see cref="LoggingConfigurationSectionName"/>)</description></item>
///     <item><description>The content root is set to the current directory (via <see cref="ContentRoot"/>)</description></item>
/// </list>
/// </summary>
/// <remarks>
/// For more possibilities, see <see cref="Host.CreateDefaultBuilder(string[])"/>.
/// </remarks>
public class DefaultHostBuilderFactory
{
    /// <summary>
    /// An instance of this class with all the default settings (can't be changed afterwards).
    /// </summary>
    internal static DefaultHostBuilderFactory Instance { get; } = new();

    /// <summary>
    /// Configures the configuration providers (e.g. settings files) that provide configuration values for the application. Defaults to
    /// <see cref="ApplyDefaultAppConfiguration"/>.
    /// </summary>
    /// <remarks>
    /// For more details, see: https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration
    /// </remarks>
    [PublicAPI]
    public Action<HostBuilderContext, IConfigurationBuilder>? AppConfigurationProvider { get; init; } = ApplyDefaultAppConfiguration;

    /// <summary>
    /// The name of the configuration section (<see cref="IConfiguration.GetSection"/>) used to configure log levels, etc. for
    /// all loggers that are enabled via <see cref="LoggingConfigurationProvider"/>. Defaults to "Logging" (the .NET default).
    /// Can be set to <c>null</c> to completely disable the ability to configure logging.
    /// </summary>
    /// <remarks>
    /// For more details, see: https://docs.microsoft.com/en-us/dotnet/core/extensions/logging#configure-logging
    /// </remarks>
    [PublicAPI]
    public string? LoggingConfigurationSectionName { get; init; } = LogLevelConfiguration.DEFAULT_LOGGING_CONFIGURATION_SECTION_NAME;

    /// <summary>
    /// Allows you to programatically set the log levels for the application. If <c>null</c>, the application's
    /// configuration (file) is used.
    /// </summary>
    /// <remarks>
    /// This configuration does not allow you to set different log levels for different log providers (e.g. console,
    /// Windows event log, ...). The log levels apply to all log providers. If you need to configure different
    /// log levels for different log providers, you'll need to use a config file (or other config source).
    /// </remarks>
    public LogLevelConfiguration? LogLevelConfiguration { get; init; }

    /// <summary>
    /// Configures the logging for the application. You can use the various <c>loggingBuilder.Add...()</c>
    /// methods to configure the desired logging. Defaults to <see cref="ApplyDefaultLoggingConfiguration"/>.
    /// Note that the configuration section for configuring the log levels etc. is specified via
    /// <see cref="LoggingConfigurationSectionName"/>.
    /// </summary>
    /// <remarks>
    /// For more details, see https://docs.microsoft.com/en-us/dotnet/core/extensions/logging-providers and
    /// https://docs.microsoft.com/en-us/dotnet/core/extensions/console-log-formatter
    /// </remarks>
    [PublicAPI]
    public Action<HostBuilderContext, ILoggingBuilder>? LoggingConfigurationProvider { get; init; } = ApplyDefaultLoggingConfiguration;

    /// <summary>
    /// The content root to use. Defaults to <see cref="DirectoryPath.GetCurrentDirectory"/>. Can later be accessed
    /// via <see cref="IHostEnvironment.ContentRootFileProvider"/>. Can be <c>null</c> in which case no content root
    /// will be set (explicitly). Used, among other things, to locate the "appsettings.json" files.
    /// </summary>
    /// <remarks>
    /// For more details on the content root, see: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/#content-root
    /// </remarks>
    /// <seealso cref="HostingHostBuilderExtensions.UseContentRoot"/>
    [PublicAPI]
    public DirectoryPath? ContentRoot { get; init; } = DirectoryPath.GetCurrentDirectory();

    /// <summary>
    /// Creates a new <see cref="IHostBuilder"/> instance.
    /// </summary>
    [MustUseReturnValue]
    public virtual IHostBuilder CreateHostBuilder()
    {
        var hostBuilder = new HostBuilder();

        var contentRoot = this.ContentRoot;
        if (contentRoot is not null)
        {
            hostBuilder.UseContentRoot(contentRoot.Value.Value);
        }

        hostBuilder.UseServiceProviderFactory(CreateServiceProviderFactory);

        if (this.AppConfigurationProvider is not null)
        {
            hostBuilder.ConfigureAppConfiguration(this.AppConfigurationProvider);
        }

        if (this.LoggingConfigurationSectionName is not null)
        {
            hostBuilder.ConfigureLogging((context, loggingBuilder) =>
            {
                // Load the logging configuration from the specified configuration section.
                loggingBuilder.AddConfiguration(context.Configuration.GetSection(this.LoggingConfigurationSectionName));
            });
        }

        if (this.LogLevelConfiguration is not null)
        {
            if (string.IsNullOrWhiteSpace(this.LoggingConfigurationSectionName))
            {
                throw new InvalidOperationException($"Can't apply property '{nameof(this.LogLevelConfiguration)}' when property '{nameof(this.LoggingConfigurationSectionName)}' is empty.");
            }

            hostBuilder.ConfigureLogging(
                (ctx, _) => this.LogLevelConfiguration.ApplyToHostBuilder(ctx, loggingConfigurationSectionName: this.LoggingConfigurationSectionName)
            );
        }

        if (this.LoggingConfigurationProvider is not null)
        {
            hostBuilder.ConfigureLogging(this.LoggingConfigurationProvider);
        }

        return hostBuilder;
    }

    /// <summary>
    /// Creates the <see cref="IServiceProviderFactory{TContainerBuilder}"/> (i.e. the dependency injection system) to be used.
    /// The default implementation uses <see cref="DefaultServiceProviderFactory"/> with all scope validations enabled
    /// (see <see cref="ServiceProviderOptions.ValidateScopes"/>).
    /// </summary>
    /// <remarks>
    /// <para>For more details, see: https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection </para>
    ///
    /// <para>This implementation is fixed on <see cref="IServiceCollection"/> as DI container. If you need a different
    /// DI container, call <see cref="IHostBuilder.UseServiceProviderFactory{TContainerBuilder}(IServiceProviderFactory{TContainerBuilder})"/>
    /// after the builder has been created by <see cref="CreateHostBuilder"/>.</para>
    /// </remarks>
    [PublicAPI, MustUseReturnValue]
    protected virtual IServiceProviderFactory<IServiceCollection> CreateServiceProviderFactory(HostBuilderContext hostBuilderContext)
    {
        var options = new ServiceProviderOptions()
        {
            // Enable all validations
            ValidateScopes = true,
            ValidateOnBuild = true,
        };

        return new DefaultServiceProviderFactory(options);
    }

    /// <summary>
    /// Enables the configuration files "appsettings.json" and "appsettings.{<see cref="HostBuilderContext.HostingEnvironment"/>}.json".
    /// Also enables loading configuration values from the environment variables (via <see cref="EnvironmentVariablesConfigurationSource"/>).
    /// </summary>
    /// <remarks>
    /// Whether the .json configuration files are reloaded when changed is configured via the "hostBuilder:reloadConfigOnChange" configuration
    /// value. The default is <c>true</c>.
    /// </remarks>
    /// <seealso cref="AppConfigurationProvider"/>
    [PublicAPI]
    public static void ApplyDefaultAppConfiguration(HostBuilderContext context, IConfigurationBuilder configurationBuilder)
    {
        IHostEnvironment env = context.HostingEnvironment;

        bool reloadOnChange = context.Configuration.GetValue("hostBuilder:reloadConfigOnChange", defaultValue: true);

        configurationBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: reloadOnChange);
        configurationBuilder.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: reloadOnChange);

        configurationBuilder.Add(new EnvironmentVariablesConfigurationSource());
    }

    /// <summary>
    /// Enables Console logging.
    /// </summary>
    /// <seealso cref="LoggingConfigurationProvider"/>
    /// <seealso cref="LoggingConfigurationSectionName"/>
    [PublicAPI]
    public static void ApplyDefaultLoggingConfiguration(HostBuilderContext context, ILoggingBuilder loggingBuilder)
    {
        loggingBuilder.AddTerminalLogger();
    }
}
