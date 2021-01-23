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
using System.Text;

using AppMotor.Core.Certificates;
using AppMotor.Core.Exceptions;

using Shouldly;

namespace AppMotor.Core.Tests.Certificates
{
    public abstract class TlsCertificateExporterTestBase
    {
        private const string PEM_START = "-----BEGIN ";

        protected static void CheckExportedBytesForCorrectFormat(ReadOnlySpan<byte> exportedBytes, CertificateFileFormats exportFormat)
        {
            switch (exportFormat)
            {
                case CertificateFileFormats.DER:
                    exportedBytes[0..2].ToArray().ShouldBe(new byte[] { 0x30, 0x82 });
                    break;

                case CertificateFileFormats.PEM:
                    exportedBytes[0..PEM_START.Length].ToArray().ShouldBe(Encoding.ASCII.GetBytes(PEM_START));
                    break;

                case CertificateFileFormats.PFX:
                    // Unfortunaltey, PFX doesn't seem to have any "magic numbers".
                    break;

                default:
                    throw new UnexpectedSwitchValueException(nameof(exportFormat), exportFormat);
            }
        }
    }
}
