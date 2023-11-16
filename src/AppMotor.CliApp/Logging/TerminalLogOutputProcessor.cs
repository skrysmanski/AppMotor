// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Collections.Concurrent;
using System.Diagnostics;

using AppMotor.CliApp.Terminals;

using Microsoft.Extensions.Logging.Console;

namespace AppMotor.CliApp.Logging;

/// <summary>
/// Provides a queue that log entries can be pushed to - as well as an output thread
/// that will do the actual log entry writing. This makes writing log entries independent
/// of the speed of the output terminal.
/// </summary>
/// <remarks>
/// Though this solution may seem like overkill, it simply mirrors the internal implementation
/// of <see cref="ConsoleLoggerProvider"/>.
/// </remarks>
internal sealed class TerminalLogOutputProcessor : IDisposable
{
    // Implementation based on: https://github.com/dotnet/runtime/blob/v6.0.9/src/libraries/Microsoft.Extensions.Logging.Console/src/ConsoleLoggerProcessor.cs

    private const int MAX_QUEUED_MESSAGES = 1024;

    private readonly BlockingCollection<LogMessage> _messageQueue = new(MAX_QUEUED_MESSAGES);

    private readonly Thread _outputThread;

    private readonly ITerminalOutput _terminal;

    public TerminalLogOutputProcessor(ITerminalOutput terminal)
    {
        this._terminal = terminal;
        this._outputThread = new Thread(ProcessLogQueue)
        {
            IsBackground = true,
            Name = "TerminalLogger processing thread",
        };
        this._outputThread.Start();
    }

    public void Dispose()
    {
        this._messageQueue.CompleteAdding();

        this._outputThread.Join(1500);

        this._messageQueue.Dispose();
    }

    public void EnqueueMessage(string message, bool logAsError)
    {
        if (!this._messageQueue.IsAddingCompleted)
        {
            try
            {
                this._messageQueue.Add(new LogMessage(message, logAsError));
                return;
            }
            catch (InvalidOperationException)
            {
                // "IsAddingCompleted" was set to "true" by another thread after
                // we've checked it above. Ignore.
            }
        }

        // Adding is completed (i.e. either this instance is disposed or there
        // was a critical exception on the log process thread). So just log the
        // message directly.
        try
        {
            WriteMessage(message, logAsError: logAsError);
        }
        catch (Exception)
        {
            // Ignore all exceptions here
        }
    }

    private void WriteMessage(string message, bool logAsError)
    {
        try
        {
            var outputWriter = logAsError ? this._terminal.Error : this._terminal.Out;
            outputWriter.WriteLine(message);
        }
        catch (Exception)
        {
            // Logging should not throw any exceptions
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }
    }

    private void ProcessLogQueue()
    {
        try
        {
            // NOTE: Calling "CompleteAdding()" in "Dispose()" does not(!) prevent us from reading the queue.
            foreach (var logMessage in this._messageQueue.GetConsumingEnumerable())
            {
                WriteMessage(logMessage.Message, logAsError: logMessage.LogAsError);
            }
        }
        catch (Exception)
        {
            try
            {
                // Mark this thread as "dead"
                this._messageQueue.CompleteAdding();
            }
            catch (Exception)
            {
                // Ignore all exceptions here
            }
        }
    }

    private readonly struct LogMessage
    {
        public readonly string Message;

        public readonly bool LogAsError;

        public LogMessage(string message, bool logAsError)
        {
            this.Message = message;
            this.LogAsError = logAsError;
        }
    }
}
