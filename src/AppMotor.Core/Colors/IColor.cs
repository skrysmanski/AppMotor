// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

namespace AppMotor.Core.Colors;

/// <summary>
/// Represents a color.
/// </summary>
public interface IColor
{
    /// <summary>
    /// The alpha component of this color (0 (fully transparent) - 255 (fully opaque)).
    /// </summary>
    [PublicAPI]
    byte A { get; }

    /// <summary>
    /// Converts this color into its corresponding RGB color.
    /// </summary>
    [PublicAPI, MustUseReturnValue]
    RgbColor ToRgb();
}
