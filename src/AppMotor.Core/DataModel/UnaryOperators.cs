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
/// The various unary (i.e. one parameters/operands) operators that can be custom implemented in C#.
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
