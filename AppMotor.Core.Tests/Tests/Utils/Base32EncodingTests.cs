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
using System.Text;

using AppMotor.Core.Utils;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Utils
{
    public sealed class Base32EncodingTests
    {
        [Theory]
        [InlineData("", "")]
        [InlineData("f", "MY======")]
        [InlineData("fo", "MZXQ====")]
        [InlineData("foo", "MZXW6===")]
        [InlineData("foob", "MZXW6YQ=")]
        [InlineData("fooba", "MZXW6YTB")]
        [InlineData("foobar", "MZXW6YTBOI======")]
        public void TestEncoding_WithPadding(string input, string expectedOutput)
        {
            var inputArray = Encoding.ASCII.GetBytes(input);
            Base32Encoding.DefaultWithPadding.Encode(inputArray).ShouldBe(expectedOutput);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("f", "MY")]
        [InlineData("fo", "MZXQ")]
        [InlineData("foo", "MZXW6")]
        [InlineData("foob", "MZXW6YQ")]
        [InlineData("fooba", "MZXW6YTB")]
        [InlineData("foobar", "MZXW6YTBOI")]
        public void TestEncoding_WithoutPadding(string input, string expectedOutput)
        {
            var inputArray = Encoding.ASCII.GetBytes(input);
            Base32Encoding.DefaultWithoutPadding.Encode(inputArray).ShouldBe(expectedOutput);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("f", "MY======")]
        [InlineData("fo", "MZXQ====")]
        [InlineData("foo", "MZXW6===")]
        [InlineData("foob", "MZXW6YQ=")]
        [InlineData("fooba", "MZXW6YTB")]
        [InlineData("foobar", "MZXW6YTBOI======")]
        public void TestDecoding_WithPadding(string expectedOutput, string input)
        {
            var expectedOutputArray = Encoding.ASCII.GetBytes(expectedOutput);
            Base32Encoding.DefaultWithPadding.Decode(input).ShouldBe(expectedOutputArray);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("f", "MY")]
        [InlineData("fo", "MZXQ")]
        [InlineData("foo", "MZXW6")]
        [InlineData("foob", "MZXW6YQ")]
        [InlineData("fooba", "MZXW6YTB")]
        [InlineData("foobar", "MZXW6YTBOI")]
        public void TestDecoding_WithoutPadding(string expectedOutput, string input)
        {
            var expectedOutputArray = Encoding.ASCII.GetBytes(expectedOutput);
            Base32Encoding.DefaultWithoutPadding.Decode(input).ShouldBe(expectedOutputArray);
        }

        [Fact]
        public void TestInvalidPaddingChar()
        {
            Should.Throw<ArgumentException>(() => new Base32Encoding(Base32Encoding.DEFAULT_SYMBOLS, paddingChar: 'A'))
                  .ParamName.ShouldBe("paddingChar");
        }

        [Fact]
        public void TestInvalidSymbolCount()
        {
            Should.Throw<ArgumentException>(() => new Base32Encoding(Base32Encoding.DEFAULT_SYMBOLS.Substring(0, 31)))
                  .ParamName.ShouldBe("symbols");
            Should.Throw<ArgumentException>(() => new Base32Encoding(Base32Encoding.DEFAULT_SYMBOLS + "0"))
                  .ParamName.ShouldBe("symbols");
        }

        [Fact]
        public void TestDuplicatedSymbol()
        {
            Should.Throw<ArgumentException>(() => new Base32Encoding(Base32Encoding.DEFAULT_SYMBOLS.Replace('H', 'A')))
                  .ParamName.ShouldBe("symbols");
        }

        [Fact]
        public void TestDecodingInvalidChar()
        {
            // 0 is an invalid symbol in the default Base32 symbols list.
            Should.Throw<FormatException>(() => Base32Encoding.DefaultWithPadding.Decode("MZX0"));
        }
    }
}
