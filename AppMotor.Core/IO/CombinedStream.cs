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
using System.Collections.Generic;
using System.IO;

using AppMotor.Core.Utils;

namespace AppMotor.Core.IO
{
    /// <summary>
    /// Allows for multiple streams to be combined into one stream.
    /// </summary>
    /// <remarks>
    /// This implementation is based on this: https://stackoverflow.com/a/3879246/614177
    /// </remarks>
    public class CombinedStream : Stream
    {
        private readonly bool _closeStreams;

        private IEnumerator<Stream>? _iterator;

        /// <summary>
        /// The current stream. Do not use directly. Use <see cref="CurrentStream"/> instead.
        /// </summary>
        private Stream? _currentStream;

        /// <summary>
        /// The current stream; or <c>null</c>, if the end of the last stream has been reached.
        /// </summary>
        private Stream? CurrentStream
        {
            get
            {
                if (this._currentStream != null)
                {
                    // End of stream not yet reached.
                    return this._currentStream;
                }

                if (this._iterator == null)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                // NOTE: Even if the end of the iterator has been reached, we can safely
                //   call this - as it will return "false" every time.
                if (this._iterator.MoveNext())
                {
                    this._currentStream = this._iterator.Current;
                }

                return this._currentStream;
            }
        }

        /// <summary>
        /// Basically the number of bytes read from the stream.
        /// </summary>
        private long _position;

        /// <inheritdoc />
        public override bool CanRead => true;

        /// <inheritdoc />
        public override bool CanSeek => false;

        /// <inheritdoc />
        public override bool CanWrite => false;

        /// <inheritdoc />
        public override long Length => throw new NotSupportedException();

        /// <inheritdoc />
        public override long Position
        {
            get => this._position;
            set => throw new NotSupportedException();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="closeStreams">Whether to close each stream once its end has been reached
        /// or when this instance is disposed.</param>
        /// <param name="source">The streams to combine in this stream. Note that even if the
        /// collection is empty, no exception will be thrown.</param>
        public CombinedStream(bool closeStreams, IEnumerable<Stream> source)
        {
            Validate.Argument.IsNotNull(source, nameof(source));

            this._iterator = source.GetEnumerator();
            this._closeStreams = closeStreams;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="closeStreams">Whether to close each stream once its end has been reached
        /// or when this instance is disposed.</param>
        /// <param name="firstStream">The first of the streams to combine in this stream.</param>
        /// <param name="secondStream">The second of the streams to combine in this stream.</param>
        /// <param name="otherStreams">The other streams to combine in this stream.</param>
        public CombinedStream(bool closeStreams, Stream firstStream, Stream secondStream, params Stream[] otherStreams)
            : this(closeStreams, ParamsUtils.Combine(firstStream, secondStream, otherStreams))
        {
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                while (this.CurrentStream != null)
                {
                    OnEndOfStream();
                }

                this._iterator?.Dispose();
                this._iterator = null;
            }

            base.Dispose(disposing);
        }

        private void OnEndOfStream()
        {
            if (this._closeStreams)
            {
                this._currentStream?.Dispose();
            }

            this._currentStream = null;
        }

        /// <inheritdoc />
        public override void Flush()
        {
            // NOTE: We can either throw an exception here or do nothing. However, when doing
            //   nothing this may not be what the user expects (as the user may except that all
            //   underlying streams should be flushed). On the other hand, calling "Flush()" on
            //   a read-only stream doesn't make sense anyways. So we decided to throw an exception
            //   here, as calling "Flush()" is considered a bug in this case.
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count)
        {
            int readBytesTotal = 0;

            while (count > 0)
            {
                var stream = this.CurrentStream;
                if (stream == null)
                {
                    break;
                }

                int readBytes = stream.Read(buffer, offset, count);
                if (readBytes == 0)
                {
                    OnEndOfStream();
                }
                else
                {
                    readBytesTotal += readBytes;
                    count -= readBytes;
                    offset += readBytes;
                }
            }

            this._position += readBytesTotal;

            return readBytesTotal;
        }

        /// <inheritdoc />
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
