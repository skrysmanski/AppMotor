// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Certificates;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Certificates;

public sealed class CertificateKeyAlgorithmOidsTests
{
    [Fact]
    // ReSharper disable once InconsistentNaming
    public void TestRSA()
    {
        CertificateKeyAlgorithmOids.RSA.Value.ShouldBe(CertificateKeyAlgorithmOids.RSA_VALUE);
        CertificateKeyAlgorithmOids.GetAlgorithmFromOid(CertificateKeyAlgorithmOids.RSA).ShouldBe(CertificateKeyAlgorithms.RSA);
    }

    [Fact]
    // ReSharper disable once InconsistentNaming
    public void TestDSA()
    {
        CertificateKeyAlgorithmOids.DSA.Value.ShouldBe(CertificateKeyAlgorithmOids.DSA_VALUE);
        CertificateKeyAlgorithmOids.GetAlgorithmFromOid(CertificateKeyAlgorithmOids.DSA).ShouldBe(CertificateKeyAlgorithms.DSA);
    }

    [Fact]
    // ReSharper disable once InconsistentNaming
    public void TestECDsa()
    {
        CertificateKeyAlgorithmOids.ECDsa.Value.ShouldBe(CertificateKeyAlgorithmOids.ECDSA_VALUE);
        CertificateKeyAlgorithmOids.GetAlgorithmFromOid(CertificateKeyAlgorithmOids.ECDsa).ShouldBe(CertificateKeyAlgorithms.ECDSA);
    }

    [Fact]
    public void TestOther()
    {
        CertificateKeyAlgorithmOids.GetAlgorithmFromOid(new CertificateOid("1.1")).ShouldBe(CertificateKeyAlgorithms.Other);
    }
}
