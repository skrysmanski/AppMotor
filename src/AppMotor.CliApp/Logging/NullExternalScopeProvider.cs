// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using Microsoft.Extensions.Logging;

namespace AppMotor.CliApp.Logging;

/// <summary>
/// Scope provider that does nothing.
/// </summary>
internal sealed class NullExternalScopeProvider : IExternalScopeProvider
{
    // Implementation based on: https://github.com/dotnet/runtime/blob/v6.0.9/src/libraries/Common/src/Extensions/Logging/NullExternalScopeProvider.cs

    public static IExternalScopeProvider Instance { get; } = new NullExternalScopeProvider();

    private NullExternalScopeProvider()
    {
    }

    public void ForEachScope<TState>(Action<object?, TState> callback, TState state)
    {
    }

    /// <inheritdoc />
    public IDisposable Push(object? state)
    {
        return NullScope.Instance;
    }

    private sealed class NullScope : IDisposable
    {
        // ReSharper disable once MemberHidesStaticFromOuterClass
        public static NullScope Instance { get; } = new();

        private NullScope()
        {
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }
    }
}
