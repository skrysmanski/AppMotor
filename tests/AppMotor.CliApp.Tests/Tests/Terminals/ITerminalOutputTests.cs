// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.Terminals;
using AppMotor.Core.Globalization;

using Moq;

using Shouldly;
using System.Globalization;

using AppMotor.Core.Utils;

using Xunit;

namespace AppMotor.CliApp.Tests.Terminals;

/// <summary>
/// Tests for <see cref="ITerminalOutput"/> and <see cref="ITerminalOutputExtensions"/>.
/// </summary>
// ReSharper disable once InconsistentNaming
public sealed class ITerminalOutputTests
{
    #region Write

    [Fact]
    public void Test_Write_String()
    {
        // Setup
        var terminalWriter = new TestTerminalWriter();
        var terminalOutput = new TestTerminalOutput(terminalWriter);

        // Test
        terminalOutput.Write("abc");

        // Verify
        terminalWriter.WriteCalls.ShouldBe(new[] { "abc" });
    }

    [Theory]
    [InlineData("de-de", 5.5, "5,5")]
    [InlineData("en-us", 5.5, "5.5")]
    [InlineData("en-us", null, null)]
    public void Test_Write_Convertible_ValueType(string culture, double? convertibleValue, string expectedOutput)
    {
        // Setup
        var terminalWriter = new TestTerminalWriter()
        {
            Culture = new CultureInfo(culture),
        };
        var terminalOutput = new TestTerminalOutput(terminalWriter);

        // Test
        terminalOutput.Write(convertibleValue);

        // Verify
        terminalWriter.WriteCalls.ShouldBe(new[] { expectedOutput });
    }

    [Theory]
    [InlineData("de-de")]
    [InlineData("en-us")]
    public void Test_Write_Convertible_RefType(string cultureAsString)
    {
        // Setup
        var culture = new CultureInfo(cultureAsString);
        var terminalWriter = new TestTerminalWriter()
        {
            Culture = culture,
        };
        var terminalOutput = new TestTerminalOutput(terminalWriter);

        var convertibleMock = new Mock<IConvertible>(MockBehavior.Strict);
        convertibleMock
            .Setup(m => m.ToString(It.IsAny<IFormatProvider>()))
            .Returns((IFormatProvider formatProvider) => ((CultureInfo)formatProvider).Name);

        // Test
        terminalOutput.Write(convertibleMock.Object);

        // Verify
        terminalWriter.WriteCalls.ShouldBe(new[] { culture.Name });
    }

    [Theory]
    [InlineData("de-de", 5.5, "5,5")]
    [InlineData("en-us", 5.5, "5.5")]
    [InlineData("en-us", null, null)]
    public void Test_Write_Object_IConvertible(string culture, double? convertibleValue, string expectedOutput)
    {
        // Setup
        var terminalWriter = new TestTerminalWriter()
        {
            Culture = new CultureInfo(culture),
        };
        var terminalOutput = new TestTerminalOutput(terminalWriter);

        object? valueAsObject = convertibleValue;

        // Test
        terminalOutput.Write(valueAsObject);

        // Verify
        terminalWriter.WriteCalls.ShouldBe(new[] { expectedOutput });
    }

    [Fact]
    public void Test_Write_Object_Regular()
    {
        // Setup
        var terminalWriter = new TestTerminalWriter();
        var terminalOutput = new TestTerminalOutput(terminalWriter);

        var testValueMock = new Mock<object>(MockBehavior.Strict);
        testValueMock
            .Setup(m => m.ToString())
            .Returns("abcdef");

        object? valueAsObject = testValueMock.Object;

        // Test
        terminalOutput.Write(valueAsObject);

        // Verify
        terminalWriter.WriteCalls.ShouldBe(new[] { "abcdef" });
    }

    [Theory]
    [InlineData("de-de", 5.5, "5,5")]
    [InlineData("en-us", 5.5, "5.5")]
    [InlineData("en-us", null, null)]
    public void Test_Write_Format(string culture, double? convertibleValue, string expectedOutput)
    {
        // Setup
        var terminalWriter = new TestTerminalWriter()
        {
            Culture = new CultureInfo(culture),
        };
        var terminalOutput = new TestTerminalOutput(terminalWriter);

        // Test
        terminalOutput.Write("abc: {0}", convertibleValue);

        // Verify
        terminalWriter.WriteCalls.ShouldBe(new[] { $"abc: {expectedOutput}" });
    }

    #endregion Write

    #region WriteLine

    [Fact]
    public void Test_WriteLine()
    {
        // Setup
        var terminalWriter = new TestTerminalWriter();
        var terminalOutput = new TestTerminalOutput(terminalWriter);

        // Test
        terminalOutput.WriteLine();

        // Verify
        terminalWriter.WriteCalls.ShouldBe(new[] { "\n" });
    }

