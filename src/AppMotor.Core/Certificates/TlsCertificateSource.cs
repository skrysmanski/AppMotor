// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using AppMotor.Core.Certificates.Pem;
using AppMotor.Core.Exceptions;
using AppMotor.Core.IO;

using JetBrains.Annotations;

namespace AppMotor.Core.Certificates;

/// <summary>
/// The source for a <see cref="TlsCertificate"/>.
/// </summary>
public abstract class TlsCertificateSource
{
    // NOTE: Strangely enough, these magic numbers also seem to be used by .pfx files.
    private static readonly byte[] PFX_MAGIC_NUMBER = [0x30, 0x82];

    // ReSharper disable once UseUtf8StringLiteral
    private static readonly byte[] PEM_MAGIC_NUMBER = Encoding.ASCII.GetBytes("-----BEGIN ");

    /// <summary>
    /// Whether this source has a separate source for the private key (e.g. if the
    /// public key and the private key are stored in two separate files).
    /// </summary>
    protected abstract bool HasSeparatePrivateKeySource { get; }

    /// <summary>
    /// Creates a certificate source from a PEM encoded string.
    /// </summary>
    /// <param name="pemEncodedCertificate">The certificate as PEM encoded string.</param>
    /// <param name="separatePemEncodedPrivateKey">The private key of the certificate as PEM encoded string, if it's
    /// separate from <paramref name="pemEncodedCertificate"/>.</param>
    [PublicAPI, MustUseReturnValue]
    public static TlsCertificateSource FromPemString(string pemEncodedCertificate, string? separatePemEncodedPrivateKey = null)
    {
        return new PemCertificateSource(pemEncodedCertificate, separatePemEncodedPrivateKey);
    }

    /// <summary>
    /// Creates a certificate source from the specified bytes.
    /// </summary>
    /// <param name="certificateBytes">The certificate</param>
    /// <param name="separatePrivateKeyBytes">The private key of the certificate, if it's separate from <paramref name="certificateBytes"/>.</param>
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

