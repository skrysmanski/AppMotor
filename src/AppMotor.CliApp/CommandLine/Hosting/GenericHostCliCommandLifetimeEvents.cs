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

using AppMotor.Core.Events;

namespace AppMotor.CliApp.CommandLine.Hosting
{
    /// <summary>
    /// The lifetime events for a <see cref="GenericHostCliCommand"/>.
    /// </summary>
    public sealed class GenericHostCliCommandLifetimeEvents
    {
        /// <summary>
        /// Triggered when the <see cref="GenericHostCliCommand"/> has fully started.
        /// </summary>
        public OneTimeEvent Started => this.StartedEventSource.Event;

        internal readonly OneTimeEventSource StartedEventSource = new();

        /// <summary>
        /// Triggered when the <see cref="GenericHostCliCommand"/> is starting a graceful shutdown.
        /// Shutdown will block until all event handlers registered on this event have completed.
        /// </summary>
        public OneTimeEvent Stopping => this.StoppingEventSource.Event;

        internal readonly OneTimeEventSource StoppingEventSource = new();

        /// <summary>
        /// Triggered when the <see cref="GenericHostCliCommand"/> has completed a graceful shutdown.
        /// The application will not exit until all event handlers registered on this event have completed.
        /// </summary>
        public OneTimeEvent Stopped => this.StoppedEventSource.Event;

        internal readonly OneTimeEventSource StoppedEventSource = new();

        internal GenericHostCliCommandLifetimeEvents()
        {
        }
    }
}
