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

using System.IO.Abstractions;
using System.Text;

using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.Core.IO;

/// <summary>
/// Represents the path to a file.
/// </summary>
/// <remarks>
/// The difference to <see cref="FileInfo"/> is that this type does not need <see cref="Path.GetFullPath(string)"/> to work.
/// Instead it just represents a string that's supposed to be a file path. Also, it's implicitly convertible from <c>string</c>
/// (and <see cref="FileInfo"/>) and supports <see cref="IFileSystem"/> in its methods.
/// </remarks>
/// <seealso cref="DirectoryPath"/>
public readonly struct FilePath : IEquatable<FilePath>
{
    private readonly string? _value;

    /// <summary>
    /// The string value of this file path.
    /// </summary>
    public string Value => this._value ?? throw new InvalidOperationException("Uninitialized instance.");

    /// <summary>
    /// The name of the file.
    ///
    /// <para>Note: Only supports paths that are natively supported on the current operating system. I.e.,
    /// if this path is in Windows path syntax (e.g. "c:\abc\def") and this code executes on
    /// a non-Windows system (e.g. Linux), the name will not be detected correctly. This is a limitation
    /// of <seealso cref="Path.GetFileName(string)"/>.</para>
    /// </summary>
    public string Name => Path.GetFileName(this.Value);

    /// <summary>
    /// Constructs a file path from a string.
    /// </summary>
    /// <param name="value">The value; must not be null, empty or just whitespace and must
    /// not end with <c>/</c> or <c>\</c>.</param>
    public FilePath(string value)
    {
        Validate.ArgumentWithName(nameof(value)).IsNotNullOrWhiteSpace(value);

        switch (value[^1])
        {
            case '/':
                throw new ArgumentException("A file path must not end with '/'.", nameof(value));
            case '\\':
                throw new ArgumentException("A file path must not end with '\\'.", nameof(value));
        }

        this._value = value;
    }

    /// <summary>
    /// Constructs a file path from a parent directory and a file name.
    /// </summary>
    /// <param name="directoryPath">The directory of the file path</param>
    /// <param name="fileName">The name of the file.</param>
    public FilePath(DirectoryPath directoryPath, string fileName)
        : this(Path.Combine(directoryPath.Value, fileName))
    {
    }

    /// <summary>
    /// Implicitly converts the specified string into a <see cref="FilePath"/> instance.
    /// </summary>
    public static implicit operator FilePath(string value)
    {
        return new(value);
    }

    /// <summary>
    /// Implicitly converts the specified string into a <see cref="FilePath"/> instance.
    /// </summary>
    [ContractAnnotation("null => null; notnull => notnull")]
    public static implicit operator FilePath?(string? value)
    {
        if (value is null)
        {
            return null;
        }

        return new(value);
    }

    /// <summary>
    /// Implicitly converts the specified <see cref="FileInfo"/> into a <see cref="FilePath"/> instance.
    /// </summary>
    public static implicit operator FilePath(FileInfo value)
    {
        return new(value.FullName);
    }

    /// <summary>
    /// Implicitly converts the specified <see cref="FileInfo"/> into a <see cref="FilePath"/> instance.
    /// </summary>
    [ContractAnnotation("null => null; notnull => notnull")]
    public static implicit operator FilePath?(FileInfo? value)
    {
        if (value is null)
        {
            return null;
        }

        return new(value.FullName);
    }

    /// <summary>
    /// Returns the absolute path of this file path.
    /// </summary>
    /// <param name="fileSystem">The file system to use; if <c>null</c>, <see cref="RealFileSystem.Instance"/> will be used.</param>
    [Pure]
    public FilePath AsAbsolutePath(IFileSystem? fileSystem = null)
    {
        return (fileSystem ?? RealFileSystem.Instance).Path.GetFullPath(this.Value);
    }

    /// <summary>
    /// Creates a <see cref="FileInfo"/> instances from this file path.
    /// </summary>
    [Pure]
    public FileInfo ToFileInfo()
    {
        return new(this.Value);
    }

    /// <summary>
    /// Checks whether the file of this file path exists.
    /// </summary>
    /// <param name="fileSystem">The file system to use; if <c>null</c>, <see cref="RealFileSystem.Instance"/> will be used.</param>
    [Pure]
    public bool Exists(IFileSystem? fileSystem = null)
    {
        return (fileSystem ?? RealFileSystem.Instance).File.Exists(this.Value);
    }

    /// <summary>
    /// Returns the size of the file this path points to.
    /// </summary>
    /// <param name="fileSystem">The file system to use; if <c>null</c>, <see cref="RealFileSystem.Instance"/> will be used.</param>
    [Pure]
    public long GetSize(IFileSystem? fileSystem = null)
    {
        return (fileSystem ?? RealFileSystem.Instance).FileInfo.FromFileName(this.Value).Length;
    }

    /// <summary>
    /// Returns the path to the directory this file is contained in.
    /// </summary>
    /// <param name="fileSystem">The file system to use; if <c>null</c>, <see cref="RealFileSystem.Instance"/> will be used.</param>
    [Pure]
    public DirectoryPath GetDirectory(IFileSystem? fileSystem = null)
    {
        return (fileSystem ?? RealFileSystem.Instance).FileInfo.FromFileName(this.Value).DirectoryName;
    }

    /// <inheritdoc cref="File.Create(string)"/>
    public Stream Create(IFileSystem? fileSystem = null)
    {
        return (fileSystem ?? RealFileSystem.Instance).File.Create(this.Value);
    }

    /// <inheritdoc cref="File.Create(string,int)"/>
    public Stream Create(int bufferSize, IFileSystem? fileSystem = null)
    {
        return (fileSystem ?? RealFileSystem.Instance).File.Create(this.Value, bufferSize);
    }

    /// <inheritdoc cref="File.Create(string,int,FileOptions)"/>
    public Stream Create(int bufferSize, FileOptions options, IFileSystem? fileSystem = null)
    {
        return (fileSystem ?? RealFileSystem.Instance).File.Create(this.Value, bufferSize, options);
    }

    /// <inheritdoc cref="File.Open(string,FileMode)"/>
    public Stream Open(FileMode mode, IFileSystem? fileSystem = null)
    {
        return (fileSystem ?? RealFileSystem.Instance).File.Open(this.Value, mode);
    }

    /// <inheritdoc cref="File.Open(string,FileMode,FileAccess)"/>
    public Stream Open(FileMode mode, FileAccess access, IFileSystem? fileSystem = null)
    {
        return (fileSystem ?? RealFileSystem.Instance).File.Open(this.Value, mode, access);
    }

    /// <inheritdoc cref="File.Open(string,FileMode,FileAccess,FileShare)"/>
    public Stream Open(FileMode mode, FileAccess access, FileShare share, IFileSystem? fileSystem = null)
    {
        return (fileSystem ?? RealFileSystem.Instance).File.Open(this.Value, mode, access, share);
    }

    /// <inheritdoc cref="File.OpenRead"/>
    public Stream OpenRead(IFileSystem? fileSystem = null)
    {
        return (fileSystem ?? RealFileSystem.Instance).File.OpenRead(this.Value);
    }

    /// <inheritdoc cref="File.OpenWrite"/>
    public Stream OpenWrite(IFileSystem? fileSystem = null)
    {
        return (fileSystem ?? RealFileSystem.Instance).File.OpenWrite(this.Value);
    }

    /// <inheritdoc cref="File.Copy(string,string,bool)"/>
    public void Copy(string destFileName, bool overwrite, IFileSystem? fileSystem = null)
    {
        (fileSystem ?? RealFileSystem.Instance).File.Copy(this.Value, destFileName, overwrite);
    }

    /// <inheritdoc cref="File.Move(string,string,bool)"/>
    public void Move(string destFileName, bool overwrite, IFileSystem? fileSystem = null)
    {
        (fileSystem ?? RealFileSystem.Instance).File.Move(this.Value, destFileName, overwrite);
    }

    /// <inheritdoc cref="File.Delete"/>
    public void Delete(IFileSystem? fileSystem = null)
    {
        (fileSystem ?? RealFileSystem.Instance).File.Delete(this.Value);
    }

    /// <inheritdoc cref="File.GetAttributes"/>
    public FileAttributes GetAttributes(IFileSystem? fileSystem = null)
    {
        return (fileSystem ?? RealFileSystem.Instance).File.GetAttributes(this.Value);
    }

    /// <inheritdoc cref="File.SetAttributes"/>
    public void SetAttributes(FileAttributes fileAttributes, IFileSystem? fileSystem = null)
    {
        (fileSystem ?? RealFileSystem.Instance).File.SetAttributes(this.Value, fileAttributes);
    }

    /// <inheritdoc cref="File.GetCreationTimeUtc"/>
    public DateTime GetCreationTimeUtc(IFileSystem? fileSystem = null)
    {
        return (fileSystem ?? RealFileSystem.Instance).File.GetCreationTimeUtc(this.Value);
    }

    /// <inheritdoc cref="File.GetLastAccessTimeUtc"/>
    public DateTime GetLastAccessTimeUtc(IFileSystem? fileSystem = null)
    {
        return (fileSystem ?? RealFileSystem.Instance).File.GetLastAccessTimeUtc(this.Value);
    }

    /// <inheritdoc cref="File.GetLastWriteTimeUtc"/>
    public DateTime GetLastWriteTimeUtc(IFileSystem? fileSystem = null)
    {
        return (fileSystem ?? RealFileSystem.Instance).File.GetLastWriteTimeUtc(this.Value);
    }

    /// <inheritdoc cref="File.SetCreationTimeUtc"/>
    public void SetCreationTimeUtc(DateTime creationTimeUtc, IFileSystem? fileSystem = null)
    {
        (fileSystem ?? RealFileSystem.Instance).File.SetCreationTimeUtc(this.Value, creationTimeUtc);
    }

    /// <inheritdoc cref="File.SetLastAccessTimeUtc"/>
    public void SetLastAccessTimeUtc(DateTime lastAccessTimeUtc, IFileSystem? fileSystem = null)
    {
        (fileSystem ?? RealFileSystem.Instance).File.SetLastAccessTimeUtc(this.Value, lastAccessTimeUtc);
    }

    /// <inheritdoc cref="File.SetLastWriteTimeUtc"/>
    public void SetLastWriteTimeUtc(DateTime lastWriteTimeUtc, IFileSystem? fileSystem = null)
    {
        (fileSystem ?? RealFileSystem.Instance).File.SetLastWriteTimeUtc(this.Value, lastWriteTimeUtc);
    }

    /// <inheritdoc cref="File.ReadAllBytes"/>
    public byte[] ReadAllBytes(IFileSystem? fileSystem = null)
    {
        return (fileSystem ?? RealFileSystem.Instance).File.ReadAllBytes(this.Value);
    }

    /// <inheritdoc cref="File.ReadAllBytesAsync"/>
    public Task<byte[]> ReadAllBytesAsync(IFileSystem? fileSystem = null, CancellationToken cancellationToken = default)
    {
        return (fileSystem ?? RealFileSystem.Instance).File.ReadAllBytesAsync(this.Value, cancellationToken);
    }

    /// <inheritdoc cref="File.ReadLines(string,Encoding)"/>
    public IEnumerable<string> ReadLines(Encoding encoding, IFileSystem? fileSystem = null)
    {
        return (fileSystem ?? RealFileSystem.Instance).File.ReadLines(this.Value, encoding);
    }

    /// <inheritdoc cref="File.ReadAllLines(string,Encoding)"/>
    public string[] ReadAllLines(Encoding encoding, IFileSystem? fileSystem = null)
    {
        return (fileSystem ?? RealFileSystem.Instance).File.ReadAllLines(this.Value, encoding);
    }

    /// <inheritdoc cref="File.ReadAllLinesAsync(string,Encoding,CancellationToken)"/>
    public Task<string[]> ReadAllLinesAsync(Encoding encoding, IFileSystem? fileSystem = null, CancellationToken cancellationToken = default)
    {
        return (fileSystem ?? RealFileSystem.Instance).File.ReadAllLinesAsync(this.Value, encoding, cancellationToken);
    }

    /// <inheritdoc cref="File.ReadAllText(string,Encoding)"/>
    public string ReadAllText(Encoding encoding, IFileSystem? fileSystem = null)
    {
        return (fileSystem ?? RealFileSystem.Instance).File.ReadAllText(this.Value, encoding);
    }

    ///<inheritdoc cref="File.ReadAllTextAsync(string,Encoding,CancellationToken)"/>
    public Task<string> ReadAllTextAsync(Encoding encoding, IFileSystem? fileSystem = null, CancellationToken cancellationToken = default)
    {
        return (fileSystem ?? RealFileSystem.Instance).File.ReadAllTextAsync(this.Value, encoding, cancellationToken);
    }

    /// <inheritdoc cref="File.AppendAllLines(string,IEnumerable{string},Encoding)"/>
    public void AppendAllLines(IEnumerable<string> contents, Encoding encoding, IFileSystem? fileSystem = null)
    {
        (fileSystem ?? RealFileSystem.Instance).File.AppendAllLines(this.Value, contents, encoding);
    }

    /// <inheritdoc cref="File.AppendAllLinesAsync(string,IEnumerable{string},Encoding,CancellationToken)"/>
    public Task AppendAllLinesAsync(IEnumerable<string> contents, Encoding encoding, IFileSystem? fileSystem = null, CancellationToken cancellationToken = default)
    {
        return (fileSystem ?? RealFileSystem.Instance).File.AppendAllLinesAsync(this.Value, contents, encoding, cancellationToken);
    }

    /// <inheritdoc cref="File.AppendAllText(string,string,Encoding)"/>
    public void AppendAllText(string contents, Encoding encoding, IFileSystem? fileSystem = null)
    {
        (fileSystem ?? RealFileSystem.Instance).File.AppendAllText(this.Value, contents, encoding);
    }

    /// <inheritdoc cref="File.AppendAllTextAsync(string,string,Encoding,CancellationToken)"/>
    public Task AppendAllTextAsync(string contents, Encoding encoding, IFileSystem? fileSystem = null, CancellationToken cancellationToken = default)
    {
        return (fileSystem ?? RealFileSystem.Instance).File.AppendAllTextAsync(this.Value, contents, encoding, cancellationToken);
    }

    /// <inheritdoc cref="File.WriteAllBytes"/>
    public void WriteAllBytes(byte[] bytes, IFileSystem? fileSystem = null)
    {
        (fileSystem ?? RealFileSystem.Instance).File.WriteAllBytes(this.Value, bytes);
    }

    /// <inheritdoc cref="File.WriteAllBytesAsync"/>
    public Task WriteAllBytesAsync(byte[] bytes, IFileSystem? fileSystem = null, CancellationToken cancellationToken = default)
    {
        return (fileSystem ?? RealFileSystem.Instance).File.WriteAllBytesAsync(this.Value, bytes, cancellationToken);
    }

    /// <inheritdoc cref="File.WriteAllLines(string,IEnumerable{string},Encoding)"/>
    public void WriteAllLines(IEnumerable<string> contents, Encoding encoding, IFileSystem? fileSystem = null)
    {
        (fileSystem ?? RealFileSystem.Instance).File.WriteAllLines(this.Value, contents, encoding);
    }

    /// <inheritdoc cref="File.WriteAllLinesAsync(string,IEnumerable{string},Encoding,CancellationToken)"/>
    public Task WriteAllLinesAsync(IEnumerable<string> contents, Encoding encoding, IFileSystem? fileSystem = null, CancellationToken cancellationToken = default)
    {
        return (fileSystem ?? RealFileSystem.Instance).File.WriteAllLinesAsync(this.Value, contents, encoding, cancellationToken);
    }

    /// <inheritdoc cref="File.WriteAllText(string,string,Encoding)"/>
    public void WriteAllText(string contents, Encoding encoding, IFileSystem? fileSystem = null)
    {
        (fileSystem ?? RealFileSystem.Instance).File.WriteAllText(this.Value, contents, encoding);
    }

    /// <inheritdoc cref="File.WriteAllTextAsync(string,string,Encoding,CancellationToken)"/>
    public Task WriteAllTextAsync(string contents, Encoding encoding, IFileSystem? fileSystem = null, CancellationToken cancellationToken = default)
    {
        return (fileSystem ?? RealFileSystem.Instance).File.WriteAllTextAsync(this.Value, contents, encoding, cancellationToken);
    }

    /// <inheritdoc />
    public bool Equals(FilePath other)
    {
        return this._value == other._value;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is FilePath other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return (this._value != null ? this._value.GetHashCode() : 0);
    }

    /// <summary>
    /// == operator
    /// </summary>
    public static bool operator ==(FilePath left, FilePath right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// != operator
    /// </summary>
    public static bool operator !=(FilePath left, FilePath right)
    {
        return !left.Equals(right);
    }

    /// <summary>
    /// Returns <see cref="Value"/>.
    /// </summary>
    public override string ToString()
    {
        return this._value ?? "";
    }
}