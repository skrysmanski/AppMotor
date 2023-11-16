// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using AppMotor.CliApp.CommandLine.Utils;

namespace AppMotor.CliApp.CommandLine;

/// <summary>
/// Represents a command line application with automatic command line argument parsing that only does
/// one function - like the <c>mv</c>, <c>rm</c>, or <c>dir</c> commands.
///
/// <para>Parameters (<see cref="CliParam{T}"/>) defined in a sub class are detected automatically (via
/// reflection).</para>
///
/// <para>If you need an application that bundles various functions, use <see cref="CliApplicationWithVerbs"/>
/// instead. If you want to specify a single <see cref="CliCommand"/> as main command (instead of defining the
/// parameters in a sub class of this class), use <see cref="CliApplicationWithCommand"/> instead.</para>
/// </summary>
public abstract class CliApplicationWithParams : CliApplicationWithCommand
{
    /// <summary>
    /// Executes this application. Implementations can access all command line parameters though the <see cref="CliParam{T}.Value"/>
    /// properties of the <see cref="CliParam{T}"/> instances declared in this class (or its base classes).
    /// </summary>
    protected abstract CliCommandExecutor Executor { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    protected CliApplicationWithParams()
    {
        this.Command = new MainCommand(this);
    }

    /// <summary>
    /// Returns all parameters defined for this application. The default implementation uses reflection to find all properties
    /// and fields of type <see cref="CliParamBase"/>. Inheritors may override this method either to filter its result or provide
    /// their own list.
    /// </summary>
    protected virtual IEnumerable<CliParamBase> GetAllParams()
    {
        return CliParamUtils.GetAllParamsFor(this);
    }

    private sealed class MainCommand : CliCommand
    {
        private readonly CliApplicationWithParams _cliApp;

        /// <inheritdoc />
        protected override CliCommandExecutor Executor => this._cliApp.Executor;

        /// <inheritdoc />
        public MainCommand(CliApplicationWithParams cliApp)
        {
            this._cliApp = cliApp;
        }

        /// <inheritdoc />
        protected override IEnumerable<CliParamBase> GetAllParams()
        {
            return this._cliApp.GetAllParams();
        }
    }
}
