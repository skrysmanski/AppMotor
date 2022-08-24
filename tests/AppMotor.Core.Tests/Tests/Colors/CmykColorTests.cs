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

/// <summary>
/// Tests for <see cref="CmykColor"/>.
/// </summary>
public sealed class CmykColorTests
{
    // For expected values, see: https://www.rapidtables.com/convert/color/rgb-to-cmyk.html
    [Theory]
    [InlineData(255,   0,   0,   0, 100, 100,   0)] // red
    [InlineData(  0, 255,   0, 100,   0, 100,   0)] // green
    [InlineData(  0,   0, 255, 100, 100,   0,   0)] // blue
    [InlineData(  0,   0,   0,   0,   0,   0, 100)] // black
    [InlineData(255, 255, 255,   0,   0,   0,   0)] // white
    [InlineData(128, 128, 128,   0,   0,   0,  50)] // gray
    [InlineData(255, 105, 180,   0,  59,  29,   0)] // hot pink
    [InlineData( 64,  64,  96,  33,  33,   0,  62)] // dark blue
    [InlineData(115,  66,  32,   0,  43,  72,  55)] // brown
    public void Test_Conversion(byte r, byte g, byte b, int expectedC, int expectedM, int expectedY, int expectedK)
    {
        // Setup
        var rgbColor = new RgbColor(a: 128, r, g, b);

        // Test
        var colorToTest = new CmykColor(rgbColor);

        // Verify
        colorToTest.A.ShouldBe((byte)128);
        Math.Round(colorToTest.C).ShouldBe(expectedC);
        Math.Round(colorToTest.M).ShouldBe(expectedM);
        Math.Round(colorToTest.Y).ShouldBe(expectedY);
        Math.Round(colorToTest.K).ShouldBe(expectedK);

        var convertedBack = colorToTest.ToRgb();

        convertedBack.ShouldBe(rgbColor);
    }

    [Theory]
    [InlineData(  0, 100, 100,   0,   0, 255, 255)] // red
    [InlineData(100,   0, 100,   0, 255,   0, 255)] // green
    [InlineData(100, 100,   0,   0, 255, 255,   0)] // blue
    [InlineData(  0,   0,   0, 100, 255, 255, 255)] // black
    [InlineData(  0,   0,   0,   0,   0,   0,   0)] // white
    [InlineData(  0,   0,   0,  50, 128, 128, 128)] // gray
    [InlineData(  0,  59,  29,   0,   0, 150,  74)] // hot pink
    [InlineData( 33,  33,   0,  62, 190, 190, 158)] // dark blue
    [InlineData(  0,  43,  72,  55, 140, 190, 223)] // brown
    public void Test_ToCmy(float c, float m, float y, float k, byte expectedC, byte expectedM, byte expectedY)
    {
        var cmyColor = new CmykColor(a: 128, c, m, y, k).ToCmy();

        cmyColor.A.ShouldBe((byte)128);
        cmyColor.C.ShouldBe(expectedC);
        cmyColor.M.ShouldBe(expectedM);
        cmyColor.Y.ShouldBe(expectedY);
    }

    [Theory]
    [ClassData(typeof(EnumTestDataGenerator<KnownColor>))]
    public void Test_KnownColors(KnownColor knownColor)
    {
        var rgbColor = new RgbColor(knownColor);

        var cmykColor = rgbColor.ToCmyk();

        cmykColor.A.ShouldBe(rgbColor.A);

        cmykColor.ToRgb().ShouldBe(rgbColor);
    }

    [Theory]
    [InlineData(12, 24, 48, 64)]
    public void Test_ToString(byte c, byte m, byte y, byte k)
    {
        new CmykColor(c, m, y, k).ToString().ShouldBe($"{nameof(CmykColor)} [A=255, C={c}, M={m}, Y={y}, K={k}]");
        new CmykColor(a: 128, c, m, y, k).ToString().ShouldBe($"{nameof(CmykColor)} [A=128, C={c}, M={m}, Y={y}, K={k}]");
    }

    [Fact]
    public void Test_Equals()
    {
        var red1 = new CmykColor(0, 100, 100, 0);
        var red2 = new CmykColor(RgbColor.Red);
        var green = new CmykColor(RgbColor.Green);

        EqualityMembersTests.TestEquals(red1, red2, green);
    }

    [Fact]
    public void Test_GetHashCode()
    {
        var red1 = new CmykColor(0, 100, 100, 0);
        var red2 = new CmykColor(RgbColor.Red);

        EqualityMembersTests.TestGetHashCode(red1, red2);
    }

    [Fact]
    public void Test_InvalidConstructorParameters()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => new CmykColor( -1, 50, 50, 50));
        Should.Throw<ArgumentOutOfRangeException>(() => new CmykColor(101, 50, 50, 50));

        Should.Throw<ArgumentOutOfRangeException>(() => new CmykColor(50,  -1, 50, 50));
        Should.Throw<ArgumentOutOfRangeException>(() => new CmykColor(50, 101, 50, 50));

        Should.Throw<ArgumentOutOfRangeException>(() => new CmykColor(50, 50,  -1, 50));
        Should.Throw<ArgumentOutOfRangeException>(() => new CmykColor(50, 50, 101, 50));

        Should.Throw<ArgumentOutOfRangeException>(() => new CmykColor(50, 50, 50,  -1));
        Should.Throw<ArgumentOutOfRangeException>(() => new CmykColor(50, 50, 50, 101));
    }
}
