#region License
// Copyright 2022 AppMotor Framework (https://github.com/skrysmanski/AppMotor)
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

using System.Globalization;

using AppMotor.Core.Extensions;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Extensions;

public sealed class NumbersExtensionsTests
{
    [Theory]
    [InlineData(0.0f, 0.0f, true)]
    [InlineData(0.0f, 0.00009f, true)]
    [InlineData(0.0f, 0.0001f, false)]
    [InlineData(0.0f, -0.00009f, true)]
    [InlineData(0.0f, -0.0001f, false)]
    public void Test_IsBasicallyEqualTo_Float(float a, float b, bool expectToBeEqual)
    {
        a.IsBasicallyEqualTo(b).ShouldBe(expectToBeEqual);
    }

    [Theory]
    [InlineData(0.0, 0.0, true)]
    [InlineData(0.0, 0.00009, true)]
    [InlineData(0.0, 0.0001, false)]
    [InlineData(0.0, -0.00009, true)]
    [InlineData(0.0, -0.0001, false)]
    public void Test_IsBasicallyEqualTo_Double(double a, double b, bool expectToBeEqual)
    {
        a.IsBasicallyEqualTo(b).ShouldBe(expectToBeEqual);
    }

    [Theory]
    [InlineData("0.0", "0.0", true)]
    [InlineData("0.0", "0.00009", true)]
    [InlineData("0.0", "0.0001", false)]
    [InlineData("0.0", "-0.00009", true)]
    [InlineData("0.0", "-0.0001", false)]
    public void Test_IsBasicallyEqualTo_Decimal(string aAsString, string bAsString, bool expectToBeEqual)
    {
        var a = decimal.Parse(aAsString, CultureInfo.InvariantCulture);
        var b = decimal.Parse(bAsString, CultureInfo.InvariantCulture);

        a.IsBasicallyEqualTo(b).ShouldBe(expectToBeEqual);
    }
}
