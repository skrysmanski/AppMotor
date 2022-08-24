// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.IO.Abstractions;

using AppMotor.Core.IO;

using JetBrains.Annotations;

namespace AppMotor.Core.Certificates.Exporting;

/// <summary>
/// Exports a single certificate blob. This can either be only a public key or both the public key and the private
/// key combined into one blob.
/// </summary>
/// <seealso cref="DoubleBlobExporter"/>
public sealed class SingleBlobExporter
{
    private readonly Func<byte[]> _bytesExporter;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="bytesExporter">The exporter function; NOTE: Since the bytes
    /// may contain sensitive data, we store just the exporter function and only
    /// use it when needed. This way the sensitive data doesn't float around in
    /// memory unnecessarily. Also note that calling this method must create a
    /// fresh copy of the data - not a shared cache.</param>
    internal SingleBlobExporter(Func<byte[]> bytesExporter)
    {
        this._bytesExporter = bytesExporter;
    }

    /// <summary>
    /// Returns the blob as byte array.
    /// </summary>
    /// <remarks>
    /// The returned byte array is freshly created. The caller of this method takes
    /// full ownership of it (and does not need to fear it is shared with some other
    /// caller).
    /// </remarks>
    [MustUseReturnValue]
    public byte[] ToBytes()
    {
        return this._bytesExporter();
    }

    /// <summary>
    /// Stores the blob in the file system.
    /// </summary>
    /// <param name="filePath">The file path to the certificate file.</param>
    /// <param name="fileSystem">The file system to use; if <c>null</c>, <see cref="RealFileSystem.Instance"/> will be used.</param>
    public void ToFile(FilePath filePath, IFileSystem? fileSystem = null)
    {
        filePath.WriteAllBytes(this._bytesExporter(), fileSystem);
    }

    /// <summary>
    /// Stores the blob in the file system.
    /// </summary>
    /// <param name="filePath">The file path to the certificate file.</param>
    /// <param name="fileSystem">The file system to use; if <c>null</c>, <see cref="RealFileSystem.Instance"/> will be used.</param>
    public async Task ToFileAsync(FilePath filePath, IFileSystem? fileSystem = null)
    {
        await filePath.WriteAllBytesAsync(this._bytesExporter(), fileSystem).ConfigureAwait(false);
    }
}