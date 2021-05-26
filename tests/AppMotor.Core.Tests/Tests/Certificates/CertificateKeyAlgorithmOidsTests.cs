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

using AppMotor.Core.Certificates;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Certificates
{
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
}
