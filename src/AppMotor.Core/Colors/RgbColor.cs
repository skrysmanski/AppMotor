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

namespace AppMotor.Core.Colors;

/// <summary>
/// Like <see cref="Color"/> but with proper equality (i.e. <c>RgbColor.Red</c> is actually equal to <c>new RgbColor(255, 0, 0)</c>).
/// </summary>
public readonly struct RgbColor : IColor, IEquatable<RgbColor>
{
    #region Known Colors
    #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    // ReSharper disable UnusedMember.Global

    public static RgbColor Transparent { get; } = new(KnownColor.Transparent);

    public static RgbColor AliceBlue { get; } = new(KnownColor.AliceBlue);

    public static RgbColor AntiqueWhite { get; } = new(KnownColor.AntiqueWhite);

    public static RgbColor Aqua { get; } = new(KnownColor.Aqua);

    public static RgbColor Aquamarine { get; } = new(KnownColor.Aquamarine);

    public static RgbColor Azure { get; } = new(KnownColor.Azure);

    public static RgbColor Beige { get; } = new(KnownColor.Beige);

    public static RgbColor Bisque { get; } = new(KnownColor.Bisque);

    public static RgbColor Black { get; } = new(KnownColor.Black);

    public static RgbColor BlanchedAlmond { get; } = new(KnownColor.BlanchedAlmond);

    public static RgbColor Blue { get; } = new(KnownColor.Blue);

    public static RgbColor BlueViolet { get; } = new(KnownColor.BlueViolet);

    public static RgbColor Brown { get; } = new(KnownColor.Brown);

    public static RgbColor BurlyWood { get; } = new(KnownColor.BurlyWood);

    public static RgbColor CadetBlue { get; } = new(KnownColor.CadetBlue);

    public static RgbColor Chartreuse { get; } = new(KnownColor.Chartreuse);

    public static RgbColor Chocolate { get; } = new(KnownColor.Chocolate);

    public static RgbColor Coral { get; } = new(KnownColor.Coral);

    public static RgbColor CornflowerBlue { get; } = new(KnownColor.CornflowerBlue);

    // ReSharper disable once IdentifierTypo
    public static RgbColor Cornsilk { get; } = new(KnownColor.Cornsilk);

    public static RgbColor Crimson { get; } = new(KnownColor.Crimson);

    public static RgbColor Cyan { get; } = new(KnownColor.Cyan);

    public static RgbColor DarkBlue { get; } = new(KnownColor.DarkBlue);

    public static RgbColor DarkCyan { get; } = new(KnownColor.DarkCyan);

    public static RgbColor DarkGoldenrod { get; } = new(KnownColor.DarkGoldenrod);

    public static RgbColor DarkGray { get; } = new(KnownColor.DarkGray);

    public static RgbColor DarkGreen { get; } = new(KnownColor.DarkGreen);

    public static RgbColor DarkKhaki { get; } = new(KnownColor.DarkKhaki);

    public static RgbColor DarkMagenta { get; } = new(KnownColor.DarkMagenta);

    public static RgbColor DarkOliveGreen { get; } = new(KnownColor.DarkOliveGreen);

    public static RgbColor DarkOrange { get; } = new(KnownColor.DarkOrange);

    public static RgbColor DarkOrchid { get; } = new(KnownColor.DarkOrchid);

    public static RgbColor DarkRed { get; } = new(KnownColor.DarkRed);

    public static RgbColor DarkSalmon { get; } = new(KnownColor.DarkSalmon);

    public static RgbColor DarkSeaGreen { get; } = new(KnownColor.DarkSeaGreen);

    public static RgbColor DarkSlateBlue { get; } = new(KnownColor.DarkSlateBlue);

    public static RgbColor DarkSlateGray { get; } = new(KnownColor.DarkSlateGray);

    public static RgbColor DarkTurquoise { get; } = new(KnownColor.DarkTurquoise);

    public static RgbColor DarkViolet { get; } = new(KnownColor.DarkViolet);

    public static RgbColor DeepPink { get; } = new(KnownColor.DeepPink);

    public static RgbColor DeepSkyBlue { get; } = new(KnownColor.DeepSkyBlue);

    public static RgbColor DimGray { get; } = new(KnownColor.DimGray);

    public static RgbColor DodgerBlue { get; } = new(KnownColor.DodgerBlue);

    public static RgbColor Firebrick { get; } = new(KnownColor.Firebrick);

    public static RgbColor FloralWhite { get; } = new(KnownColor.FloralWhite);

    public static RgbColor ForestGreen { get; } = new(KnownColor.ForestGreen);

    public static RgbColor Fuchsia { get; } = new(KnownColor.Fuchsia);

    // ReSharper disable once IdentifierTypo
    public static RgbColor Gainsboro { get; } = new(KnownColor.Gainsboro);

    public static RgbColor GhostWhite { get; } = new(KnownColor.GhostWhite);

    public static RgbColor Gold { get; } = new(KnownColor.Gold);

    public static RgbColor Goldenrod { get; } = new(KnownColor.Goldenrod);

    public static RgbColor Gray { get; } = new(KnownColor.Gray);

    public static RgbColor Green { get; } = new(KnownColor.Green);

    public static RgbColor GreenYellow { get; } = new(KnownColor.GreenYellow);

    public static RgbColor Honeydew { get; } = new(KnownColor.Honeydew);

    public static RgbColor HotPink { get; } = new(KnownColor.HotPink);

    public static RgbColor IndianRed { get; } = new(KnownColor.IndianRed);

    public static RgbColor Indigo { get; } = new(KnownColor.Indigo);

    public static RgbColor Ivory { get; } = new(KnownColor.Ivory);

    public static RgbColor Khaki { get; } = new(KnownColor.Khaki);

    public static RgbColor Lavender { get; } = new(KnownColor.Lavender);

    public static RgbColor LavenderBlush { get; } = new(KnownColor.LavenderBlush);

    public static RgbColor LawnGreen { get; } = new(KnownColor.LawnGreen);

    public static RgbColor LemonChiffon { get; } = new(KnownColor.LemonChiffon);

    public static RgbColor LightBlue { get; } = new(KnownColor.LightBlue);

    public static RgbColor LightCoral { get; } = new(KnownColor.LightCoral);

    public static RgbColor LightCyan { get; } = new(KnownColor.LightCyan);

    public static RgbColor LightGoldenrodYellow { get; } = new(KnownColor.LightGoldenrodYellow);

    public static RgbColor LightGreen { get; } = new(KnownColor.LightGreen);

    public static RgbColor LightGray { get; } = new(KnownColor.LightGray);

    public static RgbColor LightPink { get; } = new(KnownColor.LightPink);

    public static RgbColor LightSalmon { get; } = new(KnownColor.LightSalmon);

    public static RgbColor LightSeaGreen { get; } = new(KnownColor.LightSeaGreen);

    public static RgbColor LightSkyBlue { get; } = new(KnownColor.LightSkyBlue);

    public static RgbColor LightSlateGray { get; } = new(KnownColor.LightSlateGray);

    public static RgbColor LightSteelBlue { get; } = new(KnownColor.LightSteelBlue);

    public static RgbColor LightYellow { get; } = new(KnownColor.LightYellow);

    public static RgbColor Lime { get; } = new(KnownColor.Lime);

    public static RgbColor LimeGreen { get; } = new(KnownColor.LimeGreen);

    public static RgbColor Linen { get; } = new(KnownColor.Linen);

    public static RgbColor Magenta { get; } = new(KnownColor.Magenta);

    public static RgbColor Maroon { get; } = new(KnownColor.Maroon);

    public static RgbColor MediumAquamarine { get; } = new(KnownColor.MediumAquamarine);

    public static RgbColor MediumBlue { get; } = new(KnownColor.MediumBlue);

    public static RgbColor MediumOrchid { get; } = new(KnownColor.MediumOrchid);

    public static RgbColor MediumPurple { get; } = new(KnownColor.MediumPurple);

    public static RgbColor MediumSeaGreen { get; } = new(KnownColor.MediumSeaGreen);

    public static RgbColor MediumSlateBlue { get; } = new(KnownColor.MediumSlateBlue);

    public static RgbColor MediumSpringGreen { get; } = new(KnownColor.MediumSpringGreen);

    public static RgbColor MediumTurquoise { get; } = new(KnownColor.MediumTurquoise);

    public static RgbColor MediumVioletRed { get; } = new(KnownColor.MediumVioletRed);

    public static RgbColor MidnightBlue { get; } = new(KnownColor.MidnightBlue);

    public static RgbColor MintCream { get; } = new(KnownColor.MintCream);

    public static RgbColor MistyRose { get; } = new(KnownColor.MistyRose);

    public static RgbColor Moccasin { get; } = new(KnownColor.Moccasin);

    public static RgbColor NavajoWhite { get; } = new(KnownColor.NavajoWhite);

    public static RgbColor Navy { get; } = new(KnownColor.Navy);

    public static RgbColor OldLace { get; } = new(KnownColor.OldLace);

    public static RgbColor Olive { get; } = new(KnownColor.Olive);

    public static RgbColor OliveDrab { get; } = new(KnownColor.OliveDrab);

    public static RgbColor Orange { get; } = new(KnownColor.Orange);

    public static RgbColor OrangeRed { get; } = new(KnownColor.OrangeRed);

    public static RgbColor Orchid { get; } = new(KnownColor.Orchid);

    public static RgbColor PaleGoldenrod { get; } = new(KnownColor.PaleGoldenrod);

    public static RgbColor PaleGreen { get; } = new(KnownColor.PaleGreen);

    public static RgbColor PaleTurquoise { get; } = new(KnownColor.PaleTurquoise);

    public static RgbColor PaleVioletRed { get; } = new(KnownColor.PaleVioletRed);

    public static RgbColor PapayaWhip { get; } = new(KnownColor.PapayaWhip);

    public static RgbColor PeachPuff { get; } = new(KnownColor.PeachPuff);

    public static RgbColor Peru { get; } = new(KnownColor.Peru);

    public static RgbColor Pink { get; } = new(KnownColor.Pink);

    public static RgbColor Plum { get; } = new(KnownColor.Plum);

    public static RgbColor PowderBlue { get; } = new(KnownColor.PowderBlue);

    public static RgbColor Purple { get; } = new(KnownColor.Purple);

    public static RgbColor RebeccaPurple { get; } = new(KnownColor.RebeccaPurple);

    public static RgbColor Red { get; } = new(KnownColor.Red);

    public static RgbColor RosyBrown { get; } = new(KnownColor.RosyBrown);

    public static RgbColor RoyalBlue { get; } = new(KnownColor.RoyalBlue);

    public static RgbColor SaddleBrown { get; } = new(KnownColor.SaddleBrown);

    public static RgbColor Salmon { get; } = new(KnownColor.Salmon);

    public static RgbColor SandyBrown { get; } = new(KnownColor.SandyBrown);

    public static RgbColor SeaGreen { get; } = new(KnownColor.SeaGreen);

    public static RgbColor SeaShell { get; } = new(KnownColor.SeaShell);

    public static RgbColor Sienna { get; } = new(KnownColor.Sienna);

    public static RgbColor Silver { get; } = new(KnownColor.Silver);

    public static RgbColor SkyBlue { get; } = new(KnownColor.SkyBlue);

    public static RgbColor SlateBlue { get; } = new(KnownColor.SlateBlue);

    public static RgbColor SlateGray { get; } = new(KnownColor.SlateGray);

    public static RgbColor Snow { get; } = new(KnownColor.Snow);

    public static RgbColor SpringGreen { get; } = new(KnownColor.SpringGreen);

    public static RgbColor SteelBlue { get; } = new(KnownColor.SteelBlue);

    public static RgbColor Tan { get; } = new(KnownColor.Tan);

    public static RgbColor Teal { get; } = new(KnownColor.Teal);

    public static RgbColor Thistle { get; } = new(KnownColor.Thistle);

    public static RgbColor Tomato { get; } = new(KnownColor.Tomato);

    public static RgbColor Turquoise { get; } = new(KnownColor.Turquoise);

    public static RgbColor Violet { get; } = new(KnownColor.Violet);

    public static RgbColor Wheat { get; } = new(KnownColor.Wheat);

    public static RgbColor White { get; } = new(KnownColor.White);

    public static RgbColor WhiteSmoke { get; } = new(KnownColor.WhiteSmoke);

    public static RgbColor Yellow { get; } = new(KnownColor.Yellow);

    public static RgbColor YellowGreen { get; } = new(KnownColor.YellowGreen);

    // ReSharper restore UnusedMember.Global
    #pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    #endregion Known Colors

    /// <inheritdoc />
    public byte A { get; }

    /// <summary>
    /// The read component of this color (0 - 255).
    /// </summary>
    public byte R { get; }

    /// <summary>
    /// The green component of this color (0 - 255).
    /// </summary>
    public byte G { get; }

    /// <summary>
    /// The blue component of this color (0 - 255).
    /// </summary>
    public byte B { get; }

    /// <summary>
    /// Constructor. Uses 255 as value for <see cref="A"/>.
    /// </summary>
    public RgbColor(byte r, byte g, byte b)
    {
        this.A = 255;
        this.R = r;
        this.G = g;
        this.B = b;
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    public RgbColor(byte a, byte r, byte g, byte b)
    {
        this.A = a;
        this.R = r;
        this.G = g;
        this.B = b;
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="argb">The color as single integer.</param>
    // ReSharper disable once IdentifierTypo
    public RgbColor(int argb)
        : this(Color.FromArgb(argb))
    {

    }

    /// <summary>
    /// Constructor.
    /// </summary>
    public RgbColor(Color color)
        : this(color.A, color.R, color.G, color.B)
    {
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    public RgbColor(KnownColor knownColor)
        : this(Color.FromKnownColor(knownColor))
    {
    }

    /// <summary>
    /// Implicit conversion from <see cref="Color"/>.
    /// </summary>
    public static implicit operator RgbColor(Color color)
    {
        return new RgbColor(color);
    }

    /// <summary>
    /// Implicit conversion to <see cref="Color"/>.
    /// </summary>
    public static implicit operator Color(RgbColor color)
    {
        return Color.FromArgb(color.A, color.R, color.G, color.B);
    }

    /// <inheritdoc />
    public bool Equals(RgbColor other)
    {
        return this.A == other.A && this.R == other.R && this.G == other.G && this.B == other.B;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is RgbColor other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(this.A, this.R, this.G, this.B);
    }

    /// <summary>
    /// == operator
    /// </summary>
    public static bool operator ==(RgbColor left, RgbColor right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// != operator
    /// </summary>
    public static bool operator !=(RgbColor left, RgbColor right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    public RgbColor ToRgb()
    {
        return this;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{nameof(RgbColor)} [A={this.A}, R={this.R}, G={this.G}, B={this.B}]";
    }
}
