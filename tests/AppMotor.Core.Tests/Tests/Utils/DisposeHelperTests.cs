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