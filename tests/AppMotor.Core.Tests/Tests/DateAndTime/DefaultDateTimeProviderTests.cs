// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.DateAndTime;
using AppMotor.TestCore;
using AppMotor.TestCore.Shouldly;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.DateAndTime;

public sealed class DefaultDateTimeProviderTests
{
    [Fact]
    public void TestNow()
    {
        DefaultDateTimeProvider.Instance.LocalNow.ShouldBe(DateTime.Now, tolerance: TimeSpan.FromMilliseconds(TestEnvInfo.RunsInCiPipeline ? 50 : 2));
        DefaultDateTimeProvider.Instance.UtcNow.ShouldBe(DateTimeUtc.Now, tolerance: TimeSpan.FromMilliseconds(TestEnvInfo.RunsInCiPipeline ? 50 : 2));

        Thread.Sleep(TimeSpan.FromMilliseconds(500));

        // Check that is has changed
        DefaultDateTimeProvider.Instance.LocalNow.ShouldBe(DateTime.Now, tolerance: TimeSpan.FromMilliseconds(TestEnvInfo.RunsInCiPipeline ? 50 : 2));
        DefaultDateTimeProvider.Instance.UtcNow.ShouldBe(DateTimeUtc.Now, tolerance: TimeSpan.FromMilliseconds(TestEnvInfo.RunsInCiPipeline ? 50 : 2));
    }
}
