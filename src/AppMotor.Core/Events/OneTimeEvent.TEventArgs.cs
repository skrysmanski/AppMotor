// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

namespace AppMotor.Core.Events;

/// <summary>
/// The public API surface of an <see cref="OneTimeEventSource{TEventArgs}"/> (accessible via <see cref="OneTimeEventSource{TEventArgs}.Event"/>).
/// Contains the methods to register new event handlers.
/// </summary>
public sealed class OneTimeEvent<TEventArgs>
{
    private readonly object _eventRaiseLock = new();

    private readonly EventSource<TEventArgs> _eventSource = new();

    /// <summary>
    /// Whether this event has been raised or not.
    /// </summary>
    public bool HasBeenRaised { get; private set; }

    internal OneTimeEvent()
    {
    }

    /// <summary>
    /// Registers a synchronous event handler and returns its registration. Returns <c>null</c> if the event has
    /// already been raised/fired.
    /// </summary>
    /// <param name="eventHandler">The event handler</param>
    /// <returns>The event handler registration. Dispose this instance to remove the registration.</returns>
    /// <seealso cref="RegisterEventHandler(Func{TEventArgs,Task})"/>
    public IEventHandlerRegistration? RegisterEventHandler(Action<TEventArgs> eventHandler)
    {
        lock (this._eventRaiseLock)
        {
            if (this.HasBeenRaised)
            {
                // Event has been raised
                return null;
            }
            else
            {
                return this._eventSource.Event.RegisterEventHandler(eventHandler);
            }
        }
    }

    /// <summary>
    /// Registers an <c>async</c> event handler and returns its registration. Returns <c>null</c> if the event has
    /// already been raised/fired.
    /// </summary>
    /// <param name="eventHandler">The event handler</param>
    /// <returns>The event handler registration. Dispose this instance to remove the registration.</returns>
    /// <seealso cref="RegisterEventHandler(Action{TEventArgs})"/>
    public IEventHandlerRegistration? RegisterEventHandler(Func<TEventArgs, Task> eventHandler)
    {
        lock (this._eventRaiseLock)
        {
            if (this.HasBeenRaised)
            {
                // Event has been raised
                return null;
            }
            else
            {
                return this._eventSource.Event.RegisterEventHandler(eventHandler);
            }
        }
    }

    internal async Task RaiseEvent(TEventArgs eventArgs)
    {
        lock (this._eventRaiseLock)
        {
            if (this.HasBeenRaised)
            {
                return;
            }

            this.HasBeenRaised = true;
        }

        // ReSharper disable once InconsistentlySynchronizedField
        await this._eventSource.RaiseEventAsync(eventArgs).ConfigureAwait(false);
    }
}
