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

    [Fact]
    public void None_value_is_a_singleton()
    {
        //Keep code coverage happy as None is a record type
        var none1 = None.Value;
        var none2 = none1 with { };
        
        none1.Should().Be(none2);
    }
}

