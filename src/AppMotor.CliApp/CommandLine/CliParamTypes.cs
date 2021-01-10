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

namespace AppMotor.CliApp.CommandLine
{
    /// <summary>
    /// The type of a <see cref="CliParam{T}"/>.
    /// </summary>
    public enum CliParamTypes
    {
        /// <summary>
        /// The parameter is a named parameter (created via <see cref="CliParam{T}(string,string[])"/>).
        /// Named parameters can either be flags/standalone (only for <c>bool</c> parameters; like <c>--verbose</c>) or
        /// key value pairs (like <c>--message "my text"</c>).
        ///
        /// <para>Opposite of a positional parameter (see <see cref="Positional"/>).</para>
        /// </summary>
        Named,

        /// <summary>
        /// This parameter is a positional parameter (created via <see cref="CliParam{T}(string,int)"/>).
        /// Positional parameters are identified only by their position on the command line (and not by their name).
        /// For example, a copy command would be <c>copy "c:\test.txt" "d:\"</c> with both parameters being
        /// positional ones (first: source, second: destination).
        ///
        /// <para>Opposite of a named parameter (see <see cref="Named"/>).</para>
        /// </summary>
        Positional,
    }
}
