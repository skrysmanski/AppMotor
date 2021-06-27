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

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AppMotor.CliApp.CommandLine.Hosting
{
    /// <summary>
    /// The default <see cref="IHostBuilderFactory"/>. Lets you customize the <see cref="IServiceProvider"/>
    /// (by overriding <see cref="CreateServiceProviderFactory"/>), the logging settings (by overriding
    /// <see cref="ConfigureLogging"/>) and everything else (by overriding <see cref="CreateHostBuilder"/>).
    /// </summary>
    /// <seealso cref="MethodHostBuilderFactory"/>
    public class DefaultHostBuilderFactory : IHostBuilderFactory
    {
        /// <summary>
        /// An instance of this class.
        /// </summary>
        public static DefaultHostBuilderFactory Instance { get; } = new();

        /// <inheritdoc />
        public virtual IHostBuilder CreateHostBuilder()
        {
            var hostBuilder = new HostBuilder();

            hostBuilder.UseServiceProviderFactory(CreateServiceProviderFactory);
            hostBuilder.ConfigureLogging(ConfigureLogging);

            return hostBuilder;
        }

        /// <summary>
        /// Create the <see cref="IServiceProvider"/> factory (i.e. the dependency injection framework) to be used by
        /// the application.
        ///
        /// <para>The default implementation uses the built-in service provider (via <see cref="DefaultServiceProviderFactory"/>)
        /// with scope validation enabled (see <see cref="ServiceProviderOptions.ValidateScopes"/>).</para>
        /// </summary>
        [PublicAPI]
        protected virtual IServiceProviderFactory<IServiceCollection> CreateServiceProviderFactory(HostBuilderContext context)
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
        /// Configures the logging for the application. You can use the various <c>loggingBuilder.Add...()</c>
        /// methods to configure the desired logging.
        ///
        /// <para>The default implementation only enables console logging.</para>
        /// </summary>
        protected virtual void ConfigureLogging(HostBuilderContext context, ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.AddConsole();
        }
    }
}
