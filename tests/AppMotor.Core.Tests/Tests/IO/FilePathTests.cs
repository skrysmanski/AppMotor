// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.IO.Abstractions;

using AppMotor.Core.IO;

using Moq;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.IO;

public sealed class FilePathTests
{
    private const string TEST_FILE_PATH = "some/file";

    [Fact]
    public void Test_Value()
    {
        var validPath = new FilePath("/some/file");
        validPath.Value.ShouldBe("/some/file");

        var invalidPath = new FilePath();
        Should.Throw<InvalidOperationException>(() => invalidPath.Value);
    }

    [Fact]
    public void Test_ConstructionWithParentDirectory()
    {
        new FilePath("/abc/def", "ghi").Value.ShouldBe(Path.Combine("/abc/def", "ghi"));
        new FilePath(@"c:\abc\def", "ghi").Value.ShouldBe(Path.Combine(@"c:\abc\def", "ghi"));
    }

    [Fact]
    public void Test_ImplicitConversionFromString()
    {
        FilePath filePathNonNullable = "abc/def";
        filePathNonNullable.Value.ShouldBe("abc/def");

        string? nullString = null;
        FilePath? filePathNullable = nullString;
        filePathNullable.HasValue.ShouldBe(false);
    }

    [Fact]
    public void Test_ImplicitConversionFromFileInfo()
    {
        var nonNullFileInfo = new FileInfo(Environment.ExpandEnvironmentVariables("%TEMP%/abc.txt"));
        FilePath filePathNonNullable = nonNullFileInfo;
        filePathNonNullable.Value.ShouldBe(nonNullFileInfo.FullName);

        FileInfo? nullFileInfo = null;
        FilePath? filePathNullable = nullFileInfo;
        filePathNullable.HasValue.ShouldBe(false);
    }

    [Fact]
    public void Test_InvalidTrailingSlash()
    {
        Should.Throw<ArgumentException>(() => new FilePath("/abc/def/"));
        Should.Throw<ArgumentException>(() => new FilePath(@"c:\abc\def\"));
    }

    [Fact]
    public void Test_Name()
    {
        new FilePath("/abc/def").Name.ShouldBe("def");

        if (OperatingSystem.IsWindows())
        {
            new FilePath(@"c:\abc\def").Name.ShouldBe("def");
        }
    }

    [Fact]
    public void Test_AsAbsolutePath()
    {
        var testGuid = Guid.NewGuid().ToString();

        var path = new FilePath("some/file");

        var pathMock = new Mock<IPath>(MockBehavior.Strict);
        pathMock
            .Setup(m => m.GetFullPath(It.IsAny<string>()))
            .Returns(testGuid);

        var fileSystemMock = new Mock<IFileSystem>(MockBehavior.Strict);
        fileSystemMock
            .Setup(m => m.Path)
            .Returns(pathMock.Object);

        path.AsAbsolutePath(fileSystemMock.Object).ShouldBe(testGuid);
    }

    [Fact]
    public void Test_Exists()
    {
        var (path, fileMock, fileSystem) = CreateTestObjects();

        fileMock
            .Setup(m => m.Exists(TEST_FILE_PATH))
            .Returns(true);

        path.Exists(fileSystem).ShouldBe(true);
    }

    private static (FilePath, Mock<IFile>, IFileSystem) CreateTestObjects()
    {
        var path = new FilePath(TEST_FILE_PATH);

        var fileMock = new Mock<IFile>(MockBehavior.Strict);

        var fileSystemMock = new Mock<IFileSystem>(MockBehavior.Strict);
        fileSystemMock
            .Setup(m => m.File)
            .Returns(fileMock.Object);

        return (path, fileMock, fileSystemMock.Object);
    }
}
