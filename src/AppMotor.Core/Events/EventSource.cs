// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Diagnostics.CodeAnalysis;

namespace AppMotor.Core.Events;

/// <summary>
/// Same as <see cref="EventSource{TEventArgs}"/> but without event args. See <see cref="EventSource{TEventArgs}"/>
/// for more details on the concept.
/// </summary>
public class EventSource
{
    /// <summary>
    /// The public API surface of this event.
    /// </summary>
    public Event Event { get; } = new();

    /// <summary>
    /// Raises/Fires the associated event by invoking all event handlers. Use this method if you're in a synchronous method.
    /// Note that <c>async</c> event handlers are invoked by this method, too.
    /// </summary>
    [SuppressMessage("Design", "CA1030:Use events where appropriate")]
    public void RaiseEvent()
    {
        Task.Run(() => this.Event.RaiseEventAsync()).Wait();
    }

    /// <summary>
    /// Raises/Fires the associated event by invoking all event handlers. Use this method if you're in an <c>async</c> method.
    /// </summary>
    [SuppressMessage("Design", "CA1030:Use events where appropriate")]
    public async Task RaiseEventAsync()
    {
        await this.Event.RaiseEventAsync().ConfigureAwait(false);
    }
}
