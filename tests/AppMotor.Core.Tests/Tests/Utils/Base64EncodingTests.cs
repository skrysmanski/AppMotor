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

using System.Text;

using AppMotor.Core.Utils;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Utils;

public class Base64EncodingTests
{
    public static IEnumerable<object[]> TestDataWithPadding
    {
        get
        {
            yield return new object[] { "", "" };
            yield return new object[] { "f", "Zg==" };
            yield return new object[] { "fo", "Zm8=" };
            yield return new object[] { "foo", "Zm9v" };
            yield return new object[] { "foob", "Zm9vYg==" };
            yield return new object[] { "fooba", "Zm9vYmE=" };
            yield return new object[] { "foobar", "Zm9vYmFy" };
        }
    }

    [Theory]
    [MemberData(nameof(TestDataWithPadding))]
    public void TestEncoding_WithPadding(string input, string expectedOutput)
    {
        var inputArray = Encoding.ASCII.GetBytes(input);

        // Tests
        Base64Encoding.DefaultWithPadding.Encode(inputArray).ShouldBe(expectedOutput);
    }

    [Theory]
    [MemberData(nameof(TestDataWithPadding))]
    public void TestDecoding_WithPadding(string expectedOutput, string input)
    {
        var expectedOutputArray = Encoding.ASCII.GetBytes(expectedOutput);

        // Tests
        Base64Encoding.DefaultWithPadding.Decode(input).ShouldBe(expectedOutputArray);
    }

    [Fact]
    public void TestDecodingInvalidChar()
    {
        // $ is an invalid symbol in the default Base64 symbols list.
        Should.Throw<FormatException>(() => Base64Encoding.DefaultWithPadding.Decode("Zm9$"));
    }

    [Fact]
    public void TestDecodingInvalidLength()
    {
        Should.Throw<FormatException>(() => Base64Encoding.DefaultWithPadding.Decode("Zm9"));
    }
}