// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using JetBrains.Annotations;

namespace AppMotor.Core.IO;

/// <summary>
/// Represents a read-only version of <see cref="MemoryStream"/>.
/// </summary>
[PublicAPI]
public class ReadOnlyMemoryStream : ReadOnlyStream
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="buffer">The buffer to wrap in this read-only stream</param>
    public ReadOnlyMemoryStream(ArraySegment<byte> buffer)
        : base(CreateMemoryStreamFromArraySegment(buffer))
    {
    }

    [MustUseReturnValue]
    private static MemoryStream CreateMemoryStreamFromArraySegment(ArraySegment<byte> buffer)
    {
        if (buffer.Array is null)
        {
            if (buffer.Offset == 0 && buffer.Count == 0)
            {
                return new MemoryStream(Array.Empty<byte>(), 0, 0, writable: false);
            }
            else
            {
                // Judging from the code of ArraySegment, this should never happen.
                throw new ArgumentException("The array segment contains no array.", nameof(buffer));
            }
        }
        else
        {
            return new MemoryStream(buffer.Array, buffer.Offset, buffer.Count, writable: false);
        }
    }
}
