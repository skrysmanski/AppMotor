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

using JetBrains.Annotations;

namespace AppMotor.Core.Colors;

/// <summary>
/// Represents a color in the HSV/HSB (hue, saturation, value/brightness) color model.
/// </summary>
/// <remarks>
/// To keep this struct small, the HSV values are stored internally as <see cref="Half"/>.
/// However, for ease of use, the values are exposed as <c>float</c>.
/// </remarks>
/// <seealso cref="HsvColor"/>
public readonly struct HsvColor : IColor, IEquatable<HsvColor>
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
    /// The value/brightness component of this color (0% - 100%).
    /// </summary>
    public float V => (float)this._v;

    private readonly Half _v;

    /// <summary>
    /// Constructor. Uses 255 as value for <see cref="A"/>.
    /// </summary>
    public HsvColor(float h, float s, float v)
        : this(a: 255, h, s, v)
    {
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    public HsvColor(byte a, float h, float s, float v)
    {
        if (h < 0 || h >= 360)
        {
            throw new ArgumentOutOfRangeException(nameof(h), $"The value '{h}' is outside the allowed range (0 - 360].");
        }
        if (s < 0 || s > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(s), $"The value '{s}' is outside the allowed range (0 - 100).");
        }
        if (v < 0 || v > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(v), $"The value '{v}' is outside the allowed range (0 - 100).");
        }

        this.A = a;
        this._h = (Half)h;
        this._s = (Half)s;
        this._v = (Half)v;
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    public HsvColor(RgbColor color)
    {
        this.A = color.A;

        HsxConverter.ConvertRgbToHsv(color.R, color.G, color.B, out float h, out float s, out float v);

        this._h = (Half)h;
        this._s = (Half)(s * 100);
        this._v = (Half)(v * 100);
    }

    /// <inheritdoc />
    public bool Equals(HsvColor other)
    {
        return this.A == other.A && this._h == other._h && this._s == other._s && this._v == other._v;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is HsvColor other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(this.A, this._h, this._s, this._v);
    }

    /// <summary>
    /// The == operator
    /// </summary>
    public static bool operator ==(HsvColor left, HsvColor right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// The != operator
    /// </summary>
    public static bool operator !=(HsvColor left, HsvColor right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    public RgbColor ToRgb()
    {
        HsxConverter.ConvertHsvToRgb((float)this._h, (float)this._s / 100, (float)this._v / 100, out var r, out var g, out var b);

        return new RgbColor(this.A, r, g, b);
    }

    /// <summary>
    /// Converts this HSL colors into its corresponding HSL color.
    /// </summary>
    [MustUseReturnValue]
    public HslColor ToHsl()
    {
        HsxConverter.ConvertHsvToHsl(this.S / 100, this.V / 100, out var s2, out var l);

        return new HslColor(this.A, this.H, s2 * 100, l * 100);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{nameof(HsvColor)} [A={this.A}, H={this._h}, S={this._s}, V={this._v}]";
    }
}
