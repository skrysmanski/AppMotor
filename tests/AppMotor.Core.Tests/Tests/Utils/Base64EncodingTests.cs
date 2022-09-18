// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

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
