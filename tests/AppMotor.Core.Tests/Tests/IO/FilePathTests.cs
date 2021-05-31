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
using System.IO.Abstractions;
using System.Reflection.PortableExecutable;

using AppMotor.Core.IO;

using Moq;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.IO
{
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
                .Setup(m => m.Exists("some/file"))
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
}
