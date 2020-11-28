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

namespace AppMotor.CliApp
{
    /// <summary>
    /// Represents the main/execute method of a <see cref="CliApplication"/> (via <see cref="CliApplication.MainExecutor"/>).
    /// The main purpose of this class is to give the user the freedom to choose between a synchronous or <c>async</c>
    /// method and to choose between <c>void</c>, <c>bool</c>, and <c>int</c> as return type.
    /// </summary>
    public class CliApplicationExecutor
    {
        private readonly Func<string[], Task<int>> m_action;

        /// <summary>
        /// Creates an executor for a method that: is synchronous, returns no exit code (<c>void</c>), and ignores the command line params.
        ///
        /// <para>The return value is always 0.</para>
        /// </summary>
        [PublicAPI]
        public CliApplicationExecutor(Action action)
        {
            this.m_action = _ =>
            {
                action();
                return Task.FromResult(0);
            };
        }

        /// <summary>
        /// Creates an executor for a method that: is synchronous, returns no exit code (<c>void</c>), and takes in the command line params.
        ///
        /// <para>The return value is always 0.</para>
        /// </summary>
        [PublicAPI]
        public CliApplicationExecutor(Action<string[]> action)
        {
            this.m_action = args =>
            {
                action(args);
                return Task.FromResult(0);
            };
        }

        /// <summary>
        /// Creates an executor for a method that: is synchronous, returns an exit code, and ignores in the command line params.
        ///
        /// <para>The return value is directly taken as exit code.</para>
        /// </summary>
        [PublicAPI]
        public CliApplicationExecutor(Func<int> action)
        {
            this.m_action = _ =>
            {
                int retVal = action();
                return Task.FromResult(retVal);
            };
        }

        /// <summary>
        /// Creates an executor for a method that: is synchronous, returns an exit code, and takes in the command line params.
        ///
        /// <para>The return value is directly taken as exit code.</para>
        /// </summary>
        [PublicAPI]
        public CliApplicationExecutor(Func<string[], int> action)
        {
            this.m_action = args =>
            {
                int retVal = action(args);
                return Task.FromResult(retVal);
            };
        }

        /// <summary>
        /// Creates an executor for a method that: is synchronous, returns a success <c>bool</c>, and ignores in the command line params.
        ///
        /// <para>The return value of <c>true</c> is translated into the exit code 0; <c>false</c>
        /// is translated into 1.</para>
        /// </summary>
        [PublicAPI]
        public CliApplicationExecutor(Func<bool> action)
        {
            this.m_action = _ =>
            {
                bool retVal = action();
                return Task.FromResult(retVal ? 0 : 1);
            };
        }

        /// <summary>
        /// Creates an executor for a method that: is synchronous, returns a success <c>bool</c>, and takes in the command line params.
        ///
        /// <para>The return value of <c>true</c> is translated into the exit code 0; <c>false</c>
        /// is translated into 1.</para>
        /// </summary>
        [PublicAPI]
        public CliApplicationExecutor(Func<string[], bool> action)
        {
            this.m_action = args =>
            {
                bool retVal = action(args);
                return Task.FromResult(retVal ? 0 : 1);
            };
        }

        /// <summary>
        /// Creates an executor for a method that: is asynchronous, returns no exit code (<c>Task</c>/<c>void</c>), and ignores in the command line params.
        ///
        /// <para>The return value is always 0.</para>
        /// </summary>
        [PublicAPI]
        public CliApplicationExecutor(Func<Task> action)
        {
            this.m_action = async _ =>
            {
                await action();
                return 0;
            };
        }

        /// <summary>
        /// Creates an executor for a method that: is asynchronous, returns no exit code (<c>Task</c>/<c>void</c>), and takes in the command line params.
        ///
        /// <para>The return value is always 0.</para>
        /// </summary>
        [PublicAPI]
        public CliApplicationExecutor(Func<string[], Task> action)
        {
            this.m_action = async args =>
            {
                await action(args);
                return 0;
            };
        }

        /// <summary>
        /// Creates an executor for a method that: is asynchronous, returns an exit code, and ignores in the command line params.
        ///
        /// <para>The return value is directly taken as exit code.</para>
        /// </summary>
        [PublicAPI]
        public CliApplicationExecutor(Func<Task<int>> action)
        {
            this.m_action = _ => action();
        }

        /// <summary>
        /// Creates an executor for a method that: is asynchronous, returns an exit code, and takes in the command line params.
        ///
        /// <para>The return value is directly taken as exit code.</para>
        /// </summary>
        [PublicAPI]
        public CliApplicationExecutor(Func<string[], Task<int>> action)
        {
            this.m_action = action;
        }

        /// <summary>
        /// Creates an executor for a method that: is asynchronous, returns a success <c>bool</c>, and takes in the command line params.
        ///
        /// <para>The return value of <c>true</c> is translated into the exit code 0; <c>false</c>
        /// is translated into 1.</para>
        /// </summary>
        [PublicAPI]
        public CliApplicationExecutor(Func<Task<bool>> action)
        {
            this.m_action = async _ =>
            {
                bool retVal = await action();
                return retVal ? 0 : 1;
            };
        }

        /// <summary>
        /// Creates an executor for a method that: is asynchronous, returns a success <c>bool</c>, and takes in the command line params.
        ///
        /// <para>The return value of <c>true</c> is translated into the exit code 0; <c>false</c>
        /// is translated into 1.</para>
        /// </summary>
        [PublicAPI]
        public CliApplicationExecutor(Func<string[], Task<bool>> action)
        {
            this.m_action = async args =>
            {
                bool retVal = await action(args);
                return retVal ? 0 : 1;
            };
        }

        /// <summary>
        /// Executes this executor and returns the exit code.
        /// </summary>
        public async Task<int> Execute(string[] args)
        {
            return await this.m_action(args);
        }
    }
}
