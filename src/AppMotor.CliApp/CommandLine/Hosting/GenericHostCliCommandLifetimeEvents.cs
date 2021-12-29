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
using AppMotor.Core.Utils;

namespace AppMotor.CliApp.CommandLine.Hosting;

/// <summary>
/// Implementation of <see cref="IGenericHostCliCommandLifetimeEvents"/>.
/// </summary>
internal sealed class GenericHostCliCommandLifetimeEvents : Disposable, IGenericHostCliCommandLifetimeEvents
{
    public OneTimeEvent Started => this.StartedEventSource.Event;

    internal readonly OneTimeEventSource StartedEventSource = new();

    public OneTimeEvent Stopping => this._stoppingEventSource.Event;

    private readonly OneTimeEventSource _stoppingEventSource = new();

    public OneTimeEvent Stopped => this.StoppedEventSource.Event;

    internal readonly OneTimeEventSource StoppedEventSource = new();

    /// <summary>
    /// This token is canceled when the <see cref="Stopping"/> event is triggered.
    /// </summary>
    public CancellationToken CancellationToken => this._cts.Token;

    private readonly CancellationTokenSource _cts = new();

    /// <inheritdoc />
    protected override void DisposeManagedResources()
    {
        base.DisposeManagedResources();

        this._cts.Dispose();
    }

    internal void RaiseStoppingEvent()
    {
        this._cts.Cancel();
        this._stoppingEventSource.RaiseEvent();
    }
}