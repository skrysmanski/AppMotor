﻿#region License
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

using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.Core.IO
{
    /// <summary>
    /// Represents a read-only view on a <see cref="Stream"/>.
    /// </summary>
    [PublicAPI]
    public class ReadOnlyStream : AsyncDisposable, IReadOnlyStream
    {
        private readonly Stream m_underlyingStream;

        /// <inheritdoc />
        public bool CanSeek => this.m_underlyingStream.CanSeek;

        /// <inheritdoc />
        public long Position
        {
            get => this.m_underlyingStream.Position;
            set => this.m_underlyingStream.Position = value;
        }

        /// <inheritdoc />
        public long Length => this.m_underlyingStream.Length;

        /// <inheritdoc />
        public bool CanTimeout => this.m_underlyingStream.CanTimeout;

        /// <inheritdoc />
        public TimeSpan ReadTimeout
        {
            get => TimeSpan.FromMilliseconds(this.m_underlyingStream.ReadTimeout);
            set => this.m_underlyingStream.ReadTimeout = (int)value.TotalMilliseconds;
        }

        /// <inheritdoc />
        public TimeSpan WriteTimeout
        {
            get => TimeSpan.FromMilliseconds(this.m_underlyingStream.WriteTimeout);
            set => this.m_underlyingStream.WriteTimeout = (int)value.TotalMilliseconds;
        }

        public ReadOnlyStream(Stream underlyingStream)
        {
            Validate.Argument.IsNotNull(underlyingStream, nameof(underlyingStream));

            if (!underlyingStream.CanRead)
            {
                throw new ArgumentException("The specified stream is not readable.", nameof(underlyingStream));
            }

            this.m_underlyingStream = underlyingStream;
        }

        /// <inheritdoc />
        protected override void DisposeManagedResources()
        {
            this.m_underlyingStream.Dispose();

            base.DisposeManagedResources();
        }

        /// <inheritdoc />
        protected override async ValueTask DisposeAsyncCore()
        {
            await this.m_underlyingStream.DisposeAsync().ConfigureAwait(false);
        }

        /// <inheritdoc />
        public void CopyTo(Stream destination)
        {
            CopyTo(destination, bufferSize: null);
        }

        /// <inheritdoc />
        public void CopyTo(Stream destination, int? bufferSize)
        {
            if (bufferSize == null)
            {
                this.m_underlyingStream.CopyTo(destination);
            }
            else
            {
                this.m_underlyingStream.CopyTo(destination, bufferSize.Value);
            }
        }

        /// <inheritdoc />
        public Task CopyToAsync(Stream destination)
        {
            return CopyToAsync(destination, bufferSize: null, CancellationToken.None);
        }

        /// <inheritdoc />
        public Task CopyToAsync(Stream destination, int? bufferSize)
        {
            return CopyToAsync(destination, bufferSize, CancellationToken.None);
        }

        /// <inheritdoc />
        public Task CopyToAsync(Stream destination, CancellationToken cancellationToken)
        {
            return CopyToAsync(destination, bufferSize: null, CancellationToken.None);
        }

        /// <inheritdoc />
        public async Task CopyToAsync(Stream destination, int? bufferSize, CancellationToken cancellationToken)
        {
            if (bufferSize == null)
            {
                await this.m_underlyingStream.CopyToAsync(destination, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await this.m_underlyingStream.CopyToAsync(destination, bufferSize.Value, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        public int Read(byte[] buffer)
        {
            Validate.Argument.IsNotNull(buffer, nameof(buffer));

            return Read(buffer, 0, buffer.Length);
        }

        /// <inheritdoc />
        public int Read(byte[] buffer, int offset, int count)
        {
            return this.m_underlyingStream.Read(buffer, offset, count);
        }

        /// <inheritdoc />
        public byte? ReadByte()
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

        /// <inheritdoc />
        public ValueTask<int> ReadAsync(Memory<byte> buffer)
        {
            return ReadAsync(buffer, CancellationToken.None);
        }

        /// <inheritdoc />
        public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken)
        {
            return this.m_underlyingStream.ReadAsync(buffer, cancellationToken);
        }

        /// <inheritdoc />
        public long Seek(long offset, SeekOrigin origin)
        {
            return this.m_underlyingStream.Seek(offset, origin);
        }
    }
}
