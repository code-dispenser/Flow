using Flow.Core.Areas.Extensions;
using Flow.Core.Areas.Returns;
using Flow.Core.Common.Models;
using FluentAssertions;

namespace Flow.Core.Tests.Functional.Areas.Extensions;

public class Potential_ExtensionTests_OrElse
{
    [Fact]
    public void Or_else_should_only_execute_the_potential_returning_function_if_there_is_no_value()///Bind
    {
        Potential<string> withNoValue = None.Value;

        var result = withNoValue.OrElse(() => new Potential<string>("hello world"));

        result.Should().Match<Potential<string>>(r => r.HasValue == true && r.GetValueOr(string.Empty) == "hello world");
    }

    [Fact]
    public void Or_else_should_not_execute_the_potential_returning_function_if_there_is_a_value()///Bind
    {
        Potential<string> withValue = "hello world";

        var result = withValue.OrElse(() => new Potential<string>("some other world"));

        result.Should().Match<Potential<string>>(r => r.HasValue == true && r.GetValueOr(string.Empty) == "hello world");
    }
    [Fact]
    public void Or_else_should_only_execute_the_function_if_there_is_no_value()///Bind
    {
        Potential<string> withNoValue = None.Value;

        var result = withNoValue.OrElse(() => "hello world");

        result.Should().Match<Potential<string>>(r => r.HasValue == true && r.GetValueOr(string.Empty) == "hello world");
    }

    [Fact]
    public void Or_else_should_not_execute_function_if_there_is_a_value()///Bind
    {
        Potential<string> withValue = "hello world";

        var result = withValue.OrElse(() => "some other world");

        result.Should().Match<Potential<string>>(r => r.HasValue == true && r.GetValueOr(string.Empty) == "hello world");
    }
}
