// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.IO.Abstractions;

using AppMotor.Core.IO;

using JetBrains.Annotations;

namespace AppMotor.Core.Certificates.Exporting;

/// <summary>
/// Exports two certificate blobs: the private key and the public key.
/// </summary>
/// <seealso cref="SingleBlobExporter"/>
public sealed class DoubleBlobExporter
{
    private readonly byte[] _publicKeyBytes;

    private readonly Func<byte[]> _privateKeyBytesExporterFunc;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="publicKeyBytes">The public key bytes.</param>
    /// <param name="privateKeyBytesExporterFunc">The exported function for the private key bytes; NOTE:
    /// Since the bytes contain sensitive data, we store just the exporter function and only use it when
    /// needed. This way the sensitive data doesn't float around in memory unnecessarily. Also note that
    /// calling this method must create a fresh copy of the data - not a shared cache.</param>
    public DoubleBlobExporter(byte[] publicKeyBytes, Func<byte[]> privateKeyBytesExporterFunc)
    {
        this._publicKeyBytes = publicKeyBytes;
        this._privateKeyBytesExporterFunc = privateKeyBytesExporterFunc;
    }

    /// <summary>
    /// Returns the blobs as byte arrays.
    /// </summary>
    /// <remarks>
    /// The returned byte arrays are freshly created. The caller of this method takes
    /// full ownership of them (and does not need to fear they're shared with some other
    /// caller).
    /// </remarks>
    [MustUseReturnValue]
    public (byte[] publicKeyBytes, byte[] privateKeyBytes) ToBytes()
    {
        return ((byte[])this._publicKeyBytes.Clone(), this._privateKeyBytesExporterFunc());
    }

    /// <summary>
    /// Stores the blobs in the file system.
    /// </summary>
    /// <param name="publicKeyFilePath">The file path to the public key file.</param>
    /// <param name="privateKeyFilePath">The file path to the private key file.</param>
    /// <param name="fileSystem">The file system to use; if <c>null</c>, <see cref="RealFileSystem.Instance"/> will be used.</param>
    public void ToFile(FilePath publicKeyFilePath, FilePath privateKeyFilePath, IFileSystem? fileSystem = null)
    {
        publicKeyFilePath.WriteAllBytes(this._publicKeyBytes, fileSystem);
        privateKeyFilePath.WriteAllBytes(this._privateKeyBytesExporterFunc(), fileSystem);
    }

    /// <summary>
    /// Stores the blobs in the file system.
    /// </summary>
    /// <param name="publicKeyFilePath">The file path to the public key file.</param>
    /// <param name="privateKeyFilePath">The file path to the private key file.</param>
    /// <param name="fileSystem">The file system to use; if <c>null</c>, <see cref="RealFileSystem.Instance"/> will be used.</param>
    public async Task ToFileAsync(FilePath publicKeyFilePath, FilePath privateKeyFilePath, IFileSystem? fileSystem = null)
    {
        var publicKeyWriteTask = publicKeyFilePath.WriteAllBytesAsync(this._publicKeyBytes, fileSystem);
        var privateKeyWriteTask = privateKeyFilePath.WriteAllBytesAsync(this._privateKeyBytesExporterFunc(), fileSystem);

        await Task.WhenAll(publicKeyWriteTask, privateKeyWriteTask).ConfigureAwait(false);
    }
}