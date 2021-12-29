#region License
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

using AppMotor.Core.Utils;
using AppMotor.TestCore.Logging;

using Xunit.Abstractions;

namespace AppMotor.TestCore;

/// <summary>
/// Base class for test classes.
/// </summary>
public abstract class TestBase
{
    /// <summary>
    /// The "test console" - i.e. a replacement for <see cref="Console"/> to use in test.
    /// </summary>
    /// <remarks>
    /// The reason why you should use this property over <see cref="Console"/> is that tests run in parallel.
    /// Thus, it's impossible for the test runner to determine which console output belongs to which test.
    /// With this property, this problem is fixed.
    /// </remarks>
    /// <seealso cref="XUnitLoggerProviderExtensions.AddXUnitLogger"/>
    protected ITestOutputHelper TestConsole { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <remarks>
    /// Simply implement a constructor with the same signature in your test class and XUnit will automatically
    /// provide you with an instance of <see cref="ITestOutputHelper"/>.
    /// </remarks>
    protected TestBase(ITestOutputHelper testOutputHelper)
    {
        Validate.ArgumentWithName(nameof(testOutputHelper)).IsNotNull(testOutputHelper);

        this.TestConsole = testOutputHelper;
    }
}