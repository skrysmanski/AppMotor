#region License
// Copyright 2022 AppMotor Framework (https://github.com/skrysmanski/AppMotor)
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

using System.Collections;

using AppMotor.Core.Utils;

namespace AppMotor.TestCore.Utils;

/// <summary>
/// Provides all members/values of an enum as test data to be used as <c>[ClassData(typeof(EnumTestDataGenerator&lt;MyEnum&gt;))]</c>.
/// </summary>
public sealed class EnumTestDataGenerator<TEnum> : IEnumerable<object[]> where TEnum : Enum
{
    /// <inheritdoc />
    public IEnumerator<object[]> GetEnumerator()
    {
        foreach (var enumValue in EnumUtils.GetValues<TEnum>())
        {
            yield return new object[] { enumValue };
        }
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
