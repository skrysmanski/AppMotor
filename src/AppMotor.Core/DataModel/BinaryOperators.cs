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

namespace AppMotor.Core.DataModel;

/// <summary>
/// The various binary (i.e. two parameters/operands) operators that can be custom implemented in C#.
/// </summary>
/// <seealso cref="UnaryOperators"/>
public enum BinaryOperators
{
    /// <summary>
    /// + operator
    /// </summary>
    Addition,

    /// <summary>
    /// - operator
    /// </summary>
    Subtraction,

    /// <summary>
    /// * operator
    /// </summary>
    Multiply,

    /// <summary>
    /// / operator
    /// </summary>
    Division,

    /// <summary>
    /// % operator
    /// </summary>
    Modulus,

    /// <summary>
    /// == operator
    /// </summary>
    Equality,

    /// <summary>
    /// != operator
    /// </summary>
    Inequality,

    /// <summary>
    /// &lt; operator
    /// </summary>
    LessThan,

    /// <summary>
    /// > operator
    /// </summary>
    GreaterThan,

    /// <summary>
    /// &lt;= operator
    /// </summary>
    LessThanOrEqual,

    /// <summary>
    /// >= operator
    /// </summary>
    GreaterThanOrEqual,

    /// <summary>
    /// &amp; operator
    /// </summary>
    BitwiseAnd,

    /// <summary>
    /// | operator
    /// </summary>
    BitwiseOr,

    /// <summary>
    /// ^ operator
    /// </summary>
    ExclusiveOr,

    /// <summary>
    /// &lt;&lt; operator
    /// </summary>
    LeftShift,

    /// <summary>
    /// >> operator
    /// </summary>
    RightShift,
}