            default:
                throw new UnexpectedSwitchValueException(nameof(certFormat), certFormat);
        }
    }

    /// <summary>
    /// Creates a certificate source from a file on disk.
    /// </summary>
    /// <param name="certificateFilePath">The path to the certificate file.</param>
    /// <param name="fileSystem">The file system to use; if <c>null</c>, <see cref="RealFileSystem.Instance"/> will be used.</param>
    [PublicAPI, MustUseReturnValue]
    public static TlsCertificateSource FromFile(FilePath certificateFilePath, IFileSystem? fileSystem = null)
    {
        return FromFile(certificateFilePath, separatePrivateKeyFilePath: null, fileSystem);
    }

    /// <summary>
    /// Creates a certificate source from files on disk.
    /// </summary>
    /// <param name="certificateFilePath">The path to the certificate file.</param>
    /// <param name="separatePrivateKeyFilePath">The path to the private key file of the certificate. May be <c>null</c>,
    /// if not needed.</param>
    /// <param name="fileSystem">The file system to use; if <c>null</c>, <see cref="RealFileSystem.Instance"/> will be used.</param>
    [PublicAPI, MustUseReturnValue]
    public static TlsCertificateSource FromFile(FilePath certificateFilePath, FilePath? separatePrivateKeyFilePath, IFileSystem? fileSystem = null)
    {
        var certificateBytes = certificateFilePath.ReadAllBytes(fileSystem);
        if (separatePrivateKeyFilePath is null)
        {
            return FromBytes(certificateBytes);
        }
        else
        {
            var separatePrivateKeyBytes = separatePrivateKeyFilePath.Value.ReadAllBytes(fileSystem);
            return FromBytes(certificateBytes, separatePrivateKeyBytes);
        }
    }

    [MustUseReturnValue]
    private static CertificateFileFormats DetermineCertificateFormat(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length > 2 && ByteArrayEquals(bytes[0..2], PFX_MAGIC_NUMBER))
        {
            return CertificateFileFormats.PFX;
        }
        else if (bytes.Length > PEM_MAGIC_NUMBER.Length && ByteArrayEquals(bytes[0..PEM_MAGIC_NUMBER.Length], PEM_MAGIC_NUMBER))
        {
            return CertificateFileFormats.PEM;
        }
        else
        {
            throw new NotSupportedException("The certificate type could not be determined.");
        }
    }

    private static bool ByteArrayEquals(ReadOnlySpan<byte> a1, ReadOnlySpan<byte> a2)
    {
        return a1.SequenceEqual(a2);
    }

    /// <summary>
    /// Creates the underlying certificate.
    /// </summary>
    internal X509Certificate2 CreateUnderlyingCertificate(bool allowPrivateKeyExport, ReadOnlySpan<char> password)
    {
        var storageFlags = X509KeyStorageFlags.DefaultKeySet;
        if (allowPrivateKeyExport)
        {
            storageFlags |= X509KeyStorageFlags.Exportable;
        }

        X509Certificate2? primaryCert = null;

        try
        {
            primaryCert = CreatePrimaryUnderlyingCertificate(storageFlags, password);
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
                        ImportPrivateKeyInto(rsa, password);
                        return primaryCert.CopyWithPrivateKey(rsa);
                    }

                default:
                    throw new NotSupportedException($"Unsupported certificate key algorithm: {keyAlgorithm}");
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
    protected abstract X509Certificate2 CreatePrimaryUnderlyingCertificate(X509KeyStorageFlags storageFlags, ReadOnlySpan<char> password);

    /// <summary>
    /// Imports the private key into the specified RSA instance. Note that this
    /// method is only called if <see cref="HasSeparatePrivateKeySource"/> is <c>true</c>.
    /// </summary>
    protected abstract void ImportPrivateKeyInto(RSA rsa, ReadOnlySpan<char> password);

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
            this._separatePemEncodedPrivateKey = separatePemEncodedPrivateKey is not null
                ? Encoding.ASCII.GetString(separatePemEncodedPrivateKey.Value.Span)
                : null;
        }

        /// <inheritdoc />
        protected override X509Certificate2 CreatePrimaryUnderlyingCertificate(
                X509KeyStorageFlags storageFlags,
                ReadOnlySpan<char> password
            )
        {
            return new(this._pemEncodedCertificate.Span, null, storageFlags);
        }

        /// <inheritdoc />
        protected override void ImportPrivateKeyInto(RSA rsa, ReadOnlySpan<char> password)
        {
            if (password.IsEmpty)
            {
                rsa.ImportFromPem(this._separatePemEncodedPrivateKey);
            }
            else
            {
                try
                {
                    rsa.ImportFromEncryptedPem(this._separatePemEncodedPrivateKey, password);
                }
                catch (ArgumentException)
                {
                    var pemFile = new PemFileInfo(this._separatePemEncodedPrivateKey);
                    if (pemFile.Blocks.Length == 1 && pemFile.Blocks[0].BlockType.Equals("RSA PRIVATE KEY", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new NotSupportedException("PEM files in the PKCS#1 format are not supported. Only the PKCS#8 format is supported.");
                    }

                    throw;
                }
            }
        }
    }

    private sealed class PfxCertificateSource : TlsCertificateSource
    {
        private readonly ReadOnlyMemory<byte> _encodedCertificate;

        /// <inheritdoc />
        protected override bool HasSeparatePrivateKeySource => false;

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        public PfxCertificateSource(ReadOnlyMemory<byte> encodedCertificate, ReadOnlyMemory<byte>? separateEncodedPrivateKey)
        {
            if (separateEncodedPrivateKey is not null)
            {
                throw new NotSupportedException("Pfx certificates can't have a separate private key.");
            }

            this._encodedCertificate = encodedCertificate;
        }

        /// <inheritdoc />
        protected override X509Certificate2 CreatePrimaryUnderlyingCertificate(
                X509KeyStorageFlags storageFlags,
                ReadOnlySpan<char> password
            )
        {
            if (password.IsEmpty)
            {
                return new(this._encodedCertificate.Span, password: null, storageFlags);
            }
            else
            {
                return new(this._encodedCertificate.Span, password: password, storageFlags);
            }
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        protected override void ImportPrivateKeyInto(RSA rsa, ReadOnlySpan<char> password)
        {
            throw new NotSupportedException("Pfx certificates can't have a separate private key.");
        }
    }
}
