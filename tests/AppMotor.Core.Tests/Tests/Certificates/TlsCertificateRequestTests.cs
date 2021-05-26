#region License
// Copyright 2021 AppMotor Framework (https://github.com/skrysmanski/AppMotor)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;

using AppMotor.Core.Certificates;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Certificates
{
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
        }
    }
}
