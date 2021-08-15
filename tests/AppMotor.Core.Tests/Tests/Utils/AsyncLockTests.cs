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
using System.Threading;
using System.Threading.Tasks;

using AppMotor.Core.Utils;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Utils
{
    public sealed class AsyncLockTests
    {
        [Fact]
        public void Test_Acquire()
        {
            using var theLock = new AsyncLock();

            ManualResetEventSlim lock1Acquired = new();
            ManualResetEventSlim lock1Released = new();
            ManualResetEventSlim task2Started = new();

            var task1 = Task.Run(async () =>
            {
                // ReSharper disable once AccessToDisposedClosure
                using (await theLock.AcquireAsync())
                {
                    lock1Acquired.Set();
                    task2Started.Wait(10_000).ShouldBe(true);
                    await Task.Delay(200);
                    lock1Released.Set();
                }
            });

            lock1Acquired.Wait(10_000).ShouldBe(true);

            var task2 = Task.Run(async () =>
            {
                await Task.Delay(1);

                task2Started.Set();
                lock1Released.IsSet.ShouldBe(false);

                // ReSharper disable once AccessToDisposedClosure
                using (await theLock.AcquireAsync())
                {
                    lock1Released.IsSet.ShouldBe(true);
                }
            });

            Task.WaitAll(new[] { task1, task2 }, 10_000).ShouldBe(true);
        }

        [Fact]
        [SuppressMessage("ReSharper", "MethodSupportsCancellation")]
        public void Test_Acquire_CancellationToken()
        {
            using var theLock = new AsyncLock();
            using var cts = new CancellationTokenSource();

            ManualResetEventSlim lock1Acquired = new();
            ManualResetEventSlim task2Started = new();

            var task1 = Task.Run(async () =>
            {
                // ReSharper disable once AccessToDisposedClosure
                using (await theLock.AcquireAsync())
                {
                    lock1Acquired.Set();
                    task2Started.Wait(10_000).ShouldBe(true);
                    await Task.Delay(400);
                }
            });

            lock1Acquired.Wait(10_000).ShouldBe(true);

            task2Started.Set();

            cts.CancelAfter(TimeSpan.FromMilliseconds(50));

            // ReSharper disable once MustUseReturnValue
            Should.Throw<OperationCanceledException>(async () => await theLock.AcquireAsync(cts.Token));

            task1.Wait(10_000).ShouldBe(true);
        }

        [Fact]
        public async Task Test_Acquire_WithTimeout()
        {
            using var theLock = new AsyncLock();

            ManualResetEventSlim lock1Acquired = new();
            ManualResetEventSlim lock1Released = new();
            ManualResetEventSlim task2Started = new();

            var task1 = Task.Run(async () =>
            {
                // ReSharper disable once AccessToDisposedClosure
                using (await theLock.AcquireAsync())
                {
                    lock1Acquired.Set();
                    task2Started.Wait(10_000).ShouldBe(true);
                    await Task.Delay(400);
                }

                lock1Released.Set();
            });

            lock1Acquired.Wait(10_000).ShouldBe(true);

            task2Started.Set();

            // ReSharper disable once MustUseReturnValue
            Should.Throw<TimeoutException>(async () => await theLock.AcquireAsync(TimeSpan.FromMilliseconds(50)));

            lock1Released.Wait(10_000).ShouldBe(true);

            using (await theLock.AcquireAsync(TimeSpan.FromMilliseconds(50)))
            {
            }

            task1.Wait(10_000).ShouldBe(true);
        }

        [Fact]
        [SuppressMessage("ReSharper", "MethodSupportsCancellation")]
        public void Test_Acquire_WithTimeout_CancellationToken()
        {
            using var theLock = new AsyncLock();
            using var cts = new CancellationTokenSource();

            ManualResetEventSlim lock1Acquired = new();
            ManualResetEventSlim task2Started = new();

            var task1 = Task.Run(async () =>
            {
                // ReSharper disable once AccessToDisposedClosure
                using (await theLock.AcquireAsync())
                {
                    lock1Acquired.Set();
                    task2Started.Wait(10_000).ShouldBe(true);
                    await Task.Delay(400);
                }
            });

            lock1Acquired.Wait(10_000).ShouldBe(true);

            task2Started.Set();

            cts.CancelAfter(TimeSpan.FromMilliseconds(50));

            // ReSharper disable once MustUseReturnValue
            Should.Throw<OperationCanceledException>(async () => await theLock.AcquireAsync(TimeSpan.FromMilliseconds(400), cts.Token));

            task1.Wait(10_000).ShouldBe(true);
        }
    }
}
