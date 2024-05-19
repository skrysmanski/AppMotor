// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Drawing;

using AppMotor.Core.Colors;

using JetBrains.Annotations;

namespace AppMotor.Core.Extensions;

/// <summary>
/// Extension methods for <see cref="Color"/> and <see cref="RgbColor"/>.
/// </summary>
public static class ColorExtensions
{
    //
    // NOTE: I was thinking of adding support for IColor here but in the end decided against it
    //   because it would make the API surface of the colors more messy (e.g. having "HslColor.ToHsl()").
    //   Also, something like "HslColor.ToHex()" may be confusing as the result is RGB, not HSL.
    //

    /// <summary>
    /// Converts this color into its corresponding HSL color.
    /// </summary>
    [MustUseReturnValue]
    public static HslColor ToHsl(this Color color)
    {
        return new HslColor(color);
    }

    /// <summary>
    /// Converts this color into its corresponding HSL color.
    /// </summary>
    [MustUseReturnValue]
    public static HslColor ToHsl(this RgbColor color)
    {
        return new HslColor(color);
    }

    /// <summary>
    /// Converts this color into its corresponding HSV/HSB color.
    /// </summary>
    [MustUseReturnValue]
    public static HsvColor ToHsv(this Color color)
    {
        return new HsvColor(color);
    }

    /// <summary>
    /// Converts this color into its corresponding HSV/HSB color.
    /// </summary>
    [MustUseReturnValue]
    public static HsvColor ToHsv(this RgbColor color)
    {
        return new HsvColor(color);
    }

    /// <summary>
    /// Converts this color into its corresponding CMY color.
    /// </summary>
    [MustUseReturnValue]
    public static CmyColor ToCmy(this Color color)
    {
        return new CmyColor(color);
    }

    /// <summary>
    /// Converts this color into its corresponding CMY color.
    /// </summary>
    [MustUseReturnValue]
    public static CmyColor ToCmy(this RgbColor color)
    {
        return new CmyColor(color);
    }

    /// <summary>
    /// Converts this color into its corresponding CMYK color.
    /// </summary>
    [MustUseReturnValue]
    public static CmykColor ToCmyk(this Color color)
    {
        return new CmykColor(color);
    }
    /// <summary>
    /// Converts this color into its corresponding CMYK color.
    /// </summary>
    [MustUseReturnValue]
    public static CmykColor ToCmyk(this RgbColor color)
    {
        return new CmykColor(color);
    }

    /// <summary>
    /// Converts this color into its corresponding CMYK color.
    /// </summary>
    [MustUseReturnValue]
    public static HexColor ToHex(this Color color)
    {
        return new HexColor(color);
    }
    /// <summary>
    /// Converts this color into its corresponding CMYK color.
    /// </summary>
    [MustUseReturnValue]
    public static HexColor ToHex(this RgbColor color)
    {
        return new HexColor(color);
    }
}
