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
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace AppMotor.Core.Events
{
    /// <summary>
    /// An event that will only happen one time (and then never again). Primary examples of such an event
    /// are lifetime event (e.g. application started, application stopped).
    /// </summary>
    public class OneTimeEvent<TEventArgs>
    {
        private readonly object _eventRaiseLock = new();

        private bool _hasBeenRaised;

        private readonly Event<TEventArgs> _event;

        private readonly EventRaiser<TEventArgs> _eventRaiser;

        /// <summary>
        /// Whether this event has been raised or not.
        /// </summary>
        // ReSharper disable once InconsistentlySynchronizedField
        public bool HasBeenRaised => this._hasBeenRaised;

        /// <summary>
        /// Constructor.
        /// </summary>
        [PublicAPI]
        protected OneTimeEvent()
        {
            (this._event, this._eventRaiser) = Event<TEventArgs>.Create();
        }

        /// <summary>
        /// Creates an event and its event raiser.
        /// </summary>
        [MustUseReturnValue]
        [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "By design")]
        public static (OneTimeEvent<TEventArgs>, EventRaiser<TEventArgs>) Create()
        {
            var oneTimeEvent = new OneTimeEvent<TEventArgs>();

            return (oneTimeEvent, new EventRaiser<TEventArgs>(oneTimeEvent.RaiseEvent));
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
                if (this._hasBeenRaised)
                {
                    // Event has been raised
                    return null;
                }
                else
                {
                    return this._event.RegisterEventHandler(eventHandler);
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
                if (this._hasBeenRaised)
                {
                    // Event has been raised
                    return null;
                }
                else
                {
                    return this._event.RegisterEventHandler(eventHandler);
                }
            }
        }

        private async Task RaiseEvent(TEventArgs eventArgs)
        {
            lock (this._eventRaiseLock)
            {
                if (this._hasBeenRaised)
                {
                    return;
                }

                this._hasBeenRaised = true;
            }

            await this._eventRaiser.RaiseEventAsync(eventArgs).ConfigureAwait(false);
        }
    }
}
