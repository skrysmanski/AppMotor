// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

namespace AppMotor.Core.Colors;

/// <summary>
/// Represents a color in the HSL (hue, saturation, luminosity) color model.
/// </summary>
/// <remarks>
/// To keep this struct small, the HSL values are stored internally as <see cref="Half"/>.
/// However, for ease of use, the values are exposed as <c>float</c>.
/// </remarks>
/// <seealso cref="HsvColor"/>
public readonly struct HslColor : IColor, IEquatable<HslColor>
{
    /// <inheritdoc />
    public byte A { get; }

    /// <summary>
    /// The hue component of this color - in degrees (0° - 360° exclusive).
    /// </summary>
    public float H => (float)this._h;

    private readonly Half _h;

    /// <summary>
    /// The saturation component of this color (0% - 100%).
    /// </summary>
    public float S => (float)this._s;

    private readonly Half _s;

    /// <summary>
    /// The luminosity/lightness component of this color (0% - 100%).
    /// </summary>
    public float L => (float)this._l;

    private readonly Half _l;

    /// <summary>
    /// Constructor. Uses 255 as value for <see cref="A"/>.
    /// </summary>
    public HslColor(float h, float s, float l)
        : this(a: 255, h, s, l)
    {
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    public HslColor(byte a, float h, float s, float l)
    {
        if (h < 0 || h >= 360)
        {
            throw new ArgumentOutOfRangeException(nameof(h), $"The value '{h}' is outside the allowed range (0 - 360].");
        }
        if (s < 0 || s > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(s), $"The value '{s}' is outside the allowed range (0 - 100).");
        }
        if (l < 0 || l > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(l), $"The value '{l}' is outside the allowed range (0 - 100).");
        }

        this.A = a;
        this._h = (Half)h;
        this._s = (Half)s;
        this._l = (Half)l;
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    public HslColor(RgbColor color)
    {
        this.A = color.A;

        HsxConverter.ConvertRgbToHsl(color.R, color.G, color.B, out float h, out float s, out float l);

        this._h = (Half)h;
        this._s = (Half)(s * 100);
        this._l = (Half)(l * 100);
    }

    /// <inheritdoc />
    public bool Equals(HslColor other)
    {
        return this.A == other.A && this._h == other._h && this._s == other._s && this._l == other._l;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is HslColor other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(this.A, this._h, this._s, this._l);
    }

    /// <summary>
    /// The == operator
    /// </summary>
    public static bool operator ==(HslColor left, HslColor right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// The != operator
    /// </summary>
    public static bool operator !=(HslColor left, HslColor right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    public RgbColor ToRgb()
    {
        HsxConverter.ConvertHslToRgb((float)this._h, (float)this._s / 100, (float)this._l / 100, out var r, out var g, out var b);

        return new RgbColor(this.A, r, g, b);
    }

    /// <summary>
    /// Converts this HSL colors into its corresponding HSV/HSB color.
    /// </summary>
    [MustUseReturnValue]
    public HsvColor ToHsv()
    {
        HsxConverter.ConvertHslToHsv(this.S / 100, this.L / 100, out var s2, out var v);

        return new HsvColor(this.A, this.H, s2 * 100, v * 100);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{nameof(HslColor)} [A={this.A}, H={this._h}, S={this._s}, L={this._l}]";
    }
}
