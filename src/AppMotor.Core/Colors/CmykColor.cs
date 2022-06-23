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

using AppMotor.Core.Extensions;

using JetBrains.Annotations;

namespace AppMotor.Core.Colors;

/// <summary>
/// Represents a color in the CMYK (cyan, magenta, yellow, black) color model.
/// </summary>
/// <remarks>
/// To keep this struct small, the CMYK values are stored internally as <see cref="Half"/>.
/// However, for ease of use, the values are exposed as <c>float</c>.
/// </remarks>
/// <seealso cref="CmyColor"/>
public readonly struct CmykColor : IColor, IEquatable<CmykColor>
{
    private static readonly Half HALF_ZERO = (Half)0;

    /// <inheritdoc />
    public byte A { get; }

    /// <summary>
    /// The cyan component of this color (0 - 100%).
    /// </summary>
    public float C => (float)this._c * 100;

    private readonly Half _c;

    /// <summary>
    /// The magenta component of this color (0 - 100%).
    /// </summary>
    public float M => (float)this._m * 100;

    private readonly Half _m;

    /// <summary>
    /// The yellow component of this color (0 - 100%).
    /// </summary>
    public float Y => (float)this._y * 100;

    private readonly Half _y;

    /// <summary>
    /// The key/black component of this color (0 - 100%).
    /// </summary>
    public float K => (float)this._k * 100;

    private readonly Half _k;

    /// <summary>
    /// Constructor. Uses 255 as value for <see cref="A"/>.
    /// </summary>
    public CmykColor(float c, float m, float y, float k)
        : this(a: 255, c, m, y, k)
    {
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    public CmykColor(byte a, float c, float m, float y, float k)
    {
        if (c < 0 || c > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(c), $"The value '{c}' is outside the allowed range (0 - 100).");
        }
        if (m < 0 || m > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(m), $"The value '{m}' is outside the allowed range (0 - 100).");
        }
        if (y < 0 || y > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(y), $"The value '{y}' is outside the allowed range (0 - 100).");
        }
        if (k < 0 || k > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(k), $"The value '{k}' is outside the allowed range (0 - 100).");
        }

        this.A = a;
        this._c = (Half)(c / 100.0);
        this._m = (Half)(m / 100.0);
        this._y = (Half)(y / 100.0);
        this._k = (Half)(k / 100.0);
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    public CmykColor(RgbColor color)
    {
        this.A = color.A;

        var rFloat = color.R / 255.0;
        var gFloat = color.G / 255.0;
        var bFloat = color.B / 255.0;

        double maxRgb = 0;

        if (rFloat > maxRgb)
        {
            maxRgb = rFloat;
        }
        if (gFloat > maxRgb)
        {
            maxRgb = gFloat;
        }
        if (bFloat > maxRgb)
        {
            maxRgb = bFloat;
        }

        var kFloat = 1 - maxRgb;
        this._k = (Half)kFloat;

        if (maxRgb.IsBasicallyEqualTo(0))
        {
            this._c = HALF_ZERO;
            this._m = HALF_ZERO;
            this._y = HALF_ZERO;
        }
        else
        {
            this._c = (Half)((1 - rFloat - kFloat) / maxRgb);
            this._m = (Half)((1 - gFloat - kFloat) / maxRgb);
            this._y = (Half)((1 - bFloat - kFloat) / maxRgb);
        }
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    public CmykColor(CmyColor cmyColor)
        : this(cmyColor.ToRgb())
    {
    }

    /// <inheritdoc />
    public bool Equals(CmykColor other)
    {
        return this.A == other.A && this._c == other._c && this._m == other._m && this._y == other._y && this._k == other._k;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is CmykColor other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(this.A, this.C, this.M, this.Y, this.K);
    }

    /// <summary>
    /// The == operator
    /// </summary>
    public static bool operator ==(CmykColor left, CmykColor right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// The != operator
    /// </summary>
    public static bool operator !=(CmykColor left, CmykColor right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    public RgbColor ToRgb()
    {
        var maxRgb = 1.0 - (double)this._k;

        return new RgbColor(
            this.A,
            (byte)Math.Round(255.0 * (1 - (double)this._c) * maxRgb),
            (byte)Math.Round(255.0 * (1 - (double)this._m) * maxRgb),
            (byte)Math.Round(255.0 * (1 - (double)this._y) * maxRgb)
        );
    }

    /// <summary>
    /// Converts this CMYK color into its corresponding CMY color.
    /// </summary>
    /// <returns></returns>
    [MustUseReturnValue]
    public CmyColor ToCmy()
    {
        var maxRgb = 1.0 - this.K / 100;

        return new CmyColor(
            this.A,
            (byte)Math.Round(255 - 255.0 * (1 - (double)this._c) * maxRgb),
            (byte)Math.Round(255 - 255.0 * (1 - (double)this._m) * maxRgb),
            (byte)Math.Round(255 - 255.0 * (1 - (double)this._y) * maxRgb)
        );
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{nameof(CmykColor)} [A={this.A}, C={Math.Round(this.C)}, M={Math.Round(this.M)}, Y={Math.Round(this.Y)}, K={Math.Round(this.K)}]";
    }
}
