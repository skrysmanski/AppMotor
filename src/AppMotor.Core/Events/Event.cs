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

using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.Core.Events
{
    /// <summary>
    /// <para>This class represents a different approach the events and is meant as replacement for C#'s built-in <c>event</c>s. The differences to
    /// regular C# events are as follows:</para>
    ///
    /// <para>Registering an event handler happens through a method (see <see cref="RegisterEventHandler(Action{TEventArgs})"/> and
    /// <see cref="RegisterEventHandler(Func{TEventArgs,Task})"/>) - while unregistering the event handler happens through disposing the
    /// <see cref="IEventHandlerRegistration"/> instance returned by the methods mentioned before.</para>
    ///
    /// <para>This class natively supports async events (without the need to resorting to <c>async void</c>).</para>
    ///
    /// <para>This class breaks with the tradition of having a <c>sender</c> parameter. If you need a sender in your event, add it to the
    /// event args. If you don't need or want a sender, you don't need to specify one. (Experience showed that, if the sender was not immediately
    /// useful/required in the current scenario, having a sender parameter created additional cognitive load for the event owner because they
    /// then needed to decide whether to provide this parameter or not and what implications it would have when providing one. Thus, it's
    /// easier not to require such a parameter.)</para>
    /// </summary>
    public class Event<TEventArgs>
    {
        private readonly object _eventHandlersLock = new();

        private ImmutableArray<EventHandlerRegistration> _eventHandlers = ImmutableArray<EventHandlerRegistration>.Empty;

        /// <summary>
        /// Constructor.
        /// </summary>
        [PublicAPI]
        protected Event()
        {
        }

        /// <summary>
        /// Creates an event and its event raiser.
        /// </summary>
        [MustUseReturnValue]
        [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "By design")]
        public static (Event<TEventArgs>, EventRaiser<TEventArgs>) Create()
        {
            var @event = new Event<TEventArgs>();

            return (@event, @event.CreateEventRaiser());
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

        /// <summary>
        /// Creates the <see cref="EventRaiser{TEventArgs}"/> instance for this event.
        /// </summary>
        [PublicAPI, MustUseReturnValue]
        protected EventRaiser<TEventArgs> CreateEventRaiser()
        {
            return new(RaiseEvent);
        }

        private async Task RaiseEvent(TEventArgs eventArgs)
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
}
