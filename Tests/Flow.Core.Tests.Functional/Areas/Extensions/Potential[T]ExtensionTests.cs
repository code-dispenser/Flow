using Flow.Core.Areas.Extensions;
using Flow.Core.Areas.Returns;
using FluentAssertions;

namespace Flow.Core.Tests.Functional.Areas.Extensions
{
    public class Potential_ExtensionTests
    {
        [Fact]
        public void GetValueOr_should_return_the_value_when_the_potential_has_a_value()
        {
            Potential<int> potentialWithValue = Potential<int>.WithValue(42);
            
            int result = potentialWithValue.GetValueOr(0);
            result.Should().Be(42);
        }
        [Fact]
        public void GetValueOr_should_return_the_fallback_value_when_the_potential_has_no_value()
        {
            Potential<int> potentialWithoutValue = Potential<int>.WithoutValue();
            int result = potentialWithoutValue.GetValueOr(42);
            result.Should().Be(42);
        }
    }
}
