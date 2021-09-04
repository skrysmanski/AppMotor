﻿#region License
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
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Shouldly;

namespace AppMotor.TestCore.Shouldly
{
    /// <summary>
    /// <see cref="Should"/>ly extensions for tasks that should finish within a certain amount of time.
    /// </summary>
    [ShouldlyMethods]
    public static class TestTimeoutShouldlyExtension
    {
        //
        // NOTE: Shouldly assertion methods can't be "async" because in this case Shouldly's
        //   expression code extractor won't be able to find the expression.
        //

        /// <summary>
        /// Should finish within the specified amount of time.
        /// </summary>
        [PublicAPI]
        public static async Task ShouldFinishWithin(this Task task, TimeSpan timeout)
        {
            await Task.WhenAny(task, Task.Delay(timeout)).ConfigureAwait(false);

            if (!task.IsCompleted)
            {
                var message = new CorrectedCompleteInShouldlyMessage(timeout, null).ToString();
                throw new ShouldAssertException(message);
            }
        }

        /// <summary>
        /// Should finish within the specified amount of time.
        /// </summary>
        [PublicAPI]
        public static async Task<T> ShouldFinishWithin<T>(this Task<T> task, TimeSpan timeout)
        {
            await Task.WhenAny(task, Task.Delay(timeout)).ConfigureAwait(false);

            if (!task.IsCompleted)
            {
                var message = new CorrectedCompleteInShouldlyMessage(timeout, null).ToString();
                throw new ShouldAssertException(message);
            }

            return await task.ConfigureAwait(false);
        }

        private class CorrectedCompleteInShouldlyMessage : CompleteInShouldlyMessage
        {
            public CorrectedCompleteInShouldlyMessage(TimeSpan timeout, string? customMessage, [CallerMemberName] string shouldlyMethod = null!)
                : base(what: "", timeout, customMessage, shouldlyMethod)
            {
                // The "Expected" property (which is filled with the value of the base constructor's
                // "what" parameter) is used by the string formatter as first line. Unfortunately,
                // the string formatter does not use the "CodePart" at all. So have to "convince"
                // it to use it.
                this.ShouldlyAssertionContext.Expected = this.ShouldlyAssertionContext.CodePart;
            }
        }
    }
}
