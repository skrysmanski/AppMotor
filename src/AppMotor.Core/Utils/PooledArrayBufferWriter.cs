// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Buffers;

namespace AppMotor.Core.Utils;

/// <summary>
/// A <see cref="IBufferWriter{T}"/> that uses <see cref="ArrayPool{T}.Shared"/> to rent its buffer.
/// Automatically grows if needed. As such it works like <see cref="ArrayBufferWriter{T}"/> but with a rented buffer.
/// </summary>
public sealed class PooledArrayBufferWriter<T> : IBufferWriter<T>, IDisposable
{
    private const int MINIMUM_BUFFER_SIZE = 256;

    private T[]? _rentedBuffer;

    private T[] RentedBuffer => this._rentedBuffer ?? throw new ObjectDisposedException("This writer has already been disposed.");

    /// <inheritdoc cref="WrittenCount"/>
    private int _writtenCount;

    /// <summary>
    /// The number of items that have been written to this buffer.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown if the buffer has already been disposed.</exception>
    public int WrittenCount
    {
        get
        {
            CheckIfDisposed();

            return this._writtenCount;
        }
    }

    /// <summary>
    /// Returns the data written to the underlying buffer so far, as a <see cref="ReadOnlyMemory{T}"/>.
    /// </summary>
    /// <remarks>
    /// Note that this property differs from <see cref="GetMemory"/> in that the method returns a memory
    /// segment that can/should be written to next - whereas this property returns the memory segment of
    /// the items that have already been written.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">Thrown if the buffer has already been disposed.</exception>
    public ReadOnlyMemory<T> WrittenMemory => this.RentedBuffer.AsMemory(0, this._writtenCount);

    /// <summary>
    /// Returns the data written to the underlying buffer so far, as a <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown if the buffer has already been disposed.</exception>
    public ReadOnlySpan<T> WrittenSpan => this.RentedBuffer.AsSpan(0, this._writtenCount);

    /// <summary>
    /// Returns the total number of items (written or not written) within the underlying buffer.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown if the buffer has already been disposed.</exception>
    public int Capacity => this.RentedBuffer.Length;

    /// <summary>
    /// Returns the amount of space available that can still be written into without forcing the underlying buffer to grow.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown if the buffer has already been disposed.</exception>
    public int FreeCapacity => this.RentedBuffer.Length - this._writtenCount;

    /// <summary>
    /// Constructor (default capacity).
    /// </summary>
    public PooledArrayBufferWriter()
        : this(MINIMUM_BUFFER_SIZE)
    {
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="initialCapacity">The initial capacity of this buffer.</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="initialCapacity"/> is less than or equal to 0.</exception>
    public PooledArrayBufferWriter(int initialCapacity)
    {
        if (initialCapacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(initialCapacity));
        }

        this._rentedBuffer = ArrayPool<T>.Shared.Rent(initialCapacity);
        this._writtenCount = 0;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (this._rentedBuffer == null)
        {
            return;
        }

        // NOTE: We use our "Clear()" method here (instead of "clearArray" in "Return()") because it's
        //   more "efficient" (only clears data that was actually written). Also we clear the buffer here
        //   to avoid leaking its data.
        Clear();

        ArrayPool<T>.Shared.Return(this._rentedBuffer);
        this._rentedBuffer = null;
    }

    private void CheckIfDisposed()
    {
        if (this._rentedBuffer == null)
        {
            throw new ObjectDisposedException(nameof(ArrayBufferWriter<T>));
        }
    }

    /// <summary>
    /// Clears this buffer.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown if the buffer has already been disposed.</exception>
    public void Clear()
    {
        // NOTE: We clear the buffer here so that it doesn't "leak" the data if "Advance" is used without actually
        //   writing data.
        this.RentedBuffer.AsSpan(0, this._writtenCount).Clear();
        this._writtenCount = 0;
    }

    /// <inheritdoc />
    public void Advance(int count)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }
        else if (count == 0)
        {
            return;
        }

        if (this._writtenCount > this.RentedBuffer.Length - count)
        {
            throw new InvalidOperationException($"Cannot advance past the end of the buffer, which has a size of {this.RentedBuffer.Length}.");
        }

        this._writtenCount += count;
    }

    /// <inheritdoc />
    public Memory<T> GetMemory(int sizeHint = 0)
    {
        CheckAndResizeBuffer(sizeHint);
        return this.RentedBuffer.AsMemory(this._writtenCount);
    }

    /// <inheritdoc />
    public Span<T> GetSpan(int sizeHint = 0)
    {
        CheckAndResizeBuffer(sizeHint);
        return this.RentedBuffer.AsSpan(this._writtenCount);
    }

    /// <summary>
    /// Makes sure that at least <paramref name="sizeHint"/> items are still free in this buffer.
    /// If not, the buffer's is size is increased automatically.
    /// </summary>
    private void CheckAndResizeBuffer(int sizeHint)
    {
        if (sizeHint < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(sizeHint));
        }

        if (sizeHint == 0)
        {
            sizeHint = MINIMUM_BUFFER_SIZE;
        }

        var oldRentedBuffer = this.RentedBuffer;

        var availableSpace = oldRentedBuffer.Length - this._writtenCount;

        if (sizeHint > availableSpace)
        {
            var growBy = Math.Max(sizeHint, oldRentedBuffer.Length);

            var newSize = checked(oldRentedBuffer.Length + growBy);

            this._rentedBuffer = ArrayPool<T>.Shared.Rent(newSize);

            var previousBuffer = oldRentedBuffer.AsSpan(0, this._writtenCount);
            previousBuffer.CopyTo(this._rentedBuffer);
            previousBuffer.Clear();
            ArrayPool<T>.Shared.Return(oldRentedBuffer);
        }
    }
}
