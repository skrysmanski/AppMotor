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

using AppMotor.CliApp.Terminals;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.Tests.Terminals;

public sealed class ColoredSubStringTests
{
    [Fact]
    public void TestEquals_Equatable()
    {
        IEquatable<ColoredSubstring> text = (TextInBlack)"abc";

        text.Equals(null).ShouldBe(false);
        text!.Equals(text).ShouldBe(true);
        text.Equals((TextInBlack)"abc").ShouldBe(true);
        text.Equals((TextInWhite)"abc").ShouldBe(false);
        text.Equals((TextInBlack)"ab").ShouldBe(false);
    }

    [Fact]
    public void TestEquals_Object()
    {
        object text = (TextInBlack)"abc";

        text.Equals(null).ShouldBe(false);
        text!.Equals(text).ShouldBe(true);
        text.Equals((TextInBlack)"abc").ShouldBe(true);
        text.Equals((TextInWhite)"abc").ShouldBe(false);
        text.Equals((TextInBlack)"ab").ShouldBe(false);
    }

    [Fact]
    public void TestGetHashCode()
    {
        ((TextInBlack)"abc").GetHashCode().ShouldBe(((TextInBlack)"abc").GetHashCode());
        ((TextInBlack)"abc").GetHashCode().ShouldNotBe(((TextInWhite)"abc").GetHashCode());
    }
}