// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Drawing;

using AppMotor.Core.Colors;
using AppMotor.Core.Extensions;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Extensions;

/// <summary>
/// Tests for <see cref="ColorExtensions"/>.
/// </summary>
public sealed class ColorExtensionsTests
{
    [Fact]
    public void Test_ToHsl()
    {
        Should.NotThrow(() => Color.Red.ToHsl());
        Should.NotThrow(() => RgbColor.Red.ToHsl());
    }

    [Fact]
    public void Test_ToHsv()
    {
        Should.NotThrow(() => Color.Red.ToHsv());
        Should.NotThrow(() => RgbColor.Red.ToHsv());
    }

    [Fact]
    public void Test_ToCmy()
    {
        Should.NotThrow(() => Color.Red.ToCmy());
        Should.NotThrow(() => RgbColor.Red.ToCmy());
    }

    [Fact]
    public void Test_ToCmyk()
    {
        Should.NotThrow(() => Color.Red.ToCmyk());
        Should.NotThrow(() => RgbColor.Red.ToCmyk());
    }

    [Fact]
    public void Test_ToHex()
    {
        Should.NotThrow(() => Color.Red.ToHex());
        Should.NotThrow(() => RgbColor.Red.ToHex());
    }
}
