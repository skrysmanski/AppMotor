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
        public event EventHandler? Started;

        /// <summary>
        /// Triggered when the <see cref="GenericHostCliCommand"/> is starting a graceful shutdown.
        /// Shutdown will block until all callbacks registered on this token have completed.
        /// </summary>
        public event EventHandler? Stopping;

        /// <summary>
        /// Triggered when the <see cref="GenericHostCliCommand"/> has completed a graceful shutdown.
        /// The application will not exit until all callbacks registered on this token have completed.
        /// </summary>
        public event EventHandler? Stopped;

        private readonly GenericHostCliCommand _command;

        internal GenericHostCliCommandLifetimeEvents(GenericHostCliCommand command)
        {
            this._command = command;
        }

        internal void TriggerStarted()
        {
            this.Started?.Invoke(this._command, EventArgs.Empty);
        }

        internal void TriggerStopping()
        {
            this.Stopping?.Invoke(this._command, EventArgs.Empty);
        }

        internal void TriggerStopped()
        {
            this.Stopped?.Invoke(this._command, EventArgs.Empty);
        }
    }
}
