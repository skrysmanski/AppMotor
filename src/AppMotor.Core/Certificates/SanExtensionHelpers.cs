// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Formats.Asn1;
using System.Security.Cryptography.X509Certificates;

namespace AppMotor.Core.Certificates;

/// <summary>
/// Helper method for handling the "subject alternative names" TLS certificate extension.
/// </summary>
/// <remarks>
/// For a quick structure description, see: https://www.alvestrand.no/objectid/2.5.29.17.html
///
/// For the specification, see: https://datatracker.ietf.org/doc/html/rfc5280#section-4.2.1.6
///
/// For basic information on ASN1, see: https://luca.ntop.org/Teaching/Appunti/asn1.html
/// </remarks>
internal static class SanExtensionHelpers
{
    private const string SAN_OID = "2.5.29.17";

    // Tag value "2" is defined by:
    //
    //    dNSName                         [2]     IA5String,
    //
    // in: https://datatracker.ietf.org/doc/html/rfc5280#section-4.2.1.6
    private static readonly Asn1Tag DNS_NAME_TAG = new(TagClass.ContextSpecific, tagValue: 2, isConstructed: false);

    /// <summary>
    /// Returns all (supported) entries in the "subject alternative names" TLS extension. If the
    /// certificate doesn't contain the "subject alternative names" extensions, no elements will
    /// be returned. (The specification states that if the extension exists it must not be empty.)
    /// </summary>
    public static IEnumerable<string> GetSubjectAlternativeNames(this X509Certificate2 certificate)
    {
        var extension = certificate.Extensions[SAN_OID];
        if (extension is null)
        {
            yield break;
        }

        var asnReader = new AsnReader(extension.RawData, AsnEncodingRules.BER);
        var sequenceReader = asnReader.ReadSequence(Asn1Tag.Sequence);

        while (sequenceReader.HasData)
        {
            var tag = sequenceReader.PeekTag();
            if (tag != DNS_NAME_TAG)
            {
                sequenceReader.ReadEncodedValue();
                continue;
            }

            var dnsName = sequenceReader.ReadCharacterString(UniversalTagNumber.IA5String, DNS_NAME_TAG);
            yield return dnsName;
        }
    }

    public static void AddSanExtension(this CertificateRequest certificateRequest, IReadOnlyCollection<string> hostNames)
    {
        if (hostNames.Count == 0)
        {
            throw new InvalidOperationException("There must be at least one entry in the SAN extension.");
        }

        var asnWriter = new AsnWriter(AsnEncodingRules.CER);
        asnWriter.PushSequence();

        foreach (var hostName in hostNames)
        {
            asnWriter.WriteCharacterString(UniversalTagNumber.IA5String, hostName, DNS_NAME_TAG);
        }

        asnWriter.PopSequence();

        var asnValue = asnWriter.Encode();

        var extension = new X509Extension(SAN_OID, asnValue, critical: false);
        certificateRequest.CertificateExtensions.Add(extension);
    }
}
