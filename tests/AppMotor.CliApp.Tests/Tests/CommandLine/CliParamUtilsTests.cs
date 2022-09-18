// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.CommandLine;
using AppMotor.CliApp.CommandLine.Utils;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.Tests.CommandLine;

public sealed class CliParamUtilsTests
{
    [Fact]
    public void TestGetAllParamsFor()
    {
        // Setup
        var container = new TestContainer();

        // Test
        var allParams = CliParamUtils.GetAllParamsFor(container);

        // Verify
        var allParamsAsDict = allParams.ToDictionary(param => param.PrimaryName);

        foreach (var visibility in EnumUtils.GetValues<Visibilities>())
        {
            allParamsAsDict.ShouldNotContainKey(GetParamName(visibility, Scopes.Static, MemberTypes.Field));
            allParamsAsDict.ShouldNotContainKey(GetParamName(visibility, Scopes.Static, MemberTypes.Property));
            allParamsAsDict.ShouldNotContainKey(GetBaseParamName(visibility, Scopes.Static, MemberTypes.Field));
            allParamsAsDict.ShouldNotContainKey(GetBaseParamName(visibility, Scopes.Static, MemberTypes.Property));

            allParamsAsDict.ShouldContainKey(GetParamName(visibility, Scopes.Instance, MemberTypes.Field));
            allParamsAsDict.ShouldContainKey(GetParamName(visibility, Scopes.Instance, MemberTypes.Property));
            allParamsAsDict.ShouldContainKey(GetBaseParamName(visibility, Scopes.Instance, MemberTypes.Field));
            allParamsAsDict.ShouldContainKey(GetBaseParamName(visibility, Scopes.Instance, MemberTypes.Property));
        }

        allParamsAsDict.ShouldContainKey("--instance_property_param_with_backing_field");
        allParamsAsDict.ShouldContainKey("--instance_property_param_with_backing_field_base");
    }

    [Fact]
    // ReSharper disable once IdentifierTypo
    public void TestIgnorables()
    {
        // Setup
        var container = new TestContainerWithIgnorables();

        // Test
        var allParams = CliParamUtils.GetAllParamsFor(container);

        allParams.Count.ShouldBe(0);
    }

    [Fact]
    public void TestDoubleParamName()
    {
        // Setup
        var container = new TestContainerWithDoubleParamNames();

        // Test
        var ex = Should.Throw<InvalidOperationException>(() => CliParamUtils.GetAllParamsFor(container));

        ex.Message.ShouldContain("a_param_name");
    }

    [Fact]
    public void TestPositionParamSorting()
    {
        // Setup
        var container = new TestContainerWithPositionalParams();

        // Test
        var allParams = CliParamUtils.GetAllParamsFor(container);

        // Verify
        allParams.Count.ShouldBe(4);

        allParams[0].PrimaryName.ShouldBe("pos1");
        allParams[1].PrimaryName.ShouldBe("pos5");
        allParams[2].PrimaryName.ShouldBe("pos7");

        allParams[3].PrimaryName.ShouldBe("--some-param");
    }

    [MustUseReturnValue]
    private static string GetParamName(Visibilities visibility, Scopes scope, MemberTypes memberType)
    {
        return $"--{visibility}_{scope}_{memberType}_param";
    }

    [MustUseReturnValue]
    private static string GetBaseParamName(Visibilities visibility, Scopes scope, MemberTypes memberType)
    {
        return $"--{visibility}_{scope}_{memberType}_base_param";
    }

    private class TestContainer : TestContainerBase
    {
        [UsedImplicitly]
        public static readonly CliParam<int> PUBLIC_STATIC_FIELD_PARAM = new(GetParamName(Visibilities.Public, Scopes.Static, MemberTypes.Field));

        [UsedImplicitly]
        protected static readonly CliParam<int> PROTECTED_STATIC_FIELD_PARAM = new(GetParamName(Visibilities.Protected, Scopes.Static, MemberTypes.Field));

        [UsedImplicitly]
        internal static readonly CliParam<int> INTERNAL_STATIC_FIELD_PARAM = new(GetParamName(Visibilities.Internal, Scopes.Static, MemberTypes.Field));

        [UsedImplicitly]
        private static readonly CliParam<int> PRIVATE_STATIC_FIELD_PARAM = new(GetParamName(Visibilities.Private, Scopes.Static, MemberTypes.Field));



