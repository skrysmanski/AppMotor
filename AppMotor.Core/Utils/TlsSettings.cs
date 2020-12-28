#region License
// Copyright 2020 AppMotor Framework (https://github.com/skrysmanski/AppMotor)
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

using System.Net;
using System.Net.Http;
using System.Security.Authentication;

using JetBrains.Annotations;

namespace AppMotor.Core.Utils
{
    /// <summary>
    /// Provides information and management methods regarding SSL/TLS settings and protocol versions.
    ///
    /// <para>Most important members are <see cref="EnabledTlsProtocols"/> and <see cref="ApplyToCurrentProcess"/>.</para>
    /// </summary>
    public static class TlsSettings
    {
        /// <summary>
        /// The default (secure) TLS protocol versions that should be used. The default value of this list
        /// only contains protocol versions that are considered secure (as of 2020).
        ///
        /// <para>You may use this list for <see cref="HttpClientHandler.SslProtocols"/>.</para>
        /// </summary>
        /// <remarks>
        /// This list can be changed via <see cref="EnableProtocol"/> and <see cref="DisableProtocol"/>. However,
        /// you should only do this if it's absolutely necessary.
        ///
        /// <para>Also note that Microsoft recommends using <see cref="SslProtocols.None"/> - in which case
        /// the operating system selects the protocol versions (although all documentation only talks about
        /// the .NET Framework - and not about .NET Core). However, as of 2020, even Windows 10 still
        /// selects TLS 1.0 and TLS 1.1 (which are now considered no longer secure). So we do NOT do this
        /// but specify the protocol versions ourselves.</para>
        /// </remarks>
        [PublicAPI]
        public static SslProtocols EnabledTlsProtocols { get; private set; }
#pragma warning disable CA5398 // Avoid hardcoded SslProtocols values
            = SslProtocols.Tls12 | SslProtocols.Tls13;
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
}
