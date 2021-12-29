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