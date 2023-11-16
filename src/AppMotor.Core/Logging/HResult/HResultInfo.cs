// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Reflection;

using AppMotor.Core.Extensions;

using JetBrains.Annotations;

namespace AppMotor.Core.Logging;

/// <summary>
/// Provides information about <see cref="Exception.HResult"/>.
/// </summary>
public static class HResultInfo
{
    private static readonly Dictionary<int, string> s_hResultNames = GetHResultNames();

    [Pure]
    private static Dictionary<int, string> GetHResultNames()
    {
        var hResultNames = new Dictionary<int, string>();

        var fieldInfos = typeof(HResults).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

        foreach (var fieldInfo in fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly))
        {
            int hResult = (int)fieldInfo.GetRawConstantValue()!;
            hResultNames[hResult] = fieldInfo.Name;
        }

        return hResultNames;
    }

    /// <summary>
    /// Returns the name for the specified value, if it has a name. Otherwise
    /// <c>null</c> will be returned.
    /// </summary>
    [PublicAPI, Pure]
    public static string? GetHResultName(int hResult)
    {
        s_hResultNames.TryGetValue(hResult, out var hResultName);
        return hResultName;
    }

    /// <summary>
    /// Formats the specified value into a string.
    /// </summary>
    /// <param name="hResult">The value to format</param>
    /// <param name="includeName">Whether to include the value's name, if
    /// it has one (see <see cref="GetHResultName"/>).</param>
    /// <returns></returns>
    [PublicAPI, Pure]
    public static string FormatHResult(int hResult, bool includeName)
    {
        // Format the HResult property in hexadecimal notation.
        var formattedHResult = "0x{0:X8}".WithIC(hResult);

        if (includeName && s_hResultNames.TryGetValue(hResult, out var hResultName))
        {
            formattedHResult += $" ({hResultName})";
        }

        return formattedHResult;
    }
}
