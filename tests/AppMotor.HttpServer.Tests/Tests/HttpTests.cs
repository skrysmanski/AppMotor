﻿#region License
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
using System.Threading;
using System.Threading.Tasks;

using AppMotor.CliApp.HttpServer.TestUtils;
using AppMotor.Core.Net;
using AppMotor.Core.Net.Http;
using AppMotor.Core.TestUtils;
using AppMotor.HttpServer;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.HttpServer.Tests
{
    public sealed class HttpTests
    {
        [Fact]
        public async Task TestHttpApiCall()
        {
            int testPort = ServerPortProvider.GetTestPort();

            using var cts = new CancellationTokenSource();

            var app = new HttpServerApplication(
                new HttpServerPort(SocketListenAddresses.Loopback, testPort),
                new SimplePingStartup()
            );
            Task appTask = app.RunAsync(cts.Token);

            using (var httpClient = HttpClientFactory.CreateHttpClient())
            {
                // ReSharper disable once MethodSupportsCancellation
                var response = await httpClient.GetAsync($"http://localhost:{testPort}/api/ping");

                response.EnsureSuccessStatusCode();

                // ReSharper disable once MethodSupportsCancellation
                var responseString = await response.Content.ReadAsStringAsync();

                responseString.ShouldBe("Hello World!");
            }

            cts.Cancel();

            appTask.ShouldFinishWithin(TimeSpan.FromSeconds(10));
        }
    }
}
