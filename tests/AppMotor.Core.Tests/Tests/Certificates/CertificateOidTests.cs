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

using System.Security.Cryptography;

using AppMotor.Core.Certificates;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Certificates
{
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

            rsaOidById.Equals(rsaOidByName).ShouldBe(true);
            rsaOidById.Equals(otherOid).ShouldBe(false);

            rsaOidById.Equals(rsaOidById).ShouldBe(true);
            rsaOidById.Equals(new CertificateOid()).ShouldBe(false);

            (rsaOidById == rsaOidByName).ShouldBe(true);
            (rsaOidById != otherOid).ShouldBe(true);

            // ReSharper disable once SuspiciousTypeConversion.Global
            rsaOidById.Equals("").ShouldBe(false);
            rsaOidById.Equals((object)rsaOidByName).ShouldBe(true);
        }

        [Fact]
        public void Test_GetHashCode()
        {
            var rsaOidById = new CertificateOid("1.2.840.113549.1.1.1");
            var rsaOidByName = new CertificateOid("RSA");

            rsaOidById.GetHashCode().ShouldBe(rsaOidByName.GetHashCode());
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
}
