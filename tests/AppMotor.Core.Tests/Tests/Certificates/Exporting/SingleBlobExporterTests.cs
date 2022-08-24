// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.IO.Abstractions.TestingHelpers;

using AppMotor.Core.Certificates.Exporting;

using JetBrains.Annotations;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Certificates.Exporting;

public sealed class SingleBlobExporterTests
{
    private readonly byte[] _testBytes = GenerateRandomBytes(40);

    [MustUseReturnValue]
    private static byte[] GenerateRandomBytes(int byteCount)
    {
        var bytes = new byte[byteCount];
        new Random().NextBytes(bytes);
        return bytes;
    }

    [Fact]
    public void Test_ToBytes()
    {
        // Setup
        var exporter = new SingleBlobExporter(() => this._testBytes);

        // Test
        exporter.ToBytes().ShouldBe(this._testBytes);
    }

    [Fact]
    public void Test_ToFile()
    {
        const string? FILE_PATH = "/test/export.bin";

        // Setup
        var exporter = new SingleBlobExporter(() => this._testBytes);

        var mockFileSystem = new MockFileSystem();
        mockFileSystem.AddDirectory("/test");

        // Test
        exporter.ToFile(FILE_PATH, mockFileSystem);

        // Validate
        mockFileSystem.File.ReadAllBytes(FILE_PATH).ShouldBe(this._testBytes);
    }

    [Fact]
    public async Task Test_ToFileAsync()
    {
        const string? FILE_PATH = "/test/export.bin";

        // Setup
        var exporter = new SingleBlobExporter(() => this._testBytes);

        var mockFileSystem = new MockFileSystem();
        mockFileSystem.AddDirectory("/test");

        // Test
        await exporter.ToFileAsync(FILE_PATH, mockFileSystem);

        // Validate
        // ReSharper disable once MethodHasAsyncOverload
        mockFileSystem.File.ReadAllBytes(FILE_PATH).ShouldBe(this._testBytes);
    }
}