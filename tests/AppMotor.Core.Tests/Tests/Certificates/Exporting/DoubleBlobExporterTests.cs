// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.IO.Abstractions.TestingHelpers;
using System.Security.Cryptography;

using AppMotor.Core.Certificates.Exporting;

using JetBrains.Annotations;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Certificates.Exporting;

public sealed class DoubleBlobExporterTests
{
    private readonly byte[] _publicKeyBytes = GenerateRandomBytes(40);

    private readonly byte[] _privateKeyBytes = GenerateRandomBytes(40);

    [MustUseReturnValue]
    private static byte[] GenerateRandomBytes(int byteCount)
    {
        // NOTE: We use a crypto random generate here so that the public and the private key
        //   bytes are not the same.
        using var rng = RandomNumberGenerator.Create();

        byte[] bytes = new byte[byteCount];
        rng.GetBytes(bytes);

        return bytes;
    }

    [Fact]
    public void Test_ToBytes()
    {
        // Setup
        var exporter = new DoubleBlobExporter(this._publicKeyBytes, () => this._privateKeyBytes);

        // Test
        var (publicKeyBytes, privateKeyBytes) = exporter.ToBytes();

        // Validate
        publicKeyBytes.ShouldBe(this._publicKeyBytes);
        privateKeyBytes.ShouldBe(this._privateKeyBytes);
    }

    [Fact]
    public void Test_ToFile()
    {
        const string? PUBLIC_KEY_FILE_PATH = "/test/export.cert";
        const string? PRIVATE_KEY_FILE_PATH = "/test/export.key";

        // Setup
        var exporter = new DoubleBlobExporter(this._publicKeyBytes, () => this._privateKeyBytes);

        var mockFileSystem = new MockFileSystem();
        mockFileSystem.AddDirectory("/test");

        // Test
        exporter.ToFile(PUBLIC_KEY_FILE_PATH, PRIVATE_KEY_FILE_PATH, mockFileSystem);

        // Validate
        mockFileSystem.File.ReadAllBytes(PUBLIC_KEY_FILE_PATH).ShouldBe(this._publicKeyBytes);
        mockFileSystem.File.ReadAllBytes(PRIVATE_KEY_FILE_PATH).ShouldBe(this._privateKeyBytes);
    }

    [Fact]
    public async Task Test_ToFileAsync()
    {
        const string? PUBLIC_KEY_FILE_PATH = "/test/export.cert";
        const string? PRIVATE_KEY_FILE_PATH = "/test/export.key";

        // Setup
        var exporter = new DoubleBlobExporter(this._publicKeyBytes, () => this._privateKeyBytes);

        var mockFileSystem = new MockFileSystem();
        mockFileSystem.AddDirectory("/test");

        // Test
        await exporter.ToFileAsync(PUBLIC_KEY_FILE_PATH, PRIVATE_KEY_FILE_PATH, mockFileSystem);

        // Validate
        // ReSharper disable MethodHasAsyncOverload
        mockFileSystem.File.ReadAllBytes(PUBLIC_KEY_FILE_PATH).ShouldBe(this._publicKeyBytes);
        mockFileSystem.File.ReadAllBytes(PRIVATE_KEY_FILE_PATH).ShouldBe(this._privateKeyBytes);
        // ReSharper restore MethodHasAsyncOverload
    }
}