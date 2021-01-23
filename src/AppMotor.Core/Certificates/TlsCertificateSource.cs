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
using System.IO.Abstractions;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using AppMotor.Core.Exceptions;
using AppMotor.Core.IO;

using JetBrains.Annotations;

namespace AppMotor.Core.Certificates
{
    /// <summary>
    /// The source for a <see cref="TlsCertificate"/>.
    /// </summary>
    public abstract class TlsCertificateSource
    {
        private static readonly byte[] DER_MAGIC_NUMBER = { 0x30, 0x82 };

        // TODO: Are the number of dashes fixed?
        private const string PEM_MAGIC_NUMBER = "-----BEGIN ";

        /// <summary>
        /// Whether this source has a separate source for the private key (e.g. if the
        /// public key and the private key are stored in two separate files).
        /// </summary>
        protected abstract bool HasSeparatePrivateKeySource { get; }

        [PublicAPI, MustUseReturnValue]
        public static TlsCertificateSource FromPemString(string pemEncodedCertificate, string? separatePemEncodedPrivateKey = null)
        {
            return new PemCertificateSource(pemEncodedCertificate, separatePemEncodedPrivateKey);
        }

        [PublicAPI, MustUseReturnValue]
        public static TlsCertificateSource FromBytes(ReadOnlyMemory<byte> certificateBytes, ReadOnlyMemory<byte>? separatePrivateKeyBytes = null)
        {
            var certFormat = DetermineCertificateFormat(certificateBytes.Span);
            switch (certFormat)
            {
                case CertificateFileFormats.PEM:
                    return new PemCertificateSource(certificateBytes, separatePrivateKeyBytes);

                case CertificateFileFormats.PFX:
                    return new PfxCertificateSource(certificateBytes, separatePrivateKeyBytes);

                case CertificateFileFormats.DER:
                    return new DerCertificateSource(certificateBytes, separatePrivateKeyBytes);

                default:
                    throw new UnexpectedSwitchValueException(nameof(certFormat), certFormat);
            }
        }

        [PublicAPI, MustUseReturnValue]
        public static TlsCertificateSource FromFile(string certificateFilePath, IFileSystem? fileSystem = null)
        {
            return FromFile(certificateFilePath, separatePrivateKeyFilePath: null, fileSystem);
        }

        [PublicAPI, MustUseReturnValue]
        public static TlsCertificateSource FromFile(string certificateFilePath, string? separatePrivateKeyFilePath = null, IFileSystem? fileSystem = null)
        {
            fileSystem ??= RealFileSystem.Instance;

            var certificateBytes = fileSystem.File.ReadAllBytes(certificateFilePath);
            if (separatePrivateKeyFilePath is null)
            {
                return FromBytes(certificateBytes);
            }
            else
            {
                var separatePrivateKeyBytes = fileSystem.File.ReadAllBytes(separatePrivateKeyFilePath);
                return FromBytes(certificateBytes, separatePrivateKeyBytes);
            }
        }

        [MustUseReturnValue]
        private static CertificateFileFormats DetermineCertificateFormat(ReadOnlySpan<byte> bytes)
        {
            if (bytes.Length > 2 && ByteArrayEquals(bytes[0..2], DER_MAGIC_NUMBER))
            {
                return CertificateFileFormats.DER;
            }
            else if (bytes.Length > PEM_MAGIC_NUMBER.Length && ByteArrayEquals(bytes[0..PEM_MAGIC_NUMBER.Length], Encoding.ASCII.GetBytes(PEM_MAGIC_NUMBER))) // TODO: Cache the ASCII conversion, if possible
            {
                return CertificateFileFormats.PEM;
            }
            else
            {
                // Unfortunately, PFX doesn't seem to have any "magic numbers". So we leave it as last option.
                return CertificateFileFormats.PFX;
            }
        }

        private static bool ByteArrayEquals(ReadOnlySpan<byte> a1, ReadOnlySpan<byte> a2)
        {
            return a1.SequenceEqual(a2);
        }

