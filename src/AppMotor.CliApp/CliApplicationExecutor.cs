//
// NOTE: This file has been AUTOMATICALLY GENERATED from 'CliApplicationExecutor.cs.mustache'. Any changes made to
//   this file will be LOST on the next build.
//

// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

namespace AppMotor.CliApp;

/// <summary>
/// Represents the main/execute method of a <see cref="CliApplication"/> (via <see cref="CliApplication.MainExecutor"/>).
/// The main purpose of this class is to give the user the freedom to choose between a synchronous or <c>async</c>
/// method and to choose between <c>void</c>, <c>bool</c>, and <c>int</c> as return type.
/// </summary>
public class CliApplicationExecutor
{
    private readonly Func<string[], CancellationToken, Task<int>> _action;

    /// <summary>
    /// Creates an executor for a method that: is synchronous, and returns no exit code (<c>void</c>).
    ///
    /// <para>The exit code is always 0.</para>
    /// </summary>
    public CliApplicationExecutor(Action action)
    {
        this._action = (_, _) =>
        {
            action();
            return Task.FromResult(0);
        };
    }

    /// <summary>
    /// Creates an executor for a method that: is synchronous, takes in the command line params, and returns no exit code (<c>void</c>).
    ///
    /// <para>The exit code is always 0.</para>
    /// </summary>
    public CliApplicationExecutor(Action<string[]> action)
    {
        this._action = (args, _) =>
        {
            action(args);
            return Task.FromResult(0);
        };
    }

    /// <summary>
    /// Creates an executor for a method that: is synchronous, takes in the application cancellation token, and returns no exit code (<c>void</c>).
    ///
    /// <para>The exit code is always 0.</para>
    /// </summary>
    public CliApplicationExecutor(Action<CancellationToken> action)
    {
        this._action = (_, cancellationToken) =>
        {
            action(cancellationToken);
            return Task.FromResult(0);
        };
    }

    /// <summary>
    /// Creates an executor for a method that: is synchronous, takes in the command line params and the application cancellation token, and returns no exit code (<c>void</c>).
    ///
    /// <para>The exit code is always 0.</para>
    /// </summary>
    public CliApplicationExecutor(Action<string[], CancellationToken> action)
    {
        this._action = (args, cancellationToken) =>
        {
            action(args, cancellationToken);
            return Task.FromResult(0);
        };
    }

    /// <summary>
    /// Creates an executor for a method that: is synchronous, and returns an exit code.
    /// </summary>
    public CliApplicationExecutor(Func<int> action)
    {
        this._action = (_, _) =>
        {
            int retVal = action();
            return Task.FromResult(retVal);
        };
    }

    /// <summary>
    /// Creates an executor for a method that: is synchronous, takes in the command line params, and returns an exit code.
    /// </summary>
    public CliApplicationExecutor(Func<string[], int> action)
    {
        this._action = (args, _) =>
        {
            int retVal = action(args);
            return Task.FromResult(retVal);
        };
    }

    /// <summary>
    /// Creates an executor for a method that: is synchronous, takes in the application cancellation token, and returns an exit code.
    /// </summary>
    public CliApplicationExecutor(Func<CancellationToken, int> action)
    {
        this._action = (_, cancellationToken) =>
        {
            int retVal = action(cancellationToken);
            return Task.FromResult(retVal);
        };
    }

    /// <summary>
    /// Creates an executor for a method that: is synchronous, takes in the command line params and the application cancellation token, and returns an exit code.
    /// </summary>
    public CliApplicationExecutor(Func<string[], CancellationToken, int> action)
    {
        this._action = (args, cancellationToken) =>
        {
            int retVal = action(args, cancellationToken);
            return Task.FromResult(retVal);
        };
    }

    /// <summary>
    /// Creates an executor for a method that: is synchronous, and returns a success <c>bool</c>.
    ///
    /// <para>The return value of <c>true</c> is translated into the exit code 0; <c>false</c> is translated into 1.</para>
    /// </summary>
    public CliApplicationExecutor(Func<bool> action)
    {
        this._action = (_, _) =>
        {
            bool retVal = action();
            return Task.FromResult(retVal ? 0 : 1);
        };
    }

    /// <summary>
    /// Creates an executor for a method that: is synchronous, takes in the command line params, and returns a success <c>bool</c>.
    ///
    /// <para>The return value of <c>true</c> is translated into the exit code 0; <c>false</c> is translated into 1.</para>
    /// </summary>
    public CliApplicationExecutor(Func<string[], bool> action)
    {
        this._action = (args, _) =>
        {
            bool retVal = action(args);
            return Task.FromResult(retVal ? 0 : 1);
        };
    }

    /// <summary>
    /// Creates an executor for a method that: is synchronous, takes in the application cancellation token, and returns a success <c>bool</c>.
    ///
    /// <para>The return value of <c>true</c> is translated into the exit code 0; <c>false</c> is translated into 1.</para>
    /// </summary>
    public CliApplicationExecutor(Func<CancellationToken, bool> action)
    {
        this._action = (_, cancellationToken) =>
        {
            bool retVal = action(cancellationToken);
            return Task.FromResult(retVal ? 0 : 1);
        };
    }

    /// <summary>
    /// Creates an executor for a method that: is synchronous, takes in the command line params and the application cancellation token, and returns a success <c>bool</c>.
    ///
    /// <para>The return value of <c>true</c> is translated into the exit code 0; <c>false</c> is translated into 1.</para>
    /// </summary>
    public CliApplicationExecutor(Func<string[], CancellationToken, bool> action)
    {
        this._action = (args, cancellationToken) =>
        {
            bool retVal = action(args, cancellationToken);
            return Task.FromResult(retVal ? 0 : 1);
        };
    }

