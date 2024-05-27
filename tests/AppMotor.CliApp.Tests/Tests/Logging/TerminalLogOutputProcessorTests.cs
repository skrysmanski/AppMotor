// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.Logging;
using AppMotor.CliApp.Terminals;
using AppMotor.Core.DateAndTime;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.Tests.Logging;

/// <summary>
/// Tests for <see cref="TerminalLogOutputProcessor"/>.
/// </summary>
public sealed class TerminalLogOutputProcessorTests
{
    /// <summary>
    /// Tests that no log messages are lost if the processor is disposed while it's still writing messages.
    /// </summary>
    [Fact]
    public async Task Test_WriteOnDispose()
    {
        // Setup
        var writtenMessages = new List<string>();
        var writingFirstLogMessageEvent = new ManualResetEventSlim();
        var unblockOutputEvent = new ManualResetEventSlim();

        var terminalOutput = new TestTerminalOutput(
            message =>
            {
                writingFirstLogMessageEvent.Set();
                unblockOutputEvent.Wait();

                lock (writtenMessages)
                {
                    writtenMessages.Add(message);
                }
            }
        );

        var outputProcessor = new TerminalLogOutputProcessor(terminalOutput);

        // Test
        outputProcessor.EnqueueMessage("Test1", logAsError: false);
        outputProcessor.EnqueueMessage("Test2", logAsError: false);

        writingFirstLogMessageEvent.Wait();

        // NOTE: Dispose() waits for the background thread to terminate. Thus, we have to call it on a different thread.
        var disposeTask = Task.Run(() => outputProcessor.Dispose());

        Thread.Sleep(50); // give it time to call "CompleteAdding()"

        unblockOutputEvent.Set();

        await disposeTask;

        // Verify
        var startDate = DateTimeUtc.Now;
        while (true)
        {
            int messagesCount;

            lock (writtenMessages)
            {
                if (writtenMessages.Count >= 2)
                {
                    break;
                }

                messagesCount = writtenMessages.Count;
            }

            Thread.Sleep(10);

            (DateTimeUtc.Now - startDate).ShouldBeLessThan(TimeSpan.FromSeconds(2), $"Message count: {messagesCount}");
        }

        writtenMessages.ShouldBe(new[] { "Test1" + Environment.NewLine, "Test2" + Environment.NewLine });
    }

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

        public TestTerminalOutput(Action<string> writeFunc)
        {
            this.Out = new SimpleTerminalWriter(writeFunc);
        }
    }
}
