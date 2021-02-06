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

namespace AppMotor.Core.Certificates
{
    public enum CertificateFileFormats
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// The data is in PEM format.
        /// </summary>
        PEM,

        /// <summary>
        /// The data is in PFX format.
        /// </summary>
        PFX,

        // NOTE: We've dropped DER support (for now) because we couldn't find any use case
        //   where DER is required but neither PEM nor PFX are supported. Also, it was hard
        //   to find information about generating .der certificates because all examples
        //   just generate .pem certificates. With someone knowledgeable we can try to re-add
        //   support for DER. Until then, it's not worth the effort.
        //DER,

        // ReSharper restore InconsistentNaming
    }
}
