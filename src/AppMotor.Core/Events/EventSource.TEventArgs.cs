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

using System.Diagnostics.CodeAnalysis;

namespace AppMotor.Core.Events;

/// <summary>
/// <para>This class represents a different approach the events and is meant as replacement for C#'s built-in <c>event</c>s. The differences to
/// regular C# events are as follows:</para>
///
/// <para>Registering an event handler happens through a method (one of the <c>RegisterEventHandler()</c> methods of the <see cref="Event"/>
/// property) - while unregistering the event handler happens through disposing the <see cref="IEventHandlerRegistration"/> instance
/// returned by the methods mentioned before.</para>
///
/// <para>This class natively supports async events (without the need to resorting to <c>async void</c>).</para>
///
/// <para>This class breaks with the tradition of having a <c>sender</c> parameter. If you need a sender in your event, add it to the
/// event args. If you don't need or want a sender, you don't need to specify one. (Experience showed that, if the sender was not immediately
/// useful/required in the current scenario, having a sender parameter created additional cognitive load for the event owner because they
/// then needed to decide whether to provide this parameter or not and what implications it would have when providing one. Thus, it's
/// easier not to require such a parameter.)</para>
///
/// <para>This class represents the "event owner"-part of the event (i.e. the ability to raise the event) while the <see cref="Event"/>
/// property represents the public part. This means, you should instances of this class private while exposing <see cref="Event"/>
/// as a public property.</para>
/// </summary>
/// <typeparam name="TEventArgs">The type of the event args to pass to each event handler</typeparam>
public class EventSource<TEventArgs>
{
    /// <summary>
    /// The public API surface of this event.
    /// </summary>
    public Event<TEventArgs> Event { get; } = new();

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