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

using AppMotor.Core.Certificates.Exporting;
using AppMotor.Core.Exceptions;

using JetBrains.Annotations;

namespace AppMotor.Core.Certificates
{
    // TODO: Do we keep this?
    public static class CertFileExtensionHelper
    {
        [MustUseReturnValue]
        public static string GetFileExtensionFor(CertExportOptions exportOption, CertificateFileFormats exportFormat, bool includeDot = true)
        {
            string fileExtension;

            switch (exportFormat)
            {
                case CertificateFileFormats.PFX:
                    fileExtension = ".pfx";
                    break;

                case CertificateFileFormats.PEM:
                    fileExtension = ".pem";
                    break;

                case CertificateFileFormats.DER:
                    fileExtension = ".der";
                    break;

                default:
                    throw new UnexpectedSwitchValueException(nameof(exportFormat), exportFormat);
            }

            var fullFileExtension = exportOption == CertExportOptions.PublicKeyOnly ? "pub" + fileExtension : "key" + fileExtension;

            if (includeDot)
            {
                return "." + fullFileExtension;
            }
            else
            {
                return fullFileExtension;
            }
        }
    }
}
