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
using System.Collections;
using System.Collections.Generic;

using AppMotor.Core.Utils;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Utils
{
    public sealed class SimpleEqualityComparerTests
    {
        /// <summary>
        /// Tests <see cref="SimpleRefTypeEqualityComparer{T}"/> as <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        [Fact]
        public void Test_ForRefType_Generic()
        {
            IEqualityComparer<TestRefType?> comparer = new RefTypeComparer();

            comparer.Equals(null, null).ShouldBe(true);
            comparer.Equals(new TestRefType("abc"), null).ShouldBe(false);
            comparer.Equals(null, new TestRefType("abc")).ShouldBe(false);
            comparer.Equals(new TestRefType("abc"), new TestRefType("abc")).ShouldBe(true);

            comparer.GetHashCode(new TestRefType("abc")).ShouldBe("abc".GetHashCode());
            comparer.GetHashCode(null!).ShouldBe(0);
        }

        /// <summary>
        /// Tests the implementation of <see cref="SimpleRefTypeEqualityComparer{T}"/> itself (as <see cref="IEqualityComparer{T}"/>)
        /// by having an implementation that throws an every method.
        /// </summary>
        [Fact]
        public void Test_ForRefType_Unimplemented_Generic()
        {
            IEqualityComparer<string?> comparer = new UnimplementedRefTypeComparer();

            comparer.Equals(null, null).ShouldBe(true);
            comparer.Equals("abc", null).ShouldBe(false);
            comparer.Equals(null, "abc").ShouldBe(false);
            comparer.Equals("abc", "abc").ShouldBe(true);

            comparer.GetHashCode(null!).ShouldBe(0);
        }

        /// <summary>
        /// Tests <see cref="SimpleRefTypeEqualityComparer{T}"/> as (non-generic) <see cref="IEqualityComparer"/>.
        /// </summary>
        [Fact]
        public void Test_ForRefType_NonGeneric()
        {
            IEqualityComparer comparer = new RefTypeComparer();

            comparer.Equals(null, null).ShouldBe(true);
            comparer.Equals(new TestRefType("abc"), null).ShouldBe(false);
            comparer.Equals(null, new TestRefType("abc")).ShouldBe(false);
            comparer.Equals(new TestRefType("abc"), new TestRefType("abc")).ShouldBe(true);

            comparer.Equals("abc", "abc").ShouldBe(true); // wrong type - should still be true; see comment in implementation

            comparer.GetHashCode(new TestRefType("abc")).ShouldBe("abc".GetHashCode());
            comparer.GetHashCode(null!).ShouldBe(0);
        }

        /// <summary>
        /// Tests the implementation of <see cref="SimpleRefTypeEqualityComparer{T}"/> itself (as (non-generic) <see cref="IEqualityComparer"/>)
        /// by having an implementation that throws an every method.
        /// </summary>
        [Fact]
        public void Test_ForRefType_Unimplemented_NonGeneric()
        {
            IEqualityComparer comparer = new UnimplementedRefTypeComparer();

            comparer.Equals(null, null).ShouldBe(true);
            comparer.Equals("abc", null).ShouldBe(false);
            comparer.Equals(null, "abc").ShouldBe(false);
            comparer.Equals("abc", "abc").ShouldBe(true);

            comparer.GetHashCode(null!).ShouldBe(0);
        }

        /// <summary>
        /// Tests <see cref="SimpleValueTypeEqualityComparer{T}"/> as <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        [Fact]
        public void Test_ForValueType_Generic()
        {
            IEqualityComparer<int?> comparer = new ValueTypeComparer();

            comparer.Equals(null, null).ShouldBe(true);
            comparer.Equals(42, null).ShouldBe(false);
            comparer.Equals(null, 42).ShouldBe(false);
            comparer.Equals(42, 42).ShouldBe(true);

            comparer.GetHashCode(42).ShouldBe(42.GetHashCode());
            comparer.GetHashCode(null!).ShouldBe(0);
        }

        /// <summary>
        /// Tests <see cref="SimpleValueTypeEqualityComparer{T}"/> as (non-generic) <see cref="IEqualityComparer"/>.
        /// </summary>
        [Fact]
        public void Test_ForValueType_NonGeneric()
        {
            IEqualityComparer comparer = new ValueTypeComparer();

            comparer.Equals(null, null).ShouldBe(true);
            comparer.Equals(42, null).ShouldBe(false);
            comparer.Equals(null, 42).ShouldBe(false);
            comparer.Equals(42, 42).ShouldBe(true);
            comparer.Equals((int?)42, 42).ShouldBe(true);
            comparer.Equals(42, (int?)42).ShouldBe(true);
            comparer.Equals((int?)42, (int?)42).ShouldBe(true);

            comparer.GetHashCode(42).ShouldBe(42.GetHashCode());
            comparer.GetHashCode(null!).ShouldBe(0);
        }

        private sealed class TestRefType
        {
            public string Value { get; }

            public TestRefType(string value)
            {
                this.Value = value;
            }
        }

        private sealed class RefTypeComparer : SimpleRefTypeEqualityComparer<TestRefType>
        {
            /// <inheritdoc />
            protected override bool EqualsCore(TestRefType x, TestRefType y)
            {
                return x.Value.Equals(y.Value, StringComparison.Ordinal);
            }

            /// <inheritdoc />
            protected override int GetHashCodeCore(TestRefType value)
            {
                return value.Value.GetHashCode();
            }
        }

        private sealed class UnimplementedRefTypeComparer : SimpleRefTypeEqualityComparer<string>
        {
            /// <inheritdoc />
            protected override bool EqualsCore(string x, string y)
            {
                throw new NotSupportedException();
            }

            /// <inheritdoc />
            protected override int GetHashCodeCore(string value)
            {
                throw new NotSupportedException();
            }
        }

        private sealed class ValueTypeComparer : SimpleValueTypeEqualityComparer<int>
        {
            /// <inheritdoc />
            protected override bool EqualsCore(int x, int y)
            {
                return x == y;
            }

            /// <inheritdoc />
            protected override int GetHashCodeCore(int value)
            {
                return value.GetHashCode();
            }
        }
    }
}
