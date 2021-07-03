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

using System;

using AppMotor.Core.IO;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AppMotor.CliApp.CommandLine.Hosting
{
    /// <summary>
    /// <para>The default <see cref="IHostBuilderFactory"/> implementation. Lets you customize the host by
    /// setting the various properties in this class (or even by overriding <see cref="CreateHostBuilder"/>).</para>
    ///
    /// <para>By default, this factory creates hosts with the following features enabled:</para>
    ///
    /// <list type="bullet">
    ///     <item><description>Dependency injection (via <see cref="ServiceProviderFactory"/>)</description></item>
    ///     <item><description>Logging to the Console (via <see cref="LoggingConfiguration"/>)</description></item>
    ///     <item><description>The content root is set to the current directory (via <see cref="ContentRoot"/>)</description></item>
    /// </list>
    /// </summary>
    /// <remarks>
    /// For more possibilities, see <see cref="Host.CreateDefaultBuilder(string[])"/>.
    /// </remarks>
    /// <seealso cref="MethodHostBuilderFactory"/>
    public class DefaultHostBuilderFactory : IHostBuilderFactory
    {
        /// <summary>
        /// An instance of this class.
        /// </summary>
        public static DefaultHostBuilderFactory Instance { get; } = new();

        /// <summary>
        /// The function that creates the <see cref="IServiceProviderFactory{TContainerBuilder}"/> (i.e. the dependency
        /// injection system). Defaults to <see cref="CreateDefaultServiceProviderFactory"/>.
        /// </summary>
        [PublicAPI]
        public Func<HostBuilderContext, IServiceProviderFactory<IServiceCollection>> ServiceProviderFactory { get; init; } = CreateDefaultServiceProviderFactory;

        /// <summary>
        /// Configures the logging for the application. You can use the various <c>loggingBuilder.Add...()</c>
        /// methods to configure the desired logging. Defaults to <see cref="ConfigureDefaultLogging"/>.
        /// </summary>
        [PublicAPI]
        public Action<HostBuilderContext, ILoggingBuilder> LoggingConfiguration { get; init; } = ConfigureDefaultLogging;

        /// <summary>
        /// The content root to use. Defaults to <see cref="DirectoryPath.GetCurrentDirectory"/>.
        /// Can <c>null</c> in which case no content root will be set (explicitly).
        /// </summary>
        /// <remarks>
        /// For more details on the content root, see: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/#content-root
        /// </remarks>
        /// <seealso cref="HostingHostBuilderExtensions.UseContentRoot"/>
        [PublicAPI]
        public DirectoryPath? ContentRoot { get; init; } = DirectoryPath.GetCurrentDirectory();

        /// <inheritdoc />
        public virtual IHostBuilder CreateHostBuilder()
        {
            var hostBuilder = new HostBuilder();

            var contentRoot = this.ContentRoot;
            if (contentRoot is not null)
            {
                hostBuilder.UseContentRoot(contentRoot.Value.Value);
            }

            hostBuilder.UseServiceProviderFactory(this.ServiceProviderFactory);
            hostBuilder.ConfigureLogging(this.LoggingConfiguration);

            return hostBuilder;
        }

        /// <summary>
        /// Creates a <see cref="IServiceProvider"/> factory (i.e. the dependency injection framework) from .NET's built-in service provider
        /// (via <see cref="DefaultServiceProviderFactory"/>) with all scope validations enabled (see <see cref="ServiceProviderOptions.ValidateScopes"/>).
        /// </summary>
        [PublicAPI]
        public static IServiceProviderFactory<IServiceCollection> CreateDefaultServiceProviderFactory(HostBuilderContext context)
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
        /// Enables Console logging.
        /// </summary>
        [PublicAPI]
        public static void ConfigureDefaultLogging(HostBuilderContext context, ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.AddConsole();
        }
    }
}
