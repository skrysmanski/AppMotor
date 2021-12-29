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