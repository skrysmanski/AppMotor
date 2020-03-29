#region License
// Copyright 2020 - 2020 AppWeave.Core (https://github.com/skrysmanski/AppWeave.Core)
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
using System.Numerics;

using AppWeave.Core.DataModel;
using AppWeave.Core.Logging;

using Shouldly;

using Xunit;

namespace AppWeave.Core.Tests.Logging
{
    public sealed class LoggableValuesTests
    {
        [Fact]
        public void TestIsSimpleLoggableType()
        {
            // setup
            typeof(MyCustomLoggableType2).MarkWith<SimpleLoggableValueMarker>();
            typeof(MyCustomSensitiveType2).MarkWith<SensitiveValueMarker>();

            // test
            typeof(object).IsSimpleLoggableType().ShouldBe(false);
            typeof(Exception).IsSimpleLoggableType().ShouldBe(false);

            typeof(string).IsSimpleLoggableType().ShouldBe(true);

            typeof(byte).IsSimpleLoggableType().ShouldBe(true);
            typeof(byte?).IsSimpleLoggableType().ShouldBe(true);
            typeof(sbyte).IsSimpleLoggableType().ShouldBe(true);
            typeof(sbyte?).IsSimpleLoggableType().ShouldBe(true);
            typeof(short).IsSimpleLoggableType().ShouldBe(true);
            typeof(short?).IsSimpleLoggableType().ShouldBe(true);
            typeof(ushort).IsSimpleLoggableType().ShouldBe(true);
            typeof(ushort?).IsSimpleLoggableType().ShouldBe(true);
            typeof(int).IsSimpleLoggableType().ShouldBe(true);
            typeof(int?).IsSimpleLoggableType().ShouldBe(true);
            typeof(uint).IsSimpleLoggableType().ShouldBe(true);
            typeof(uint?).IsSimpleLoggableType().ShouldBe(true);
            typeof(long).IsSimpleLoggableType().ShouldBe(true);
            typeof(long?).IsSimpleLoggableType().ShouldBe(true);
            typeof(ulong).IsSimpleLoggableType().ShouldBe(true);
            typeof(ulong?).IsSimpleLoggableType().ShouldBe(true);

            typeof(float).IsSimpleLoggableType().ShouldBe(true);
            typeof(float?).IsSimpleLoggableType().ShouldBe(true);
            typeof(double).IsSimpleLoggableType().ShouldBe(true);
            typeof(double?).IsSimpleLoggableType().ShouldBe(true);
            typeof(decimal).IsSimpleLoggableType().ShouldBe(true);
            typeof(decimal?).IsSimpleLoggableType().ShouldBe(true);

            typeof(BigInteger).IsSimpleLoggableType().ShouldBe(true);
            typeof(BigInteger?).IsSimpleLoggableType().ShouldBe(true);

            typeof(bool).IsSimpleLoggableType().ShouldBe(true);
            typeof(bool?).IsSimpleLoggableType().ShouldBe(true);
            typeof(char).IsSimpleLoggableType().ShouldBe(true);
            typeof(char?).IsSimpleLoggableType().ShouldBe(true);
            typeof(Guid).IsSimpleLoggableType().ShouldBe(true);
            typeof(Guid?).IsSimpleLoggableType().ShouldBe(true);
            typeof(DateTime).IsSimpleLoggableType().ShouldBe(true);
            typeof(DateTime?).IsSimpleLoggableType().ShouldBe(true);
            typeof(TimeSpan).IsSimpleLoggableType().ShouldBe(true);
            typeof(TimeSpan?).IsSimpleLoggableType().ShouldBe(true);
            typeof(DateTimeOffset).IsSimpleLoggableType().ShouldBe(true);
            typeof(DateTimeOffset?).IsSimpleLoggableType().ShouldBe(true);

            // Enums
            typeof(MyEnum).IsSimpleLoggableType().ShouldBe(true);
            typeof(MyEnum?).IsSimpleLoggableType().ShouldBe(true);

            // Custom types
            typeof(MyCustomLoggableType1).IsSimpleLoggableType().ShouldBe(true);
            typeof(MyCustomLoggableType2).IsSimpleLoggableType().ShouldBe(true);

            typeof(MyCustomSensitiveType1).IsSimpleLoggableType().ShouldBe(false);
            typeof(MyCustomSensitiveType2).IsSimpleLoggableType().ShouldBe(false);
        }

        private enum MyEnum
        {
        }

        private sealed class MyCustomLoggableType1 : ISimpleLoggableValue
        {
        }

        private sealed class MyCustomLoggableType2
        {
        }

        private sealed class MyCustomSensitiveType1 : ISimpleLoggableValue, ISensitiveValue
        {
        }

        private sealed class MyCustomSensitiveType2 : ISimpleLoggableValue
        {
        }
    }
}
