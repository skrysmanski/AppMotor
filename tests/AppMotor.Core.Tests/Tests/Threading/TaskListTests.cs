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

using AppMotor.Core.TestUtils;
using AppMotor.Core.Threading;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Threading
{
    public sealed class TaskListTests
    {
        [Fact]
        public void TestEmptyList()
        {
            var taskList = new TaskList();

            taskList.Count.ShouldBe(0);
            taskList.ExecuteGenericEnumerator().Count.ShouldBe(0);
            taskList.ExecuteNonGenericEnumerator<Task>().Count.ShouldBe(0);

            Should.Throw<ArgumentOutOfRangeException>(() => taskList[0]);

            Should.NotThrow(() => taskList.WhenAll());
            Should.Throw<InvalidOperationException>(() => taskList.WhenAny());
        }

        [Fact]
        public void TestEmptyList_GenericTaskList()
        {
            var taskList = new TaskList<int>();

            taskList.Count.ShouldBe(0);
            taskList.ExecuteGenericEnumerator().Count.ShouldBe(0);
            taskList.ExecuteNonGenericEnumerator<Task<int>>().Count.ShouldBe(0);

            Should.Throw<ArgumentOutOfRangeException>(() => taskList[0]);

            Should.NotThrow(() => taskList.WhenAll());
            Should.Throw<InvalidOperationException>(() => taskList.WhenAny());
        }

        /// <summary>
        /// Tests that using the + operator on a task list doesn't modify the original task list.
        /// </summary>
        [Fact]
        public async Task TestAddOperator()
        {
            var taskList1 = new TaskList();

            var taskList2 = taskList1 + DoSomethingAsync();

            taskList1.Count.ShouldBe(0);
            taskList2.Count.ShouldBe(1);

            await taskList2.WhenAll(); // just cleanup
        }

        /// <summary>
        /// Tests that using the + operator on a task list doesn't modify the original task list.
        /// </summary>
        [Fact]
        public async Task TestAddOperator_GenericTaskList()
        {
            var taskList1 = new TaskList<int>();

            var taskList2 = taskList1 + DoSomethingAndReturnSomethingAsync();

            taskList1.Count.ShouldBe(0);
            taskList2.Count.ShouldBe(1);

            await taskList2.WhenAll();
        }

        [Fact]
        public async Task TestGetEnumerator()
        {
            // setup
            var taskList = new TaskList();

            taskList += DoSomethingAsync();
            taskList += DoSomethingAsync();

            taskList.Count.ShouldBe(2);

            // test
            taskList.ExecuteGenericEnumerator().Count.ShouldBe(2);
            taskList.ExecuteNonGenericEnumerator<Task>().Count.ShouldBe(2);

            // cleanup
            await taskList.WhenAll();
        }

        [Fact]
        public async Task TestGetEnumerator_GenericTaskList()
        {
            // setup
            var taskList = new TaskList<int>();

            taskList += DoSomethingAndReturnSomethingAsync();
            taskList += DoSomethingAndReturnSomethingAsync();

            taskList.Count.ShouldBe(2);

            // test
            taskList.ExecuteGenericEnumerator().Count.ShouldBe(2);
            taskList.ExecuteNonGenericEnumerator<Task>().Count.ShouldBe(2);

            // cleanup
            await taskList.WhenAll();
        }

        [Fact]
        public async Task TestIndexer()
        {
            // setup
            var taskList = new TaskList();

            var task1 = DoSomethingAsync();
            var task2 = DoSomethingAsync();

            taskList += task1;
            taskList += task2;

            taskList.Count.ShouldBe(2);

            // test
            taskList[0].ShouldBeSameAs(task1);
            taskList[1].ShouldBeSameAs(task2);
            Should.Throw<ArgumentOutOfRangeException>(() => taskList[2]);
            Should.Throw<ArgumentOutOfRangeException>(() => taskList[-1]);

            // cleanup
            await taskList.WhenAll();
        }

        [Fact]
        public async Task TestIndexer_GenericTaskList()
        {
            // setup
            var taskList = new TaskList<int>();

            var task1 = DoSomethingAndReturnSomethingAsync();
            var task2 = DoSomethingAndReturnSomethingAsync();

            taskList += task1;
            taskList += task2;

            taskList.Count.ShouldBe(2);

            // test
            taskList[0].ShouldBeSameAs(task1);
            taskList[1].ShouldBeSameAs(task2);
            Should.Throw<ArgumentOutOfRangeException>(() => taskList[2]);
            Should.Throw<ArgumentOutOfRangeException>(() => taskList[-1]);

            // cleanup
            await taskList.WhenAll();
        }

        [Fact]
        public async Task TestWhenAll()
        {
            // setup
            var taskList = new TaskList();

            var task1 = DoSomethingAsync();
            var task2 = DoSomethingAsync(milliseconds: 20);

            taskList += task1;
            taskList += task2;

            taskList.Count.ShouldBe(2);

            // test
            await taskList.WhenAll();

            task1.Status.ShouldBe(TaskStatus.RanToCompletion);
            task2.Status.ShouldBe(TaskStatus.RanToCompletion);
        }

        [Fact]
        public async Task TestWhenAll_GenericTaskList()
        {
            // setup
            var taskList = new TaskList<int>();

            var task1 = DoSomethingAndReturnSomethingAsync(result: 42);
            var task2 = DoSomethingAndReturnSomethingAsync(milliseconds: 20, result: 43);

            taskList += task1;
            taskList += task2;

            taskList.Count.ShouldBe(2);

            // test
            var result = await taskList.WhenAll();

            result[0].ShouldBe(42);
            result[1].ShouldBe(43);

            task1.Status.ShouldBe(TaskStatus.RanToCompletion);
            task2.Status.ShouldBe(TaskStatus.RanToCompletion);
        }

        [Fact]
        public async Task TestWhenAny()
        {
            // setup
            var taskList = new TaskList();

            var task1 = DoSomethingAsync(milliseconds: 5);
            var task2 = DoSomethingAsync(milliseconds: 100);

            taskList += task1;
            taskList += task2;

            taskList.Count.ShouldBe(2);

            // test
            var completedTask = await taskList.WhenAny();

            completedTask.ShouldBeSameAs(task1);
            task1.Status.ShouldBe(TaskStatus.RanToCompletion);

            // cleanup
            await taskList.WhenAll();
        }

        [Fact]
        public async Task TestWhenAny_GenericTaskList()
        {
            // setup
            var taskList = new TaskList<int>();

            var task1 = DoSomethingAndReturnSomethingAsync(milliseconds: 5);
            var task2 = DoSomethingAndReturnSomethingAsync(milliseconds: 500);

            taskList += task1;
            taskList += task2;

            taskList.Count.ShouldBe(2);

            // test
            var completedTask = await taskList.WhenAny();

            completedTask.ShouldBeSameAs(task1);
            task1.Status.ShouldBe(TaskStatus.RanToCompletion);

            // cleanup
            await taskList.WhenAll();
        }

        private static async Task DoSomethingAsync(int milliseconds = 10)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(milliseconds));
        }

        private static async Task<int> DoSomethingAndReturnSomethingAsync(int milliseconds = 10, int result = 42)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(milliseconds));

            return result;
        }
    }
}
