// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Collections;

using Xunit.Abstractions;

namespace AppMotor.TestCore.TestData;

/// <summary>
/// Base class to test data providers for xUnit. To use a test data class, attribute
/// the test method with <c>[ClassData(typeof(...))]</c>.
/// </summary>
public abstract class TestDataBase : IEnumerable<object?[]>
{
    /// <summary>
    /// Returns the test case data for a test method. Each item in the enumeration is a separate test case
    /// and each array element is the value for a parameter.
    /// </summary>
    ///
    /// <remarks>
    /// <para>IMPORTANT: If you want to be able to run a single specific test case, make sure the array
    /// only contains basic types. If any unsupported type is used, the test runner won't be able to execute
    /// single test cases (but instead will always execute all test cases). Supported types are basically any
    /// type that can be passed to a C# attribute as property. This means that custom types are definitely
    /// not supported.</para>
    ///
    /// <para>The reason for this is that xUnit's test discovery phase returns the test data serialized. If a
    /// value can't be serialized, the discovery phase basically just returns the test method itself without
    /// its test cases.</para>
    ///
    /// <para>You can make custom types serializable by implementing <see cref="IXunitSerializable"/>. In
    /// <see cref="IXunitSerializable.Deserialize"/> use <c>info.GetValue()</c> to fill the properties
    /// of the test data class. In <see cref="IXunitSerializable.Serialize"/>, use <c>info.AddValue()</c>
    /// to serialize the properties of the test data.</para>
    /// </remarks>
    ///
    /// <remarks>
    /// For custom test data types returned by this method, you can override <c>ToString()</c> to change the
    /// name of the test case data.
    /// </remarks>
    public abstract IEnumerator<object?[]> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
