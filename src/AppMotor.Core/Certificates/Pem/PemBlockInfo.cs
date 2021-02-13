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

using JetBrains.Annotations;

namespace AppMotor.Core.Certificates.Pem
{
    /// <summary>
    /// Information about a block inside of a PEM file.
    /// </summary>
    public sealed class PemBlockInfo
    {
        /// <summary>
        /// The block type; i.e. the text after "----BEGIN " and "-----END ". For example,
        /// "CERTIFICATE", "PUBLIC KEY", "RSA PRIVATE KEY", ...
        /// </summary>
        /// <remarks>
        /// For this, see also: https://stackoverflow.com/a/5356351/614177
        /// </remarks>
        [PublicAPI]
        public string BlockType { get; }

        /// <summary>
        /// The range of the blocks contents within the PEM file. Excludes the "----BEGIN" and
        /// "-----END" lines. The index is byte-based (not line-based).
        /// </summary>
        /// <remarks>
        /// For security reasons, the actual contents of the block are not stored inside
        /// this instance - as they could be sensitive (i.e. a private key). This is why
        /// we only store the range.
        /// </remarks>
        [PublicAPI]
        public Range BlockContentRange { get; }

        public PemBlockInfo(string blockType, Range blockContentRange)
        {
            this.BlockType = blockType;
            this.BlockContentRange = blockContentRange;
        }
    }
}
