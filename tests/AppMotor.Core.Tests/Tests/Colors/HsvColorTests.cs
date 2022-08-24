// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Drawing;

using AppMotor.Core.Colors;
using AppMotor.Core.Extensions;
using AppMotor.TestCore;
using AppMotor.TestCore.Utils;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Colors;

public sealed class HsvColorTests
{
    // For expected values, see: https://www.rapidtables.com/convert/color/rgb-to-hsl.html
    [Theory]
    [InlineData(255,   0,   0,   0.0, 100.0, 100.0)] // red
    [InlineData(  0, 255,   0, 120.0, 100.0, 100.0)] // green
    [InlineData(  0,   0, 255, 240.0, 100.0, 100.0)] // blue
    [InlineData(  0,   0,   0,   0.0,   0.0,   0.0)] // black
    [InlineData(255, 255, 255,   0.0,   0.0, 100.0)] // white
    [InlineData(128, 128, 128,   0.0,   0.0,  50.2)] // gray
    [InlineData(255, 105, 180, 330.0,  58.8, 100.0)] // hot pink
    [InlineData( 64,  64,  96, 240.0,  33.3,  37.7)] // dark blue
    [InlineData(115,  66,  32,  24.6,  72.2,  45.1)] // brown
    public void Test_Conversion(byte r, byte g, byte b, double expectedH, double expectedS, double expectedV)
    {
        // Setup
        var rgbColor = new RgbColor(a: 128, r, g, b);

        // Test
        var colorToTest = new HsvColor(rgbColor);

        // Verify
        colorToTest.A.ShouldBe((byte)128);
        Math.Round(colorToTest.H, digits: 1).ShouldBe(expectedH);
        Math.Round(colorToTest.S, digits: 1).ShouldBe(expectedS);
        Math.Round(colorToTest.V, digits: 1).ShouldBe(expectedV);

        var convertedBack = colorToTest.ToRgb();

        convertedBack.ShouldBe(rgbColor);
    }

    [Theory]
    [InlineData(  0, 100.0, 100.0,   0.0, 100.0,  50.0)] // red
    [InlineData(120, 100.0, 100.0, 120.0, 100.0,  50.0)] // green
    [InlineData(240, 100.0, 100.0, 240.0, 100.0,  50.0)] // blue
    [InlineData(  0,   0.0,   0.0,   0.0,   0.0,   0.0)] // black
    [InlineData(  0,   0.0, 100.0,   0.0,   0.0, 100.0)] // white
    [InlineData(  0,   0.0,  50.2,   0.0,   0.0,  50.2)] // gray
    [InlineData(330,  58.8, 100.0, 330.0, 100.0,  70.6)] // hot pink
    [InlineData(240,  33.3,  37.7, 240.0,  20.0,  31.4)] // dark blue
    [InlineData(24.6, 72.2,  45.1,  24.6,  56.5,  28.8)] // brown
    public void Test_ToHsl(float h, float s, float v, double expectedH2, double expectedS2, double expectedL)
    {
        var hslColor = new HsvColor(a: 128, h, s, v).ToHsl();

        hslColor.A.ShouldBe((byte)128);
        Math.Round(hslColor.H, digits: 1).ShouldBe(expectedH2);
        Math.Round(hslColor.S, digits: 1).ShouldBe(expectedS2);
        Math.Round(hslColor.L, digits: 1).ShouldBe(expectedL);
    }

    [Theory]
    [ClassData(typeof(EnumTestDataGenerator<KnownColor>))]
    public void Test_KnownColors(KnownColor knownColor)
    {
        var rgbColor = new RgbColor(knownColor);

        var hsvColor = rgbColor.ToHsv();

        hsvColor.A.ShouldBe(rgbColor.A);

        hsvColor.ToRgb().ShouldBe(rgbColor);
    }

    [Theory]
    [InlineData(12, 24, 48)]
    public void Test_ToString(float h, float s, float v)
    {
        new HsvColor(h, s, v).ToString().ShouldBe($"{nameof(HsvColor)} [A=255, H={h}, S={s}, V={v}]");
        new HsvColor(a: 128, h, s, v).ToString().ShouldBe($"{nameof(HsvColor)} [A=128, H={h}, S={s}, V={v}]");
    }

    [Fact]
    public void Test_Equals()
    {
        var red1 = new HsvColor(0, 100, 100);
        var red2 = new HsvColor(RgbColor.Red);
        var green = new HsvColor(RgbColor.Green);

        EqualityMembersTests.TestEquals(red1, red2, green);
    }

    [Fact]
    public void Test_GetHashCode()
    {
        var red1 = new HsvColor(0, 100, 100);
        var red2 = new HsvColor(RgbColor.Red);

        EqualityMembersTests.TestGetHashCode(red1, red2);
    }

    [Fact]
    public void Test_InvalidConstructorParameters()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => new HsvColor(-1, 50, 50));
        Should.Throw<ArgumentOutOfRangeException>(() => new HsvColor(360, 50, 50));

        Should.Throw<ArgumentOutOfRangeException>(() => new HsvColor(100, -1, 50));
        Should.Throw<ArgumentOutOfRangeException>(() => new HsvColor(100, 101, 50));

        Should.Throw<ArgumentOutOfRangeException>(() => new HsvColor(100, 50, -1));
        Should.Throw<ArgumentOutOfRangeException>(() => new HsvColor(100, 50, 101));
    }
}
