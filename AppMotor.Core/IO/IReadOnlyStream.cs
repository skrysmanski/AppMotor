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

        void CopyTo(Stream destination);

        void CopyTo(Stream destination, int? bufferSize);

        Task CopyToAsync(Stream destination);

        Task CopyToAsync(Stream destination, int? bufferSize);

        Task CopyToAsync(Stream destination, CancellationToken cancellationToken);

        Task CopyToAsync(Stream destination, int? bufferSize, CancellationToken cancellationToken);

        [MustUseReturnValue]
        int Read(byte[] buffer);

        [MustUseReturnValue]
        int Read(byte[] buffer, int offset, int count);

        [MustUseReturnValue]
        byte? ReadByte();

        ValueTask<int> ReadAsync(Memory<byte> buffer);

        ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken);

        long Seek(long offset, SeekOrigin origin);
    }
}
