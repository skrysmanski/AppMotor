// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.AppBuilding;
using AppMotor.TestCore.Logging;

using Microsoft.Extensions.Hosting;

using Xunit.Abstractions;

namespace AppMotor.TestCore.AppBuilding;

/// <summary>
/// A variation of <see cref="DefaultHostBuilderFactory"/> for use in XUnit tests. Redirects all logging to XUnit's
/// <see cref="ITestOutputHelper"/>.
/// </summary>
public class XUnitHostBuilderFactory : DefaultHostBuilderFactory
{
    private readonly ITestOutputHelper _testOutputHelper;

    /// <summary>
    /// Constructor.
    /// </summary>
    public XUnitHostBuilderFactory(ITestOutputHelper testOutputHelper)
    {
        this._testOutputHelper = testOutputHelper;
        this.DefaultLogger = DefaultLogger.None;
    }

    /// <inheritdoc />
    public override IHostBuilder CreateHostBuilder()
    {
        var hostBuilder = base.CreateHostBuilder();

        hostBuilder.ConfigureLogging(loggingBuilder => loggingBuilder.AddXUnitLogger(this._testOutputHelper));

        return hostBuilder;
    }
}
