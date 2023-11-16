// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.IO;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.IO;

public sealed class CombinedStreamTests
{
    private static readonly Random s_random = new Random(Seed: 12345);

    [Fact]
    public void Test_ReadSingleBytes()
    {
        const int BYTE_COUNT = 16;

        // Setup
        var bytes1 = CreateRandomBytes(BYTE_COUNT);
        var bytes2 = CreateRandomBytes(BYTE_COUNT);
        var bytes3 = CreateRandomBytes(BYTE_COUNT);

        // Test
        var combinedStream = new CombinedStream(
            closeStreams: true,
            new MemoryStream(bytes1, writable: false),
            new MemoryStream(bytes2, writable: false),
            new MemoryStream(bytes3, writable: false)
        );

        var readBytes = new List<byte>(BYTE_COUNT * 3);

        while (true)
        {
            int readByte = combinedStream.ReadByte();
            if (readByte == -1)
            {
                break;
            }

            readBytes.Add((byte)readByte);
        }

        readBytes.Count.ShouldBe(BYTE_COUNT * 3);
        readBytes.ShouldBe(bytes1.Concat(bytes2).Concat(bytes3).ToList());

        combinedStream.Position.ShouldBe(BYTE_COUNT * 3);
    }

    [Fact]
    public void Test_ReadAllBytes()
    {
        const int BYTE_COUNT = 16;

        // Setup
        var bytes1 = CreateRandomBytes(BYTE_COUNT);
        var bytes2 = CreateRandomBytes(BYTE_COUNT);
        var bytes3 = CreateRandomBytes(BYTE_COUNT);

        // Test
        var combinedStream = new CombinedStream(
            closeStreams: true,
            new MemoryStream(bytes1, writable: false),
            new MemoryStream(bytes2, writable: false),
            new MemoryStream(bytes3, writable: false)
        );

        var readBytes = new byte[BYTE_COUNT * 3];

        int readByteCount = combinedStream.Read(readBytes);
        readByteCount.ShouldBe(BYTE_COUNT * 3);

        combinedStream.ReadByte().ShouldBe(-1);

        readBytes.ShouldBe(bytes1.Concat(bytes2).Concat(bytes3).ToList());

        combinedStream.Position.ShouldBe(BYTE_COUNT * 3);
    }

