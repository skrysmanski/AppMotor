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
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.Core.IO
{
    /// <summary>
    /// Represents the path to a directory.
    ///
    /// <para>Note: The implementation of <see cref="Delete"/> behaves differently than <see cref="Directory.Delete(string)"/>
    /// if the directory is not empty. See there for details.</para>
    /// </summary>
    /// <remarks>
    /// The difference to <see cref="DirectoryInfo"/> is that this type does not need <see cref="Path.GetFullPath(string)"/> to work.
    /// Instead it just represents a string that's supposed to be a directory path. Also, it's implicitly convertible from <c>string</c>
    /// (and <see cref="DirectoryInfo"/>) and supports <see cref="IFileSystem"/> in its methods.
    /// </remarks>
    /// <seealso cref="FilePath"/>
    public readonly struct DirectoryPath : IEquatable<DirectoryPath>
    {
        private readonly string? _value;

        /// <summary>
        /// The string value of this directory path. Note that this value will never end with '/' or '\'.
        /// </summary>
        public string Value => this._value ?? throw new InvalidOperationException("Uninitialized instance.");

        /// <summary>
        /// The name of the directory.
        /// </summary>
        public string Name => Path.GetFileName(this.Value);

        /// <summary>
        /// Constructs a directory path from a string.
        /// </summary>
        public DirectoryPath(string value)
        {
            Validate.ArgumentWithName(nameof(value)).IsNotNullOrWhiteSpace(value);

            // NOTE: We trim '/' or '\' from the end so that the "Name" property works correctly.
            this._value = value.TrimEnd('/', '\\');
        }

        /// <summary>
        /// Constructs a directory path from a parent directory and a directory name.
        /// </summary>
        /// <param name="directoryPath">The parent directory of the new directory path</param>
        /// <param name="fileName">The name of the directory.</param>
        public DirectoryPath(DirectoryPath directoryPath, string fileName)
            : this(Path.Combine(directoryPath.Value, fileName))
        {
        }

        /// <summary>
        /// Implicitly converts the specified string into a <see cref="DirectoryPath"/> instance.
        /// </summary>
        public static implicit operator DirectoryPath(string value)
        {
            return new(value);
        }

        /// <summary>
        /// Implicitly converts the specified string into a <see cref="DirectoryPath"/> instance.
        /// </summary>
        [ContractAnnotation("null => null; notnull => notnull")]
        public static implicit operator DirectoryPath?(string? value)
        {
            if (value is null)
            {
                return null;
            }

            return new(value);
        }

        /// <summary>
        /// Implicitly converts the specified <see cref="DirectoryInfo"/> into a <see cref="DirectoryPath"/> instance.
        /// </summary>
        public static implicit operator DirectoryPath(DirectoryInfo value)
        {
            return new(value.FullName);
        }

        /// <summary>
        /// Implicitly converts the specified <see cref="DirectoryInfo"/> into a <see cref="DirectoryPath"/> instance.
        /// </summary>
        [ContractAnnotation("null => null; notnull => notnull")]
        public static implicit operator DirectoryPath?(DirectoryInfo? value)
        {
            if (value is null)
            {
                return null;
            }

            return new(value.FullName);
        }

        /// <summary>
        /// Returns the absolute path of this directory path.
        /// </summary>
        /// <param name="fileSystem">The file system to use; if <c>null</c>, <see cref="RealFileSystem.Instance"/> will be used.</param>
        [Pure]
        public DirectoryPath AsAbsolutePath(IFileSystem? fileSystem = null)
        {
            return (fileSystem ?? RealFileSystem.Instance).Path.GetFullPath(this.Value);
        }

        /// <summary>
        /// Creates a <see cref="DirectoryInfo"/> instances from this file path.
        /// </summary>
        [Pure]
        public DirectoryInfo ToDirectoryInfo()
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
            return (fileSystem ?? RealFileSystem.Instance).Directory.Exists(this.Value);
        }

        /// <inheritdoc cref="Directory.CreateDirectory(string)"/>
        public void CreateDirectory(IFileSystem? fileSystem = null)
        {
            (fileSystem ?? RealFileSystem.Instance).Directory.CreateDirectory(this.Value);
        }

        /// <inheritdoc cref="Directory.Move"/>
        public void Move(string destDirName, IFileSystem? fileSystem = null)
        {
            (fileSystem ?? RealFileSystem.Instance).Directory.Move(this.Value, destDirName);
        }

        /// <summary>
        /// Deletes this directory - no matter whether it's empty or not.
        ///
        /// <para>This method differs from <see cref="Directory.Delete(string)"/> in that this method also deletes
        /// non-empty directories.</para>
        /// </summary>
        public void Delete(IFileSystem? fileSystem = null)
        {
            (fileSystem ?? RealFileSystem.Instance).Directory.Delete(this.Value, recursive: true);
        }

        /// <inheritdoc cref="Directory.GetParent"/>
        public DirectoryPath GetParent(IFileSystem? fileSystem = null)
        {
            return (fileSystem ?? RealFileSystem.Instance).Directory.GetParent(this.Value).FullName;
        }

        /// <inheritdoc cref="Directory.GetDirectoryRoot"/>
        public DirectoryPath GetDirectoryRoot(IFileSystem? fileSystem = null)
        {
            return (fileSystem ?? RealFileSystem.Instance).Directory.GetDirectoryRoot(this.Value);
        }

        /// <inheritdoc cref="Directory.GetDirectories(string)"/>
        public DirectoryPath[] GetDirectories(IFileSystem? fileSystem = null)
        {
            return EnumerateDirectories(fileSystem).ToArray();
        }

        /// <inheritdoc cref="Directory.GetDirectories(string,string)"/>
        public DirectoryPath[] GetDirectories(string searchPattern, IFileSystem? fileSystem = null)
        {
            return EnumerateDirectories(searchPattern, fileSystem).ToArray();
        }

        /// <inheritdoc cref="Directory.GetDirectories(string,string,SearchOption)"/>
        public DirectoryPath[] GetDirectories(string searchPattern, SearchOption searchOption, IFileSystem? fileSystem = null)
        {
            return EnumerateDirectories(searchPattern, searchOption, fileSystem).ToArray();
        }

        /// <inheritdoc cref="Directory.GetDirectories(string,string,EnumerationOptions)"/>
        public DirectoryPath[] GetDirectories(string searchPattern, EnumerationOptions enumerationOptions, IFileSystem? fileSystem = null)
        {
            return EnumerateDirectories(searchPattern, enumerationOptions, fileSystem).ToArray();
        }

        /// <inheritdoc cref="Directory.GetFiles(string)"/>
        public FilePath[] GetFiles(IFileSystem? fileSystem = null)
        {
            return EnumerateFiles(fileSystem).ToArray();
        }

        /// <inheritdoc cref="Directory.GetFiles(string,string)"/>
        public FilePath[] GetFiles(string searchPattern, IFileSystem? fileSystem = null)
        {
            return EnumerateFiles(searchPattern, fileSystem).ToArray();
        }

        /// <inheritdoc cref="Directory.GetFiles(string,string,SearchOption)"/>
        public FilePath[] GetFiles(string searchPattern, SearchOption searchOption, IFileSystem? fileSystem = null)
        {
            return EnumerateFiles(searchPattern, searchOption, fileSystem).ToArray();
        }

        /// <inheritdoc cref="Directory.GetFiles(string,string,EnumerationOptions)"/>
        public FilePath[] GetFiles(string searchPattern, EnumerationOptions enumerationOptions, IFileSystem? fileSystem = null)
        {
            return EnumerateFiles(searchPattern, enumerationOptions, fileSystem).ToArray();
        }

        /// <inheritdoc cref="Directory.GetFileSystemEntries(string)"/>
        public string[] GetFileSystemEntries(IFileSystem? fileSystem = null)
        {
            return (fileSystem ?? RealFileSystem.Instance).Directory.GetFileSystemEntries(this.Value);
        }

        /// <inheritdoc cref="Directory.GetFileSystemEntries(string,string)"/>
        public string[] GetFileSystemEntries(string searchPattern, IFileSystem? fileSystem = null)
        {
            return (fileSystem ?? RealFileSystem.Instance).Directory.GetFileSystemEntries(this.Value, searchPattern);
        }

        /// <inheritdoc cref="Directory.EnumerateDirectories(string)"/>
        public IEnumerable<DirectoryPath> EnumerateDirectories(IFileSystem? fileSystem = null)
        {
            return (fileSystem ?? RealFileSystem.Instance).Directory.EnumerateDirectories(this.Value).Select(dir => new DirectoryPath(dir));
        }

        /// <inheritdoc cref="Directory.EnumerateDirectories(string,string)"/>
        public IEnumerable<DirectoryPath> EnumerateDirectories(string searchPattern, IFileSystem? fileSystem = null)
        {
            return (fileSystem ?? RealFileSystem.Instance).Directory.EnumerateDirectories(this.Value, searchPattern).Select(dir => new DirectoryPath(dir));
        }

        /// <inheritdoc cref="Directory.EnumerateDirectories(string,string,SearchOption)"/>
        public IEnumerable<DirectoryPath> EnumerateDirectories(string searchPattern, SearchOption searchOption, IFileSystem? fileSystem = null)
        {
            return (fileSystem ?? RealFileSystem.Instance).Directory.EnumerateDirectories(this.Value, searchPattern, searchOption).Select(dir => new DirectoryPath(dir));
        }

        /// <inheritdoc cref="Directory.EnumerateDirectories(string,string,EnumerationOptions)"/>
        public IEnumerable<DirectoryPath> EnumerateDirectories(string searchPattern, EnumerationOptions enumerationOptions, IFileSystem? fileSystem = null)
        {
            return (fileSystem ?? RealFileSystem.Instance).Directory.EnumerateDirectories(this.Value, searchPattern, enumerationOptions).Select(dir => new DirectoryPath(dir));
        }

        /// <inheritdoc cref="Directory.EnumerateFiles(string)"/>
        public IEnumerable<FilePath> EnumerateFiles(IFileSystem? fileSystem = null)
        {
            return (fileSystem ?? RealFileSystem.Instance).Directory.EnumerateFiles(this.Value).Select(dir => new FilePath(dir));
        }

        /// <inheritdoc cref="Directory.EnumerateFiles(string,string)"/>
        public IEnumerable<FilePath> EnumerateFiles(string searchPattern, IFileSystem? fileSystem = null)
        {
            return (fileSystem ?? RealFileSystem.Instance).Directory.EnumerateFiles(this.Value, searchPattern).Select(dir => new FilePath(dir));
        }

        /// <inheritdoc cref="Directory.EnumerateFiles(string,string,SearchOption)"/>
        public IEnumerable<FilePath> EnumerateFiles(string searchPattern, SearchOption searchOption, IFileSystem? fileSystem = null)
        {
            return (fileSystem ?? RealFileSystem.Instance).Directory.EnumerateFiles(this.Value, searchPattern, searchOption).Select(dir => new FilePath(dir));
        }

        /// <inheritdoc cref="Directory.EnumerateFiles(string,string,EnumerationOptions)"/>
        public IEnumerable<FilePath> EnumerateFiles(string searchPattern, EnumerationOptions enumerationOptions, IFileSystem? fileSystem = null)
        {
            return (fileSystem ?? RealFileSystem.Instance).Directory.EnumerateFiles(this.Value, searchPattern, enumerationOptions).Select(dir => new FilePath(dir));
        }

        /// <inheritdoc cref="Directory.EnumerateFileSystemEntries(string)"/>
        public IEnumerable<string> EnumerateFileSystemEntries(IFileSystem? fileSystem = null)
        {
            return (fileSystem ?? RealFileSystem.Instance).Directory.EnumerateFileSystemEntries(this.Value);
        }

        /// <inheritdoc cref="Directory.EnumerateFileSystemEntries(string,string)"/>
        public IEnumerable<string> EnumerateFileSystemEntries(string searchPattern, IFileSystem? fileSystem = null)
        {
            return (fileSystem ?? RealFileSystem.Instance).Directory.EnumerateFileSystemEntries(this.Value, searchPattern);
        }

        /// <inheritdoc cref="Directory.EnumerateFileSystemEntries(string,string,SearchOption)"/>
        public IEnumerable<string> EnumerateFileSystemEntries(string searchPattern, SearchOption searchOption, IFileSystem? fileSystem = null)
        {
            return (fileSystem ?? RealFileSystem.Instance).Directory.EnumerateFileSystemEntries(this.Value, searchPattern, searchOption);
        }

        /// <inheritdoc cref="Directory.EnumerateFileSystemEntries(string,string,EnumerationOptions)"/>
        public IEnumerable<string> EnumerateFileSystemEntries(string searchPattern, EnumerationOptions enumerationOptions, IFileSystem? fileSystem = null)
        {
            return (fileSystem ?? RealFileSystem.Instance).Directory.EnumerateFileSystemEntries(this.Value, searchPattern, enumerationOptions);
        }

        /// <inheritdoc cref="Directory.GetCreationTimeUtc"/>
        public DateTime GetCreationTimeUtc(IFileSystem? fileSystem = null)
        {
            return (fileSystem ?? RealFileSystem.Instance).Directory.GetCreationTimeUtc(this.Value);
        }

        /// <inheritdoc cref="Directory.GetLastAccessTimeUtc"/>
        public DateTime GetLastAccessTimeUtc(IFileSystem? fileSystem = null)
        {
            return (fileSystem ?? RealFileSystem.Instance).Directory.GetLastAccessTimeUtc(this.Value);
        }

        /// <inheritdoc cref="Directory.GetLastWriteTimeUtc"/>
        public DateTime GetLastWriteTimeUtc(IFileSystem? fileSystem = null)
        {
            return (fileSystem ?? RealFileSystem.Instance).Directory.GetLastWriteTimeUtc(this.Value);
        }

        /// <inheritdoc cref="Directory.SetCreationTimeUtc"/>
        public void SetCreationTimeUtc(DateTime creationTimeUtc, IFileSystem? fileSystem = null)
        {
            (fileSystem ?? RealFileSystem.Instance).Directory.SetCreationTimeUtc(this.Value, creationTimeUtc);
        }

        /// <inheritdoc cref="Directory.SetLastAccessTimeUtc"/>
        public void SetLastAccessTimeUtc(DateTime lastAccessTimeUtc, IFileSystem? fileSystem = null)
        {
            (fileSystem ?? RealFileSystem.Instance).Directory.SetLastAccessTimeUtc(this.Value, lastAccessTimeUtc);
        }

        /// <inheritdoc cref="Directory.SetLastWriteTimeUtc"/>
        public void SetLastWriteTimeUtc(DateTime lastWriteTimeUtc, IFileSystem? fileSystem = null)
        {
            (fileSystem ?? RealFileSystem.Instance).Directory.SetLastWriteTimeUtc(this.Value, lastWriteTimeUtc);
        }

        /// <inheritdoc />
        public bool Equals(DirectoryPath other)
        {
            return this._value == other._value;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is DirectoryPath other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (this._value != null ? this._value.GetHashCode() : 0);
        }

        /// <summary>
        /// == operator
        /// </summary>
        public static bool operator ==(DirectoryPath left, DirectoryPath right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// != operator
        /// </summary>
        public static bool operator !=(DirectoryPath left, DirectoryPath right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this._value ?? "";
        }
    }
}
