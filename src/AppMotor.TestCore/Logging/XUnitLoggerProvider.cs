﻿#region License
// Copyright 2021 AppMotor Framework (https://github.com/skrysmanski/AppMotor)
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

using Microsoft.Extensions.Logging;

using Xunit.Abstractions;

namespace AppMotor.TestCore.Logging;

/// <summary>
/// Logger provider for <see cref="XUnitLogger"/>.
/// </summary>
internal sealed class XUnitLoggerProvider : ILoggerProvider
{
    private readonly ITestOutputHelper _testOutputHelper;

    private readonly TestLoggerStatistics _loggerStatistics;

    public XUnitLoggerProvider(ITestOutputHelper testOutputHelper, TestLoggerStatistics loggerStatistics)
    {
        this._testOutputHelper = testOutputHelper;
        this._loggerStatistics = loggerStatistics;
    }

    /// <inheritdoc />
    public void Dispose()
    {
    }

    /// <inheritdoc />
    public ILogger CreateLogger(string categoryName)
    {
        return new XUnitLogger(this._testOutputHelper, categoryName, this._loggerStatistics);
    }
}