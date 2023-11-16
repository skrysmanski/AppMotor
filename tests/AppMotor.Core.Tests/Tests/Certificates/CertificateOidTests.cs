// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Security.Cryptography;

using AppMotor.Core.Certificates;
using AppMotor.TestCore;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Certificates;

public sealed class CertificateOidTests
{
    [Fact]
    public void Test_Value()
    {
        new CertificateOid("1.2.840.113549.1.1.1").Value.ShouldBe("1.2.840.113549.1.1.1");
        new CertificateOid("RSA").Value.ShouldBe("1.2.840.113549.1.1.1");
    }

    [Fact]
    public void Test_Equals()
    {
        var rsaOidById = new CertificateOid("1.2.840.113549.1.1.1");
        var rsaOidByName = new CertificateOid("RSA");
        var otherOid = new CertificateOid("3DES");

        EqualityMembersTests.TestEquals(rsaOidById, rsaOidByName, otherOid);
    }

    [Fact]
    public void Test_GetHashCode()
    {
        var rsaOidById = new CertificateOid("1.2.840.113549.1.1.1");
        var rsaOidByName = new CertificateOid("RSA");

        EqualityMembersTests.TestGetHashCode(rsaOidById, rsaOidByName);
    }

    [Fact]
    public void Test_ToString()
    {
        var rsaOidById = new CertificateOid("1.2.840.113549.1.1.1");
        var rsaOidByName = new CertificateOid("RSA");

        rsaOidById.ToString().ShouldBe("1.2.840.113549.1.1.1");
        rsaOidByName.ToString().ShouldBe("1.2.840.113549.1.1.1");

        new CertificateOid().ToString().ShouldBe("");
    }

    [Fact]
    public void Test_ImplicitConversion()
    {
        Oid oid = new CertificateOid("1.2.840.113549.1.1.1");
        CertificateOid certOid = oid;

        certOid.Value.ShouldBe("1.2.840.113549.1.1.1");
    }
}
