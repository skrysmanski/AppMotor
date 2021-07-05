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
using System.Threading.Tasks;

using AppMotor.Core.Events;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Events
{
    public sealed class EventSourceTests
    {
        private readonly Guid _testGuid = Guid.NewGuid();

        private int _syncHandlerCallCount;

        private int _asyncHandlerCallCount;

        [Fact]
        public void Test_BasicUsage()
        {
            var eventSource = new EventSource<TestEventArgs>();

            var syncRegistration = eventSource.Event.RegisterEventHandler(OnTestEvent);
            var asyncRegistration = eventSource.Event.RegisterEventHandler(OnTestEventAsync);

            eventSource.RaiseEvent(new TestEventArgs(this._testGuid));

            this._syncHandlerCallCount.ShouldBe(1);
            this._asyncHandlerCallCount.ShouldBe(1);

            eventSource.RaiseEvent(new TestEventArgs(this._testGuid));

            this._syncHandlerCallCount.ShouldBe(2);
            this._asyncHandlerCallCount.ShouldBe(2);

            syncRegistration.Dispose();

            eventSource.RaiseEvent(new TestEventArgs(this._testGuid));

            this._syncHandlerCallCount.ShouldBe(2);
            this._asyncHandlerCallCount.ShouldBe(3);

            asyncRegistration.Dispose();

            eventSource.RaiseEvent(new TestEventArgs(this._testGuid));

            this._syncHandlerCallCount.ShouldBe(2);
            this._asyncHandlerCallCount.ShouldBe(3);
        }

        [Fact]
        public async Task Test_BasicUsage_Async()
        {
            var eventSource = new EventSource<TestEventArgs>();

            var syncRegistration = eventSource.Event.RegisterEventHandler(OnTestEvent);
            var asyncRegistration = eventSource.Event.RegisterEventHandler(OnTestEventAsync);

            await eventSource.RaiseEventAsync(new TestEventArgs(this._testGuid));

            this._syncHandlerCallCount.ShouldBe(1);
            this._asyncHandlerCallCount.ShouldBe(1);

            await eventSource.RaiseEventAsync(new TestEventArgs(this._testGuid));

            this._syncHandlerCallCount.ShouldBe(2);
            this._asyncHandlerCallCount.ShouldBe(2);

            syncRegistration.Dispose();

            await eventSource.RaiseEventAsync(new TestEventArgs(this._testGuid));

            this._syncHandlerCallCount.ShouldBe(2);
            this._asyncHandlerCallCount.ShouldBe(3);

            asyncRegistration.Dispose();

            await eventSource.RaiseEventAsync(new TestEventArgs(this._testGuid));

            this._syncHandlerCallCount.ShouldBe(2);
            this._asyncHandlerCallCount.ShouldBe(3);
        }

        private void OnTestEvent(TestEventArgs eventArgs)
        {
            eventArgs.TestGuid.ShouldBe(this._testGuid);
            this._syncHandlerCallCount++;
        }

        private async Task OnTestEventAsync(TestEventArgs eventArgs)
        {
            await Task.Delay(0);
            eventArgs.TestGuid.ShouldBe(this._testGuid);
            this._asyncHandlerCallCount++;
        }

        private sealed class TestEventArgs
        {
            public Guid TestGuid { get; }

            public TestEventArgs(Guid testGuid)
            {
                this.TestGuid = testGuid;
            }
        }
    }
}
