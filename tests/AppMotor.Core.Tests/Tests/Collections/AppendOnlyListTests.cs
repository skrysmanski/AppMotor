// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Reflection;

using AppMotor.Core.Collections;
using AppMotor.Core.TestUtils;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Collections;

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
        GetUnderlyingList(list1).ShouldNotBeSameAs(originalUnderlyingList);

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

        list1.AppendRange(new[] { "value1a", "value1b" });
        list1.Count.ShouldBe(2);
        // This "Append()" call should not have create a new underlying list instance.
        GetUnderlyingList(list1).ShouldBeSameAs(originalUnderlyingList);

        var list2 = list1.CloneShallow();
        // CloneShallow() should not create new copies of the underlying list instance.
        GetUnderlyingList(list1).ShouldBeSameAs(originalUnderlyingList);
        GetUnderlyingList(list2).ShouldBeSameAs(originalUnderlyingList);

        list2.Count.ShouldBe(2);
        list2.AppendRange(new[] { "value2a", "value2b" });
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

        list1.AppendRange(new[] { "value1a", "value1b" });
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

    [Fact]
    public void TestIndexer_OutOfRange()
    {
        var list = new AppendOnlyList<string>();

        Should.Throw<ArgumentOutOfRangeException>(() => list[0]);
        Should.Throw<ArgumentOutOfRangeException>(() => list[-1]);

        list.Append("abc");

        Should.NotThrow(() => list[0]);
        Should.Throw<ArgumentOutOfRangeException>(() => list[1]);
        Should.Throw<ArgumentOutOfRangeException>(() => list[-1]);
    }

    [Fact]
    public void TestGetEnumerator()
    {
        var list = new AppendOnlyList<string>();

        list.ExecuteGenericEnumerator().Count.ShouldBe(0);

        list.Append("abc");
        list.Append("abc2");

        list.ExecuteGenericEnumerator().ShouldBe(list);
    }

    [Fact]
    public void TestGetEnumerator_NonGeneric()
    {
        var list = new AppendOnlyList<string>();

        list.ExecuteNonGenericEnumerator<string>().Count.ShouldBe(0);

        list.Append("abc");
        list.Append("abc2");

        list.ExecuteNonGenericEnumerator<string>().ShouldBe(list);
    }

    [Fact]
    public void TestCollectionConstructor()
    {
        var sourceList = new List<string>()
        {
            "abc",
            "def",
            "ghi",
        };

        var appendOnlyList1 = new AppendOnlyList<string>(sourceList);
        appendOnlyList1.ShouldBe(sourceList);

        var appendOnlyList2 = new AppendOnlyList<string>((IEnumerable<string>)appendOnlyList1);
        GetUnderlyingList(appendOnlyList2).ShouldBeSameAs(GetUnderlyingList(appendOnlyList1));
    }

    private static List<T> GetUnderlyingList<T>(AppendOnlyList<T> appendOnlyList)
    {
        var underlyingListField = appendOnlyList.GetType().GetField(
            "_underlyingList",
            BindingFlags.Instance | BindingFlags.NonPublic
        );
        underlyingListField.ShouldNotBeNull();

        var underlyingList = (List<T>?)underlyingListField.GetValue(appendOnlyList);
        underlyingList.ShouldNotBeNull();

        return underlyingList;
    }
}