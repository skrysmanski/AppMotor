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

using AppMotor.Core.Utils;

using JetBrains.Annotations;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AppMotor.CliApp.CommandLine.Hosting;

/// <summary>
/// Configures the log levels of an application. Instances of this class are usually used with
/// <see cref="DefaultHostBuilderFactory.LogLevelConfiguration"/>.
/// </summary>
public sealed class LogLevelConfiguration
{
    private const string DEFAULT_CATEGORY_NAME = "Default";

    /// <summary>
    /// The default log level to use.
    /// </summary>
    [PublicAPI]
    public LogLevel DefaultLogLevel { get; }

    /// <summary>
    /// The log levels for log categories. For information on how to specify log categories,
    /// see: https://docs.microsoft.com/en-us/dotnet/core/extensions/logging#log-category
    /// </summary>
    [PublicAPI]
    public Dictionary<string, LogLevel> LogCategoryLevels { get; init; } = new();

    /// <summary>
    /// Constructor.
    /// </summary>
    public LogLevelConfiguration(LogLevel defaultLogLevel)
    {
        this.DefaultLogLevel = defaultLogLevel;
    }

    /// <summary>
    /// Applies this configuration to a <see cref="HostBuilderContext"/>.
    /// </summary>
    /// <param name="context">The context of a host builder</param>
    /// <param name="loggingConfigurationSectionName">The configuration section name for the logging section; see
    /// <see cref="DefaultHostBuilderFactory.LoggingConfigurationSectionName"/> for more details.</param>
    public void ApplyToHostBuilder(HostBuilderContext context, string loggingConfigurationSectionName = DefaultHostBuilderFactory.DEFAULT_LOGGING_CONFIGURATION_SECTION_NAME)
    {
        Validate.ArgumentWithName(nameof(loggingConfigurationSectionName)).IsNotNullOrWhiteSpace(loggingConfigurationSectionName);

        context.Configuration[$"{loggingConfigurationSectionName}:LogLevel:{DEFAULT_CATEGORY_NAME}"] = this.DefaultLogLevel.ToString();

        foreach (var (logCategory, logLevel) in this.LogCategoryLevels)
        {
            if (string.IsNullOrWhiteSpace(logCategory))
            {
                throw new InvalidOperationException("Log category names must not be empty.");
            }

            if (logCategory.Equals(DEFAULT_CATEGORY_NAME, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"The log category name '{DEFAULT_CATEGORY_NAME}' is reserved and can't be used.");
            }

            context.Configuration[$"{loggingConfigurationSectionName}:LogLevel:{logCategory}"] = logLevel.ToString();
        }
    }
}