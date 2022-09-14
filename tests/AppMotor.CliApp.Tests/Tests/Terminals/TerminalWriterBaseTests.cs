// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Text;

using AppMotor.CliApp.Terminals;
using AppMotor.CliApp.Terminals.Formatting;

using JetBrains.Annotations;

using Moq;
using Moq.Protected;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.Tests.Terminals;

/// <summary>
/// Tests for <see cref="TerminalWriterBase"/>.
/// </summary>
public sealed class TerminalWriterBaseTests
{
    [Theory]
    [InlineData("abc", true)]
    [InlineData(" ", true)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void Test_Write(string? input, bool shouldBeCalled)
    {
        // Setup
        var terminalWriterMock = new Mock<TerminalWriterBase>(MockBehavior.Loose);

        // Test
        terminalWriterMock.Object.Write(input);

        // Verify
        if (shouldBeCalled)
        {
            terminalWriterMock.Protected().Verify(TestTerminalWriter.NAME_FOR_WRITE_CORE_METHOD, Times.Once(), ItExpr.IsAny<string>());
            terminalWriterMock.Protected().Verify(TestTerminalWriter.NAME_FOR_WRITE_CORE_METHOD, Times.Once(), input);
        }
        else
        {
            terminalWriterMock.Protected().Verify(TestTerminalWriter.NAME_FOR_WRITE_CORE_METHOD, Times.Never(), ItExpr.IsAny<string>());
        }
    }

    [Theory]
    [InlineData("abc")]
    [InlineData(" ")]
    [InlineData("")]
    [InlineData(null)]
    public void Test_WriteLine(string? input)
    {
        // Setup
        var terminalWriterMock = new Mock<TerminalWriterBase>(MockBehavior.Loose);

        // Test
        terminalWriterMock.Object.WriteLine(input);

        // Verify
        terminalWriterMock.Protected().Verify(TestTerminalWriter.NAME_FOR_WRITE_CORE_METHOD, Times.Once(), ItExpr.IsAny<string>());
        terminalWriterMock.Protected().Verify(TestTerminalWriter.NAME_FOR_WRITE_CORE_METHOD, Times.Once(), input + Environment.NewLine);
    }

    [Theory]
    [InlineData(NewLineTypes.SystemDefault)]
    [InlineData(NewLineTypes.CrLf)]
    [InlineData(NewLineTypes.Lf)]
    [InlineData(null)]
    public void Test_NewLine(NewLineTypes? newLineType)
    {
        // Setup
        var terminalWriterMock = new Mock<TerminalWriterBase>(MockBehavior.Loose);

        // Test
        if (newLineType != null)
        {
            terminalWriterMock.Object.NewLine = newLineType.Value;
            terminalWriterMock.Object.NewLine.ShouldBe(newLineType.Value);
        }
        else
        {
            terminalWriterMock.Object.NewLine.ShouldBe(NewLineTypes.SystemDefault);
        }

        terminalWriterMock.Object.WriteLine("");

        // Verify
        terminalWriterMock.Protected().Verify(TestTerminalWriter.NAME_FOR_WRITE_CORE_METHOD, Times.Once(), ItExpr.IsAny<string>());
        terminalWriterMock.Protected().Verify(TestTerminalWriter.NAME_FOR_WRITE_CORE_METHOD, Times.Once(), newLineType?.GetNewLineCharacters() ?? Environment.NewLine);
    }

    [Fact]
    public void Test_SuppressAllAnsiSequencesStreamParserBase()
    {
        // Setup
        var parser = TestTerminalWriter.CreateSuppressAllAnsiSequencesStreamParser();

        // Test
        parser.ParseNext("abc \u001b[4mmy_test\u001b[0m def");

        // Verify
        parser.Output.ShouldBe("abc my_test def");
    }

    [Fact]
    public void Test_SuppressAnsiColorSequencesStreamParserBase()
    {
        // Setup
        var parser = TestTerminalWriter.CreateSuppressAnsiColorSequencesStreamParser();

        // Test
        parser.ParseNext("abc \u001b[4;6mmy_test\u001b[0m \u001b[33mdef\u001b[0m");

        // Verify
        parser.Output.ShouldBe("abc \u001b[4m\u001b[6mmy_test\u001b[0m def\u001b[0m");
    }

    private abstract class TestTerminalWriter : TerminalWriterBase
    {
        public const string NAME_FOR_WRITE_CORE_METHOD = nameof(WriteCore);

        [MustUseReturnValue]
        public static ITestAnsiStreamParser CreateSuppressAllAnsiSequencesStreamParser()
        {
            return new TestSuppressAllAnsiSequencesStreamParser();
        }

        [MustUseReturnValue]
        public static ITestAnsiStreamParser CreateSuppressAnsiColorSequencesStreamParser()
        {
            return new TestSuppressAnsiColorSequencesStreamParser();
        }

        public interface ITestAnsiStreamParser
        {
            string Output { get; }

            void ParseNext(ReadOnlySpan<char> text);
        }

        private sealed class TestSuppressAllAnsiSequencesStreamParser : SuppressAllAnsiSequencesStreamParserBase, ITestAnsiStreamParser
        {
            private readonly StringBuilder _outputBuilder = new();

            /// <inheritdoc />
            public string Output => this._outputBuilder.ToString();

            /// <inheritdoc />
            protected override void OnText(ReadOnlySpan<char> text)
            {
                this._outputBuilder.Append(text);
            }

        }

        private sealed class TestSuppressAnsiColorSequencesStreamParser : SuppressAnsiColorSequencesStreamParserBase, ITestAnsiStreamParser
        {
            private readonly StringBuilder _outputBuilder = new();

            /// <inheritdoc />
            public string Output => this._outputBuilder.ToString();

            /// <inheritdoc />
            protected override void OnText(ReadOnlySpan<char> text)
            {
                this._outputBuilder.Append(text);
            }

            /// <inheritdoc />
            protected override void OnNonColorAnsiEscapeSequence(ReadOnlySpan<char> escapeSequenceContents)
            {
                this._outputBuilder.Append(AnsiEscapeSequence.Create(escapeSequenceContents));
            }
        }
    }
}
