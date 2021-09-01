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
using System.Threading;

using AppMotor.Core.Utils;
using AppMotor.TestCore.Shouldly;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Utils
{
    public sealed class DefaultDateTimeProviderTests
    {
        [Fact]
        public void TestNow()
        {
            DefaultDateTimeProvider.Instance.LocalNow.ShouldBe(DateTime.Now, tolerance: TimeSpan.FromMilliseconds(2));
            DefaultDateTimeProvider.Instance.UtcNow.ShouldBe(DateTimeUtc.Now, tolerance: TimeSpan.FromMilliseconds(2));

            Thread.Sleep(TimeSpan.FromMilliseconds(500));

            // Check that is has changed
            DefaultDateTimeProvider.Instance.LocalNow.ShouldBe(DateTime.Now, tolerance: TimeSpan.FromMilliseconds(2));
            DefaultDateTimeProvider.Instance.UtcNow.ShouldBe(DateTimeUtc.Now, tolerance: TimeSpan.FromMilliseconds(2));
        }
    }
}
