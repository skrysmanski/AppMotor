// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

namespace AppMotor.Core.Exceptions;

/// <summary>
/// Thrown if you try to access properties of an uninitialized struct.
/// </summary>
[PublicAPI]
public class UninitializedStructPropertyException : InvalidOperationException;
