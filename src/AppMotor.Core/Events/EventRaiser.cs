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

namespace AppMotor.Core.Events
{
    /// <summary>
    /// This class gives you the ability to raise a <see cref="Event{TEventArgs}"/>. You get an instance of this class
    /// during creation of the associated event (i.e. via <see cref="Event{TEventArgs}.Create"/>).
    /// </summary>
    /// <remarks>
    /// This class is separate from <see cref="Event{TEventArgs}"/> to give the event owner control over who can raise
    /// the event. This is similar to regular C# events where only the containing class can raise the event (but everyone
    /// can register to it).
    /// </remarks>
    public sealed class EventRaiser<TEventArgs>
    {
        private readonly Func<TEventArgs, Task> _eventRaiser;

        /// <summary>
        /// Constructor. Note that this constructor primarily exists for subclasses of <see cref="Event{TEventArgs}"/> so that
        /// they can override <see cref="Event{TEventArgs}.CreateEventRaiser"/>. Regular users of <see cref="Event{TEventArgs}"/>
        /// should not call this constructor but use <see cref="Event{TEventArgs}.Create"/> instead.
        /// </summary>
        /// <param name="eventRaiser">The method used to raise this event.</param>
        public EventRaiser(Func<TEventArgs, Task> eventRaiser)
        {
            this._eventRaiser = eventRaiser;
        }

        /// <summary>
        /// Raises/Fires the associated event by invoking all event handlers. Use this method if you're in a synchronous method.
        /// Note that <c>async</c> event handlers are invoked by this method, too.
        /// </summary>
        /// <param name="eventArgs">The event args</param>
        [SuppressMessage("Design", "CA1030:Use events where appropriate")]
        public void RaiseEvent(TEventArgs eventArgs)
        {
            Task.Run(() => this._eventRaiser(eventArgs)).Wait();
        }

        /// <summary>
        /// Raises/Fires the associated event by invoking all event handlers. Use this method if you're in an <c>async</c> method.
        /// </summary>
        /// <param name="eventArgs">The event args</param>
        [SuppressMessage("Design", "CA1030:Use events where appropriate")]
        public async Task RaiseEventAsync(TEventArgs eventArgs)
        {
            await this._eventRaiser(eventArgs).ConfigureAwait(false);
        }
    }
}
