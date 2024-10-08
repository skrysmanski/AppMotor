﻿// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Collections.Immutable;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.ComponentModel;

using AppMotor.CliApp.CommandLine.Utils;
using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.CliApp.CommandLine;

/// <summary>
/// Non-generic base class for <see cref="CliParam{T}"/>.
///
/// <para>Note: You should not use this class directly. Instead, use <see cref="CliParam{T}"/>.</para>
/// </summary>
public abstract class CliParamBase
{
    /// <summary>
    /// The primary name for this parameter.
    /// </summary>
    [PublicAPI]
    public string PrimaryName => this.Names[0];

    /// <summary>
    /// Whether the parameter is named or positional.
    /// </summary>
    [PublicAPI]
    public CliParamTypes ParameterType => this.PositionIndex is null ? CliParamTypes.Named : CliParamTypes.Positional;

    /// <summary>
    /// The names (or aliases) of this parameter.
    ///
    /// <para>Note: Positional parameters (see <see cref="ParameterType"/>) always only have one name.</para>
    /// </summary>
    [PublicAPI]
    public ImmutableArray<string> Names { get; }

    /// <summary>
    /// The position of this positional parameter among all other positional parameters; positional parameters
    /// are ordered by this value. No two positional parameter can have the same position index.
    ///
    /// <para>Is <c>null</c> for named parameters (see <see cref="ParameterType"/>).</para>
    /// </summary>
    /// <remarks>
    /// This property is a safety feature. It primarily exists because positional parameters must be ordered, and because,
    /// by default, we use reflection to find all parameters of a command. If this property didn't exist, we would rely
    /// on the order of <see cref="Type.GetProperties()"/> (which may be arbitrary) - or, if the order is always the same
    /// in which the properties are declared - we would require developers to order their properties in a certain way, which
    /// is problematic because the order of members in a type normally doesn't matter (and through refactoring the
    /// order of the properties could change and thus change the order of the positional parameters would change).
    /// </remarks>
    [PublicAPI]
    public int? PositionIndex { get; }

    /// <summary>
    /// The help text for this parameter.
    /// </summary>
    [PublicAPI, Localizable(true)]
    public string? HelpText { get; init; }

    internal abstract Symbol UnderlyingImplementation { get; }

    /// <summary>
    /// Creates a named parameter (in contrast to a positional one). See <see cref="CliParamTypes.Named"/> for more details.
    /// </summary>
    /// <param name="names">The names/aliases for this parameter; must be a valid named parameter name (i.e. start with
    /// either "--" or "-") - see <see cref="CliParamNameValidation.CheckIfNameIsValid"/> for more details.</param>
    protected CliParamBase(IEnumerable<string> names)
    {
        var allNames = names.ToImmutableArray();

        if (allNames.Length == 0)
        {
            throw new ArgumentException("No names have been specified.", nameof(names));
        }

        var namesSet = new HashSet<string>();

        foreach (var name in allNames)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Parameter names can't be null or whitespace.", nameof(names));
            }

            if (namesSet.Contains(name))
            {
                // NOTE: Due to the potentially important order of the parameters (for help generation) it was
                //   decided that it's easier to throw an exception in this case than to deduplicate the names.
                throw new ArgumentException($"Passing the same name ('{name}') multiple times is not allowed.", nameof(names));
            }

            // NOTE: We want "ArgumentExceptions" here for "names" - not "ValueExceptions" for "name".
            Validate.ArgumentWithName(nameof(names)).IsValidParameterName(name, CliParamTypes.Named);

            namesSet.Add(name);
        }

        this.Names = allNames;
    }

    /// <summary>
    /// Creates a positional parameter (in contrast to a named parameter). See <see cref="CliParamTypes.Positional"/> for more details.
    /// </summary>
    /// <param name="name">The name of this parameter; only used for generating the help text; must be a valid positional
    /// parameter name - see <see cref="CliParamNameValidation.CheckIfNameIsValid"/> for more details.</param>
    /// <param name="positionIndex">The position of this parameter among all other positional parameters; positional parameters
    /// are ordered by this value</param>
    protected CliParamBase(string name, int positionIndex)
    {
        Validate.ArgumentWithName(nameof(name)).IsValidParameterName(name, CliParamTypes.Positional);

        this.Names = [name];
        this.PositionIndex = positionIndex;
    }

    internal abstract void SetValueFromParseResult(ParseResult parseResult);

    /// <inheritdoc />
    public override string ToString()
    {
        return this.PrimaryName;
    }
}
