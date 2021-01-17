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
using System.Collections.Generic;
using System.Collections.ObjectModel;

using AppMotor.Core.Exceptions;
using AppMotor.Core.Utils;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Utils
{
    // ReSharper disable NotResolvedInText

    public sealed class ValidateTests
    {
        private static readonly string? NULL_STRING = null;

        private static readonly List<string>? NULL_LIST = null;

        [Fact]
        public void TestNotNull_RefType_ForArgument()
        {
            Should.NotThrow(() => Validate.ArgumentWithName("abc").IsNotNull(""));

            var exception = Should.Throw<ArgumentNullException>(() => Validate.ArgumentWithName("abc").IsNotNull((object?)null));
            exception.Message.ShouldContain(Validate.ExceptionMessages.VALUE_IS_NULL);
            exception.Message.ShouldContain("abc", Case.Sensitive);
        }

        [Fact]
        public void TestNotNull_RefType_ForValue()
        {
            Should.NotThrow(() => Validate.ValueWithName("abc").IsNotNull(""));

            var exception = Should.Throw<ValueNullException>(() => Validate.ValueWithName("abc").IsNotNull((object?)null));
            exception.Message.ShouldContain(Validate.ExceptionMessages.VALUE_IS_NULL);
            exception.Message.ShouldContain("abc", Case.Sensitive);
        }

        [Fact]
        public void TestNotNull_ValueType_ForArgument()
        {
            Should.NotThrow(() => Validate.ArgumentWithName("abc").IsNotNull((int?)42));

            var exception = Should.Throw<ArgumentNullException>(() => Validate.ArgumentWithName("abc").IsNotNull((int?)null));
            exception.Message.ShouldContain(Validate.ExceptionMessages.VALUE_IS_NULL);
            exception.Message.ShouldContain("abc", Case.Sensitive);
        }

        [Fact]
        public void TestNotNull_ValueType_ForValue()
        {
            Should.NotThrow(() => Validate.ValueWithName("abc").IsNotNull((int?)42));

            var exception = Should.Throw<ValueNullException>(() => Validate.ValueWithName("abc").IsNotNull((int?)null));
            exception.Message.ShouldContain(Validate.ExceptionMessages.VALUE_IS_NULL);
            exception.Message.ShouldContain("abc", Case.Sensitive);
        }

        [Fact]
        public void Test_NotNull_GenericType_ForArgument()
        {
            static void RunTest<T>(T notNullValue, T nullValue)
            {
                Should.NotThrow(() => Validate.ArgumentWithName("abc").IsNotNullUnconstrained(notNullValue));

                var exception = Should.Throw<ArgumentNullException>(() => Validate.ArgumentWithName("abc").IsNotNullUnconstrained(nullValue));
                exception.Message.ShouldContain(Validate.ExceptionMessages.VALUE_IS_NULL);
                exception.Message.ShouldContain("abc", Case.Sensitive);
            }

            RunTest<string?>(notNullValue: "abc", nullValue: null);
        }

        [Fact]
        public void Test_NotNull_GenericType_ForValue()
        {
            static void RunTest<T>(T notNullValue, T nullValue)
            {
                Should.NotThrow(() => Validate.ValueWithName("abc").IsNotNullUnconstrained(notNullValue));

                var exception = Should.Throw<ValueNullException>(() => Validate.ValueWithName("abc").IsNotNullUnconstrained(nullValue));
                exception.Message.ShouldContain(Validate.ExceptionMessages.VALUE_IS_NULL);
                exception.Message.ShouldContain("abc", Case.Sensitive);
            }

            RunTest<string?>(notNullValue: "abc", nullValue: null);
        }

        [Fact]
        public void TestNotNullOrEmpty_String_ForArgument()
        {
            Should.NotThrow(() => Validate.ArgumentWithName("abc").IsNotNullOrEmpty("a test"));

            var exception1 = Should.Throw<ArgumentNullException>(() => Validate.ArgumentWithName("abc").IsNotNullOrEmpty(NULL_STRING));
            exception1.Message.ShouldContain(Validate.ExceptionMessages.VALUE_IS_NULL);
            exception1.Message.ShouldContain("abc", Case.Sensitive);

            var exception2 = Should.Throw<ArgumentException>(() => Validate.ArgumentWithName("abc").IsNotNullOrEmpty(""));
            exception2.Message.ShouldContain(Validate.ExceptionMessages.STRING_IS_EMPTY);
            exception2.Message.ShouldContain("abc", Case.Sensitive);
        }

        [Fact]
        public void TestNotNullOrEmpty_String_ForValue()
        {
            Should.NotThrow(() => Validate.ValueWithName("abc").IsNotNullOrEmpty("a test"));

            var exception1 = Should.Throw<ValueNullException>(() => Validate.ValueWithName("abc").IsNotNullOrEmpty(NULL_STRING));
            exception1.Message.ShouldContain(Validate.ExceptionMessages.VALUE_IS_NULL);
            exception1.Message.ShouldContain("abc", Case.Sensitive);

            var exception2 = Should.Throw<ValueException>(() => Validate.ValueWithName("abc").IsNotNullOrEmpty(""));
            exception2.Message.ShouldContain(Validate.ExceptionMessages.STRING_IS_EMPTY);
            exception2.Message.ShouldContain("abc", Case.Sensitive);
        }

        [Fact]
        public void TestIsNotNullOrWhiteSpace_ForArgument()
        {
            Should.NotThrow(() => Validate.ArgumentWithName("abc").IsNotNullOrWhiteSpace("a test"));

            var exception1 = Should.Throw<ArgumentNullException>(() => Validate.ArgumentWithName("abc").IsNotNullOrWhiteSpace(NULL_STRING));
            exception1.Message.ShouldContain(Validate.ExceptionMessages.VALUE_IS_NULL);
            exception1.Message.ShouldContain("abc", Case.Sensitive);

            var exception2 = Should.Throw<ArgumentException>(() => Validate.ArgumentWithName("abc").IsNotNullOrWhiteSpace(""));
            exception2.Message.ShouldContain(Validate.ExceptionMessages.STRING_IS_EMPTY);
            exception2.Message.ShouldContain("abc", Case.Sensitive);

            var exception3 = Should.Throw<ArgumentException>(() => Validate.ArgumentWithName("abc").IsNotNullOrWhiteSpace(" "));
            exception3.Message.ShouldContain(Validate.ExceptionMessages.STRING_IS_WHITE_SPACES);
            exception3.Message.ShouldContain("abc", Case.Sensitive);

            Should.Throw<ArgumentException>(() => Validate.ArgumentWithName("abc").IsNotNullOrWhiteSpace("  "));
        }

        [Fact]
        public void TestIsNotNullOrWhiteSpace_ForValue()
        {
            Should.NotThrow(() => Validate.ValueWithName("abc").IsNotNullOrWhiteSpace("a test"));

            var exception1 = Should.Throw<ValueNullException>(() => Validate.ValueWithName("abc").IsNotNullOrWhiteSpace(NULL_STRING));
            exception1.Message.ShouldContain(Validate.ExceptionMessages.VALUE_IS_NULL);
            exception1.Message.ShouldContain("abc", Case.Sensitive);

            var exception2 = Should.Throw<ValueException>(() => Validate.ValueWithName("abc").IsNotNullOrWhiteSpace(""));
            exception2.Message.ShouldContain(Validate.ExceptionMessages.STRING_IS_EMPTY);
            exception2.Message.ShouldContain("abc", Case.Sensitive);

            var exception3 = Should.Throw<ValueException>(() => Validate.ValueWithName("abc").IsNotNullOrWhiteSpace(" "));
            exception3.Message.ShouldContain(Validate.ExceptionMessages.STRING_IS_WHITE_SPACES);
            exception3.Message.ShouldContain("abc", Case.Sensitive);

            Should.Throw<ValueException>(() => Validate.ValueWithName("abc").IsNotNullOrWhiteSpace("  "));
        }

        [Fact]
        public void TestNotNullOrEmpty_Collection_ForArgument()
        {
            Should.NotThrow(() => Validate.ArgumentWithName("abc").IsNotNullOrEmpty(new List<int>() { 42 }));

            var exception1 = Should.Throw<ArgumentNullException>(() => Validate.ArgumentWithName("abc").IsNotNullOrEmpty(NULL_LIST));
            exception1.Message.ShouldContain(Validate.ExceptionMessages.VALUE_IS_NULL);
            exception1.Message.ShouldContain("abc", Case.Sensitive);

            var exception2 = Should.Throw<ArgumentException>(() => Validate.ArgumentWithName("abc").IsNotNullOrEmpty(new List<int>()));
            exception2.Message.ShouldContain(Validate.ExceptionMessages.COLLECTION_IS_EMPTY);
            exception2.Message.ShouldContain("abc", Case.Sensitive);

            Should.Throw<ArgumentException>(() => Validate.ArgumentWithName("abc").IsNotNullOrEmpty(Array.Empty<string>()));
            Should.Throw<ArgumentException>(() => Validate.ArgumentWithName("abc").IsNotNullOrEmpty(new Dictionary<string, int>()));
        }

        [Fact]
        public void TestNotNullOrEmpty_Collection_ForValue()
        {
            Should.NotThrow(() => Validate.ValueWithName("abc").IsNotNullOrEmpty(new List<int>() { 42 }));

            var exception1 = Should.Throw<ValueNullException>(() => Validate.ValueWithName("abc").IsNotNullOrEmpty(NULL_LIST));
            exception1.Message.ShouldContain(Validate.ExceptionMessages.VALUE_IS_NULL);
            exception1.Message.ShouldContain("abc", Case.Sensitive);

            var exception2 = Should.Throw<ValueException>(() => Validate.ValueWithName("abc").IsNotNullOrEmpty(new List<int>()));
            exception2.Message.ShouldContain(Validate.ExceptionMessages.COLLECTION_IS_EMPTY);
            exception2.Message.ShouldContain("abc", Case.Sensitive);

            Should.Throw<ValueException>(() => Validate.ValueWithName("abc").IsNotNullOrEmpty(Array.Empty<string>()));
            Should.Throw<ValueException>(() => Validate.ValueWithName("abc").IsNotNullOrEmpty(new Dictionary<string, int>()));
        }

        [Fact]
        public void TestIsNotReadOnly_ForArgument()
        {
            Should.NotThrow(() => Validate.ArgumentWithName("abc").IsNotReadOnly(new List<string>()));

            var exception = Should.Throw<CollectionIsReadOnlyArgumentException>(() => Validate.ArgumentWithName("abc").IsNotReadOnly(new ReadOnlyCollection<string>(new List<string>())));
            exception.Message.ShouldContain("abc", Case.Sensitive);
        }

        [Fact]
        public void TestIsNotReadOnly_ForValue()
        {
            Should.NotThrow(() => Validate.ValueWithName("abc").IsNotReadOnly(new List<string>()));

            var exception = Should.Throw<CollectionIsReadOnlyValueException>(() => Validate.ValueWithName("abc").IsNotReadOnly(new ReadOnlyCollection<string>(new List<string>())));
            exception.Message.ShouldContain("abc", Case.Sensitive);
        }
    }
}
