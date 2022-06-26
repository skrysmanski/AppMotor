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
