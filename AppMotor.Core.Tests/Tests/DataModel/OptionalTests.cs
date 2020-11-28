#region License
// Copyright 2020 AppMotor Framework (https://github.com/skrysmanski/AppMotor)
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

using AppMotor.Core.DataModel;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.DataModel
{
    public sealed class OptionalTests
    {
        [Fact]
        public void TestEmpty_NullableRefType()
        {
            var optional = Optional<string?>.UNSET;

            optional.IsSet.ShouldBe(false);
            Should.Throw<InvalidOperationException>(() => optional.Value);
            optional.ToString().ShouldBe("<not set>");
        }

        [Fact]
        public void TestEmpty_NonNullableRefType()
        {
            var optional = new Optional<string>();

            optional.IsSet.ShouldBe(false);
            Should.Throw<InvalidOperationException>(() => optional.Value);
            optional.ToString().ShouldBe("<not set>");
        }

        [Fact]
        public void TestEmpty_NullableValueType()
        {
            var optional = Optional<int?>.UNSET;

            optional.IsSet.ShouldBe(false);
            Should.Throw<InvalidOperationException>(() => optional.Value);
            optional.ToString().ShouldBe("<not set>");
        }

        [Fact]
        public void TestEmpty_NonNullableValueType()
        {
            var optional = new Optional<int>();

            optional.IsSet.ShouldBe(false);
            Should.Throw<InvalidOperationException>(() => optional.Value);
            optional.ToString().ShouldBe("<not set>");
        }

        [Fact]
        public void Test_NullableRefType()
        {
            Optional<string?> optional = "abc";

            optional.IsSet.ShouldBe(true);
            optional.Value.ShouldBe("abc");
            optional.ToString().ShouldBe("abc");
        }

        [Fact]
        public void Test_NonNullableRefType()
        {
            Optional<string> optional = "abc";

            optional.IsSet.ShouldBe(true);
            optional.Value.ShouldBe("abc");
            optional.ToString().ShouldBe("abc");
        }

        [Fact]
        public void Test_NullableValueType()
        {
            Optional<int?> optional = 42;

            optional.IsSet.ShouldBe(true);
            optional.Value.ShouldBe(42);
            optional.ToString().ShouldBe("42");
        }

        [Fact]
        public void Test_NonNullableValueType()
        {
            Optional<int> optional = 42;

            optional.IsSet.ShouldBe(true);
            optional.Value.ShouldBe(42);
            optional.ToString().ShouldBe("42");
        }
    }
}
