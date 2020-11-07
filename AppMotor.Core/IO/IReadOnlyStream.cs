#region License
// Copyright 2020 AppMotor Framework (https://github.com/skrysmanski/AppMotor)
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
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace AppMotor.Core.IO
{
    /// <summary>
    /// Represents a <see cref="Stream"/> that can only be read (but not written).
    /// </summary>
    [PublicAPI]
    public interface IReadOnlyStream : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Whether the stream can change its position - either via <see cref="Position"/> or <see cref="Seek"/>.
        /// </summary>
        bool CanSeek { get; }

        /// <summary>
        /// The position within this stream. Note that the setter can only
        /// be used if <see cref="CanSeek"/> is <c>true</c>.
        /// </summary>
        /// <seealso cref="Seek"/>
        long Position { get; set; }

        /// <summary>
        /// The length of the stream in bytes.
        /// </summary>
        long Length { get; }

        /// <summary>
        /// Whether this stream supports read timeouts or not. If supported, the read timeout can be read or set
        /// via <see cref="ReadTimeout"/>.
        /// </summary>
        bool CanTimeout { get; }

        /// <summary>
        /// The timeout for read operations. Can only be used if <see cref="CanTimeout"/> is <c>true</c>.
        /// </summary>
        TimeSpan ReadTimeout { get; set; }

        /// <summary>
        /// Copies this stream's content into <paramref name="destination"/>.
        /// </summary>
        /// <param name="destination">The target of the copy operation</param>
        /// <param name="bufferSize">How many bytes to copy at most; if <c>null</c>,
        /// the complete remainder of this stream will be copied.</param>
        /// <seealso cref="CopyToAsync"/>
        void CopyTo(Stream destination, int? bufferSize = null);

        /// <summary>
        /// Copies this stream's content into <paramref name="destination"/>.
        /// </summary>
        /// <param name="destination">The target of the copy operation</param>
        /// <param name="bufferSize">How many bytes to copy at most; if <c>null</c>,
        /// the complete remainder of this stream will be copied.</param>
        /// <param name="cancellationToken">A cancellation token to cancel to copy operation.</param>
        /// <seealso cref="CopyTo"/>
        [MustUseReturnValue]
        Task CopyToAsync(Stream destination, int? bufferSize = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads the next byte from the stream. Returns <c>null</c>, if the end of the stream has been
        /// reached.
        /// </summary>
        [MustUseReturnValue]
        byte? ReadByte();

        /// <summary>
        /// Reads the next bytes into <paramref name="buffer"/>. Returns the number of bytes that were
        /// actually read; or 0 if the end of the stream has been reached.
        ///
        /// <para>Note: This method may return less bytes than would fit into the buffer even if the end
        /// of the stream has not yet been reached.</para>
        /// </summary>
        /// <seealso cref="ReadAsync"/>
        [MustUseReturnValue]
        int Read(Span<byte> buffer);

        /// <summary>
        /// Reads the next bytes into <paramref name="buffer"/>. Returns the number of bytes that were
        /// actually read; or 0 if the end of the stream has been reached.
        ///
        /// <para>Note: This method may return less bytes than would fit into the buffer even if the end
        /// of the stream has not yet been reached.</para>
        /// </summary>
        /// <seealso cref="Read"/>
        [MustUseReturnValue]
        ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the position within the current stream. Can only be used if <see cref="CanSeek"/> is
        /// <c>true</c>.
        /// </summary>
        /// <param name="offset">The number of bytes to move the current position based on <paramref name="origin"/>.</param>
        /// <param name="origin">The reference point used to obtain the new position.</param>
        /// <returns>The new position within this stream.</returns>
        long Seek(long offset, SeekOrigin origin);
    }
}
