#region License
// Copyright 2020 - 2020 AppWeave.Core (https://github.com/skrysmanski/AppWeave.Core)
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

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.Core.Threading
{
    /// <summary>
    /// A (very simple) list of tasks that - for convenience purposes - has a "+=" operator.
    ///
    /// <para>Note: This class is not thread-safe.</para>
    /// </summary>
    /// <seealso cref="TaskList{T}"/>
    public class TaskList : IReadOnlyList<Task>
    {
        [NotNull, ItemNotNull]
        private readonly List<Task> m_underlyingList = new List<Task>();

        /// <inheritdoc />
        public int Count => this.m_underlyingList.Count;

        /// <inheritdoc />
        [NotNull]
        public Task this[int index] => this.m_underlyingList[index];

        /// <summary>
        /// Adds a task to this list.
        /// </summary>
        [NotNull]
        public static TaskList operator+([NotNull] TaskList taskList, [NotNull] Task task)
        {
            taskList.Add(task);
            return taskList;
        }

        /// <summary>
        /// Adds a task to this list.
        /// </summary>
        [PublicAPI]
        public void Add([NotNull] Task task)
        {
            // Tasks must not be null or else "Task.WhenAll()" will throw an exception
            Verify.ParamNotNull(task, nameof(task));

            this.m_underlyingList.Add(task);
        }

        /// <summary>
        /// Calls <see cref="Task.WhenAll(IEnumerable{Task})"/> for this list.
        /// </summary>
        [PublicAPI]
        public Task WhenAll()
        {
            return Task.WhenAll(this.m_underlyingList);
        }

        /// <summary>
        /// Calls <see cref="Task.WhenAny(IEnumerable{Task})"/> for this list.
        /// </summary>
        [PublicAPI]
        public Task WhenAny()
        {
            return Task.WhenAny(this.m_underlyingList);
        }

        /// <inheritdoc />
        public IEnumerator<Task> GetEnumerator()
        {
            return this.m_underlyingList.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    /// <summary>
    /// A (very simple) list of tasks that - for convenience purposes - has a "+=" operator.
    ///
    /// <para>Note: This class is not thread-safe.</para>
    /// </summary>
    /// <seealso cref="TaskList"/>
    public class TaskList<T> : IReadOnlyList<Task<T>>
    {
        [NotNull, ItemNotNull]
        private readonly List<Task<T>> m_underlyingList = new List<Task<T>>();

        /// <inheritdoc />
        public int Count => this.m_underlyingList.Count;

        /// <inheritdoc />
        [NotNull]
        public Task<T> this[int index] => this.m_underlyingList[index];

        /// <summary>
        /// Adds a task to this list.
        /// </summary>
        [NotNull]
        public static TaskList<T> operator+([NotNull] TaskList<T> taskList, [NotNull] Task<T> task)
        {
            taskList.Add(task);
            return taskList;
        }

        /// <summary>
        /// Adds a task to this list.
        /// </summary>
        [PublicAPI]
        public void Add([NotNull] Task<T> task)
        {
            // Tasks must not be null or else "Task.WhenAll()" will throw an exception
            Verify.ParamNotNull(task, nameof(task));

            this.m_underlyingList.Add(task);
        }

        /// <summary>
        /// Calls <see cref="Task.WhenAll(IEnumerable{Task})"/> for this list.
        /// </summary>
        [PublicAPI]
        public Task WhenAll()
        {
            return Task.WhenAll(this.m_underlyingList);
        }

        /// <summary>
        /// Calls <see cref="Task.WhenAny(IEnumerable{Task})"/> for this list.
        /// </summary>
        [PublicAPI]
        public Task WhenAny()
        {
            return Task.WhenAny(this.m_underlyingList);
        }

        /// <inheritdoc />
        public IEnumerator<Task<T>> GetEnumerator()
        {
            return this.m_underlyingList.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}
