// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

namespace AppMotor.Core.Events;

/// <summary>
/// A registration for <see cref="Event{TEventHandler}"/>. Dispose this registration
/// to remove/delete the registration.
/// </summary>
public interface IEventHandlerRegistration : IDisposable
{
}
