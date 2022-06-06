#region License
// Copyright 2022 AppMotor Framework (https://github.com/skrysmanski/AppMotor)
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

using System.Drawing;

using AppMotor.Core.Colors;
using AppMotor.TestCore;
using AppMotor.TestCore.Utils;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Colors;

/// <summary>
/// Tests for <see cref="CmyColor"/>.
/// </summary>
public sealed class CmyColorTests
{
    [Theory]
    [InlineData(255,   0,   0,   0, 255, 255)] // red
    [InlineData(  0, 255,   0, 255,   0, 255)] // green
    [InlineData(  0,   0, 255, 255, 255,   0)] // blue
    [InlineData(  0,   0,   0, 255, 255, 255)] // black
    [InlineData(255, 255, 255,   0,   0,   0)] // white
    [InlineData(128, 128, 128, 127, 127, 127)] // gray
    [InlineData(255, 105, 180,   0, 150,  75)] // hot pink
    [InlineData( 64,  64,  96, 191, 191, 159)] // dark blue
    [InlineData(115,  66,  32, 140, 189, 223)] // brown
    public void Test_Conversion(byte r, byte g, byte b, byte expectedC, byte expectedM, byte expectedY)
    {
        // Setup
        var rgbColor = Color.FromArgb(alpha: 128, r, g, b);

        // Test
        var colorToTest = new CmyColor(rgbColor);

        // Verify
        colorToTest.A.ShouldBe((byte)128);
        colorToTest.C.ShouldBe(expectedC);
        colorToTest.M.ShouldBe(expectedM);
        colorToTest.Y.ShouldBe(expectedY);

        var convertedBack = colorToTest.ToRgb();

        convertedBack.ShouldBe(rgbColor);
    }

    [Theory]
    [InlineData(  0, 255, 255,   0, 100, 100,   0)] // red
    [InlineData(255,   0, 255, 100,   0, 100,   0)] // green
    [InlineData(255, 255,   0, 100, 100,   0,   0)] // blue
    [InlineData(255, 255, 255,   0,   0,   0, 100)] // black
    [InlineData(  0,   0,   0,   0,   0,   0,   0)] // white
    [InlineData(127, 127, 127,   0,   0,   0,  50)] // gray
    [InlineData(  0, 150,  75,   0,  59,  29,   0)] // hot pink
    [InlineData(191, 191, 159,  33,  33,   0,  62)] // dark blue
    [InlineData(140, 189, 223,   0,  43,  72,  55)] // brown
    public void Test_ToCmyk(byte c, byte m, byte y, int expectedC, int expectedM, int expectedY, int expectedK)
    {
        var cmykColor = new CmyColor(a: 128, c, m, y).ToCmyk();

        cmykColor.A.ShouldBe((byte)128);
        Math.Round(cmykColor.C).ShouldBe(expectedC);
        Math.Round(cmykColor.M).ShouldBe(expectedM);
        Math.Round(cmykColor.Y).ShouldBe(expectedY);
        Math.Round(cmykColor.K).ShouldBe(expectedK);
    }

    [Theory]
    [ClassData(typeof(EnumTestDataGenerator<KnownColor>))]
    public void Test_KnownColors(KnownColor knownColor)
    {
        var rgbColor = Color.FromKnownColor(knownColor);
        rgbColor = Color.FromArgb(rgbColor.A, rgbColor.R, rgbColor.G, rgbColor.B);

        var cmyColor = rgbColor.ToCmy();

        cmyColor.A.ShouldBe(rgbColor.A);

        cmyColor.ToRgb().ShouldBe(rgbColor);
    }

    [Theory]
    [InlineData(12, 24, 48)]
    public void Test_ToString(byte c, byte m, byte y)
    {
        new CmyColor(c, m, y).ToString().ShouldBe($"{nameof(CmyColor)} [A=255, C={c}, M={m}, Y={y}]");
        new CmyColor(a: 128, c, m, y).ToString().ShouldBe($"{nameof(CmyColor)} [A=128, C={c}, M={m}, Y={y}]");
    }

    [Fact]
    public void Test_Equals()
    {
        var red1 = new CmyColor(Color.Red);
        var red2 = new CmyColor(Color.Red);
        var green = new CmyColor(Color.Green);

        EqualityMembersTests.TestEquals(red1, red2, green);
    }

    [Fact]
    public void Test_GetHashCode()
    {
        var red1 = new CmyColor(Color.Red);
        var red2 = new CmyColor(Color.Red);

        EqualityMembersTests.TestGetHashCode(red1, red2);
    }
}
