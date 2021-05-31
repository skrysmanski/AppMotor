﻿#region License
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
using System.IO.Abstractions;

using AppMotor.Core.IO;

using Moq;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.IO
{
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
}
