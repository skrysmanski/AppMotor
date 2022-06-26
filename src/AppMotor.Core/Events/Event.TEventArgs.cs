#region License
// Copyright 2021 AppMotor Framework (https://github.com/skrysmanski/AppMotor)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.Collections.Immutable;

using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.Core.Events;

/// <summary>
/// The public API surface of an <see cref="EventSource{TEventArgs}"/> (accessible via <see cref="EventSource{TEventArgs}.Event"/>).
/// Contains the methods to register new event handlers.
/// </summary>
public sealed class Event<TEventArgs>
{
    private readonly object _eventHandlersLock = new();

    private ImmutableArray<EventHandlerRegistration> _eventHandlers = ImmutableArray<EventHandlerRegistration>.Empty;

    /// <summary>
    /// Constructor.
    /// </summary>
    internal Event()
    {
    }

    /// <summary>
    /// Registers a synchronous event handler and returns its registration.
    /// </summary>
    /// <param name="eventHandler">The event handler</param>
    /// <returns>The event handler registration. Dispose this instance to remove the registration.</returns>
    /// <seealso cref="RegisterEventHandler(Func{TEventArgs,Task})"/>
    [MustUseReturnValue]
    public IEventHandlerRegistration RegisterEventHandler(Action<TEventArgs> eventHandler)
    {
        var registration = new EventHandlerRegistration(
            this,
            eventArgs =>
            {
                eventHandler(eventArgs);
                return Task.CompletedTask;
            }
        );

        lock (this._eventHandlersLock)
        {
            this._eventHandlers = this._eventHandlers.Add(registration);
        }

        return registration;
    }

    /// <summary>
    /// Registers an <c>async</c> event handler and returns its registration.
    /// </summary>
    /// <param name="eventHandler">The event handler</param>
    /// <returns>The event handler registration. Dispose this instance to remove the registration.</returns>
    /// <seealso cref="RegisterEventHandler(Action{TEventArgs})"/>
    [MustUseReturnValue]
    public IEventHandlerRegistration RegisterEventHandler(Func<TEventArgs, Task> eventHandler)
    {
        var registration = new EventHandlerRegistration(this, eventHandler);

        lock (this._eventHandlersLock)
        {
            this._eventHandlers = this._eventHandlers.Add(registration);
        }

        return registration;
    }

    private void RemoveRegistration(EventHandlerRegistration registration)
    {
        lock (this._eventHandlersLock)
        {
            this._eventHandlers = this._eventHandlers.Remove(registration);
        }
    }

    internal async Task RaiseEvent(TEventArgs eventArgs)
    {
        // ReSharper disable once InconsistentlySynchronizedField
        ImmutableArray<EventHandlerRegistration> eventHandlers = this._eventHandlers;
        if (eventHandlers.IsEmpty)
        {
            return;
        }

        foreach (var eventHandlerRegistration in eventHandlers)
        {
            await eventHandlerRegistration.EventHandler(eventArgs).ConfigureAwait(false);
        }
    }

    private sealed class EventHandlerRegistration : Disposable, IEventHandlerRegistration
    {
        public readonly Func<TEventArgs, Task> EventHandler;

        private readonly Event<TEventArgs> _containingEvent;

        public EventHandlerRegistration(Event<TEventArgs> containingEvent, Func<TEventArgs, Task> eventHandler)
        {
            this.EventHandler = eventHandler;
            this._containingEvent = containingEvent;
        }

        /// <inheritdoc />
        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();

            this._containingEvent.RemoveRegistration(this);
        }
    }
}
