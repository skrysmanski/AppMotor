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

using JetBrains.Annotations;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Utils
{
    // ReSharper disable NotResolvedInText

    public sealed class VerifyTests
    {
        [CanBeNull]
        private static readonly string NULL_STRING = null;

        [CanBeNull]
        private static readonly List<string> NULL_LIST = null;

        [Fact]
        public void TestNotNull_RefType_ForArgument()
        {
            Should.NotThrow(() => Verify.Argument.IsNotNull("", "abc"));

            var exception = Should.Throw<ArgumentNullException>(() => Verify.Argument.IsNotNull((object)null, "abc"));
            exception.Message.ShouldContain(Verify.ExceptionMessages.VALUE_IS_NULL);
            exception.Message.ShouldContain("abc", Case.Sensitive);
        }

        [Fact]
        public void TestNotNull_RefType_ForValue()
        {
            Should.NotThrow(() => Verify.Value.IsNotNull("", "abc"));

            var exception = Should.Throw<ValueNullException>(() => Verify.Value.IsNotNull((object)null, "abc"));
            exception.Message.ShouldContain(Verify.ExceptionMessages.VALUE_IS_NULL);
            exception.Message.ShouldContain("abc", Case.Sensitive);
        }

        [Fact]
        public void TestNotNull_ValueType_ForArgument()
        {
            Should.NotThrow(() => Verify.Argument.IsNotNull((int?)42, "abc"));

            var exception = Should.Throw<ArgumentNullException>(() => Verify.Argument.IsNotNull((int?)null, "abc"));
            exception.Message.ShouldContain(Verify.ExceptionMessages.VALUE_IS_NULL);
            exception.Message.ShouldContain("abc", Case.Sensitive);
        }

        [Fact]
        public void TestNotNull_ValueType_ForValue()
        {
            Should.NotThrow(() => Verify.Value.IsNotNull((int?)42, "abc"));

            var exception = Should.Throw<ValueNullException>(() => Verify.Value.IsNotNull((int?)null, "abc"));
            exception.Message.ShouldContain(Verify.ExceptionMessages.VALUE_IS_NULL);
            exception.Message.ShouldContain("abc", Case.Sensitive);
        }

        [Fact]
        public void TestNotNullOrEmpty_String_ForArgument()
        {
            Should.NotThrow(() => Verify.Argument.IsNotNullOrEmpty("a test", "abc"));

            var exception1 = Should.Throw<ArgumentNullException>(() => Verify.Argument.IsNotNullOrEmpty(NULL_STRING, "abc"));
            exception1.Message.ShouldContain(Verify.ExceptionMessages.VALUE_IS_NULL);
            exception1.Message.ShouldContain("abc", Case.Sensitive);

            var exception2 = Should.Throw<ArgumentException>(() => Verify.Argument.IsNotNullOrEmpty("", "abc"));
            exception2.Message.ShouldContain(Verify.ExceptionMessages.STRING_IS_EMPTY);
            exception2.Message.ShouldContain("abc", Case.Sensitive);
        }

        [Fact]
        public void TestNotNullOrEmpty_String_ForValue()
        {
            Should.NotThrow(() => Verify.Value.IsNotNullOrEmpty("a test", "abc"));

            var exception1 = Should.Throw<ValueNullException>(() => Verify.Value.IsNotNullOrEmpty(NULL_STRING, "abc"));
            exception1.Message.ShouldContain(Verify.ExceptionMessages.VALUE_IS_NULL);
            exception1.Message.ShouldContain("abc", Case.Sensitive);

            var exception2 = Should.Throw<ValueException>(() => Verify.Value.IsNotNullOrEmpty("", "abc"));
            exception2.Message.ShouldContain(Verify.ExceptionMessages.STRING_IS_EMPTY);
            exception2.Message.ShouldContain("abc", Case.Sensitive);
        }

        [Fact]
        public void TestIsNotNullOrWhiteSpace_ForArgument()
        {
            Should.NotThrow(() => Verify.Argument.IsNotNullOrWhiteSpace("a test", "abc"));

            var exception1 = Should.Throw<ArgumentNullException>(() => Verify.Argument.IsNotNullOrWhiteSpace(NULL_STRING, "abc"));
            exception1.Message.ShouldContain(Verify.ExceptionMessages.VALUE_IS_NULL);
            exception1.Message.ShouldContain("abc", Case.Sensitive);

            var exception2 = Should.Throw<ArgumentException>(() => Verify.Argument.IsNotNullOrWhiteSpace("", "abc"));
            exception2.Message.ShouldContain(Verify.ExceptionMessages.STRING_IS_EMPTY);
            exception2.Message.ShouldContain("abc", Case.Sensitive);

            var exception3 = Should.Throw<ArgumentException>(() => Verify.Argument.IsNotNullOrWhiteSpace(" ", "abc"));
            exception3.Message.ShouldContain(Verify.ExceptionMessages.STRING_IS_WHITE_SPACES);
            exception3.Message.ShouldContain("abc", Case.Sensitive);

            Should.Throw<ArgumentException>(() => Verify.Argument.IsNotNullOrWhiteSpace("  ", "abc"));
        }

        [Fact]
        public void TestIsNotNullOrWhiteSpace_ForValue()
        {
            Should.NotThrow(() => Verify.Value.IsNotNullOrWhiteSpace("a test", "abc"));

            var exception1 = Should.Throw<ValueNullException>(() => Verify.Value.IsNotNullOrWhiteSpace(NULL_STRING, "abc"));
            exception1.Message.ShouldContain(Verify.ExceptionMessages.VALUE_IS_NULL);
            exception1.Message.ShouldContain("abc", Case.Sensitive);

            var exception2 = Should.Throw<ValueException>(() => Verify.Value.IsNotNullOrWhiteSpace("", "abc"));
            exception2.Message.ShouldContain(Verify.ExceptionMessages.STRING_IS_EMPTY);
            exception2.Message.ShouldContain("abc", Case.Sensitive);

            var exception3 = Should.Throw<ValueException>(() => Verify.Value.IsNotNullOrWhiteSpace(" ", "abc"));
            exception3.Message.ShouldContain(Verify.ExceptionMessages.STRING_IS_WHITE_SPACES);
            exception3.Message.ShouldContain("abc", Case.Sensitive);

            Should.Throw<ValueException>(() => Verify.Value.IsNotNullOrWhiteSpace("  ", "abc"));
        }

        [Fact]
        public void TestNotNullOrEmpty_Collection_ForArgument()
        {
            Should.NotThrow(() => Verify.Argument.IsNotNullOrEmpty(new List<int>() { 42 }, "abc"));

            var exception1 = Should.Throw<ArgumentNullException>(() => Verify.Argument.IsNotNullOrEmpty(NULL_LIST, "abc"));
            exception1.Message.ShouldContain(Verify.ExceptionMessages.VALUE_IS_NULL);
            exception1.Message.ShouldContain("abc", Case.Sensitive);

            var exception2 = Should.Throw<ArgumentException>(() => Verify.Argument.IsNotNullOrEmpty(new List<int>(), "abc"));
            exception2.Message.ShouldContain(Verify.ExceptionMessages.COLLECTION_IS_EMPTY);
            exception2.Message.ShouldContain("abc", Case.Sensitive);

            Should.Throw<ArgumentException>(() => Verify.Argument.IsNotNullOrEmpty(Array.Empty<string>(), "abc"));
            Should.Throw<ArgumentException>(() => Verify.Argument.IsNotNullOrEmpty(new Dictionary<string, int>(), "abc"));
        }

        [Fact]
        public void TestNotNullOrEmpty_Collection_ForValue()
        {
            Should.NotThrow(() => Verify.Value.IsNotNullOrEmpty(new List<int>() { 42 }, "abc"));

            var exception1 = Should.Throw<ValueNullException>(() => Verify.Value.IsNotNullOrEmpty(NULL_LIST, "abc"));
            exception1.Message.ShouldContain(Verify.ExceptionMessages.VALUE_IS_NULL);
            exception1.Message.ShouldContain("abc", Case.Sensitive);

            var exception2 = Should.Throw<ValueException>(() => Verify.Value.IsNotNullOrEmpty(new List<int>(), "abc"));
            exception2.Message.ShouldContain(Verify.ExceptionMessages.COLLECTION_IS_EMPTY);
            exception2.Message.ShouldContain("abc", Case.Sensitive);

            Should.Throw<ValueException>(() => Verify.Value.IsNotNullOrEmpty(Array.Empty<string>(), "abc"));
            Should.Throw<ValueException>(() => Verify.Value.IsNotNullOrEmpty(new Dictionary<string, int>(), "abc"));
        }

        [Fact]
        public void TestIsNotReadOnly_ForArgument()
        {
            Should.NotThrow(() => Verify.Argument.IsNotReadOnly(new List<string>(), "abc"));

            // ReSharper disable once AssignNullToNotNullAttribute
            var exception1 = Should.Throw<ArgumentNullException>(() => Verify.Argument.IsNotReadOnly((List<string>)null, "abc"));
            exception1.Message.ShouldContain(Verify.ExceptionMessages.VALUE_IS_NULL);
            exception1.Message.ShouldContain("abc", Case.Sensitive);

            var exception2 = Should.Throw<CollectionIsReadOnlyArgumentException>(() => Verify.Argument.IsNotReadOnly(new ReadOnlyCollection<string>(new List<string>()), "abc"));
            exception2.Message.ShouldContain("abc", Case.Sensitive);
        }

        [Fact]
        public void TestIsNotReadOnly_ForValue()
        {
            Should.NotThrow(() => Verify.Value.IsNotReadOnly(new List<string>(), "abc"));

            // ReSharper disable once AssignNullToNotNullAttribute
            var exception1 = Should.Throw<ValueNullException>(() => Verify.Value.IsNotReadOnly((List<string>)null, "abc"));
            exception1.Message.ShouldContain(Verify.ExceptionMessages.VALUE_IS_NULL);
            exception1.Message.ShouldContain("abc", Case.Sensitive);

            var exception2 = Should.Throw<CollectionIsReadOnlyValueException>(() => Verify.Value.IsNotReadOnly(new ReadOnlyCollection<string>(new List<string>()), "abc"));
            exception2.Message.ShouldContain("abc", Case.Sensitive);
        }
    }
}
