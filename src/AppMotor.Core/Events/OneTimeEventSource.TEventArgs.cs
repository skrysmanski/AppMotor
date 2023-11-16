// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Diagnostics.CodeAnalysis;

namespace AppMotor.Core.Events;

/// <summary>
/// An event that will only happen one time (and then never again). Primary examples of such an event
/// are lifetime event (e.g. application started, application stopped).
/// </summary>
public class OneTimeEventSource<TEventArgs>
{
    /// <summary>
    /// The public API surface of this event.
    /// </summary>
    public OneTimeEvent<TEventArgs> Event { get; } = new();

    /// <summary>
    /// Raises/Fires the associated event by invoking all event handlers. Use this method if you're in a synchronous method.
    /// Note that <c>async</c> event handlers are invoked by this method, too.
    /// </summary>
    /// <param name="eventArgs">The event args</param>
    [SuppressMessage("Design", "CA1030:Use events where appropriate")]
    public void RaiseEvent(TEventArgs eventArgs)
    {
        Task.Run(() => this.Event.RaiseEvent(eventArgs)).Wait();
    }

    /// <summary>
    /// Raises/Fires the associated event by invoking all event handlers. Use this method if you're in an <c>async</c> method.
    /// </summary>
    /// <param name="eventArgs">The event args</param>
    [SuppressMessage("Design", "CA1030:Use events where appropriate")]
    public async Task RaiseEventAsync(TEventArgs eventArgs)
    {
        await this.Event.RaiseEvent(eventArgs).ConfigureAwait(false);
    }
}