// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Certificates;
using AppMotor.TestCore;

using Xunit;

namespace AppMotor.Core.Tests.Certificates;

public sealed class CertificatePublicKeyTests
{
    [Fact]
    public void Test_Equals()
    {
        // Setup
        using var cert1 = TlsCertificate.CreateSelfSigned("example.com", TimeSpan.FromDays(2));
        using var cert2 = TlsCertificate.CreateSelfSigned("example.com", TimeSpan.FromDays(2));

        // Test
        EqualityMembersTests.TestEquals(cert1.PublicKey, cert2.PublicKey);
    }

    [Fact]
    public void Test_GetHashCode()
    {
        // Setup
        using var cert1 = TlsCertificate.CreateSelfSigned("example.com", TimeSpan.FromDays(2));

        // Test
        EqualityMembersTests.TestGetHashCode(cert1.PublicKey);
    }
}
