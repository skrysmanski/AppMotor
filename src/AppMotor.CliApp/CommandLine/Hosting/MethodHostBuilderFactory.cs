// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using Microsoft.Extensions.Hosting;

namespace AppMotor.CliApp.CommandLine.Hosting;

/// <summary>
/// A <see cref="IHostBuilderFactory"/> wrapper for a factory method/function.
/// </summary>
public class MethodHostBuilderFactory : IHostBuilderFactory
{
    private readonly Func<IHostBuilder> _factoryFunc;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="factoryFunc">The factory method/function</param>
    public MethodHostBuilderFactory(Func<IHostBuilder> factoryFunc)
    {
        this._factoryFunc = factoryFunc;
    }

    /// <inheritdoc />
    public IHostBuilder CreateHostBuilder()
    {
        return this._factoryFunc();
    }
}