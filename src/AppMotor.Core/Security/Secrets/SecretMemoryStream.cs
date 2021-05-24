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
using System.IO;

using AppMotor.Core.Exceptions;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.Core.Security.Secrets
{
    public sealed class SecretMemoryStream : Stream
    {
        private const int MIN_CAPACITY = 256;

        [PublicAPI]
        public const int MAX_LENGTH = int.MaxValue;

        private int _position;

        private int _length;

        private readonly bool _isWritable;

        private SecretsArray<byte> _buffer;

        private bool IsOpen => this._buffer.DisposedState == DisposedStates.NotDisposed;

        public override bool CanRead => this.IsOpen;

        public override bool CanSeek => this.IsOpen;

        public override bool CanWrite => this.IsOpen && this._isWritable;

        public override long Length => this._length;

        public override long Position
        {
            get => this._position;
            set => Seek(value, SeekOrigin.Begin);
        }

        [PublicAPI]
        public int Capacity
        {
            get
            {
                EnsureNotClosed();
                return this._buffer.Length;
            }
            set
            {
                // Only update the capacity if the MS is expandable and the value is different than the current capacity.
                // Special behavior if the MS isn't expandable: we don't throw if value is the same as the current capacity
                if (value < this.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "You can't reduce the capacity below the stream's length.");
                }

                EnsureNotClosed();
                EnsureWritable();

                if (value != this._buffer.Length)
                {
                    var newBuffer = new SecretsArray<byte>(value);

                    if (this._length > 0)
                    {
                        try
                        {
                            Buffer.BlockCopy(this._buffer.UnderlyingArray, 0, newBuffer.UnderlyingArray, 0, this._length);
                        }
                        catch (Exception)
                        {
                            newBuffer.Dispose();
                            throw;
                        }
                    }

                    var oldBuffer = this._buffer;
                    this._buffer = newBuffer;
                    oldBuffer.Dispose();
                }
            }
        }

        public SecretMemoryStream() : this(MIN_CAPACITY)
        {
        }

        public SecretMemoryStream(int requiredMinimumCapacity)
        {
            this._buffer = new SecretsArray<byte>(Math.Max(requiredMinimumCapacity, MIN_CAPACITY));
            this._isWritable = true;
        }

        public SecretMemoryStream(SecretBytes bytes)
        {
            this._buffer = bytes.GetUnderlyingArray();
            this._isWritable = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._buffer.Dispose();
            }

            base.Dispose(disposing);
        }

        private void EnsureNotClosed()
        {
            if (!this.IsOpen)
            {
                throw new ObjectDisposedException("This stream has already been disposed.", (Exception?)null);
            }
        }

        private void EnsureWritable()
        {
            if (!this.CanWrite)
            {
                throw new InvalidOperationException("This stream is not writable.");
            }
        }

        public override void Flush()
        {
            // Nothing to do
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            EnsureNotClosed();

            long newPosition;

            switch (origin)
            {
                case SeekOrigin.Begin:
                {
                    newPosition = offset;
                    break;
                }
                case SeekOrigin.Current:
                {
                    newPosition = this._position + offset;
                    break;
                }
                case SeekOrigin.End:
                {
                    newPosition = this._length + offset;
                    break;
                }

                default:
                    throw new UnexpectedSwitchValueException(nameof(origin), origin);
            }

            if (newPosition < 0)
            {
                throw new IOException("You can't move the position before the begin of the stream.");
            }
            if (newPosition >= MAX_LENGTH)
            {
                throw new IOException("You can't move the position beyond the maximum possible length of the stream.");
            }

            // NOTE: The position can very well be set beyond the stream's current length or capacity.
            this._position = (int)newPosition;

            return this._position;
        }

        public override void SetLength(long value)
        {
            if (value < 0 || value > MAX_LENGTH)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            EnsureNotClosed();

            EnsureCapacity((int)value);

            this._length = (int)value;

            if (this._position > this._length)
            {
                this._position = this._length;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            Validate.ArgumentWithName(nameof(buffer)).IsNotNull(buffer);

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "This value can't be negative.");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "This value can't be negative.");
            }

            if (offset + count > buffer.Length)
            {
                throw new ArgumentException("The segment defined by offset and count goes outside of buffer.");
            }

            EnsureNotClosed();

            int bytesToRead = this._length - this._position;
            if (bytesToRead > count)
            {
                bytesToRead = count;
            }
            if (bytesToRead <= 0)
            {
                return 0;
            }

            // Apparently it's more efficient to use a for loop when writing small amounts of bytes (up to 8 bytes).
            if (bytesToRead <= 8)
            {
                var bufferArray = this._buffer.UnderlyingArray;
                for (int x = 0; x < bytesToRead; x++)
                {
                    buffer[offset + x] = bufferArray[this._position + x];
                }
            }
            else
            {
                Buffer.BlockCopy(this._buffer.UnderlyingArray, this._position, buffer, offset, bytesToRead);
            }

            this._position += bytesToRead;

            return bytesToRead;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Validate.ArgumentWithName(nameof(buffer)).IsNotNull(buffer);

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "This value can't be negative.");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "This value can't be negative.");
            }

            if (offset + count > buffer.Length)
            {
                throw new ArgumentException("The segment defined by offset and count goes outside of buffer.");
            }

            EnsureNotClosed();
            EnsureWritable();

            int newPositionAfterWriteOperation = this._position + count;

            // Check for overflow
            if (newPositionAfterWriteOperation < 0)
            {
                throw new IOException("The stream exceeds the maximum length.");
            }

            if (newPositionAfterWriteOperation > this._length)
            {
                if (newPositionAfterWriteOperation > this.Capacity)
                {
                    EnsureCapacity(newPositionAfterWriteOperation);
                }

                this._length = newPositionAfterWriteOperation;
            }

            // Apparently it's more efficient to use a for loop when writing small amounts of bytes (up to 8 bytes).
            if (count <= 8)
            {
                var bufferArray = this._buffer.UnderlyingArray;
                for (int x = 0; x < count; x++)
                {
                    bufferArray[this._position + x] = buffer[offset + x];
                }
            }
            else
            {
                Buffer.BlockCopy(buffer, offset, this._buffer.UnderlyingArray, this._position, count);
            }

            this._position = newPositionAfterWriteOperation;
        }

        private void EnsureCapacity(int value)
        {
            var currentCapacity = this.Capacity;

            if (value > currentCapacity)
            {
                int newCapacity;

                if ((ulong)(currentCapacity * 2) > MAX_LENGTH)
                {
                    newCapacity = Math.Max(value, MAX_LENGTH);
                }
                else
                {
                    if (value < currentCapacity * 2)
                    {
                        newCapacity = currentCapacity * 2;
                    }
                    else
                    {
                        newCapacity = value;
                    }
                }

                this.Capacity = newCapacity;
            }
        }

    }
}
