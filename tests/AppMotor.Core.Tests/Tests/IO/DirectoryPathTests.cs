// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.IO.Abstractions;

using AppMotor.Core.IO;

using Moq;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.IO;

public sealed class DirectoryPathTests
{
    private const string TEST_DIRECTORY_PATH = "some/dir";

    [Fact]
    public void Test_Value()
    {
        var validPath = new DirectoryPath("/some/dir");
        validPath.Value.ShouldBe("/some/dir");

        var invalidPath = new DirectoryPath();
        Should.Throw<InvalidOperationException>(() => invalidPath.Value);
    }

    [Fact]
    public void Test_ConstructionWithParentDirectory()
    {
        new DirectoryPath("/abc/def", "ghi").Value.ShouldBe(Path.Combine("/abc/def", "ghi"));
        new DirectoryPath(@"c:\abc\def", "ghi").Value.ShouldBe(Path.Combine(@"c:\abc\def", "ghi"));
    }

    [Fact]
    public void Test_ImplicitConversionFromString()
    {
        DirectoryPath filePathNonNullable = "abc/def";
        filePathNonNullable.Value.ShouldBe("abc/def");

        string? nullString = null;
        DirectoryPath? filePathNullable = nullString;
        filePathNullable.HasValue.ShouldBe(false);
    }

    [Fact]
    public void Test_ImplicitConversionFromFileInfo()
    {
        var nonNullDirectoryInfo = new DirectoryInfo(Environment.ExpandEnvironmentVariables("%TEMP%"));
        DirectoryPath filePathNonNullable = nonNullDirectoryInfo;
        filePathNonNullable.Value.ShouldBe(nonNullDirectoryInfo.FullName);

        DirectoryInfo? nullDirectoryInfo = null;
        DirectoryPath? filePathNullable = nullDirectoryInfo;
        filePathNullable.HasValue.ShouldBe(false);
    }

    [Fact]
    public void Test_RemovalOfTrailingSlash()
    {
        new DirectoryPath("/abc/def/").Value.ShouldBe("/abc/def");
        new DirectoryPath("/abc/def//").Value.ShouldBe("/abc/def");
        new DirectoryPath(@"c:\abc\def\").Value.ShouldBe(@"c:\abc\def");
        new DirectoryPath(@"c:\abc\def\\").Value.ShouldBe(@"c:\abc\def");

        new DirectoryPath(@"c:\abc\def\/\").Value.ShouldBe(@"c:\abc\def");
    }

    [Fact]
    public void Test_Name()
    {
        new DirectoryPath("/abc/def").Name.ShouldBe("def");
        new DirectoryPath("/abc/def/").Name.ShouldBe("def");

        if (OperatingSystem.IsWindows())
        {
            new DirectoryPath(@"c:\abc\def").Name.ShouldBe("def");
            new DirectoryPath(@"c:\abc\def\").Name.ShouldBe("def");
        }
    }

    [Fact]
    public void Test_AsAbsolutePath()
    {
        var testGuid = Guid.NewGuid().ToString();

        var path = new DirectoryPath("some/dir");

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
        var (path, directoryMock, fileSystem) = CreateTestObjects();

        directoryMock
            .Setup(m => m.Exists(TEST_DIRECTORY_PATH))
            .Returns(true);

        path.Exists(fileSystem).ShouldBe(true);
    }

    private static (DirectoryPath, Mock<IDirectory>, IFileSystem) CreateTestObjects()
    {
        var path = new DirectoryPath(TEST_DIRECTORY_PATH);

        var directoryMock = new Mock<IDirectory>(MockBehavior.Strict);

        var fileSystemMock = new Mock<IFileSystem>(MockBehavior.Strict);
        fileSystemMock
            .Setup(m => m.Directory)
            .Returns(directoryMock.Object);

        return (path, directoryMock, fileSystemMock.Object);
    }
}