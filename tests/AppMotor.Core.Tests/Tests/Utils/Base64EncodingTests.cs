// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Text;

using AppMotor.Core.Utils;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Utils;

public sealed class Base64EncodingTests
{
    public static IEnumerable<object[]> TestDataWithPadding
    {
        get
        {
            yield return ["", ""];
            yield return ["f", "Zg=="];
            yield return ["fo", "Zm8="];
            yield return ["foo", "Zm9v"];
            yield return ["foob", "Zm9vYg=="];
            yield return ["fooba", "Zm9vYmE="];
            yield return ["foobar", "Zm9vYmFy"];
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
