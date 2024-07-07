// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

namespace AppMotor.Core.ComponentModel;

/// <summary>
/// The various unary (i.e. one parameter/operand) operators that can be custom implemented in C#.
/// </summary>
/// <seealso cref="BinaryOperators"/>
public enum UnaryOperators
{
    /// <summary>
    /// + operator
    /// </summary>
    UnaryPlus,

    /// <summary>
    /// - operator
    /// </summary>
    UnaryNegation,

    /// <summary>
    /// ++ operator
    /// </summary>
    Increment,

    /// <summary>
    /// -- operator
    /// </summary>
    Decrement,

    /// <summary>
    /// ! operator
    /// </summary>
    LogicalNot,

    /// <summary>
    /// ~ operator
    /// </summary>
    OnesComplement,
}