        [UsedImplicitly]
        public static CliParam<int> PublicStaticPropertyParam { get; } = new(GetParamName(Visibilities.Public, Scopes.Static, MemberTypes.Property));

        [UsedImplicitly]
        protected static CliParam<int> ProtectedStaticPropertyParam { get; } = new(GetParamName(Visibilities.Protected, Scopes.Static, MemberTypes.Property));

        [UsedImplicitly]
        internal static CliParam<int> InternalStaticPropertyParam { get; } = new(GetParamName(Visibilities.Internal, Scopes.Static, MemberTypes.Property));

        [UsedImplicitly]
        private static CliParam<int> PrivateStaticPropertyParam { get; } = new(GetParamName(Visibilities.Private, Scopes.Static, MemberTypes.Property));



        [UsedImplicitly]
        public CliParam<string> PublicPropertyParam { get; } = new(GetParamName(Visibilities.Public, Scopes.Instance, MemberTypes.Property));

        [UsedImplicitly]
        protected CliParam<string> ProtectedPropertyParam { get; } = new(GetParamName(Visibilities.Protected, Scopes.Instance, MemberTypes.Property));

        [UsedImplicitly]
        internal CliParam<string> InternalPropertyParam { get; } = new(GetParamName(Visibilities.Internal, Scopes.Instance, MemberTypes.Property));

        [UsedImplicitly]
        private CliParam<string> PrivatePropertyParam { get; } = new(GetParamName(Visibilities.Private, Scopes.Instance, MemberTypes.Property));



        [UsedImplicitly]
        public readonly CliParam<string> PublicFieldParam = new(GetParamName(Visibilities.Public, Scopes.Instance, MemberTypes.Field));

        [UsedImplicitly]
        protected readonly CliParam<string> ProtectedFieldParam = new(GetParamName(Visibilities.Protected, Scopes.Instance, MemberTypes.Field));

        [UsedImplicitly]
        internal readonly CliParam<string> InternalFieldParam = new(GetParamName(Visibilities.Internal, Scopes.Instance, MemberTypes.Field));

        [UsedImplicitly]
        // ReSharper disable once InconsistentNaming
        private readonly CliParam<string> PrivateFieldParam = new(GetParamName(Visibilities.Private, Scopes.Instance, MemberTypes.Field));



        private readonly CliParam<string> _propertyParamWithBackingField = new("--instance_property_param_with_backing_field");

        /// <summary>
        /// This parameter must only occur once.
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once ConvertToAutoProperty
        public CliParam<string> PropertyParamWithBackingField => this._propertyParamWithBackingField;

        /// <summary>
        /// Has the same value as <see cref="PropertyParamWithBackingField"/>.
        /// </summary>
        [UsedImplicitly]
        public CliParam<string> PropertyParamWithBackingField2 => this.PropertyParamWithBackingField;
    }

    private class TestContainerBase
    {
        [UsedImplicitly]
        public static readonly CliParam<int> PUBLIC_STATIC_FIELD_BASE_PARAM = new(GetBaseParamName(Visibilities.Public, Scopes.Static, MemberTypes.Field));

        [UsedImplicitly]
        protected static readonly CliParam<int> PROTECTED_STATIC_FIELD_BASE_PARAM = new(GetBaseParamName(Visibilities.Protected, Scopes.Static, MemberTypes.Field));

        [UsedImplicitly]
        internal static readonly CliParam<int> INTERNAL_STATIC_FIELD_BASE_PARAM = new(GetBaseParamName(Visibilities.Internal, Scopes.Static, MemberTypes.Field));

        [UsedImplicitly]
        private static readonly CliParam<int> PRIVATE_STATIC_FIELD_BASE_PARAM = new(GetBaseParamName(Visibilities.Private, Scopes.Static, MemberTypes.Field));



        [UsedImplicitly]
        public static CliParam<int> PublicStaticPropertyBaseParam { get; } = new(GetBaseParamName(Visibilities.Public, Scopes.Static, MemberTypes.Property));

        [UsedImplicitly]
        protected static CliParam<int> ProtectedStaticPropertyBaseParam { get; } = new(GetBaseParamName(Visibilities.Protected, Scopes.Static, MemberTypes.Property));

