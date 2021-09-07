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

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using AppMotor.TestCore;

using Xunit;
using Xunit.Abstractions;

namespace AppMotor.CliApp.HttpServer.Tests
{
    public sealed class IpBaseTests : TestBase
    {
        /// <inheritdoc />
        public IpBaseTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public async Task RunIpTest()
        {
            var startedEvent = new ManualResetEventSlim();
            Task serverTask = RunServer(startedEvent);

            startedEvent.Wait();

            RunClient("abc");

            await serverTask;
        }

        private void RunClient(string message)
        {
            // Create a TcpClient.
            // Note, for this client to work you need to have a TcpServer
            // connected to the same address as specified by the server, port
            // combination.
            const int PORT = 13000;

            TcpClient client = new("::1", PORT);

            // Translate the passed message into ASCII and store it as a Byte array.
            byte[] data = Encoding.ASCII.GetBytes(message);

            // Get a client stream for reading and writing.
            //  Stream stream = client.GetStream();

            NetworkStream stream = client.GetStream();

            // Send the message to the connected TcpServer.
            stream.Write(data, 0, data.Length);

            this.TestConsole.WriteLine("Sent: {0}", message);

            // Receive the TcpServer.response.

            // Buffer to store the response bytes.
            data = new byte[256];

            // String to store the response ASCII representation.

            // Read the first batch of the TcpServer response bytes.
            int bytes = stream.Read(data, 0, data.Length);
            string responseData = Encoding.ASCII.GetString(data, 0, bytes);
            this.TestConsole.WriteLine("Received: {0}", responseData);

            // Close everything.
            stream.Close();
            client.Close();
        }

        private async Task RunServer(ManualResetEventSlim startedEvent)
        {
            // Set the TcpListener on port 13000.
            const int PORT = 13000;

//var x = new IPEndPoint(IPAddress.IPv6Any, 15000);

            var server = new TcpListener(IPAddress.IPv6Any, port: PORT);

// Start listening for client requests.
            server.Start();

// Buffer for reading data
            byte[] bytes = new byte[256];

            this.TestConsole.WriteLine("Waiting for a connection... ");

            // Perform a blocking call to accept requests.
            // You could also use server.AcceptSocket() here.
            Task<TcpClient> clientTask = server.AcceptTcpClientAsync();

            startedEvent.Set();

            var client = await clientTask;

            this.TestConsole.WriteLine("Connected!");

            // Get a stream object for reading and writing
            NetworkStream stream = client.GetStream();

            int i;

            // Loop to receive all the data sent by the client.
            while ((i = await stream.ReadAsync(bytes, 0, bytes.Length)) != 0)
            {
                // Translate data bytes to a ASCII string.
                var data = Encoding.ASCII.GetString(bytes, 0, i);
                this.TestConsole.WriteLine("Received: {0}", data);

                // Process the data sent by the client.
                data = data.ToUpper();

                byte[] msg = Encoding.ASCII.GetBytes(data);

                // Send back a response.
                await stream.WriteAsync(msg, 0, msg.Length);
                this.TestConsole.WriteLine("Sent: {0}", data);
            }

            // Shutdown and end connection
            client.Close();
        }
    }
}
