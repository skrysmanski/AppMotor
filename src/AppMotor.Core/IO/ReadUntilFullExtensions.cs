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

using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.Core.IO;

/// <summary>
/// <c>Read()</c> extension methods for various stream implementations that guarantee the specified
/// numbers of bytes will be read. The regular <c>Read()</c> methods on streams (e.g. <see cref="Stream.Read(byte[],int,int)"/>)
/// are not required to fill the buffer - even if the end of the stream hasn't been reached.
/// </summary>
public static class ReadUntilFullExtensions
{
    /// <summary>
    /// Reads from this stream until the buffer is full or the end of the stream has been reached.
    ///
    /// <para>Note: Unlike <see cref="IReadOnlyStream.Read(Span{byte})"/>, this method guarantees
    /// that the whole <paramref name="buffer"/> will be filled - unless the end of the
    /// stream is reached (in which case less bytes will be read).
    /// </para>
    /// </summary>
    /// <param name="stream">the stream to read from</param>
    /// <param name="buffer">the buffer to read into</param>
    /// <returns>The number of bytes that have been read. Only smaller than the size of <paramref name="buffer"/>
    /// if the end of the stream has been reached.</returns>
    [PublicAPI, MustUseReturnValue]
    public static int ReadUntilFull(this IReadOnlyStream stream, Span<byte> buffer)
    {
        Validate.ArgumentWithName(nameof(stream)).IsNotNull(stream);

        int offset = 0;
        int count = buffer.Length;

        int totalBytesRead = 0;

        while (count > 0)
        {
            int readBytes = stream.Read(buffer.Slice(offset, count));
            if (readBytes == 0)
            {
                // End of stream
                break;
            }

            totalBytesRead += readBytes;
            offset += readBytes;
            count -= readBytes;
        }

        return totalBytesRead;
    }

    /// <summary>
    /// Reads from this stream until the buffer is full or the end of the stream has been reached.
    ///
    /// <para>Note: Unlike <see cref="IReadOnlyStream.ReadAsync(Memory{byte},CancellationToken)"/>, this method guarantees
    /// that the whole <paramref name="buffer"/> will be filled - unless the end of the
    /// stream is reached (in which case less bytes will be read).
    /// </para>
    /// </summary>
    /// <param name="stream">the stream to read from</param>
    /// <param name="buffer">the buffer to read into</param>
    /// <param name="cancellationToken">The cancellation token to cancel this read operation</param>
    /// <returns>The number of bytes that have been read. Only smaller than the size of <paramref name="buffer"/>
    /// if the end of the stream has been reached.</returns>
    [PublicAPI, MustUseReturnValue]
    public static async ValueTask<int> ReadUntilFullAsync(this IReadOnlyStream stream, Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        Validate.ArgumentWithName(nameof(stream)).IsNotNull(stream);

        int offset = 0;
        int count = buffer.Length;

        int totalBytesRead = 0;

        while (count > 0)
        {
            int readBytes = await stream.ReadAsync(buffer.Slice(offset, count), cancellationToken).ConfigureAwait(false);
            if (readBytes == 0)
            {
                // End of stream
                break;
            }

            totalBytesRead += readBytes;
            offset += readBytes;
            count -= readBytes;
        }

        return totalBytesRead;
    }

    /// <summary>
    /// Reads from this stream until the buffer is full or the end of the stream has been reached.
    ///
    /// <para>Note: Unlike <see cref="Stream.Read(Span{byte})"/>, this method guarantees
    /// that the whole <paramref name="buffer"/> will be filled - unless the end of the
    /// stream is reached (in which case less bytes will be read).
    /// </para>
    /// </summary>
    /// <param name="stream">the stream to read from</param>
    /// <param name="buffer">the buffer to read into</param>
    /// <returns>The number of bytes that have been read. Only smaller than the size of <paramref name="buffer"/>
    /// if the end of the stream has been reached.</returns>
    [PublicAPI, MustUseReturnValue]
    public static int ReadUntilFull(this Stream stream, Span<byte> buffer)
    {
        Validate.ArgumentWithName(nameof(stream)).IsNotNull(stream);

        int offset = 0;
        int count = buffer.Length;

        int totalBytesRead = 0;

        while (count > 0)
        {
            int readBytes = stream.Read(buffer.Slice(offset, count));
            if (readBytes == 0)
            {
                // End of stream
                break;
            }

            totalBytesRead += readBytes;
            offset += readBytes;
            count -= readBytes;
        }

        return totalBytesRead;
    }

    /// <summary>
    /// Reads from this stream until the buffer is full or the end of the stream has been reached.
    ///
    /// <para>Note: Unlike <see cref="Stream.ReadAsync(Memory{byte},CancellationToken)"/>, this method guarantees
    /// that the whole <paramref name="buffer"/> will be filled - unless the end of the
    /// stream is reached (in which case less bytes will be read).
    /// </para>
    /// </summary>
    /// <param name="stream">the stream to read from</param>
    /// <param name="buffer">the buffer to read into</param>
    /// <param name="cancellationToken">The cancellation token to cancel this read operation</param>
    /// <returns>The number of bytes that have been read. Only smaller than the size of <paramref name="buffer"/>
    /// if the end of the stream has been reached.</returns>
    [PublicAPI, MustUseReturnValue]
    public static async ValueTask<int> ReadUntilFullAsync(this Stream stream, Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        Validate.ArgumentWithName(nameof(stream)).IsNotNull(stream);

        int offset = 0;
        int count = buffer.Length;

        int totalBytesRead = 0;

        while (count > 0)
        {
            int readBytes = await stream.ReadAsync(buffer.Slice(offset, count), cancellationToken).ConfigureAwait(false);
            if (readBytes == 0)
            {
                // End of stream
                break;
            }

            totalBytesRead += readBytes;
            offset += readBytes;
            count -= readBytes;
        }

        return totalBytesRead;
    }
}