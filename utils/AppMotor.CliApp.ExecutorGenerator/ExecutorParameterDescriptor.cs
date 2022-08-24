// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

namespace AppMotor.CliApp.ExecutorGenerator;

internal sealed class ExecutorParameterDescriptor
{
    public string ParameterType { get; }

    public string ParameterName { get; }

    public string ParameterDescription { get; }

    public ExecutorParameterDescriptor(string parameterType, string parameterName, string parameterDescription)
    {
        this.ParameterType = parameterType;
        this.ParameterName = parameterName;
        this.ParameterDescription = parameterDescription;
    }
}