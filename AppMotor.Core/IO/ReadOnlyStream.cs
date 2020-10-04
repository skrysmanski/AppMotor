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
        public void CopyTo(Stream destination, int? bufferSize = null)
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
        public async Task CopyToAsync(Stream destination, int? bufferSize = null, CancellationToken cancellationToken = default)
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
        public byte? ReadByte()
        {
            byte[] sharedBuffer = ArrayPool<byte>.Shared.Rent(1);
            try
            {
                var numRead = Read(sharedBuffer.AsSpan());
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
        public int Read(Span<byte> buffer)
        {
            // NOTE: While the default implementation of "Stream.Read(Span)" may seem to have
            //   worse performance than "Read(byte[],int,int)" (because of the array being copied),
            //   most Stream implementations override this method to have a similar performance as
            //   "Read(byte[],int,int)". Although there is no way to detect this, the Span-based API
            //   is the newer one - so we're going to support it (instead of the older API).

            return this.m_underlyingStream.Read(buffer);
        }

        /// <inheritdoc />
        public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
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
