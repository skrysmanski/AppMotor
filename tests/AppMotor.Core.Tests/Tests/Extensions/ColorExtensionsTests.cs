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

using System.Drawing;

using AppMotor.Core.Colors;
using AppMotor.Core.Extensions;

using Shouldly;

using Xunit;

namespace AppMotor.Core.Tests.Extensions;

/// <summary>
/// Tests for <see cref="ColorExtensions"/>.
/// </summary>
public sealed class ColorExtensionsTests
{
    [Fact]
    public void Test_ToHsl()
    {
        Should.NotThrow(() => Color.Red.ToHsl());
        Should.NotThrow(() => RgbColor.Red.ToHsl());
    }

    [Fact]
    public void Test_ToHsv()
    {
        Should.NotThrow(() => Color.Red.ToHsv());
        Should.NotThrow(() => RgbColor.Red.ToHsv());
    }

    [Fact]
    public void Test_ToCmy()
    {
        Should.NotThrow(() => Color.Red.ToCmy());
        Should.NotThrow(() => RgbColor.Red.ToCmy());
    }

    [Fact]
    public void Test_ToCmyk()
    {
        Should.NotThrow(() => Color.Red.ToCmyk());
        Should.NotThrow(() => RgbColor.Red.ToCmyk());
    }

    [Fact]
    public void Test_ToHex()
    {
        Should.NotThrow(() => Color.Red.ToHex());
        Should.NotThrow(() => RgbColor.Red.ToHex());
    }
}
