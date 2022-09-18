// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

namespace AppMotor.Core.Certificates.Pem;

/// <summary>
/// Information about a block inside of a PEM file.
/// </summary>
public sealed class PemBlockInfo
{
    /// <summary>
    /// The block type; i.e. the text after "----BEGIN " and "-----END ". For example,
    /// "CERTIFICATE", "PUBLIC KEY", "RSA PRIVATE KEY", ...
    /// </summary>
    /// <remarks>
    /// For this, see also: https://stackoverflow.com/a/5356351/614177
    /// </remarks>
    [PublicAPI]
    public string BlockType { get; }

    /// <summary>
    /// The range of the blocks contents within the PEM file. Excludes the "----BEGIN" and
    /// "-----END" lines. The index is byte-based (not line-based).
    /// </summary>
    /// <remarks>
    /// For security reasons, the actual contents of the block are not stored inside
    /// this instance - as they could be sensitive (i.e. a private key). This is why
    /// we only store the range.
    /// </remarks>
    [PublicAPI]
    public Range BlockContentRange { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    public PemBlockInfo(string blockType, Range blockContentRange)
    {
        this.BlockType = blockType;
        this.BlockContentRange = blockContentRange;
    }
}
