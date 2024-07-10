// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Text;
using System.Text.RegularExpressions;

using JetBrains.Annotations;

namespace AppMotor.Core.Utils;

/// <summary>
/// Provides a type's name in C# syntax.
/// </summary>
public static class CSharpTypeName
{
    private static readonly Regex GENERIC_NAME_REGEX = new(@"^(.+)`\d+$", RegexOptions.Compiled);

    /// <summary>
    /// Returns the name of the specified type in C# syntax: e.g. "List&lt;string&gt;" instead of "List`1"
    /// or "ushort" instead of "UInt16".
    /// </summary>
    /// <param name="type">The type for which to return the name.</param>
    /// <param name="includeNamespaceFlags">Specifies for which types to include the namespace in the returned
    /// string. Defaults to <see cref="IncludeNamespaceFlags.None"/>.</param>
    [MustUseReturnValue]
    public static string GetName(Type type, IncludeNamespaceFlags includeNamespaceFlags = IncludeNamespaceFlags.None)
    {
        return GetName(type, IncludeNamespace);

        bool IncludeNamespace(Type typeToCheck)
        {
            if (includeNamespaceFlags == IncludeNamespaceFlags.None)
            {
                return false;
            }

            var @namespace = typeToCheck.Namespace!;

            if (@namespace == "System")
            {
                return includeNamespaceFlags.HasFlag(IncludeNamespaceFlags.SystemNamespace);
            }
            else if (@namespace.StartsWith("System.", StringComparison.Ordinal))
            {
                return includeNamespaceFlags.HasFlag(IncludeNamespaceFlags.SystemSubNamespaces);
            }
            else
            {
                return includeNamespaceFlags.HasFlag(IncludeNamespaceFlags.OtherNamespaces);
            }
        }
    }

    /// <summary>
    /// Returns the name of the specified type in C# syntax: e.g. "List&lt;string&gt;" instead of "List`1"
    /// or "ushort" instead of "UInt16".
    /// </summary>
    /// <param name="type">The type for which to return the name.</param>
    /// <param name="includeNamespacePredicate">A delegate that returns for each type whether to include the
    /// type's name in the returned string or not. Note that this delegate will never be called for types for
    /// which a C# keyword exists (i.e. <c>string</c>, <c>int</c>, ...).</param>
    [MustUseReturnValue]
    public static string GetName(Type type, Predicate<Type> includeNamespacePredicate)
    {
        //
        // Unresolved generic type parameter (e.g. "T" or "TKey").
        //
        if (type.IsGenericTypeParameter)
        {
            return type.Name;
        }

        //
        // Array
        //
        if (type.IsArray)
        {
            var arrayElementTypeName = GetName(type.GetElementType()!, includeNamespacePredicate);

            if (type.IsSZArray)
            {
                // One-dimensional array.
                // NOTE: If the type is "string[][]" then "arrayElementTypeName" will be "string[]".
                return arrayElementTypeName + "[]";
            }
            else
            {
                // Multi-dimensional array
                int arrayRank = type.GetArrayRank();

                var nameBuilder = new StringBuilder();
                nameBuilder.Append(arrayElementTypeName)
                           .Append('[')
                           .Append(',', repeatCount: arrayRank - 1)
                           .Append(']');
                return nameBuilder.ToString();
            }
        }

        //
        // Nullable value type
        //
        var underlyingType = Nullable.GetUnderlyingType(type);
        if (underlyingType is not null)
        {
            return GetName(underlyingType, includeNamespacePredicate) + "?";
        }

        //
        // C# keyword types
        //
        switch (type.FullName)
        {
            case "System.String":
                return "string";
            case "System.Object":
                return "object";
            case "System.Boolean":
                return "bool";

            case "System.Byte":
                return "byte";
            case "System.SByte":
                return "sbyte";
            case "System.Int16":
                return "short";
            case "System.UInt16":
                return "ushort";
            case "System.Int32":
                return "int";
            case "System.UInt32":
                return "uint";
            case "System.Int64":
                return "long";
            case "System.UInt64":
                return "ulong";

            case "System.Single":
                return "float";
            case "System.Double":
                return "double";
            case "System.Decimal":
                return "decimal";
        }

        bool includeNamespace = type.Namespace is not null && includeNamespacePredicate(type);

        if (!type.IsGenericType)
        {
            //
            // Non-generic types
            //

            // "Type" objects have the actual type of "RuntimeType". Simplify this to "Type"
            // as "RuntimeType" is internal and never surfaces in any code.
            if (type.FullName == "System.RuntimeType")
            {
                return includeNamespace ? "System.Type" : "Type";
            }

            return includeNamespace ? type.FullName! : type.Name;
        }
        else
        {
            //
            // Generic types
            //
            var nameBuilder = new StringBuilder();

            if (includeNamespace)
            {
                nameBuilder.Append(type.Namespace).Append('.');
            }

            var match = GENERIC_NAME_REGEX.Match(type.Name);
            var cleanedName = match.Groups[1].Value;
            nameBuilder.Append(cleanedName);

            nameBuilder.Append('<');

            var genericArguments = type.GetGenericArguments();

            for (var i = 0; i < genericArguments.Length; i++)
            {
                if (i != 0)
                {
                    nameBuilder.Append(", ");
                }

                var genericArgumentType = genericArguments[i];
                var genericArgumentName = GetName(genericArgumentType, includeNamespacePredicate);
                nameBuilder.Append(genericArgumentName);
            }

            nameBuilder.Append('>');

            return nameBuilder.ToString();
        }
    }

    /// <summary>
    /// Specifies which namespaces to include in <see cref="GetName(Type,IncludeNamespaceFlags)"/>.
    /// </summary>
    [Flags]
    public enum IncludeNamespaceFlags
    {
        /// <summary>
        /// No namespaces will be included.
        /// </summary>
        None = 0,

        /// <summary>
        /// The namespace will be included for types that are directly inside the <c>System</c> namespace.
        /// </summary>
        /// <remarks>
        /// Namespaces for types that are located in the sub-namespaces of <c>System</c> namespace will not be included
        /// through this flag. Use <see cref="SystemSubNamespaces"/> for that.
        /// </remarks>
        SystemNamespace = 1 << 0,

        /// <summary>
        /// The namespace will be included for types that are located in sub-namespaces of <c>System</c> - like
        /// <c>System.Reflection</c> or <c>System.Collections</c>.
        /// </summary>
        /// <remarks>
        /// Namespaces for types that are directly located inside the <c>System</c> namespace will not be included
        /// through this flag. Use <see cref="SystemNamespace"/> for that.
        /// </remarks>
        SystemSubNamespaces = 1 << 1,

        /// <summary>
        /// The namespace will be included for types that are located in any namespace that's not the <c>System</c>
        /// namespace or its sub-namespaces.
        /// </summary>
        OtherNamespaces = 1 << 2,

        /// <summary>
        /// The namespace will be included for all types (except for types that have a C# keyword like <c>int</c>
        /// or <c>string</c>).
        /// </summary>
        All = SystemNamespace | SystemSubNamespaces | OtherNamespaces,
    }
}
