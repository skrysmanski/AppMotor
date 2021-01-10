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

using System;

using AppMotor.CliApp.CommandLine;
using AppMotor.CliApp.CommandLine.Utils;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.Tests.CommandLine
{
    public sealed class ParamsCollectionBuilderTests
    {
        [Fact]
        public void TestBasicBuilding()
        {
            var builder = new ParamsCollectionBuilder();

            builder.Build().Length.ShouldBe(0);

            var param1 = new CliParam<int>("--value1");
            var param2 = new CliParam<int>("--value2");
            builder.AddParam(param1);
            builder.AddParam(param2);

            builder.Build().ShouldBe(new[] { param1, param2 });
        }

        [Fact]
        public void TestDuplicateAliasDetection()
        {
            var builder = new ParamsCollectionBuilder();

            var param1 = new CliParam<int>("--value1");
            builder.AddParam(param1);

            var param2 = new CliParam<int>("--value1");
            var ex = Should.Throw<InvalidOperationException>(() => builder.AddParam(param2));
            ex.Message.ShouldContain("--value1");
        }
    }
}