    [Fact]
    public void Test_ReadAcrossStreamBoundaries()
    {
        // Setup
        var bytes1 = CreateRandomBytes(7);
        var bytes2 = CreateRandomBytes(14);
        var bytes3 = CreateRandomBytes(21);

        // Test
        var combinedStream = new CombinedStream(
            closeStreams: true,
            new MemoryStream(bytes1, writable: false),
            new MemoryStream(bytes2, writable: false),
            new MemoryStream(bytes3, writable: false)
        );

        var readBytesTotal = new List<byte>(7 + 14 + 21);

        // Deliberately choose a size that does not align with the buffer sizes from above.
        var readBytes = new byte[5];

        while (true)
        {
            int readByteCount = combinedStream.Read(readBytes);

            if (readByteCount == 5)
            {
                // Ok; we should read 5 bytes every time - until the last read.
                readBytesTotal.AddRange(readBytes);
            }
            else if (readByteCount == 2)
            {
                // Ok; the last segment.
                readBytesTotal.AddRange(readBytes[0..2]);
                break;
            }
            else
            {
                throw new Exception($"'readByteCount' should have been 5 or 2 but was {readByteCount}");
            }
        }

        combinedStream.Read(readBytes).ShouldBe(0);

        readBytesTotal.ShouldBe(bytes1.Concat(bytes2).Concat(bytes3).ToList());

        combinedStream.Position.ShouldBe(bytes1.Length + bytes2.Length + bytes3.Length);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void TestCloseStreams(bool closeStreams)
    {
        const int BYTE_COUNT = 16;

        // Setup
        var bytes = CreateRandomBytes(BYTE_COUNT);
        var stream1 = new TestMemoryStream(bytes, writable: false);
        var stream2 = new TestMemoryStream(bytes, writable: false);
        var stream3 = new TestMemoryStream(bytes, writable: false);

        // Test
        var combinedStream = new CombinedStream(
            closeStreams: closeStreams,
            stream1,
            stream2,
            stream3
        );

        var readBytes = new byte[BYTE_COUNT];

        int readByteCount = combinedStream.Read(readBytes);
        readByteCount.ShouldBe(BYTE_COUNT);

        readByteCount = combinedStream.Read(readBytes);
        readByteCount.ShouldBe(BYTE_COUNT);
        stream1.IsDisposed.ShouldBe(closeStreams);

        readByteCount = combinedStream.Read(readBytes);
        readByteCount.ShouldBe(BYTE_COUNT);
        stream2.IsDisposed.ShouldBe(closeStreams);

        combinedStream.ReadByte().ShouldBe(-1);
        stream3.IsDisposed.ShouldBe(closeStreams);

        combinedStream.Dispose();
        stream1.IsDisposed.ShouldBe(closeStreams);
        stream2.IsDisposed.ShouldBe(closeStreams);
        stream3.IsDisposed.ShouldBe(closeStreams);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void TestDispose(bool closeStreams)
    {
        const int BYTE_COUNT = 16;

        // Setup
        var bytes = CreateRandomBytes(BYTE_COUNT);
        var stream1 = new TestMemoryStream(bytes, writable: false);
        var stream2 = new TestMemoryStream(bytes, writable: false);
        var stream3 = new TestMemoryStream(bytes, writable: false);

        // Test
        var combinedStream = new CombinedStream(
            closeStreams: closeStreams,
            stream1,
            stream2,
            stream3
        );

        var readBytes = new byte[BYTE_COUNT];

        int readByteCount = combinedStream.Read(readBytes);
        readByteCount.ShouldBe(BYTE_COUNT);

        stream1.IsDisposed.ShouldBe(false);
        stream2.IsDisposed.ShouldBe(false);
        stream3.IsDisposed.ShouldBe(false);

        combinedStream.Dispose();

        stream1.IsDisposed.ShouldBe(closeStreams);
        stream2.IsDisposed.ShouldBe(closeStreams);
        stream3.IsDisposed.ShouldBe(closeStreams);

        Should.Throw<ObjectDisposedException>(() => combinedStream.Read(readBytes));
    }

    [Fact]
    public void TestEmptyListConstructor()
    {
        Should.NotThrow(() => new CombinedStream(closeStreams: true, Array.Empty<Stream>()));
    }

    [Fact]
    public void TestReadOnly()
    {
        const int BYTE_COUNT = 16;

        // Setup
        var bytes = CreateRandomBytes(BYTE_COUNT);
        var stream1 = new TestMemoryStream(bytes, writable: false);
        var stream2 = new TestMemoryStream(bytes, writable: false);

        var combinedStream = new CombinedStream(
            closeStreams: true,
            stream1,
            stream2
        );

        // Test
        combinedStream.CanRead.ShouldBe(true);
        combinedStream.CanSeek.ShouldBe(false);
        combinedStream.CanWrite.ShouldBe(false);

        Should.Throw<NotSupportedException>(() => combinedStream.Length);
        Should.Throw<NotSupportedException>(() => combinedStream.Position = 42);
        Should.Throw<NotSupportedException>(() => combinedStream.Seek(2, SeekOrigin.Begin));
        Should.Throw<NotSupportedException>(() => combinedStream.SetLength(5));
        Should.Throw<NotSupportedException>(() => combinedStream.Write(bytes));
        Should.Throw<NotSupportedException>(() => combinedStream.Flush());
    }

    private static byte[] CreateRandomBytes(int count)
    {
        var bytes = new byte[count];

        s_random.NextBytes(bytes);

        return bytes;
    }

    private sealed class TestMemoryStream : MemoryStream
    {
        public bool IsDisposed { get; private set; }

        /// <inheritdoc />
        public TestMemoryStream(byte[] buffer, bool writable) : base(buffer, writable)
        {
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                this.IsDisposed = true;
            }
        }
    }
}
