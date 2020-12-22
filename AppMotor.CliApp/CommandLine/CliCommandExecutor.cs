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

using JetBrains.Annotations;

namespace AppMotor.CliApp.CommandLine
{
    /// <summary>
    /// Represents the main/execute method of a <see cref="CliCommand"/> (via <see cref="CliCommand.Executor"/>).
    /// The main purpose of this class is to give the user the freedom to choose between a synchronous or <c>async</c>
    /// method and to choose between <c>void</c>, <c>bool</c>, and <c>int</c> as return type.
    /// </summary>
    public class CliCommandExecutor
    {
        private readonly Func<Task<int>> _action;

        /// <summary>
        /// Creates an executor for an synchronous method that returns nothing (<c>void</c>).
        ///
        /// <para>The return value is always 0.</para>
        /// </summary>
        [PublicAPI]
        public CliCommandExecutor(Action action)
        {
            this._action = () =>
            {
                action();
                return Task.FromResult(0);
            };
        }

        /// <summary>
        /// Creates an executor for an synchronous method that returns an <c>int</c>.
        ///
        /// <para>The return value is directly taken as exit code.</para>
        /// </summary>
        [PublicAPI]
        public CliCommandExecutor(Func<int> action)
        {
            this._action = () =>
            {
                int retVal = action();
                return Task.FromResult(retVal);
            };
        }

        /// <summary>
        /// Creates an executor for an synchronous method that returns a <c>bool</c>.
        ///
        /// <para>The return value of <c>true</c> is translated into the exit code 0; <c>false</c>
        /// is translated into 1.</para>
        /// </summary>
        [PublicAPI]
        public CliCommandExecutor(Func<bool> action)
        {
            this._action = () =>
            {
                bool retVal = action();
                return Task.FromResult(retVal ? 0 : 1);
            };
        }

        /// <summary>
        /// Creates an executor for an async method that returns nothing (<c>Task</c>/<c>void</c>).
        ///
        /// <para>The return value is always 0.</para>
        /// </summary>
        [PublicAPI]
        public CliCommandExecutor(Func<Task> action)
        {
            this._action = async () =>
            {
                await action().ConfigureAwait(continueOnCapturedContext: false);
                return 0;
            };
        }

        /// <summary>
        /// Creates an executor for an async method that returns an <c>int</c>.
        ///
        /// <para>The return value is directly taken as exit code.</para>
        /// </summary>
        [PublicAPI]
        public CliCommandExecutor(Func<Task<int>> action)
        {
            this._action = action;
        }

        /// <summary>
        /// Creates an executor for an async method that returns a <c>bool</c>.
        ///
        /// <para>The return value of <c>true</c> is translated into the exit code 0; <c>false</c>
        /// is translated into 1.</para>
        /// </summary>
        [PublicAPI]
        public CliCommandExecutor(Func<Task<bool>> action)
        {
            this._action = async () =>
            {
                bool retVal = await action().ConfigureAwait(continueOnCapturedContext: false);
                return retVal ? 0 : 1;
            };
        }

        /// <summary>
        /// Executes this executor and returns the exit code.
        /// </summary>
        public async Task<int> Execute()
        {
            return await this._action().ConfigureAwait(continueOnCapturedContext: false);
        }
    }
}
