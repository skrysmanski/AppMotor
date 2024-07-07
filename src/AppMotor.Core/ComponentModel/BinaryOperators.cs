// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.Core.Extensions;

namespace AppMotor.Core.ComponentModel;

/// <summary>
/// The various binary (i.e. two parameters/operands) operators that can be custom implemented in C#.
/// </summary>
/// <seealso cref="TypeExtensions.GetOperator(Type,BinaryOperators)"/>
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
