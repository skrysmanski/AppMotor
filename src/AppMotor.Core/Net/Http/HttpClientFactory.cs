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
using System.Net.Http;
using System.Security.Authentication;

using AppMotor.Core.Certificates;
using AppMotor.Core.Utils;

namespace AppMotor.Core.Net.Http;

/// <summary>
/// Creates instances of <see cref="HttpClient"/>. Use this class if you need something other than
/// the default settings.
/// </summary>
public static class HttpClientFactory
{
    /// <summary>
    /// Creates a new <see cref="HttpClient"/> instance.
    /// </summary>
    /// <param name="serverCertificate">The server's TLS certificate; if <c>null</c> (and HTTPS is used), it'll
    /// be downloaded from the server and checked against the system CAs. This parameter is useful when
    /// using self-signed certificates.</param>
    /// <param name="tlsProtocolsToUse">The TLS protocols to use; if <c>null</c>, <see cref="TlsSettings.EnabledTlsProtocols"/>
    /// will be used (which is the recommended way)</param>
    /// <returns></returns>
    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Bug")] // BUG: https://github.com/dotnet/roslyn-analyzers/issues/4636
    public static HttpClient CreateHttpClient(
            TlsCertificate? serverCertificate = null,
            SslProtocols? tlsProtocolsToUse = null
        )
    {
        var handler = new HttpClientHandler();
        try
        {
            // Enable strict TLS protocol version
            handler.SslProtocols = tlsProtocolsToUse ?? TlsSettings.EnabledTlsProtocols;

            if (serverCertificate is not null)
            {
                // Enable check for self-signed server certificate.
                var certHelper = new CustomServerCertificateValidationHelper(serverCertificate);
                handler.ServerCertificateCustomValidationCallback = certHelper.ValidationCallback;
            }

            // Required by CA5399
            handler.CheckCertificateRevocationList = true;

            return new HttpClient(handler);
        }
        catch (Exception)
        {
            handler.Dispose();
            throw;
        }
    }
}