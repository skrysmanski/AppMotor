﻿#region License
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
    public sealed class CertificatePublicKeyTests
    {
        [Fact]
        public void Test_Equals()
        {
            // Setup
            using var cert1 = TlsCertificate.CreateSelfSigned("example.com", TimeSpan.FromDays(2));
            using var cert2 = TlsCertificate.CreateSelfSigned("example.com", TimeSpan.FromDays(2));

            // Test
            cert1.PublicKey.Equals(cert1.PublicKey).ShouldBe(true);
            cert1.PublicKey.Equals(cert2.PublicKey).ShouldBe(false);

            cert1.PublicKey.Equals((object?)cert1.PublicKey).ShouldBe(true);
            cert1.PublicKey.Equals((object?)cert2.PublicKey).ShouldBe(false);

            // ReSharper disable once EqualExpressionComparison
            (cert1.PublicKey == cert1.PublicKey).ShouldBe(true);
            (cert1.PublicKey != cert2.PublicKey).ShouldBe(true);

            cert1.PublicKey.Equals(null).ShouldBe(false);
        }

        [Fact]
        public void Test_GetHashCode()
        {
            // Setup
            using var cert1 = TlsCertificate.CreateSelfSigned("example.com", TimeSpan.FromDays(2));

            // Test
            cert1.PublicKey.GetHashCode().ShouldNotBe(0);
        }
    }
}
