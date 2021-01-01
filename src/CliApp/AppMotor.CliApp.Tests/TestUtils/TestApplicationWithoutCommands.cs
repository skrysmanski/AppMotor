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

using System;
using System.Collections.Generic;

using AppMotor.CliApp.CommandLine;

namespace AppMotor.CliApp.TestUtils
{
    internal class TestApplicationWithoutCommands : TestApplicationWithoutCommandsBase
    {
        private readonly Action _mainAction;

        private readonly List<CliParamBase> _params = new();

        /// <inheritdoc />
        protected override CliCommandExecutor Executor => new(Execute);

        public TestApplicationWithoutCommands(Action mainAction, params CliParamBase[] cliParams)
        {
            this._mainAction = mainAction;

            this._params.AddRange(cliParams);
        }

        /// <inheritdoc />
        protected override IEnumerable<CliParamBase> GetAllParams()
        {
            return this._params;
        }

        private void Execute()
        {
            this._mainAction();
        }
    }
}
