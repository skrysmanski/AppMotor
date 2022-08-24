// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.CommandLine.Hosting;

using Microsoft.Extensions.Hosting;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.Tests.CommandLine.Hosting;

public sealed class MethodHostBuilderFactoryTests
{
    [Fact]
    public void TestCreateHostBuilder()
    {
        // Setup
        bool called = false;
        HostBuilder? createdHostBuilder = null;

        var factory = new MethodHostBuilderFactory(() =>
        {
            called = true;
            createdHostBuilder = new HostBuilder();
            return createdHostBuilder;
        });

        // Test
        var result = factory.CreateHostBuilder();
        result.ShouldNotBeNull();
        result.ShouldBeSameAs(createdHostBuilder);
        called.ShouldBe(true);
    }
}