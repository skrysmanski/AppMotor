// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.CommandLine;
using AppMotor.CliApp.CommandLine.Utils;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.Tests.CommandLine;

public sealed class ParamsCollectionBuilderTests
{
    [Fact]
    public void TestBasicBuilding()
    {
        var builder = new ParamsCollectionBuilder();

        builder.Build().Length.ShouldBe(0);

        var param1 = new CliParam<int>("--value1");
        var param2 = new CliParam<int>("--value2");
        builder.AddParam(param1);
        builder.AddParam(param2);

        builder.Build().ShouldBe(new[] { param1, param2 });
    }

    [Fact]
    public void TestDuplicateAliasDetection()
    {
        var builder = new ParamsCollectionBuilder();

        var param1 = new CliParam<int>("--value1");
        builder.AddParam(param1);

        var param2 = new CliParam<int>("--value1");
        var ex = Should.Throw<InvalidOperationException>(() => builder.AddParam(param2));
        ex.Message.ShouldContain("--value1");
    }
}