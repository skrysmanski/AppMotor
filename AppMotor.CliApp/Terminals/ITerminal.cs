#region License
// Copyright 2020 AppMotor Framework (https://github.com/skrysmanski/AppMotor)
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

namespace AppMotor.CliApp.Terminals
{
    /// <summary>
    /// Represents a terminal/console without any "fancy" functionality; i.e. it supports reading and writing but
    /// no interaction with the terminal window - like setting the terminal's title, cursor position or obtaining
    /// the terminal's width. For advanced features, see <see cref="ITerminalWindow"/>.
    /// </summary>
    public interface ITerminal : IInputTerminal, IOutputTerminal, IErrorOutputTerminal
    {
    }
}
