// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

namespace AppMotor.Core.Colors;

/// <summary>
/// Represents a color in the CMY (cyan, magenta, yellow) color model.
/// </summary>
/// <seealso cref="CmykColor"/>
public readonly struct CmyColor : IColor, IEquatable<CmyColor>
{
    /// <inheritdoc />
    public byte A { get; }

    /// <summary>
    /// The cyan component of this color (0 - 255).
    /// </summary>
    public byte C { get; }

    /// <summary>
    /// The magenta component of this color (0 - 255).
    /// </summary>
    public byte M { get; }

    /// <summary>
    /// The yellow component of this color (0 - 255).
    /// </summary>
    public byte Y { get; }

    /// <summary>
    /// Constructor. Uses 255 as value for <see cref="A"/>.
    /// </summary>
    public CmyColor(byte c, byte m, byte y)
    {
        this.A = 255;
        this.C = c;
        this.M = m;
        this.Y = y;
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    public CmyColor(byte a, byte c, byte m, byte y)
    {
        this.A = a;
        this.C = c;
        this.M = m;
        this.Y = y;
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    public CmyColor(RgbColor color)
    {
        this.A = color.A;
        this.C = (byte)(255 - color.R);
        this.M = (byte)(255 - color.G);
        this.Y = (byte)(255 - color.B);
    }

    /// <inheritdoc />
    public bool Equals(CmyColor other)
    {
        return this.A == other.A && this.C == other.C && this.M == other.M && this.Y == other.Y;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is CmyColor other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(this.A, this.C, this.M, this.Y);
    }

    /// <summary>
    /// The == operator
    /// </summary>
    public static bool operator ==(CmyColor left, CmyColor right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// The != operator
    /// </summary>
    public static bool operator !=(CmyColor left, CmyColor right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    public RgbColor ToRgb()
    {
        return new RgbColor(this.A, (byte)(255 - this.C), (byte)(255 - this.M), (byte)(255 - this.Y));
    }

    /// <summary>
    /// Converts this CMY color into its corresponding CMYK color.
    /// </summary>
    [MustUseReturnValue]
    public CmykColor ToCmyk()
    {
        return new CmykColor(this);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{nameof(CmyColor)} [A={this.A}, C={this.C}, M={this.M}, Y={this.Y}]";
    }
}