    [Fact]
    public void Test_WriteLine_String()
    {
        // Setup
        var terminalWriter = new TestTerminalWriter();
        var terminalOutput = new TestTerminalOutput(terminalWriter);

        // Test
        terminalOutput.WriteLine("abc");

        // Verify
        terminalWriter.WriteCalls.ShouldBe(new[] { "abc\n" });
    }

    [Theory]
    [InlineData("de-de", 5.5, "5,5")]
    [InlineData("en-us", 5.5, "5.5")]
    [InlineData("en-us", null, null)]
    public void Test_WriteLine_Convertible_ValueType(string culture, double? convertibleValue, string expectedOutput)
    {
        // Setup
        var terminalWriter = new TestTerminalWriter()
        {
            Culture = new CultureInfo(culture),
        };
        var terminalOutput = new TestTerminalOutput(terminalWriter);

        // Test
        terminalOutput.WriteLine(convertibleValue);

        // Verify
        terminalWriter.WriteCalls.ShouldBe(new[] { expectedOutput + "\n" });
    }

    [Theory]
    [InlineData("de-de")]
    [InlineData("en-us")]
    public void Test_WriteLine_Convertible_RefType(string cultureAsString)
    {
        // Setup
        var culture = new CultureInfo(cultureAsString);
        var terminalWriter = new TestTerminalWriter()
        {
            Culture = culture,
        };
        var terminalOutput = new TestTerminalOutput(terminalWriter);

        var convertibleMock = new Mock<IConvertible>(MockBehavior.Strict);
        convertibleMock
            .Setup(m => m.ToString(It.IsAny<IFormatProvider>()))
            .Returns((IFormatProvider formatProvider) => ((CultureInfo)formatProvider).Name);

        // Test
        terminalOutput.WriteLine(convertibleMock.Object);

        // Verify
        terminalWriter.WriteCalls.ShouldBe(new[] { culture.Name + "\n" });
    }

    [Theory]
    [InlineData("de-de", 5.5, "5,5")]
    [InlineData("en-us", 5.5, "5.5")]
    [InlineData("en-us", null, null)]
    public void Test_WriteLine_Object_IConvertible(string culture, double? convertibleValue, string expectedOutput)
    {
        // Setup
        var terminalWriter = new TestTerminalWriter()
        {
            Culture = new CultureInfo(culture),
        };
        var terminalOutput = new TestTerminalOutput(terminalWriter);

        object? valueAsObject = convertibleValue;

        // Test
        terminalOutput.WriteLine(valueAsObject);

        // Verify
        terminalWriter.WriteCalls.ShouldBe(new[] { expectedOutput + "\n" });
    }

    [Fact]
    public void Test_WriteLine_Object_Regular()
    {
        // Setup
        var terminalWriter = new TestTerminalWriter();
        var terminalOutput = new TestTerminalOutput(terminalWriter);

        var testValueMock = new Mock<object>(MockBehavior.Strict);
        testValueMock
            .Setup(m => m.ToString())
            .Returns("abcdef");

        object? valueAsObject = testValueMock.Object;

        // Test
        terminalOutput.WriteLine(valueAsObject);

        // Verify
        terminalWriter.WriteCalls.ShouldBe(new[] { "abcdef\n" });
    }

    [Theory]
    [InlineData("de-de", 5.5, "5,5")]
    [InlineData("en-us", 5.5, "5.5")]
    [InlineData("en-us", null, null)]
    public void Test_WriteLine_Format(string culture, double? convertibleValue, string expectedOutput)
    {
        // Setup
        var terminalWriter = new TestTerminalWriter()
        {
            Culture = new CultureInfo(culture),
        };
        var terminalOutput = new TestTerminalOutput(terminalWriter);

        // Test
        terminalOutput.WriteLine("abc: {0}", convertibleValue);

        // Verify
        terminalWriter.WriteCalls.ShouldBe(new[] { $"abc: {expectedOutput}\n" });
    }

    #endregion WriteLine

    private sealed class TestTerminalOutput : ITerminalOutput
    {
        /// <inheritdoc />
        public ITerminalWriter Out { get; }

        /// <inheritdoc />
        public bool IsOutputRedirected => throw new NotSupportedException();

        /// <inheritdoc />
        public ITerminalWriter Error => throw new NotSupportedException();

        /// <inheritdoc />
        public bool IsErrorRedirected => throw new NotSupportedException();

        public TestTerminalOutput(ITerminalWriter @out)
        {
            this.Out = @out;
        }
    }

    private sealed class TestTerminalWriter : ITerminalWriter
    {
        /// <inheritdoc />
        public LineTerminators LineTerminator { get; set; } = LineTerminators.Lf;

        /// <inheritdoc />
        public IFormatProvider Culture { get; set; } = UICulture.FormatsAndSorting;

        /// <inheritdoc />
        public bool EnableColors { get; set; } = true;

        public List<string?> WriteCalls { get; } = new();

        /// <inheritdoc />
        public void Write(string? value)
        {
            this.WriteCalls.Add(value);
        }
    }
}
