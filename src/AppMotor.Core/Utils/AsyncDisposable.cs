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

using System;
using System.Threading.Tasks;

namespace AppMotor.Core.Utils;

/// <summary>
/// Implements <see cref="IAsyncDisposable"/> (and <see cref="IDisposable"/>) and an easy to use
/// way.
/// </summary>
public abstract class AsyncDisposable : Disposable, IAsyncDisposable
{
    // NOTE: For details about this implementation, see: https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (BeginDispose())
        {
            try
            {
                await DisposeAsyncCore().ConfigureAwait(continueOnCapturedContext: false);

                DisposeUnmanagedResources();

                GC.SuppressFinalize(this);
            }
            catch (Exception)
            {
                EndDispose(exception: true);
                throw;
            }

            EndDispose(exception: false);
        }
    }

    /// <summary>
    /// Disposes any managed and unmanaged resources. Note that this must dispose the
    /// same resources as <see cref="Disposable.DisposeManagedResources"/> and
    /// <see cref="Disposable.DisposeUnmanagedResources"/>.
    /// </summary>
    /// <remarks>
    /// In addition to this method, inheritors must also implements the respective methods
    /// from <see cref="Disposable"/>.
    /// </remarks>
    protected abstract ValueTask DisposeAsyncCore();
}