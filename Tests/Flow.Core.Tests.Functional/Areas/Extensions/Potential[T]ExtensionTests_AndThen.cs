using Flow.Core.Areas.Extensions;
using Flow.Core.Areas.Returns;
using Flow.Core.Common.Models;
using FluentAssertions;
using FluentAssertions.Execution;

namespace Flow.Core.Tests.Functional.Areas.Extensions;


public class Potential_ExtensionTests_AndThen
{
    [Fact]
    public void And_then_should_only_execute_the_potential_returning_function_if_there_is_a_value()///Bind
    {
        Potential<int> withValue = 42;

        var result = withValue.AndThen(value => new Potential<string>(value.ToString() + " world"));

        result.Should().Match<Potential<string>>(r => r.HasValue == true && r.Reduce(string.Empty) == "42 world");
    }

    [Fact]
    public void And_then_should_not_execute_the_potential_returning_function_if_there_is_no_value_and_return_no_value()///Bind
    {
        Potential<int> withoutValue = None.Value;

        var result = withoutValue.AndThen(value => new Potential<string>(value + " world"));//type should still change to Potential<string>

        result.Should().Match<Potential<string>>(r => r.HasValue == false && r.Reduce(String.Empty) == String.Empty);
    }


    [Fact]
    public void And_then_should_only_execute_the_function_if_there_is_a_value()//Map
    {
        Potential<int> withValue = 42;

        var result = withValue.AndThen(value => value.ToString() + " world");

        result.Should().Match<Potential<string>>(r => r.HasValue == true && r.Reduce(string.Empty) == "42 world");

    }
    [Fact]
    public void And_then_should_not_execute_the_function_if_there_is_no_value_and_return_no_value()///Bind
    {
        Potential<int> withoutValue = None.Value;

        var result = withoutValue.AndThen(value => value + " world");//type should still change to Potential<string>

        result.Should().Match<Potential<string>>(r => r.HasValue == false && r.Reduce(String.Empty) == String.Empty);
    }

}
