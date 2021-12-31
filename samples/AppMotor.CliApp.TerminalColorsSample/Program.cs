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

using AppMotor.CliApp.Terminals;
using AppMotor.Core.Extensions;
using AppMotor.Core.Utils;

int maxColorNameLength = EnumUtils.GetValues<ConsoleColor>().Max(consoleColor => consoleColor.ToString().Length);
var colorNameFormatString = $"{{0,-{maxColorNameLength + 2}}}";

foreach (var consoleColor in EnumUtils.GetValues<ConsoleColor>())
{
    Terminal.WriteLine(colorNameFormatString.WithIC($"{consoleColor}:") + new ColoredSubstring(consoleColor, consoleColor.ToString()));
}
