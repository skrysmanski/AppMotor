// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Events;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Events;

public sealed class OneTimeEventSourceTests
{
    private readonly Guid _testGuid = Guid.NewGuid();

    private int _syncHandlerCallCount;

    private int _asyncHandlerCallCount;

    [Fact]
    public void Test_BasicUsage_WithoutEventArgs()
    {
        var eventSource = new OneTimeEventSource();

        eventSource.Event.HasBeenRaised.ShouldBe(false);

        var syncRegistration = eventSource.Event.RegisterEventHandler(OnTestEvent);
        syncRegistration.ShouldNotBeNull();
        var asyncRegistration = eventSource.Event.RegisterEventHandlerAsync(OnTestEventAsync).Result;
        asyncRegistration.ShouldNotBeNull();

        eventSource.RaiseEvent();

        this._syncHandlerCallCount.ShouldBe(1);
        this._asyncHandlerCallCount.ShouldBe(1);
        eventSource.Event.HasBeenRaised.ShouldBe(true);

        eventSource.RaiseEvent();

        this._syncHandlerCallCount.ShouldBe(1);
        this._asyncHandlerCallCount.ShouldBe(1);
        eventSource.Event.HasBeenRaised.ShouldBe(true);

        eventSource.Event.RegisterEventHandler(OnTestEvent).ShouldBeNull();
        eventSource.Event.RegisterEventHandlerAsync(OnTestEventAsync).Result.ShouldBeNull();
    }

    [Fact]
    public void Test_BasicUsage_WithEventArgs()
    {
        var eventSource = new OneTimeEventSource<TestEventArgs>();

        eventSource.Event.HasBeenRaised.ShouldBe(false);

        var syncRegistration = eventSource.Event.RegisterEventHandler(OnTestEvent);
        syncRegistration.ShouldNotBeNull();
        var asyncRegistration = eventSource.Event.RegisterEventHandler(OnTestEventAsync);
        asyncRegistration.ShouldNotBeNull();

        eventSource.RaiseEvent(new TestEventArgs(this._testGuid));

        this._syncHandlerCallCount.ShouldBe(1);
        this._asyncHandlerCallCount.ShouldBe(1);
        eventSource.Event.HasBeenRaised.ShouldBe(true);

        eventSource.RaiseEvent(new TestEventArgs(this._testGuid));

        this._syncHandlerCallCount.ShouldBe(1);
        this._asyncHandlerCallCount.ShouldBe(1);
        eventSource.Event.HasBeenRaised.ShouldBe(true);

        eventSource.Event.RegisterEventHandler(OnTestEvent).ShouldBeNull();
        eventSource.Event.RegisterEventHandler(OnTestEventAsync).ShouldBeNull();
    }

    [Fact]
    public async Task Test_BasicUsage_WithoutEventArgs_Async()
    {
        var eventSource = new OneTimeEventSource();

        eventSource.Event.HasBeenRaised.ShouldBe(false);

        var syncRegistration = eventSource.Event.RegisterEventHandler(OnTestEvent);
        syncRegistration.ShouldNotBeNull();
        var asyncRegistration = await eventSource.Event.RegisterEventHandlerAsync(OnTestEventAsync);
        asyncRegistration.ShouldNotBeNull();

        await eventSource.RaiseEventAsync();

        this._syncHandlerCallCount.ShouldBe(1);
        this._asyncHandlerCallCount.ShouldBe(1);
        eventSource.Event.HasBeenRaised.ShouldBe(true);

        await eventSource.RaiseEventAsync();

        this._syncHandlerCallCount.ShouldBe(1);
        this._asyncHandlerCallCount.ShouldBe(1);
        eventSource.Event.HasBeenRaised.ShouldBe(true);

        eventSource.Event.RegisterEventHandler(OnTestEvent).ShouldBeNull();
        (await eventSource.Event.RegisterEventHandlerAsync(OnTestEventAsync)).ShouldBeNull();
    }

    [Fact]
    public async Task Test_BasicUsage_WithEventArgs_Async()
    {
        var eventSource = new OneTimeEventSource<TestEventArgs>();

        eventSource.Event.HasBeenRaised.ShouldBe(false);

        var syncRegistration = eventSource.Event.RegisterEventHandler(OnTestEvent);
        syncRegistration.ShouldNotBeNull();
        var asyncRegistration = eventSource.Event.RegisterEventHandler(OnTestEventAsync);
        asyncRegistration.ShouldNotBeNull();

        await eventSource.RaiseEventAsync(new TestEventArgs(this._testGuid));

        this._syncHandlerCallCount.ShouldBe(1);
        this._asyncHandlerCallCount.ShouldBe(1);
        eventSource.Event.HasBeenRaised.ShouldBe(true);

        await eventSource.RaiseEventAsync(new TestEventArgs(this._testGuid));

        this._syncHandlerCallCount.ShouldBe(1);
        this._asyncHandlerCallCount.ShouldBe(1);
        eventSource.Event.HasBeenRaised.ShouldBe(true);

        eventSource.Event.RegisterEventHandler(OnTestEvent).ShouldBeNull();
        eventSource.Event.RegisterEventHandler(OnTestEventAsync).ShouldBeNull();
    }

    private void OnTestEvent()
    {
        this._syncHandlerCallCount++;
    }

    private void OnTestEvent(TestEventArgs eventArgs)
    {
        eventArgs.TestGuid.ShouldBe(this._testGuid);
        this._syncHandlerCallCount++;
    }

    private async Task OnTestEventAsync()
    {
        await Task.Delay(0);
        this._asyncHandlerCallCount++;
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