        [UsedImplicitly]
        internal static CliParam<int> InternalStaticPropertyBaseParam { get; } = new(GetBaseParamName(Visibilities.Internal, Scopes.Static, MemberTypes.Property));

        [UsedImplicitly]
        private static CliParam<int> PrivateStaticPropertyBaseParam { get; } = new(GetBaseParamName(Visibilities.Private, Scopes.Static, MemberTypes.Property));



        [UsedImplicitly]
        public CliParam<string> PublicPropertyBaseParam { get; } = new(GetBaseParamName(Visibilities.Public, Scopes.Instance, MemberTypes.Property));

        [UsedImplicitly]
        protected CliParam<string> ProtectedPropertyBaseParam { get; } = new(GetBaseParamName(Visibilities.Protected, Scopes.Instance, MemberTypes.Property));

        [UsedImplicitly]
        internal CliParam<string> InternalPropertyBaseParam { get; } = new(GetBaseParamName(Visibilities.Internal, Scopes.Instance, MemberTypes.Property));

        [UsedImplicitly]
        private CliParam<string> PrivatePropertyBaseParam { get; } = new(GetBaseParamName(Visibilities.Private, Scopes.Instance, MemberTypes.Property));



        [UsedImplicitly]
        public readonly CliParam<string> PublicFieldBaseParam = new(GetBaseParamName(Visibilities.Public, Scopes.Instance, MemberTypes.Field));

        [UsedImplicitly]
        protected readonly CliParam<string> ProtectedFieldBaseParam = new(GetBaseParamName(Visibilities.Protected, Scopes.Instance, MemberTypes.Field));

        [UsedImplicitly]
        internal readonly CliParam<string> InternalFieldBaseParam = new(GetBaseParamName(Visibilities.Internal, Scopes.Instance, MemberTypes.Field));

        [UsedImplicitly]
        // ReSharper disable once InconsistentNaming
        private readonly CliParam<string> PrivateFieldBaseParam = new(GetBaseParamName(Visibilities.Private, Scopes.Instance, MemberTypes.Field));



        private readonly CliParam<string> _propertyParamWithBackingBaseField = new("--instance_property_param_with_backing_field_base");

        /// <summary>
        /// This parameter must only occur once.
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once ConvertToAutoProperty
        public CliParam<string> PropertyParamWithBackingBaseField => this._propertyParamWithBackingBaseField;

        /// <summary>
        /// Has the same value as <see cref="PropertyParamWithBackingBaseField"/>.
        /// </summary>
        [UsedImplicitly]
        public CliParam<string> PropertyParamWithBackingBaseField2 => this.PropertyParamWithBackingBaseField;
    }

    // ReSharper disable once IdentifierTypo
    private sealed class TestContainerWithIgnorables
    {
        [UsedImplicitly]
        public int SomeProperty { get; } = 42;

        [UsedImplicitly]
        public readonly string SomeField = "abc";

        [UsedImplicitly]
        public CliParam<string>? NullParam { get; } = null;

        [UsedImplicitly]
        public CliParam<bool>? WriteOnlyParam
        {
            // ReSharper disable once ValueParameterNotUsed
            set
            {
                // Does nothing
            }
        }
    }

    private sealed class TestContainerWithDoubleParamNames
    {
        [UsedImplicitly]
        public CliParam<string> Param1 { get; } = new("--a_param_name");

        [UsedImplicitly]
        public CliParam<string> Param2 { get; } = new("--a_param_name"); // same name as param1
    }

    private sealed class TestContainerWithPositionalParams
    {
        [UsedImplicitly]
        public CliParam<string> PosParam1 { get; } = new("pos5", positionIndex: 5);

        [UsedImplicitly]
        public CliParam<string> NamedParam { get; } = new("--some-param");

        [UsedImplicitly]
        public CliParam<string> PosParam2 { get; } = new("pos1", positionIndex: 1);

        [UsedImplicitly]
        public CliParam<string> PosParam3 { get; } = new("pos7", positionIndex: 7);
    }

    private enum Visibilities
    {
        Public,
        Protected,
        Internal,
        Private,
    }

    private enum Scopes
    {
        Instance,
        Static,
    }

    private enum MemberTypes
    {
        Field,
        Property,
    }
}
