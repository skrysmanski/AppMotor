// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Runtime.CompilerServices;

using AppMotor.Core.Extensions;

using JetBrains.Annotations;

namespace AppMotor.Core.Colors;

internal static class HsxConverter
{
    public static void ConvertHslToHsv(float s, float l, out float s2, out float v)
    {
        v = l + s * Math.Min(l, 1 - l);

        if (v.IsBasicallyEqualTo(0))
        {
            s2 = 0;
        }
        else
        {
            s2 = 2 * (1 - l / v);
        }
    }

    public static void ConvertRgbToHsv(byte r, byte g, byte b, out float h, out float s, out float v)
    {
        ConvertRgbToHsl(r, g, b, out h, out var s2, out var l);
        ConvertHslToHsv(s2, l, out s, out v);
    }

    public static void ConvertRgbToHsl(byte r, byte g, byte b, out float h, out float s, out float l)
    {
        // Implementation based on .NET's "Color" class.

        if (r == g && g == b)
        {
            h = 0;
            s = 0;
            l = r / (float)byte.MaxValue;
            return;
        }

        MinMaxRgb(r, g, b, out int min, out int max);

        float delta = max - min;

        //
        // Hue
        //
        if (r == max)
        {
            h = (g - b) / delta;
        }
        else if (g == max)
        {
            h = (b - r) / delta + 2f;
        }
        else
        {
            h = (r - g) / delta + 4f;
        }

        h *= 60f;
        if (h < 0f)
        {
            h += 360f;
        }

        //
        // Saturation
        //
        int div = max + min;
        if (div > byte.MaxValue)
        {
            div = byte.MaxValue * 2 - max - min;
        }

        s = delta / div;

        //
        // Luminosity
        //
        l = (max + min) / (byte.MaxValue * 2.0f);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void MinMaxRgb(byte r, byte g, byte b, out int min, out int max)
    {
        if (r > g)
        {
            max = r;
            min = g;
        }
        else
        {
            max = g;
            min = r;
        }
        if (b > max)
        {
            max = b;
        }
        else if (b < min)
        {
            min = b;
        }
    }

    public static void ConvertHsvToHsl(float s, float v, out float s2, out float l)
    {
        l = v * (1 - s / 2);

        if (l is > 0 and < 1)
        {
            s2 = (v - l) / Math.Min(l, 1 - l);
        }
        else
        {
            s2 = 0;
        }
    }

    public static void ConvertHsvToRgb(float h, float s, float v, out byte r, out byte g, out byte b)
    {
        ConvertHsvToHsl(s, v, out var s2, out var l);
        ConvertHslToRgb(h, s2, l, out r, out g, out b);
    }

    public static void ConvertHslToRgb(float h, float s, float l, out byte r, out byte g, out byte b)
    {
        // Implementation based on: https://github.com/hvalidi/ColorMine/blob/master/ColorMine/ColorSpaces/Conversions/HslConverter.cs

        var rAsFloat = 0.0f;
        var gAsFloat = 0.0f;
        var bAsFloat = 0.0f;

        if (!l.IsBasicallyEqualTo(0))
        {
            if (s.IsBasicallyEqualTo(0))
            {
                rAsFloat = gAsFloat = bAsFloat = l;
            }
            else
            {
                var rangedH = h / 360.0;

                var temp2 = l < 0.5 ? l * (1.0 + s) : l + s - l * s;
                var temp1 = 2.0 * l - temp2;

                rAsFloat = (float)GetColorComponent(temp1, temp2, rangedH + 1.0 / 3.0);
                gAsFloat = (float)GetColorComponent(temp1, temp2, rangedH);
                bAsFloat = (float)GetColorComponent(temp1, temp2, rangedH - 1.0 / 3.0);
            }
        }

        r = (byte)Math.Round(255 * rAsFloat);
        g = (byte)Math.Round(255 * gAsFloat);
        b = (byte)Math.Round(255 * bAsFloat);
    }

    [MustUseReturnValue]
    private static double GetColorComponent(double temp1, double temp2, double temp3)
    {
        temp3 = MoveIntoRange(temp3);

        if (temp3 < 1.0 / 6.0)
        {
            return temp1 + (temp2 - temp1) * 6.0 * temp3;
        }

        if (temp3 < 0.5)
        {
            return temp2;
        }

        if (temp3 < 2.0 / 3.0)
        {
            return temp1 + (temp2 - temp1) * (2.0 / 3.0 - temp3) * 6.0;
        }

        return temp1;
    }

    [MustUseReturnValue]
    private static double MoveIntoRange(double value)
    {
        return value switch
        {
            < 0.0 => value + 1,
            > 1.0 => value - 1,
            _ => value,
        };
    }
}
