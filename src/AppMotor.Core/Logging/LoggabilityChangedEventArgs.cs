// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.Core.Logging;

/// <summary>
/// Used by <see cref="LoggableValues.LoggabilityChanged"/>.
/// </summary>
public sealed class LoggabilityChangedEventArgs : EventArgs
{
    /// <summary>
    /// The type for which the loggability has changed. You may use the methods
    /// of <see cref="LoggableValues"/> to determine the new loggability.
    /// </summary>
    [PublicAPI]
    public Type Type { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="type">The type for which the loggability has changed.</param>
    public LoggabilityChangedEventArgs(Type type)
    {
        Validate.ArgumentWithName(nameof(type)).IsNotNull(type);

        this.Type = type;
    }
}
