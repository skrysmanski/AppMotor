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

using JetBrains.Annotations;

namespace AppMotor.Core.Colors;

public static class ColorExtensions
{
    [MustUseReturnValue]
    public static HslColor ToHsl(this Color color)
    {
        return new HslColor(color);
    }

    [MustUseReturnValue]
    public static HsvColor ToHsv(this Color color)
    {
        return new HsvColor(color);
    }

    [MustUseReturnValue]
    public static CmyColor ToCmy(this Color color)
    {
        return new CmyColor(color);
    }

    [MustUseReturnValue]
    public static CmykColor ToCmyk(this Color color)
    {
        return new CmykColor(color);
    }
}
