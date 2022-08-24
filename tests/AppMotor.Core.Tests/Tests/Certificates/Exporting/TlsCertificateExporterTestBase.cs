// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Text;

using AppMotor.Core.Certificates;
using AppMotor.Core.Exceptions;

using Shouldly;

namespace AppMotor.Core.Tests.Certificates.Exporting;

public abstract class TlsCertificateExporterTestBase
{
    private const string PEM_START = "-----BEGIN ";

    protected static void CheckExportedBytesForCorrectFormat(ReadOnlySpan<byte> exportedBytes, CertificateFileFormats exportFormat)
    {
        switch (exportFormat)
        {
            case CertificateFileFormats.PEM:
                exportedBytes[0..PEM_START.Length].ToArray().ShouldBe(Encoding.ASCII.GetBytes(PEM_START));
                break;

            case CertificateFileFormats.PFX:
                // Unfortunately, PFX doesn't seem to have any "magic numbers".
                break;

            default:
                throw new UnexpectedSwitchValueException(nameof(exportFormat), exportFormat);
        }
    }
}