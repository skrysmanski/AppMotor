// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

namespace AppMotor.Core.Certificates;

/// <summary>
/// The various file formats for storing certificates.
/// </summary>
public enum CertificateFileFormats
{
    // ReSharper disable InconsistentNaming

    /// <summary>
    /// The data is in PEM format.
    /// </summary>
    PEM,

    /// <summary>
    /// The data is in PFX format.
    /// </summary>
    PFX,

    // NOTE: We've dropped DER support (for now) because we couldn't find any use case
    //   where DER is required but neither PEM nor PFX are supported. Also, it was hard
    //   to find information about generating .der certificates because all examples
    //   just generate .pem certificates. With someone knowledgeable we can try to re-add
    //   support for DER. Until then, it's not worth the effort.
    //DER,

    // ReSharper restore InconsistentNaming
}
