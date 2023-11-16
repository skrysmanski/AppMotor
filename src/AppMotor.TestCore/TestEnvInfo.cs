// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

namespace AppMotor.TestCore;

/// <summary>
/// Information about the test environment a test runs in.
/// </summary>
public static class TestEnvInfo
{
    /// <summary>
    /// Whether the tests are executed in a CI pipeline. Use this to increase timeouts
    /// when tests fail due to high load in the CI pipeline.
    /// </summary>
    /// <remarks>
    /// To make this property <c>true</c>, you need to define the environment variable "RUNS_IN_CI" and set its value to "true".
    /// </remarks>
    public static bool RunsInCiPipeline => s_runsInCiPipelineLazy.Value;

    private static readonly Lazy<bool> s_runsInCiPipelineLazy = new(() => Environment.GetEnvironmentVariable("RUNS_IN_CI") == "true");
}