    /// <summary>
    /// Creates an executor for a method that: is asynchronous, and returns no exit code (<c>Task</c>/<c>void</c>).
    ///
    /// <para>The exit code is always 0.</para>
    /// </summary>
    public CliApplicationExecutor(Func<Task> action)
    {
        this._action = async (_, _) =>
        {
            await action().ConfigureAwait(continueOnCapturedContext: false);
            return 0;
        };
    }

    /// <summary>
    /// Creates an executor for a method that: is asynchronous, takes in the command line params, and returns no exit code (<c>Task</c>/<c>void</c>).
    ///
    /// <para>The exit code is always 0.</para>
    /// </summary>
    public CliApplicationExecutor(Func<string[], Task> action)
    {
        this._action = async (args, _) =>
        {
            await action(args).ConfigureAwait(continueOnCapturedContext: false);
            return 0;
        };
    }

    /// <summary>
    /// Creates an executor for a method that: is asynchronous, takes in the application cancellation token, and returns no exit code (<c>Task</c>/<c>void</c>).
    ///
    /// <para>The exit code is always 0.</para>
    /// </summary>
    public CliApplicationExecutor(Func<CancellationToken, Task> action)
    {
        this._action = async (_, cancellationToken) =>
        {
            await action(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            return 0;
        };
    }

    /// <summary>
    /// Creates an executor for a method that: is asynchronous, takes in the command line params and the application cancellation token, and returns no exit code (<c>Task</c>/<c>void</c>).
    ///
    /// <para>The exit code is always 0.</para>
    /// </summary>
    public CliApplicationExecutor(Func<string[], CancellationToken, Task> action)
    {
        this._action = async (args, cancellationToken) =>
        {
            await action(args, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            return 0;
        };
    }

    /// <summary>
    /// Creates an executor for a method that: is asynchronous, and returns an exit code.
    /// </summary>
    public CliApplicationExecutor(Func<Task<int>> action)
    {
        this._action = async (_, _) =>
        {
            int retVal = await action().ConfigureAwait(continueOnCapturedContext: false);
            return retVal;
        };
    }

    /// <summary>
    /// Creates an executor for a method that: is asynchronous, takes in the command line params, and returns an exit code.
    /// </summary>
    public CliApplicationExecutor(Func<string[], Task<int>> action)
    {
        this._action = async (args, _) =>
        {
            int retVal = await action(args).ConfigureAwait(continueOnCapturedContext: false);
            return retVal;
        };
    }

    /// <summary>
    /// Creates an executor for a method that: is asynchronous, takes in the application cancellation token, and returns an exit code.
    /// </summary>
    public CliApplicationExecutor(Func<CancellationToken, Task<int>> action)
    {
        this._action = async (_, cancellationToken) =>
        {
            int retVal = await action(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            return retVal;
        };
    }

    /// <summary>
    /// Creates an executor for a method that: is asynchronous, takes in the command line params and the application cancellation token, and returns an exit code.
    /// </summary>
    public CliApplicationExecutor(Func<string[], CancellationToken, Task<int>> action)
    {
        this._action = action;
    }

    /// <summary>
    /// Creates an executor for a method that: is asynchronous, and returns a success <c>bool</c>.
    ///
    /// <para>The return value of <c>true</c> is translated into the exit code 0; <c>false</c> is translated into 1.</para>
    /// </summary>
    public CliApplicationExecutor(Func<Task<bool>> action)
    {
        this._action = async (_, _) =>
        {
            bool retVal = await action().ConfigureAwait(continueOnCapturedContext: false);
            return retVal ? 0 : 1;
        };
    }

    /// <summary>
    /// Creates an executor for a method that: is asynchronous, takes in the command line params, and returns a success <c>bool</c>.
    ///
    /// <para>The return value of <c>true</c> is translated into the exit code 0; <c>false</c> is translated into 1.</para>
    /// </summary>
    public CliApplicationExecutor(Func<string[], Task<bool>> action)
    {
        this._action = async (args, _) =>
        {
            bool retVal = await action(args).ConfigureAwait(continueOnCapturedContext: false);
            return retVal ? 0 : 1;
        };
    }

    /// <summary>
    /// Creates an executor for a method that: is asynchronous, takes in the application cancellation token, and returns a success <c>bool</c>.
    ///
    /// <para>The return value of <c>true</c> is translated into the exit code 0; <c>false</c> is translated into 1.</para>
    /// </summary>
    public CliApplicationExecutor(Func<CancellationToken, Task<bool>> action)
    {
        this._action = async (_, cancellationToken) =>
        {
            bool retVal = await action(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            return retVal ? 0 : 1;
        };
    }

    /// <summary>
    /// Creates an executor for a method that: is asynchronous, takes in the command line params and the application cancellation token, and returns a success <c>bool</c>.
    ///
    /// <para>The return value of <c>true</c> is translated into the exit code 0; <c>false</c> is translated into 1.</para>
    /// </summary>
    public CliApplicationExecutor(Func<string[], CancellationToken, Task<bool>> action)
    {
        this._action = async (args, cancellationToken) =>
        {
            bool retVal = await action(args, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            return retVal ? 0 : 1;
        };
    }

    /// <summary>
    /// Executes this executor and returns the exit code.
    /// </summary>
    public async Task<int> Execute(string[] args, CancellationToken cancellationToken)
    {
        return await this._action(args, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
    }
}
