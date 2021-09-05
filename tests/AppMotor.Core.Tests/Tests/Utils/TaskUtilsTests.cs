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
using System.Threading;
using System.Threading.Tasks;

using AppMotor.Core.Utils;
using AppMotor.TestCore.Extensions;

using Xunit;

namespace AppMotor.Core.Tests.Utils
{
    public sealed class TaskUtilsTests
    {
        [Fact]
        public async Task Test_DelaySafe()
        {
            await TaskUtils.DelaySafe(TimeSpan.FromSeconds(0.2), CancellationToken.None).OrTimeoutAfter(TimeSpan.FromSeconds(0.5));

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(0.2));
            await TaskUtils.DelaySafe(TimeSpan.FromSeconds(20), cts.Token).OrTimeoutAfter(TimeSpan.FromSeconds(0.5));
        }
    }
}
