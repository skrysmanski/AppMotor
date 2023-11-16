// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Certificates;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Certificates;

public sealed class TlsCertificateRequestTests
{
    [Fact]
    public void Test_CreateCertificateRequest()
    {
        // Test
        var req = Should.NotThrow(() => new TlsCertificateRequest("example.com"));
        req.Dispose();
    }

    [Theory]
    [InlineData(null, typeof(ArgumentNullException))]
    [InlineData("", typeof(ArgumentException))]
    [InlineData("   ", typeof(ArgumentException))]
    public void Test_CreateCertificateRequest_InvalidName(string? hostname, Type expectedExceptionType)
    {
        // Test
        Should.Throw(() => new TlsCertificateRequest(hostname!), expectedExceptionType);
    }

    [Fact]
    public void Test_CreateSelfSignedCertificate()
    {
        // Test
        using var certificateRequest = new TlsCertificateRequest("example.com");
        var cert = certificateRequest.CreateSelfSignedCertificate(TimeSpan.FromDays(20));

        // Validate
        cert.SubjectName.Name.ShouldBe("CN=example.com");
        cert.NotAfter.Date.ShouldBe(DateTime.Today + TimeSpan.FromDays(20));
        cert.SubjectAlternativeNames.ShouldBe(new[] { "example.com" });
    }

    [Fact]
    public void Test_MultipleHostNames()
    {
        // Test
        using var certificateRequest = new TlsCertificateRequest("example.com", new[] { "example2.com", "www.example.com", "support.example.com" });
        var cert = certificateRequest.CreateSelfSignedCertificate(TimeSpan.FromDays(20));

        // Validate
        cert.SubjectName.Name.ShouldBe("CN=example.com");
        cert.NotAfter.Date.ShouldBe(DateTime.Today + TimeSpan.FromDays(20));
        cert.SubjectAlternativeNames.ShouldBe(new[] { "example.com", "example2.com", "www.example.com", "support.example.com" });

    }
}
