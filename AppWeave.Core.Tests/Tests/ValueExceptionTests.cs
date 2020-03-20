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
