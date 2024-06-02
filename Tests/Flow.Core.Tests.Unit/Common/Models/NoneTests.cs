using Flow.Core.Common.Models;
using FluentAssertions;

namespace Flow.Core.Tests.Unit.Common.Models;

public class NoneTests
{
    [Fact]
    public void To_string_should_return_the_empty_set_character()
    {
        None noValue = None.Value;

        noValue.ToString().Should().Be("Ø");
    }
}
