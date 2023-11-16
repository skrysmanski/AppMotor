// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Events;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Events;

public sealed class EventSourceTests
{
    private readonly Guid _testGuid = Guid.NewGuid();

    private int _syncHandlerCallCount;

    private int _asyncHandlerCallCount;

    [Fact]
    public void Test_BasicUsage_WithoutEventArgs()
    {
        var eventSource = new EventSource();

        var syncRegistration = eventSource.Event.RegisterEventHandler(OnTestEvent);
        var asyncRegistration = eventSource.Event.RegisterEventHandler(OnTestEventAsync);

        eventSource.RaiseEvent();

        this._syncHandlerCallCount.ShouldBe(1);
        this._asyncHandlerCallCount.ShouldBe(1);

        eventSource.RaiseEvent();

        this._syncHandlerCallCount.ShouldBe(2);
        this._asyncHandlerCallCount.ShouldBe(2);

        syncRegistration.Dispose();

        eventSource.RaiseEvent();

        this._syncHandlerCallCount.ShouldBe(2);
        this._asyncHandlerCallCount.ShouldBe(3);

        asyncRegistration.Dispose();

        eventSource.RaiseEvent();

        this._syncHandlerCallCount.ShouldBe(2);
        this._asyncHandlerCallCount.ShouldBe(3);
    }

    [Fact]
    public void Test_BasicUsage_WithEventArgs()
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
    public async Task Test_BasicUsage_WithoutEventArgs_Async()
    {
        var eventSource = new EventSource();

        var syncRegistration = eventSource.Event.RegisterEventHandler(OnTestEvent);
        var asyncRegistration = eventSource.Event.RegisterEventHandler(OnTestEventAsync);

        await eventSource.RaiseEventAsync();

        this._syncHandlerCallCount.ShouldBe(1);
        this._asyncHandlerCallCount.ShouldBe(1);

        await eventSource.RaiseEventAsync();

        this._syncHandlerCallCount.ShouldBe(2);
        this._asyncHandlerCallCount.ShouldBe(2);

        syncRegistration.Dispose();

        await eventSource.RaiseEventAsync();

        this._syncHandlerCallCount.ShouldBe(2);
        this._asyncHandlerCallCount.ShouldBe(3);

        asyncRegistration.Dispose();

        await eventSource.RaiseEventAsync();

        this._syncHandlerCallCount.ShouldBe(2);
        this._asyncHandlerCallCount.ShouldBe(3);
    }

    [Fact]
    public async Task Test_BasicUsage_WithEventArgs_Async()
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

    private void OnTestEvent()
    {
        this._syncHandlerCallCount++;
    }

    private async Task OnTestEventAsync()
    {
        await Task.Delay(0);
        this._asyncHandlerCallCount++;
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
