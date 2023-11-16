// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

namespace AppMotor.Core.Certificates.Pem;

/// <summary>
/// Exception thrown for format errors in PEM files.
/// </summary>
public sealed class PemFormatException : Exception
{
    /// <summary>
    /// Constructor.
    /// </summary>
    public PemFormatException(string? message) : base(message)
    {
    }
}
