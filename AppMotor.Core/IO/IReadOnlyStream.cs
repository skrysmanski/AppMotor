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
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace AppMotor.Core.IO
{
    /// <summary>
    /// Represents a stream that can only be read (but not written).
    /// </summary>
    [PublicAPI]
    public interface IReadOnlyStream : IDisposable, IAsyncDisposable
    {
        bool CanSeek { get; }

        long Position { get; set; }

        long Length { get; }

        bool CanTimeout { get; }

        TimeSpan ReadTimeout { get; set; }

        TimeSpan WriteTimeout { get; set; }

        void CopyTo(Stream destination)
        {
            CopyTo(destination, bufferSize: null);
        }

        void CopyTo(Stream destination, int? bufferSize);

        Task CopyToAsync(Stream destination)
        {
            return CopyToAsync(destination, CancellationToken.None);
        }

        Task CopyToAsync(Stream destination, int? bufferSize)
        {
            return CopyToAsync(destination, bufferSize, CancellationToken.None);
        }

        Task CopyToAsync(Stream destination, CancellationToken cancellationToken)
        {
            return CopyToAsync(destination, bufferSize: null, cancellationToken);
        }

        Task CopyToAsync(Stream destination, int? bufferSize, CancellationToken cancellationToken);

        int Read(byte[] buffer, int offset, int count);

        byte? ReadByte()
        {
            byte[] sharedBuffer = ArrayPool<byte>.Shared.Rent(1);
            try
            {
                var numRead = Read(sharedBuffer, 0, 1);
                if (numRead == 0)
                {
                    return null;
                }

                return sharedBuffer[0];
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(sharedBuffer);
            }
        }

        ValueTask<int> ReadAsync(Memory<byte> buffer)
        {
            return ReadAsync(buffer, CancellationToken.None);
        }

        ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken);

        long Seek(long offset, SeekOrigin origin);
    }
}
