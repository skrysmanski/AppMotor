#region License
// Copyright 2020 - 2020 AppWeave.Core (https://github.com/skrysmanski/AppWeave.Core)
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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using AppWeave.Core.Extensions;

using JetBrains.Annotations;

using Shouldly;

using Xunit;

using ExceptionExtensions = AppWeave.Core.Extensions.ExceptionExtensions;

namespace AppWeave.Core.Tests.Extensions
{
    /// <summary>
    /// Tests for <see cref="ExceptionExtensions"/>.
    /// </summary>
    public sealed class ExceptionExtensionsTests
    {
        [Fact]
        public void TestAddData()
        {
            // setup
            var exception = new Exception();

            // test
            exception.AddData("abc", 42);

            // verify
            exception.GetData()["abc"].ShouldBe(42);
        }

        /// <summary>
        /// Verifies that <see cref="ExceptionExtensions.AddData"/> does not
        /// throw an exception if the exception data dictionary is read-only.
        /// </summary>
        [Fact]
        public void TestAddData_ReadOnlyData()
        {
            // setup
            var exception = new ExceptionWithReadOnlyData();

            // test our assumptions
            exception.Data.IsReadOnly.ShouldBe(true);

            // tests
            exception.AddData("abc", 42); // should not do anything
            exception.GetData().Count.ShouldBe(0);
        }

        /// <summary>
        /// Tests adding a non-serializable type as exception data. This tests is just for compatibility
        /// reasons as this was a requirement in the .NET Framework - but this requirement has been
        /// dropped with .NET Core.
        /// </summary>
        [Fact]
        public void TestAddData_NonSerializableData()
        {
            // setup
            var exception = new Exception();

            // test
            exception.AddData("abc", new SomeDataClass()); // must not throw
        }

        private sealed class ExceptionWithReadOnlyData : Exception
        {
            /// <inheritdoc />
            public override IDictionary Data { get; } = new ReadOnlyDictionary<object, object>(new Dictionary<object, object>());
        }

        private sealed class SomeDataClass
        {
            [UsedImplicitly]
            public int SomeProperty { get; } = 42;
        }
    }
}
