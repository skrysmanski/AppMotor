// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Collections;

namespace AppMotor.TestCore.TestData;

/// <summary>
/// Base class to test data providers for xUnit. To use a test data class, attribute
/// the test method with <c>[ClassData(typeof(...))]</c>.
/// </summary>
public abstract class TestDataBase : IEnumerable<object?[]>
{
    /// <summary>
    /// Provides the test data. Each item in the enumeration is a separate test case
    /// and each array element is the value for a parameter.
    /// </summary>
    public abstract IEnumerator<object?[]> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
