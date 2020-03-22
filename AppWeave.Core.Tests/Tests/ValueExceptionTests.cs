/*
Copyright 2020 AppWeave.Core (https://github.com/skrysmanski/AppWeave.Core)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using AppWeave.Core.Exceptions;

using Shouldly;

using Xunit;

namespace AppWeave.Core.Tests
{
    public sealed class ValueExceptionTests
    {
        [Fact]
        public void TestDefaultConstructor()
        {
            var ex = new ValueException();

            ex.ValueName.ShouldBe(null);
            ex.Message.ShouldBe(ValueException.DEFAULT_MESSAGE);
        }
    }
}
