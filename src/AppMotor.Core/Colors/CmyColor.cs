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
    public CmyColor(Color color)
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
    public Color ToRgb()
    {
        return Color.FromArgb(this.A, 255 - this.C, 255 - this.M, 255 - this.Y);
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
