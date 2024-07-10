// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Utils;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Utils;

/// <summary>
/// Tests for <see cref="CSharpTypeName"/>.
/// </summary>
public sealed class CSharpTypeNameTests
{
    [Theory]
    [InlineData(typeof(string), "string")]
    [InlineData(typeof(object), "object")]
    [InlineData(typeof(bool), "bool")]
    [InlineData(typeof(byte), "byte")]
    [InlineData(typeof(sbyte), "sbyte")]
    [InlineData(typeof(short), "short")]
    [InlineData(typeof(ushort), "ushort")]
    [InlineData(typeof(int), "int")]
    [InlineData(typeof(uint), "uint")]
    [InlineData(typeof(long), "long")]
    [InlineData(typeof(ulong), "ulong")]
    [InlineData(typeof(float), "float")]
    [InlineData(typeof(double), "double")]
    [InlineData(typeof(decimal), "decimal")]
    [InlineData(typeof(int?), "int?")]
    [InlineData(typeof(object[]), "object[]")]
    [InlineData(typeof(int[][]), "int[][]")]
    [InlineData(typeof(float[,]), "float[,]")]
    [InlineData(typeof(List<string>), "List<string>")]
    [InlineData(typeof(List<List<string>>), "List<List<string>>")]
    [InlineData(typeof(IReadOnlyDictionary<string, int>), "IReadOnlyDictionary<string, int>")]
    [InlineData(typeof(List<>), "List<T>")]
    [InlineData(typeof(Dictionary<,>), "Dictionary<TKey, TValue>")]
    public void Test_GetName(Type type, string expectedName)
    {
        CSharpTypeName.GetName(type).ShouldBe(expectedName);
    }

    [Theory]
    [InlineData(typeof(string), CSharpTypeName.IncludeNamespaceFlags.All, "string")]
    [InlineData(typeof(Uri), CSharpTypeName.IncludeNamespaceFlags.None, "Uri")]
    [InlineData(typeof(Uri), CSharpTypeName.IncludeNamespaceFlags.SystemNamespace, "System.Uri")]
    [InlineData(typeof(Uri), CSharpTypeName.IncludeNamespaceFlags.SystemSubNamespaces, "Uri")]
    [InlineData(typeof(Uri), CSharpTypeName.IncludeNamespaceFlags.OtherNamespaces, "Uri")]
    [InlineData(typeof(List<string>), CSharpTypeName.IncludeNamespaceFlags.None, "List<string>")]
    [InlineData(typeof(List<string>), CSharpTypeName.IncludeNamespaceFlags.SystemNamespace, "List<string>")]
    [InlineData(typeof(List<string>), CSharpTypeName.IncludeNamespaceFlags.SystemSubNamespaces, "System.Collections.Generic.List<string>")]
    [InlineData(typeof(List<string>), CSharpTypeName.IncludeNamespaceFlags.OtherNamespaces, "List<string>")]
    [InlineData(typeof(TheoryAttribute), CSharpTypeName.IncludeNamespaceFlags.None, "TheoryAttribute")]
    [InlineData(typeof(TheoryAttribute), CSharpTypeName.IncludeNamespaceFlags.SystemNamespace, "TheoryAttribute")]
    [InlineData(typeof(TheoryAttribute), CSharpTypeName.IncludeNamespaceFlags.SystemSubNamespaces, "TheoryAttribute")]
    [InlineData(typeof(TheoryAttribute), CSharpTypeName.IncludeNamespaceFlags.OtherNamespaces, "Xunit.TheoryAttribute")]
    [InlineData(typeof(List<Uri>), CSharpTypeName.IncludeNamespaceFlags.None, "List<Uri>")]
    [InlineData(typeof(List<Uri>), CSharpTypeName.IncludeNamespaceFlags.SystemSubNamespaces, "System.Collections.Generic.List<Uri>")]
    [InlineData(typeof(List<Uri>), CSharpTypeName.IncludeNamespaceFlags.SystemNamespace, "List<System.Uri>")]
    [InlineData(typeof(List<Uri>), CSharpTypeName.IncludeNamespaceFlags.SystemNamespace | CSharpTypeName.IncludeNamespaceFlags.SystemSubNamespaces, "System.Collections.Generic.List<System.Uri>")]
    public void Test_GetName_NamespaceFlags(Type type, CSharpTypeName.IncludeNamespaceFlags namespaceFlags, string expectedName)
    {
        CSharpTypeName.GetName(type, namespaceFlags).ShouldBe(expectedName);
    }

    [Fact]
    public void Test_GetName_NamespacePredicate()
    {
        CSharpTypeName.GetName(typeof(List<Uri>), type => type.Namespace == "System.Collections.Generic").ShouldBe("System.Collections.Generic.List<Uri>");
    }

    [Fact]
    public void Test_GetName_Type()
    {
        // ReSharper disable once PossibleMistakenCallToGetType
        var runtimeTypeType = typeof(int).GetType();

        CSharpTypeName.GetName(runtimeTypeType).ShouldBe("Type");
        CSharpTypeName.GetName(runtimeTypeType, CSharpTypeName.IncludeNamespaceFlags.SystemSubNamespaces).ShouldBe("Type");
        CSharpTypeName.GetName(runtimeTypeType, CSharpTypeName.IncludeNamespaceFlags.SystemNamespace).ShouldBe("System.Type");
        CSharpTypeName.GetName(runtimeTypeType, CSharpTypeName.IncludeNamespaceFlags.OtherNamespaces).ShouldBe("Type");
    }
}
