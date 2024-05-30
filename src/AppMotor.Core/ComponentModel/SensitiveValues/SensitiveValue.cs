// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

namespace AppMotor.Core.ComponentModel;

/// <summary>
/// Represents a sensitive value (e.g. a password or access token). Users of
/// this class can make certain that the value is never logged or displayed
/// by accident.
/// </summary>
/// <seealso cref="SensitiveValueMarker"/>
public interface ISensitiveValue;

/// <summary>
/// A <see cref="TypeMarker"/> alternative to <see cref="ISensitiveValue"/>.
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public sealed class SensitiveValueMarker : TypeMarker;
