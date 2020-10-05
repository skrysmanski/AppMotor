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
using System.IO;
using System.Text;
using System.Threading.Tasks;

using AppMotor.Core.IO;
using AppMotor.Core.Utils;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Utils
{
    public sealed class Base32EncodingTests
    {
        public static IEnumerable<object[]> TestDataWithPadding
        {
            get
            {
                yield return new object[] { "", "" };
                yield return new object[] { "f", "MY======" };
                yield return new object[] { "fo", "MZXQ====" };
                yield return new object[] { "foo", "MZXW6===" };
                yield return new object[] { "foob", "MZXW6YQ=" };
                yield return new object[] { "fooba", "MZXW6YTB" };
                yield return new object[] { "foobar", "MZXW6YTBOI======" };
            }
        }

        public static IEnumerable<object[]> TestDataWithoutPadding
        {
            get
            {
                yield return new object[] { "", "" };
                yield return new object[] { "f", "MY" };
                yield return new object[] { "fo", "MZXQ" };
                yield return new object[] { "foo", "MZXW6" };
                yield return new object[] { "foob", "MZXW6YQ" };
                yield return new object[] { "fooba", "MZXW6YTB" };
                yield return new object[] { "foobar", "MZXW6YTBOI" };
            }
        }

        [Theory]
        [MemberData(nameof(TestDataWithPadding))]
        public void TestEncoding_WithPadding(string input, string expectedOutput)
        {
            var inputArray = Encoding.ASCII.GetBytes(input);
            using var inputStream = new ReadOnlyMemoryStream(inputArray);
            using var outputWriter = new StringWriter();

            // Tests
            Base32Encoding.DefaultWithPadding.Encode(inputArray).ShouldBe(expectedOutput);

            Base32Encoding.DefaultWithPadding.Encode(inputStream, outputWriter);
            outputWriter.ToString().ShouldBe(expectedOutput);
        }

        [Theory]
        [MemberData(nameof(TestDataWithPadding))]
        public async Task TestEncodingAsync_WithPadding(string input, string expectedOutput)
        {
            var inputArray = Encoding.ASCII.GetBytes(input);
            await using var inputStream = new ReadOnlyMemoryStream(inputArray);
            await using var outputWriter = new StringWriter();

            // Tests
            await Base32Encoding.DefaultWithPadding.EncodeAsync(inputStream, outputWriter);
            outputWriter.ToString().ShouldBe(expectedOutput);
        }

        [Theory]
        [MemberData(nameof(TestDataWithoutPadding))]
        public void TestEncoding_WithoutPadding(string input, string expectedOutput)
        {
            var inputArray = Encoding.ASCII.GetBytes(input);
            using var inputStream = new ReadOnlyMemoryStream(inputArray);
            using var outputWriter = new StringWriter();

            // Tests
            Base32Encoding.DefaultWithoutPadding.Encode(inputArray).ShouldBe(expectedOutput);

            Base32Encoding.DefaultWithoutPadding.Encode(inputStream, outputWriter);
            outputWriter.ToString().ShouldBe(expectedOutput);
        }

        [Theory]
        [MemberData(nameof(TestDataWithoutPadding))]
        public async Task TestEncodingAsync_WithoutPadding(string input, string expectedOutput)
        {
            var inputArray = Encoding.ASCII.GetBytes(input);
            await using var inputStream = new ReadOnlyMemoryStream(inputArray);
            await using var outputWriter = new StringWriter();

            // Tests
            await Base32Encoding.DefaultWithoutPadding.EncodeAsync(inputStream, outputWriter);
            outputWriter.ToString().ShouldBe(expectedOutput);
        }

        [Theory]
        [MemberData(nameof(TestDataWithPadding))]
        public void TestDecoding_WithPadding(string expectedOutput, string input)
        {
            using var inputReader = new StringReader(input);
            using var outputStream = new MemoryStream();
            var expectedOutputArray = Encoding.ASCII.GetBytes(expectedOutput);

            // Tests
            Base32Encoding.DefaultWithPadding.Decode(input).ShouldBe(expectedOutputArray);

            Base32Encoding.DefaultWithPadding.Decode(inputReader, outputStream);
            outputStream.ToArray().ShouldBe(expectedOutputArray);
        }

        [Theory]
        [MemberData(nameof(TestDataWithPadding))]
        public async Task TestDecodingAsync_WithPadding(string expectedOutput, string input)
        {
            using var inputReader = new StringReader(input);
            await using var outputStream = new MemoryStream();
            var expectedOutputArray = Encoding.ASCII.GetBytes(expectedOutput);

            // Tests
            await Base32Encoding.DefaultWithPadding.DecodeAsync(inputReader, outputStream);
            outputStream.ToArray().ShouldBe(expectedOutputArray);
        }

        [Theory]
        [MemberData(nameof(TestDataWithoutPadding))]
        public void TestDecoding_WithoutPadding(string expectedOutput, string input)
        {
            using var inputReader = new StringReader(input);
            using var outputStream = new MemoryStream();
            var expectedOutputArray = Encoding.ASCII.GetBytes(expectedOutput);

            // Tests
            Base32Encoding.DefaultWithoutPadding.Decode(input).ShouldBe(expectedOutputArray);

            Base32Encoding.DefaultWithoutPadding.Decode(inputReader, outputStream);
            outputStream.ToArray().ShouldBe(expectedOutputArray);
        }

        [Theory]
        [MemberData(nameof(TestDataWithoutPadding))]
        public async Task TestDecodingAsync_WithoutPadding(string expectedOutput, string input)
        {
            using var inputReader = new StringReader(input);
            await using var outputStream = new MemoryStream();
            var expectedOutputArray = Encoding.ASCII.GetBytes(expectedOutput);

            // Tests
            await Base32Encoding.DefaultWithoutPadding.DecodeAsync(inputReader, outputStream);
            outputStream.ToArray().ShouldBe(expectedOutputArray);
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
