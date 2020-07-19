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

    public sealed class VerifyTests
    {
        [Fact]
        public void TestNotNull_RefType_ForArgument()
        {
            Should.NotThrow(() => Verify.Argument.NotNull("", "abc"));

            var exception = Should.Throw<ArgumentNullException>(() => Verify.Argument.NotNull((object)null, "abc"));
            exception.Message.ShouldContain(Verify.ExceptionMessages.VALUE_IS_NULL);
            exception.Message.ShouldContain("abc", Case.Sensitive);
        }

        [Fact]
        public void TestNotNull_RefType_ForValue()
        {
            Should.NotThrow(() => Verify.Value.NotNull("", "abc"));

            var exception = Should.Throw<ValueNullException>(() => Verify.Value.NotNull((object)null, "abc"));
            exception.Message.ShouldContain(Verify.ExceptionMessages.VALUE_IS_NULL);
            exception.Message.ShouldContain("abc", Case.Sensitive);
        }

        [Fact]
        public void TestNotNull_ValueType_ForArgument()
        {
            Should.NotThrow(() => Verify.Argument.NotNull((int?)42, "abc"));

            var exception = Should.Throw<ArgumentNullException>(() => Verify.Argument.NotNull((int?)null, "abc"));
            exception.Message.ShouldContain(Verify.ExceptionMessages.VALUE_IS_NULL);
            exception.Message.ShouldContain("abc", Case.Sensitive);
        }

        [Fact]
        public void TestNotNull_ValueType_ForValue()
        {
            Should.NotThrow(() => Verify.Value.NotNull((int?)42, "abc"));

            var exception = Should.Throw<ValueNullException>(() => Verify.Value.NotNull((int?)null, "abc"));
            exception.Message.ShouldContain(Verify.ExceptionMessages.VALUE_IS_NULL);
            exception.Message.ShouldContain("abc", Case.Sensitive);
        }

        [Fact]
        public void TestNotNullOrEmpty_String_ForArgument()
        {
            Should.NotThrow(() => Verify.Argument.NotNullOrEmpty("a test", "abc"));

            var exception1 = Should.Throw<ArgumentNullException>(() => Verify.Argument.NotNullOrEmpty(null, "abc"));
            exception1.Message.ShouldContain(Verify.ExceptionMessages.VALUE_IS_NULL);
            exception1.Message.ShouldContain("abc", Case.Sensitive);

            var exception2 = Should.Throw<ArgumentException>(() => Verify.Argument.NotNullOrEmpty("", "abc"));
            exception2.Message.ShouldContain(Verify.ExceptionMessages.STRING_IS_EMPTY);
            exception2.Message.ShouldContain("abc", Case.Sensitive);
        }

        [Fact]
        public void TestNotNullOrEmpty_String_ForValue()
        {
            Should.NotThrow(() => Verify.Value.NotNullOrEmpty("a test", "abc"));

            var exception1 = Should.Throw<ValueNullException>(() => Verify.Value.NotNullOrEmpty(null, "abc"));
            exception1.Message.ShouldContain(Verify.ExceptionMessages.VALUE_IS_NULL);
            exception1.Message.ShouldContain("abc", Case.Sensitive);

            var exception2 = Should.Throw<ValueException>(() => Verify.Value.NotNullOrEmpty("", "abc"));
            exception2.Message.ShouldContain(Verify.ExceptionMessages.STRING_IS_EMPTY);
            exception2.Message.ShouldContain("abc", Case.Sensitive);
        }

        [Fact]
        public void TestIsNotNullOrWhiteSpace_ForArgument()
        {
            Should.NotThrow(() => Verify.Argument.NotNullOrWhiteSpace("a test", "abc"));

            var exception1 = Should.Throw<ArgumentNullException>(() => Verify.Argument.NotNullOrWhiteSpace(null, "abc"));
            exception1.Message.ShouldContain(Verify.ExceptionMessages.VALUE_IS_NULL);
            exception1.Message.ShouldContain("abc", Case.Sensitive);

            var exception2 = Should.Throw<ArgumentException>(() => Verify.Argument.NotNullOrWhiteSpace("", "abc"));
            exception2.Message.ShouldContain(Verify.ExceptionMessages.STRING_IS_EMPTY);
            exception2.Message.ShouldContain("abc", Case.Sensitive);

            var exception3 = Should.Throw<ArgumentException>(() => Verify.Argument.NotNullOrWhiteSpace(" ", "abc"));
            exception3.Message.ShouldContain(Verify.ExceptionMessages.STRING_IS_WHITE_SPACES);
            exception3.Message.ShouldContain("abc", Case.Sensitive);

            Should.Throw<ArgumentException>(() => Verify.Argument.NotNullOrWhiteSpace("  ", "abc"));
        }

        [Fact]
        public void TestIsNotNullOrWhiteSpace_ForValue()
        {
            Should.NotThrow(() => Verify.Value.NotNullOrWhiteSpace("a test", "abc"));

            var exception1 = Should.Throw<ValueNullException>(() => Verify.Value.NotNullOrWhiteSpace(null, "abc"));
            exception1.Message.ShouldContain(Verify.ExceptionMessages.VALUE_IS_NULL);
            exception1.Message.ShouldContain("abc", Case.Sensitive);

            var exception2 = Should.Throw<ValueException>(() => Verify.Value.NotNullOrWhiteSpace("", "abc"));
            exception2.Message.ShouldContain(Verify.ExceptionMessages.STRING_IS_EMPTY);
            exception2.Message.ShouldContain("abc", Case.Sensitive);

            var exception3 = Should.Throw<ValueException>(() => Verify.Value.NotNullOrWhiteSpace(" ", "abc"));
            exception3.Message.ShouldContain(Verify.ExceptionMessages.STRING_IS_WHITE_SPACES);
            exception3.Message.ShouldContain("abc", Case.Sensitive);

            Should.Throw<ValueException>(() => Verify.Value.NotNullOrWhiteSpace("  ", "abc"));
        }

        [Fact]
        public void TestIsNotReadOnly_ForArgument()
        {
            Should.NotThrow(() => Verify.Argument.NotReadOnly(new List<string>(), "abc"));

            // ReSharper disable once AssignNullToNotNullAttribute
            var exception1 = Should.Throw<ArgumentNullException>(() => Verify.Argument.NotReadOnly((List<string>)null, "abc"));
            exception1.Message.ShouldContain(Verify.ExceptionMessages.VALUE_IS_NULL);
            exception1.Message.ShouldContain("abc", Case.Sensitive);

            var exception2 = Should.Throw<CollectionIsReadOnlyArgumentException>(() => Verify.Argument.NotReadOnly(new ReadOnlyCollection<string>(new List<string>()), "abc"));
            exception2.Message.ShouldContain("abc", Case.Sensitive);
        }

        [Fact]
        public void TestIsNotReadOnly_ForValue()
        {
            Should.NotThrow(() => Verify.Value.NotReadOnly(new List<string>(), "abc"));

            // ReSharper disable once AssignNullToNotNullAttribute
            var exception1 = Should.Throw<ValueNullException>(() => Verify.Value.NotReadOnly((List<string>)null, "abc"));
            exception1.Message.ShouldContain(Verify.ExceptionMessages.VALUE_IS_NULL);
            exception1.Message.ShouldContain("abc", Case.Sensitive);

            var exception2 = Should.Throw<CollectionIsReadOnlyValueException>(() => Verify.Value.NotReadOnly(new ReadOnlyCollection<string>(new List<string>()), "abc"));
            exception2.Message.ShouldContain("abc", Case.Sensitive);
        }
    }
}
