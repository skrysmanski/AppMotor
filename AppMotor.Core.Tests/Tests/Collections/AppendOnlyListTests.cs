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

using System.Collections.Generic;
using System.Reflection;

using AppWeave.Core.Collections;

using JetBrains.Annotations;

using Shouldly;

using Xunit;

namespace AppWeave.Core.Tests.Collections
{
    public sealed class AppendOnlyListTests
    {
        [Fact]
        public void TestAppend()
        {
            var list1 = new AppendOnlyList<string>();
            var originalUnderlyingList = GetUnderlyingList(list1);

            list1.Append("value1");
            list1.Count.ShouldBe(1);
            // This "Append()" call should not have create a new underlying list instance.
            GetUnderlyingList(list1).ShouldBeSameAs(originalUnderlyingList);

            var list2 = list1.CloneShallow();
            // CloneShallow() should not create new copies of the underlying list instance.
            GetUnderlyingList(list1).ShouldBeSameAs(originalUnderlyingList);
            GetUnderlyingList(list2).ShouldBeSameAs(originalUnderlyingList);

            list2.Count.ShouldBe(1);
            list2.Append("value2");
            list2.Count.ShouldBe(2);
            list1.Count.ShouldBe(1); // list1 should be unchanged
            // This "Append()" call should also not have create a new underlying list instance.
            // "list1" should still use the same underlying list as "list2" (just restrict itself
            // to a smaller range).
            GetUnderlyingList(list1).ShouldBeSameAs(originalUnderlyingList);
            GetUnderlyingList(list2).ShouldBeSameAs(originalUnderlyingList);

            var list3 = list1.CloneShallow();
            // CloneShallow() should not create new copies of the underlying list instance.
            GetUnderlyingList(list1).ShouldBeSameAs(originalUnderlyingList);
            GetUnderlyingList(list3).ShouldBeSameAs(originalUnderlyingList);

            list3.Append("value3");
            list2.Count.ShouldBe(2); // list2 should be unchanged
            list1.Count.ShouldBe(1); // list1 should be unchanged
            list3.Count.ShouldBe(2);
            // This "Append()" call should have created a new underlying list instance
            // for "list3" - but "list1" and "list2" should still use the original list
            // instance ("list2" because its "Append()" method was called first).
            GetUnderlyingList(list1).ShouldBeSameAs(originalUnderlyingList);
            GetUnderlyingList(list2).ShouldBeSameAs(originalUnderlyingList);
            GetUnderlyingList(list3).ShouldNotBeSameAs(originalUnderlyingList);

            list1[0].ShouldBe("value1");

            list2[0].ShouldBe("value1");
            list2[1].ShouldBe("value2");

            list3[0].ShouldBe("value1");
            list3[1].ShouldBe("value3");
        }

        [Fact]
        public void TestAppendRange()
        {
            var list1 = new AppendOnlyList<string>();
            var originalUnderlyingList = GetUnderlyingList(list1);

            list1.AppendRange(new [] { "value1a", "value1b" });
            list1.Count.ShouldBe(2);
            // This "Append()" call should not have create a new underlying list instance.
            GetUnderlyingList(list1).ShouldBeSameAs(originalUnderlyingList);

            var list2 = list1.CloneShallow();
            // CloneShallow() should not create new copies of the underlying list instance.
            GetUnderlyingList(list1).ShouldBeSameAs(originalUnderlyingList);
            GetUnderlyingList(list2).ShouldBeSameAs(originalUnderlyingList);

            list2.Count.ShouldBe(2);
            list2.AppendRange(new [] { "value2a", "value2b" });
            list2.Count.ShouldBe(4);
            list1.Count.ShouldBe(2); // list1 should be unchanged
            // This "Append()" call should also not have create a new underlying list instance.
            // "list1" should still use the same underlying list as "list2" (just restrict itself
            // to a smaller range).
            GetUnderlyingList(list1).ShouldBeSameAs(originalUnderlyingList);
            GetUnderlyingList(list2).ShouldBeSameAs(originalUnderlyingList);

            var list3 = list1.CloneShallow();
            // CloneShallow() should not create new copies of the underlying list instance.
            GetUnderlyingList(list1).ShouldBeSameAs(originalUnderlyingList);
            GetUnderlyingList(list3).ShouldBeSameAs(originalUnderlyingList);

            list3.AppendRange(list2);
            list2.Count.ShouldBe(4); // list2 should be unchanged
            list1.Count.ShouldBe(2); // list1 should be unchanged
            list3.Count.ShouldBe(6);
            // This "Append()" call should have created a new underlying list instance
            // for "list3" - but "list1" and "list2" should still use the original list
            // instance ("list2" because its "Append()" method was called first).
            GetUnderlyingList(list1).ShouldBeSameAs(originalUnderlyingList);
            GetUnderlyingList(list2).ShouldBeSameAs(originalUnderlyingList);
            GetUnderlyingList(list3).ShouldNotBeSameAs(originalUnderlyingList);

            list1[0].ShouldBe("value1a");
            list1[1].ShouldBe("value1b");

            list2[0].ShouldBe("value1a");
            list2[1].ShouldBe("value1b");
            list2[2].ShouldBe("value2a");
            list2[3].ShouldBe("value2b");

            list3[0].ShouldBe("value1a");
            list3[1].ShouldBe("value1b");
            list3[2].ShouldBe("value1a");
            list3[3].ShouldBe("value1b");
            list3[4].ShouldBe("value2a");
            list3[5].ShouldBe("value2b");
        }

        [Fact]
        public void TestAppendRange_SelfAppend()
        {
            var list1 = new AppendOnlyList<string>();
            var originalUnderlyingList = GetUnderlyingList(list1);

            list1.AppendRange(new [] { "value1a", "value1b" });
            list1.Count.ShouldBe(2);
            // This "Append()" call should not have create a new underlying list instance.
            GetUnderlyingList(list1).ShouldBeSameAs(originalUnderlyingList);

            list1.AppendRange(list1);
            // AppendRange() should not create new copies of the underlying list instance.
            GetUnderlyingList(list1).ShouldBeSameAs(originalUnderlyingList);

            list1.Count.ShouldBe(4);

            list1[0].ShouldBe("value1a");
            list1[1].ShouldBe("value1b");
            list1[2].ShouldBe("value1a");
            list1[3].ShouldBe("value1b");
        }

        [NotNull]
        private static List<T> GetUnderlyingList<T>([NotNull] AppendOnlyList<T> appendOnlyList)
        {
            var underlyingListField = appendOnlyList.GetType().GetField(
                "m_underlyingList",
                BindingFlags.Instance | BindingFlags.NonPublic
            );
            underlyingListField.ShouldNotBeNull();

            var underlyingList = (List<T>)underlyingListField.GetValue(appendOnlyList);
            underlyingList.ShouldNotBeNull();

            return underlyingList;
        }
    }
}