        // TODO: Add password parameter
        /// <summary>
        /// Creates the underlying certificate.
        /// </summary>
        /// <seealso cref="TlsCertificate(TlsCertificateSource)"/>
        internal X509Certificate2 CreateUnderlyingCertificate(bool allowPrivateKeyExport)
        {
            var storageFlags = X509KeyStorageFlags.DefaultKeySet;
            if (allowPrivateKeyExport)
            {
                storageFlags |= X509KeyStorageFlags.Exportable;
            }

            X509Certificate2? primaryCert = null;

            try
            {
                primaryCert = CreatePrimaryUnderlyingCertificate(storageFlags);
                if (!this.HasSeparatePrivateKeySource)
                {
                    return primaryCert;
                }

                if (primaryCert.HasPrivateKey)
                {
                    throw new InvalidOperationException("The certificate already has a private key but you explicitly specified a separate private key.");
                }

                var keyAlgorithm = CertificateKeyAlgorithmOids.GetAlgorithmFromOid(primaryCert.PublicKey.Oid);
                switch (keyAlgorithm)
                {
                    case CertificateKeyAlgorithms.RSA:
                        using (var rsa = RSA.Create())
                        {
                            ImportPrivateKeyInto(rsa);
                            return primaryCert.CopyWithPrivateKey(rsa);
                        }

                    default:
                        throw new InvalidOperationException($"Unsupported certificate key algorithm: {keyAlgorithm}");
                }

            }
            catch (Exception)
            {
                primaryCert?.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Loads the certificate from the "primary" source. The primary source can either
        /// be only the public key or the public and the private key combined.
        /// </summary>
        /// <seealso cref="HasSeparatePrivateKeySource"/>
        // TODO: All implementations seem to be the same here
        protected abstract X509Certificate2 CreatePrimaryUnderlyingCertificate(X509KeyStorageFlags storageFlags);

        /// <summary>
        /// Imports the private key into the specified RSA instance. Note that this
        /// method is only called if <see cref="HasSeparatePrivateKeySource"/> is <c>true</c>.
        /// </summary>
        protected abstract void ImportPrivateKeyInto(RSA rsa);

        private sealed class PemCertificateSource : TlsCertificateSource
        {
            private readonly ReadOnlyMemory<byte> _pemEncodedCertificate;

            private readonly string? _separatePemEncodedPrivateKey;

            /// <inheritdoc />
            protected override bool HasSeparatePrivateKeySource => this._separatePemEncodedPrivateKey is not null;

            public PemCertificateSource(string pemEncodedCertificate, string? separatePemEncodedPrivateKey)
            {
                this._pemEncodedCertificate = Encoding.ASCII.GetBytes(pemEncodedCertificate);
                this._separatePemEncodedPrivateKey = separatePemEncodedPrivateKey;
            }

            public PemCertificateSource(ReadOnlyMemory<byte> pemEncodedCertificate, ReadOnlyMemory<byte>? separatePemEncodedPrivateKey)
            {
                this._pemEncodedCertificate = pemEncodedCertificate;
                this._separatePemEncodedPrivateKey = separatePemEncodedPrivateKey is not null ? Encoding.ASCII.GetString(separatePemEncodedPrivateKey.Value.Span) : null;
            }

            /// <inheritdoc />
            protected override X509Certificate2 CreatePrimaryUnderlyingCertificate(X509KeyStorageFlags storageFlags)
            {
                return new(this._pemEncodedCertificate.Span, null, storageFlags);
            }

            /// <inheritdoc />
            protected override void ImportPrivateKeyInto(RSA rsa)
            {
                rsa.ImportFromPem(this._separatePemEncodedPrivateKey);
            }
        }

        private sealed class PfxCertificateSource : TlsCertificateSource
        {
            private readonly ReadOnlyMemory<byte> _encodedCertificate;

            /// <inheritdoc />
            protected override bool HasSeparatePrivateKeySource => false;

            public PfxCertificateSource(ReadOnlyMemory<byte> encodedCertificate, ReadOnlyMemory<byte>? separateEncodedPrivateKey)
            {
                if (separateEncodedPrivateKey is not null)
                {
                    throw new NotSupportedException("Pfx certificates can't have a separate private key.");
                }

                this._encodedCertificate = encodedCertificate;
            }

            /// <inheritdoc />
            protected override X509Certificate2 CreatePrimaryUnderlyingCertificate(X509KeyStorageFlags storageFlags)
            {
                return new(this._encodedCertificate.Span, null, storageFlags);
            }

            /// <inheritdoc />
            protected override void ImportPrivateKeyInto(RSA rsa)
            {
                throw new NotSupportedException("Pfx certificates can't have a separate private key.");
            }
        }

        private sealed class DerCertificateSource : TlsCertificateSource
        {
            private readonly ReadOnlyMemory<byte> _encodedCertificate;

            private readonly ReadOnlyMemory<byte>? _separateEncodedPrivateKey;

            /// <inheritdoc />
            protected override bool HasSeparatePrivateKeySource => this._separateEncodedPrivateKey is not null;

            public DerCertificateSource(ReadOnlyMemory<byte> encodedCertificate, ReadOnlyMemory<byte>? separateEncodedPrivateKey)
            {
                this._encodedCertificate = encodedCertificate;
                this._separateEncodedPrivateKey = separateEncodedPrivateKey;
            }

            /// <inheritdoc />
            protected override X509Certificate2 CreatePrimaryUnderlyingCertificate(X509KeyStorageFlags storageFlags)
            {
                return new(this._encodedCertificate.Span, null, storageFlags);
            }

            /// <inheritdoc />
            protected override void ImportPrivateKeyInto(RSA rsa)
            {
                rsa.ImportPkcs8PrivateKey(this._separateEncodedPrivateKey!.Value.Span, out _);
            }
        }
    }
}
