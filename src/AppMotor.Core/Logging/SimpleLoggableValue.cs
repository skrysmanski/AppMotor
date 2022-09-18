// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.DataModel;

namespace AppMotor.Core.Logging;

/// <summary>
/// Represents a simple loggable value in the sense that it's a (more or less) value
/// that ideally fits onto one line in a log file. You may implement this interface
/// for a type whose values should be included in <see cref="ExceptionLogExtensions.GetLoggableProperties"/>
/// and therefor in <see cref="ExtendedExceptionStringExtensions.ToStringExtended"/>.
/// </summary>
/// <seealso cref="SimpleLoggableValueMarker"/>
public interface ISimpleLoggableValue
{
}

/// <summary>
/// A <see cref="TypeMarker"/> alternative to <see cref="ISimpleLoggableValue"/>.
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public sealed class SimpleLoggableValueMarker : TypeMarker
{
}
