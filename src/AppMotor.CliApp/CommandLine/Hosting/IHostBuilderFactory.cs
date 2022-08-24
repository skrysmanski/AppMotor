// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

using Microsoft.Extensions.Hosting;

namespace AppMotor.CliApp.CommandLine.Hosting;

/// <summary>
/// Factory for <see cref="IHostBuilder"/>.
/// </summary>
/// <remarks>
/// The main use case behind this interface is to allow users to define the <see cref="IHostBuilder"/>
/// they want to use with <see cref="GenericHostCliCommand"/>. Having an interface here (instead of
/// just a factory method) gives implementers more freedom in how they want to make the host builder
/// configurable (e.g. something like <see cref="DefaultHostBuilderFactory"/> would not be possible
/// with a factory method).
/// </remarks>
public interface IHostBuilderFactory
{
    /// <summary>
    /// Creates a new <see cref="IHostBuilder"/> instance.
    /// </summary>
    [MustUseReturnValue]
    IHostBuilder CreateHostBuilder();
}