// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Globalization;
using System.Text;

using AppMotor.Core.Utils;

namespace AppMotor.Core.Colors;

/// <summary>
/// A color in HTML/CSS/Internet hex notation; e.g. <c>#FFAACC</c>. Supports
/// both 3 and 4 component colors (i.e. with or without alpha component) as
/// well as components with two characters (e.g. <c>#FFAACC</c>) or components
/// with one character (e.g. <c>#FAC</c>).
/// </summary>
public readonly struct HexColor : IColor, IEquatable<HexColor>
{
    private readonly RgbColor _color;

    /// <inheritdoc />
    public byte A => this._color.A;

    /// <summary>
    /// This color in (normalized) hex string notation; e.g. <c>#FFDF00C2</c>
    /// </summary>
    public string HexString => $"#{this._color.A:X2}{this._color.R:X2}{this._color.G:X2}{this._color.B:X2}";

    /// <summary>
    /// Constructor.
    /// </summary>
    public HexColor(string hexColor)
    {
        Validate.ArgumentWithName(nameof(hexColor)).IsNotNullOrEmpty(hexColor);

        int start = hexColor[0] == '#' ? 1 : 0;
        var colorAsSpan = hexColor.AsSpan(start);

        switch (colorAsSpan.Length)
        {
            // Single digit hex color with 3 components (e.g. "#fac")
            case 3:
            {
                var sb = new StringBuilder(capacity: 6);
                sb.Append(colorAsSpan[0], repeatCount: 2);
                sb.Append(colorAsSpan[1], repeatCount: 2);
                sb.Append(colorAsSpan[2], repeatCount: 2);
                colorAsSpan = sb.ToString();
                break;
            }

            // Single digit hex color with 4 components (e.g. "#bfac")
            case 4:
            {
                var sb = new StringBuilder(capacity: 6);
                sb.Append(colorAsSpan[0], repeatCount: 2);
                sb.Append(colorAsSpan[1], repeatCount: 2);
                sb.Append(colorAsSpan[2], repeatCount: 2);
                sb.Append(colorAsSpan[3], repeatCount: 2);
                colorAsSpan = sb.ToString();
                break;
            }

            case 6:
            case 8:
                // Ok
                break;

            default:
                throw new ArgumentException($"The string '{hexColor}' is not a valid hex color string.");
        }

        // ReSharper disable once IdentifierTypo
        uint argb;

        if (colorAsSpan.Length == 6)
        {
            if (!uint.TryParse(colorAsSpan, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint rgb))
            {
                throw new ArgumentException($"The string '{hexColor}' is not a valid hex color string.");
            }
            argb = rgb + 0xFF000000;
        }
        else
        {
            if (!uint.TryParse(colorAsSpan, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out argb))
            {
                throw new ArgumentException($"The string '{hexColor}' is not a valid hex color string.");
            }
        }

        this._color = new RgbColor((int)argb);
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    public HexColor(RgbColor color)
    {
        this._color = color;
    }

    /// <inheritdoc />
    public bool Equals(HexColor other)
    {
        return this._color.Equals(other._color);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is HexColor other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return this._color.GetHashCode();
    }

    /// <summary>
    /// == operator
    /// </summary>
    public static bool operator ==(HexColor left, HexColor right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// != operator
    /// </summary>
    public static bool operator !=(HexColor left, HexColor right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    public RgbColor ToRgb()
    {
        return this._color;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return this.HexString;
    }
}
