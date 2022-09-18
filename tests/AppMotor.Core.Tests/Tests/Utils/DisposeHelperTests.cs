// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Utils;

using JetBrains.Annotations;

using Moq;

using Xunit;

namespace AppMotor.Core.Tests.Utils;

public sealed class DisposeHelperTests
{
    [Fact]
    public async Task Test_DisposeWithAsyncSupport_WithAsyncDisposable()
    {
        var disposableMock = new Mock<IMixedDisposable>(MockBehavior.Loose);
        IDisposable disposable = disposableMock.Object;

        await DisposeHelper.DisposeWithAsyncSupport(disposable);

        disposableMock.Verify(m => m.DisposeAsync(), Times.Once);
        disposableMock.Verify(m => m.Dispose(), Times.Never);
    }

    [Fact]
    public async Task Test_DisposeWithAsyncSupport_WithoutAsyncDisposable()
    {
        var disposableMock = new Mock<IDisposable>(MockBehavior.Loose);
        IDisposable disposable = disposableMock.Object;

        await DisposeHelper.DisposeWithAsyncSupport(disposable);

        disposableMock.Verify(m => m.Dispose(), Times.Once);
    }

    [UsedImplicitly(ImplicitUseKindFlags.Access)]
    public interface IMixedDisposable : IDisposable, IAsyncDisposable
    {

    }
}
