// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Utils;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Utils;

/// <summary>
/// Tests for <see cref="PooledArrayBufferWriter{T}"/>.
/// </summary>
public sealed class PooledArrayBufferWriterTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_BasicFunctionality(bool specifyInitialCapacity)
    {
        using var bufferWriter = specifyInitialCapacity ? new PooledArrayBufferWriter<int>(initialCapacity: 50) : new PooledArrayBufferWriter<int>();

        bufferWriter.Capacity.ShouldBeGreaterThanOrEqualTo(50);
        bufferWriter.FreeCapacity.ShouldBe(bufferWriter.Capacity);
        bufferWriter.WrittenCount.ShouldBe(0);
        bufferWriter.WrittenMemory.Length.ShouldBe(0);
        bufferWriter.WrittenSpan.Length.ShouldBe(0);

        bufferWriter.GetSpan().Length.ShouldBe(bufferWriter.Capacity);
        bufferWriter.GetMemory().Length.ShouldBe(bufferWriter.Capacity);

        var writeSpan = bufferWriter.GetSpan();
        writeSpan[0] = 12;
        writeSpan[1] = 24;
        writeSpan[2] = 48;

        bufferWriter.Advance(3);

        bufferWriter.FreeCapacity.ShouldBe(bufferWriter.Capacity - 3);
        bufferWriter.WrittenCount.ShouldBe(3);

        bufferWriter.WrittenMemory.ShouldBe(new[] { 12, 24, 48 });
        bufferWriter.WrittenSpan.Length.ShouldBe(3);

        // Dispose tests
        bufferWriter.Dispose();

        Should.Throw<ObjectDisposedException>(() => bufferWriter.GetSpan());
        Should.Throw<ObjectDisposedException>(() => bufferWriter.GetMemory());
        Should.Throw<ObjectDisposedException>(() => bufferWriter.Advance(2));
        Should.Throw<ObjectDisposedException>(() => bufferWriter.Capacity);
        Should.Throw<ObjectDisposedException>(() => bufferWriter.FreeCapacity);
        Should.Throw<ObjectDisposedException>(() => bufferWriter.WrittenCount);
        Should.Throw<ObjectDisposedException>(() => bufferWriter.WrittenMemory);
        Should.Throw<ObjectDisposedException>(() => bufferWriter.WrittenSpan.Length);

        bufferWriter.Dispose(); // should not throw
    }

    [Fact]
    public void Test_Clear()
    {
        // Setup
        using var bufferWriter = new PooledArrayBufferWriter<int>();

        var writeSpan = bufferWriter.GetSpan();
        writeSpan[0] = 12;
        writeSpan[1] = 24;
        writeSpan[2] = 48;

        bufferWriter.Advance(3);

        // Verify assumptions
        bufferWriter.WrittenCount.ShouldBe(3);
        bufferWriter.WrittenMemory.ShouldBe(new[] { 12, 24, 48 });

        // Test
        bufferWriter.Clear();

        // Verify
        bufferWriter.WrittenCount.ShouldBe(0);
        bufferWriter.WrittenMemory.Length.ShouldBe(0);
        bufferWriter.WrittenSpan.Length.ShouldBe(0);

        // Test that the memory is actually cleared
        bufferWriter.Advance(3);
        bufferWriter.WrittenMemory.ShouldBe(new[] { 0, 0, 0 });

        // Dispose tests
        // ReSharper disable once DisposeOnUsingVariable
        bufferWriter.Dispose();
        Should.Throw<ObjectDisposedException>(() => bufferWriter.Clear());
    }

    [Theory]
    [InlineData(-50)]
    [InlineData(-1)]
    [InlineData(0)]
    public void Test_Constructor_InvalidCapacity(int capacity)
    {
        Should.Throw<ArgumentOutOfRangeException>(() => new PooledArrayBufferWriter<int>(initialCapacity: capacity));
    }

    [Fact]
    public void Test_Advance_Count()
    {
        using var bufferWriter = new PooledArrayBufferWriter<int>();

        bufferWriter.Advance(0); // should not throw

        Should.Throw<ArgumentOutOfRangeException>(() => bufferWriter.Advance(-1));

        bufferWriter.Advance(bufferWriter.Capacity); // should not throw

        Should.Throw<InvalidOperationException>(() => bufferWriter.Advance(1));
    }

    [Fact]
    public void Test_InvalidSizeHint()
    {
        using var bufferWriter = new PooledArrayBufferWriter<int>();

        Should.Throw<ArgumentOutOfRangeException>(() => bufferWriter.GetSpan(sizeHint: -1));
        Should.Throw<ArgumentOutOfRangeException>(() => bufferWriter.GetMemory(sizeHint: -1));
    }
}
