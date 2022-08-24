// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Drawing;
using System.Reflection;

using AppMotor.Core.Colors;
using AppMotor.TestCore;
using AppMotor.TestCore.Utils;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Colors;

/// <summary>
/// Tests for <see cref="RgbColor"/>.
/// </summary>
public sealed class RgbColorTests
{
    [Theory]
    [InlineData(255, 255,   0,   0)] // red
    [InlineData(128,   0, 255,   0)] // green
    [InlineData(  0,   0,   0, 255)] // blue
    [InlineData(255,   0,   0,   0)] // black
    [InlineData(255, 255, 255, 255)] // white
    [InlineData(255, 128, 128, 128)] // gray
    [InlineData(255, 255, 105, 180)] // hot pink
    [InlineData(255,  64,  64,  96)] // dark blue
    [InlineData(255, 115,  66,  32)] // brown
    public void Test_ConstructorsAndConversion(byte a, byte r, byte g, byte b)
    {
        // Test
        var rgbColorWithAlpha = new RgbColor(a, r, g, b);

        // Verify
        rgbColorWithAlpha.A.ShouldBe(a);
        rgbColorWithAlpha.R.ShouldBe(r);
        rgbColorWithAlpha.G.ShouldBe(g);
        rgbColorWithAlpha.B.ShouldBe(b);

        // Test
        var rgbColorWithoutAlpha = new RgbColor(r, g, b);

        // Verify
        rgbColorWithoutAlpha.A.ShouldBe((byte)255);
        rgbColorWithoutAlpha.R.ShouldBe(r);
        rgbColorWithoutAlpha.G.ShouldBe(g);
        rgbColorWithoutAlpha.B.ShouldBe(b);

        // Test
        // ReSharper disable once IdentifierTypo
        long argb = ((long)a << 24) | ((long)r << 16) | ((long)g << 8) | b;
        var rgbColorFromInt = new RgbColor((int)argb);

        // Verify
        rgbColorFromInt.ShouldBe(rgbColorWithAlpha);

        // Test and Verify
        rgbColorWithAlpha.ToRgb().ShouldBe(rgbColorWithAlpha);
    }

    [Theory]
    [InlineData(255, 255,   0,   0)] // red
    [InlineData(128,   0, 255,   0)] // green
    [InlineData(  0,   0,   0, 255)] // blue
    [InlineData(255,   0,   0,   0)] // black
    [InlineData(255, 255, 255, 255)] // white
    [InlineData(255, 128, 128, 128)] // gray
    [InlineData(255, 255, 105, 180)] // hot pink
    [InlineData(255,  64,  64,  96)] // dark blue
    [InlineData(255, 115,  66,  32)] // brown
    public void Test_ImplicitConversion(byte a, byte r, byte g, byte b)
    {
        // Setup
        var color = Color.FromArgb(a, r, g, b);

        // Test 1
        RgbColor rgbColor = color;

        // Verify 1
        rgbColor.A.ShouldBe(a);
        rgbColor.R.ShouldBe(r);
        rgbColor.G.ShouldBe(g);
        rgbColor.B.ShouldBe(b);

        // Test 1
        Color color2 = rgbColor;

        // Verify 1
        color2.A.ShouldBe(a);
        color2.R.ShouldBe(r);
        color2.G.ShouldBe(g);
        color2.B.ShouldBe(b);
    }

    [Theory]
    [ClassData(typeof(EnumTestDataGenerator<KnownColor>))]
    public void Test_KnownColors(KnownColor knownColor)
    {
        new RgbColor(knownColor).ShouldBe(new RgbColor(Color.FromKnownColor(knownColor)));

        switch (knownColor)
        {
            case KnownColor.ActiveBorder:
            case KnownColor.ActiveCaption:
            case KnownColor.ActiveCaptionText:
            case KnownColor.AppWorkspace:
            case KnownColor.ButtonFace:
            case KnownColor.ButtonHighlight:
            case KnownColor.ButtonShadow:
            case KnownColor.Control:
            case KnownColor.ControlDark:
            case KnownColor.ControlDarkDark:
            case KnownColor.ControlLight:
            case KnownColor.ControlLightLight:
            case KnownColor.ControlText:
            case KnownColor.Desktop:
            case KnownColor.GradientActiveCaption:
            case KnownColor.GradientInactiveCaption:
            case KnownColor.GrayText:
            case KnownColor.Highlight:
            case KnownColor.HighlightText:
            case KnownColor.HotTrack:
            case KnownColor.InactiveBorder:
            case KnownColor.InactiveCaption:
            case KnownColor.InactiveCaptionText:
            case KnownColor.Info:
            case KnownColor.InfoText:
            case KnownColor.Menu:
            case KnownColor.MenuBar:
            case KnownColor.MenuHighlight:
            case KnownColor.MenuText:
            case KnownColor.ScrollBar:
            case KnownColor.Window:
            case KnownColor.WindowFrame:
            case KnownColor.WindowText:
                // We don't have color "constants" for these colors.
                break;

            default:
                new RgbColor(knownColor).ShouldBe(typeof(RgbColor).GetProperty(knownColor.ToString(), BindingFlags.Public | BindingFlags.Static)!.GetValue(null));
                break;
        }
    }

    [Theory]
    [InlineData(12, 24, 48)]
    public void Test_ToString(byte r, byte g, byte b)
    {
        new RgbColor(r, g, b).ToString().ShouldBe($"{nameof(RgbColor)} [A=255, R={r}, G={g}, B={b}]");
        new RgbColor(a: 128, r, g, b).ToString().ShouldBe($"{nameof(RgbColor)} [A=128, R={r}, G={g}, B={b}]");
    }

    [Fact]
    public void Test_Equals()
    {
        var red1 = new RgbColor(255, 0, 0);
        var red2 = RgbColor.Red;
        var green = RgbColor.Green;

        EqualityMembersTests.TestEquals(red1, red2, green);
    }

    [Fact]
    public void Test_GetHashCode()
    {
        var red1 = new RgbColor(255, 0, 0);
        var red2 = RgbColor.Red;

        EqualityMembersTests.TestGetHashCode(red1, red2);
    }
}
