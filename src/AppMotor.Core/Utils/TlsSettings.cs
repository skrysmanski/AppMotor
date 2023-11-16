// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Net;
using System.Security.Authentication;

using AppMotor.Core.Net.Http;

using JetBrains.Annotations;

namespace AppMotor.Core.Utils;

/// <summary>
/// Provides information and management methods regarding SSL/TLS settings and protocol versions.
///
/// <para>Most important members are <see cref="EnabledTlsProtocols"/> and <see cref="ApplyToCurrentProcess"/>.</para>
/// </summary>
public static class TlsSettings
{
    /// <summary>
    /// The default (secure) TLS protocol versions that should be used for the current process. The default
    /// value of this list only contains protocol versions that are considered secure (as of 2021).
    ///
    /// <para>This list is automatically used by <see cref="HttpClientFactory"/>.</para>
    /// </summary>
    /// <remarks>
    /// This list can be changed via <see cref="EnableProtocol"/> and <see cref="DisableProtocol"/>. However,
    /// you should only do this if it's absolutely necessary.
    ///
    /// <para>Also note that Microsoft recommends using <see cref="SslProtocols.None"/> - in which case
    /// the operating system selects the protocol versions (although all documentation only talks about
    /// the .NET Framework - and not about .NET Core). However, as of 2020, even Windows 10 still
    /// selects TLS 1.0 and TLS 1.1 (which are no longer considered secure). So we do NOT do this
    /// but specify the protocol versions ourselves.</para>
    /// </remarks>
    [PublicAPI]
#pragma warning disable CA5398 // Avoid hardcoded SslProtocols values
    public static SslProtocols EnabledTlsProtocols { get; private set; } = SslProtocols.Tls12 | SslProtocols.Tls13;
#pragma warning restore CA5398 // Avoid hardcoded SslProtocols values

    /// <summary>
    /// Applies <see cref="EnabledTlsProtocols"/> to the current process (where possible). Currently, this
    /// configures <see cref="ServicePointManager.SecurityProtocol"/>.
    ///
    /// <para>Note: If you use the AppCore.CliApp package, this method is automatically called for you.</para>
    /// </summary>
    [PublicAPI]
    public static void ApplyToCurrentProcess()
    {
        // NOTE: The enum "SecurityProtocolType" uses exactly the same values as "SslProtocols".
        //   See: https://github.com/dotnet/runtime/blob/master/src/libraries/System.Net.ServicePoint/src/System/Net/SecurityProtocolType.cs
        ServicePointManager.SecurityProtocol = (SecurityProtocolType)EnabledTlsProtocols;
    }

    /// <summary>
    /// Adds the specified protocol to <see cref="EnabledTlsProtocols"/> and calls <see cref="ApplyToCurrentProcess"/>
    /// afterwards.
    ///
    /// <para>Note: You should only do this if it's absolutely necessary.</para>
    /// </summary>
    [PublicAPI]
    public static void EnableProtocol(SslProtocols protocol)
    {
        EnabledTlsProtocols |= protocol;

        ApplyToCurrentProcess();
    }

    /// <summary>
    /// Removes the specified protocol from <see cref="EnabledTlsProtocols"/> and calls <see cref="ApplyToCurrentProcess"/>
    /// afterwards.
    /// </summary>
    [PublicAPI]
    public static void DisableProtocol(SslProtocols protocol)
    {
        EnabledTlsProtocols &= ~protocol;

        ApplyToCurrentProcess();
    }
}
