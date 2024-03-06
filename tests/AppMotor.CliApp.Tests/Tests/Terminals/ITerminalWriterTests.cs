// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Globalization;

using AppMotor.CliApp.Terminals;
using AppMotor.Core.Globalization;

using Moq;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.Tests.Terminals;

/// <summary>
/// Tests for <see cref="ITerminalWriter"/>.
/// </summary>
// ReSharper disable once InconsistentNaming
public sealed class ITerminalWriterTests
{
    #region Write

    /// <summary>
    /// This test primarily exists to (manually) verify that passing a string value directly
    /// calls the interface method (rather one of the extension methods or default interface methods).
    /// </summary>
    [Fact]
    public void Test_Write_String()
    {
        // Setup
        var terminalWriter = new TestTerminalWriter();

        // Test
        terminalWriter.Write("abc");

        // Verify
        terminalWriter.WriteCalls.ShouldBe(new[] { "abc" });
    }

    [Theory]
    [InlineData("de-de", 5.5, "5,5")]
    [InlineData("en-us", 5.5, "5.5")]
    [InlineData("en-us", null, null)]
    public void Test_Write_Convertible_ValueType(string culture, double? convertibleValue, string? expectedOutput)
    {
        // Setup
        var terminalWriter = new TestTerminalWriter()
        {
            Culture = new CultureInfo(culture),
        };

        // Test
        terminalWriter.Write(convertibleValue);

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

        var convertibleMock = new Mock<IConvertible>(MockBehavior.Strict);
        convertibleMock
            .Setup(m => m.ToString(It.IsAny<IFormatProvider>()))
            .Returns((IFormatProvider formatProvider) => ((CultureInfo)formatProvider).Name);

        // Test
        terminalWriter.Write(convertibleMock.Object);

        // Verify
        terminalWriter.WriteCalls.ShouldBe(new[] { culture.Name });
    }

    [Theory]
    [InlineData("de-de", 5.5, "5,5")]
    [InlineData("en-us", 5.5, "5.5")]
    [InlineData("en-us", null, null)]
    public void Test_Write_Object_IConvertible(string culture, double? convertibleValue, string? expectedOutput)
    {
        // Setup
        var terminalWriter = new TestTerminalWriter()
        {
            Culture = new CultureInfo(culture),
        };

        object? valueAsObject = convertibleValue;

        // Test
        terminalWriter.Write(valueAsObject);

        // Verify
        terminalWriter.WriteCalls.ShouldBe(new[] { expectedOutput });
    }

    [Fact]
    public void Test_Write_Object_Regular()
    {
        // Setup
        var terminalWriter = new TestTerminalWriter();

        var testValueMock = new Mock<object>(MockBehavior.Strict);
        testValueMock
            .Setup(m => m.ToString())
            .Returns("abcdef");

        object valueAsObject = testValueMock.Object;

        // Test
        terminalWriter.Write(valueAsObject);

        // Verify
        terminalWriter.WriteCalls.ShouldBe(new[] { "abcdef" });
    }

    [Theory]
    [InlineData("de-de", 5.5, "5,5")]
    [InlineData("en-us", 5.5, "5.5")]
    [InlineData("en-us", null, null)]
    public void Test_Write_Format(string culture, double? convertibleValue, string? expectedOutput)
    {
        // Setup
        var terminalWriter = new TestTerminalWriter()
        {
            Culture = new CultureInfo(culture),
        };

        // Test
        terminalWriter.Write("abc: {0}", convertibleValue);

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

        // Test
        terminalWriter.WriteLine();

        // Verify
        terminalWriter.WriteCalls.ShouldBe(new[] { Environment.NewLine });
    }

    [Fact]
    public void Test_WriteLine_String()
    {
        // Setup
        var terminalWriter = new TestTerminalWriter();

        // Test
        terminalWriter.WriteLine("abc");

        // Verify
        terminalWriter.WriteCalls.ShouldBe(new[] { "abc" + Environment.NewLine });
    }

    [Theory]
    [InlineData("de-de", 5.5, "5,5")]
    [InlineData("en-us", 5.5, "5.5")]
    [InlineData("en-us", null, null)]
    public void Test_WriteLine_Convertible_ValueType(string culture, double? convertibleValue, string? expectedOutput)
    {
        // Setup
        var terminalWriter = new TestTerminalWriter()
        {
            Culture = new CultureInfo(culture),
        };

        // Test
        terminalWriter.WriteLine(convertibleValue);

        // Verify
        terminalWriter.WriteCalls.ShouldBe(new[] { expectedOutput + Environment.NewLine });
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

        var convertibleMock = new Mock<IConvertible>(MockBehavior.Strict);
        convertibleMock
            .Setup(m => m.ToString(It.IsAny<IFormatProvider>()))
            .Returns((IFormatProvider formatProvider) => ((CultureInfo)formatProvider).Name);

        // Test
        terminalWriter.WriteLine(convertibleMock.Object);

        // Verify
        terminalWriter.WriteCalls.ShouldBe(new[] { culture.Name + Environment.NewLine });
    }

    [Theory]
    [InlineData("de-de", 5.5, "5,5")]
    [InlineData("en-us", 5.5, "5.5")]
    [InlineData("en-us", null, null)]
    public void Test_WriteLine_Object_IConvertible(string culture, double? convertibleValue, string? expectedOutput)
    {
        // Setup
        var terminalWriter = new TestTerminalWriter()
        {
            Culture = new CultureInfo(culture),
        };

        object? valueAsObject = convertibleValue;

        // Test
        terminalWriter.WriteLine(valueAsObject);

        // Verify
        terminalWriter.WriteCalls.ShouldBe(new[] { expectedOutput + Environment.NewLine });
    }

    [Fact]
    public void Test_WriteLine_Object_Regular()
    {
        // Setup
        var terminalWriter = new TestTerminalWriter();

        var testValueMock = new Mock<object>(MockBehavior.Strict);
        testValueMock
            .Setup(m => m.ToString())
            .Returns("abcdef");

        object valueAsObject = testValueMock.Object;

        // Test
        terminalWriter.WriteLine(valueAsObject);

        // Verify
        terminalWriter.WriteCalls.ShouldBe(new[] { "abcdef" + Environment.NewLine });
    }

    [Theory]
    [InlineData("de-de", 5.5, "5,5")]
    [InlineData("en-us", 5.5, "5.5")]
    [InlineData("en-us", null, null)]
    public void Test_WriteLine_Format(string culture, double? convertibleValue, string? expectedOutput)
    {
        // Setup
        var terminalWriter = new TestTerminalWriter()
        {
            Culture = new CultureInfo(culture),
        };

        // Test
        terminalWriter.WriteLine("abc: {0}", convertibleValue);

        // Verify
        terminalWriter.WriteCalls.ShouldBe(new[] { $"abc: {expectedOutput}{Environment.NewLine}" });
    }

    #endregion WriteLine

    private sealed class TestTerminalWriter : ITerminalWriter
    {
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
