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

using JetBrains.Annotations;

using Microsoft.Extensions.Hosting;

namespace AppMotor.CliApp.CommandLine.Hosting;

/// <summary>
/// Factory for <see cref="IHostBuilder"/>.
/// </summary>
/// <remarks>
/// The main use case behind this interface is to allow users to define the <see cref="IHostBuilder"/>
/// they want to use with <see cref="GenericHostCliCommand"/>. Having an interface here (instead of
/// just a factory method) gives implementers more freedom in how they want to make the host builder
/// configurable (e.g. something like <see cref="DefaultHostBuilderFactory"/> would not be possible
/// with a factory method).
/// </remarks>
public interface IHostBuilderFactory
{
    /// <summary>
    /// Creates a new <see cref="IHostBuilder"/> instance.
    /// </summary>
    [MustUseReturnValue]
    IHostBuilder CreateHostBuilder();
}