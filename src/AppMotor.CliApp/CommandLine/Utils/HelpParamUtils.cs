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

using AppMotor.Core.Utils;

using JetBrains.Annotations;

namespace AppMotor.CliApp.CommandLine.Utils
{
    /// <summary>
    /// Provides utility methods for the help parameter/command.
    /// </summary>
    public static class HelpParamUtils
    {
        /// <summary>
        /// The default variant of the help param names (see <see cref="IsHelpParamName"/>).
        /// </summary>
        public static string DefaultHelpParamName => "--help";

        /// <summary>
        /// The name of the (virtual) help command.
        /// </summary>
        public static string HelpCommandName => "help";

        /// <summary>
        /// Checks whether the specified parameter/argument represents the help param.
        /// </summary>
        [MustUseReturnValue]
        public static bool IsHelpParamName(string arg)
        {
            Validate.ArgumentWithName(nameof(arg)).IsNotNull(arg);

            switch (arg.ToLowerInvariant())
            {
                case "-h":
                case "/h":
                case "--help":
                case "-?":
                case "/?":
                    return true;

                default:
                    return false;
            }
        }
    }
}
