// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Certificates;
using AppMotor.Core.Extensions;

using Shouldly;

namespace AppMotor.TestCore;

/// <summary>
/// Provides standardized tests for implemented equality members (i.e. <see cref="IEquatable{T}"/>,
/// <see cref="object.Equals(object?)"/>, <c>==</c> and <c>!=</c> operators, ...
/// </summary>
public static class EqualityMembersTests
{
    /// <summary>
    /// Tests equality members.
    /// </summary>
    /// <param name="baseItem">An item</param>
    /// <param name="differentItem">An item that's different than <paramref name="baseItem"/></param>
    public static void TestEquals<T>(T baseItem, T differentItem) where T : IEquatable<T>
    {
        TestEquals(baseItem, baseItem, differentItem);
    }

    /// <summary>
    /// Tests equality members.
    /// </summary>
    /// <param name="baseItem">An item</param>
    /// <param name="equalItem"></param>
    /// <param name="differentItem">An item that's different than <paramref name="baseItem"/></param>
    public static void TestEquals<T>(T baseItem, T equalItem, T differentItem) where T : IEquatable<T>
    {
        baseItem.Equals(equalItem).ShouldBe(true);
        baseItem.Equals(differentItem).ShouldBe(false);

        baseItem.Equals(equalItem).ShouldBe(true);
        baseItem.Equals(new CertificateOid()).ShouldBe(false);

        typeof(T).GetOperator("==")!.Invoke(null, new object[] { baseItem, equalItem }).ShouldBe(true);
        typeof(T).GetOperator("==")!.Invoke(null, new object[] { baseItem, differentItem }).ShouldBe(false);
        typeof(T).GetOperator("!=")!.Invoke(null, new object[] { baseItem, equalItem }).ShouldBe(false);
        typeof(T).GetOperator("!=")!.Invoke(null, new object[] { baseItem, differentItem }).ShouldBe(true);

        // ReSharper disable once SuspiciousTypeConversion.Global
        baseItem.Equals("").ShouldBe(false);
        baseItem.Equals((object)equalItem).ShouldBe(true);
        baseItem.Equals(null).ShouldBe(false);
    }

    /// <summary>
    /// Tests the <c>GetHashCode()</c> override.
    /// </summary>
    /// <param name="baseItem">An item</param>
    public static void TestGetHashCode<T>(T baseItem) where T : notnull
    {
        baseItem.GetHashCode().ShouldNotBe(0);
    }

    /// <summary>
    /// Tests the <c>GetHashCode()</c> override.
    /// </summary>
    /// <param name="baseItem">An item</param>
    /// <param name="equalItem">An item that's supposed to be equal to <paramref name="baseItem"/></param>
    public static void TestGetHashCode<T>(T baseItem, T equalItem) where T : notnull
    {
        baseItem.GetHashCode().ShouldNotBe(0);
        baseItem.GetHashCode().ShouldBe(equalItem.GetHashCode());
    }
}
