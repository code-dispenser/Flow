using Flow.Core.Areas.Returns;
using Flow.Core.Common.Models;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit.Sdk;

namespace Flow.Core.Tests.Unit.Areas.Returns;

public class MightTests
{
    [Fact]
    public void The_static_with_value_method_should_create_a_might_with_a_value()
    {
        Might<int> withValue = Might<int>.WithValue(42);
           
        using (new AssertionScope())
        {
            withValue.Should().Match<Might<int>>(m => m.HasValue == true && m.HasNoValue == false);
            withValue.GetValueOr(0).Should().Be(42);
        }

    }
    [Fact]
    public void The_static_without_value_method_should_create_a_might_without_a_value()
    {
        Might<int> withoutValue = Might<int>.WithoutValue();

        using (new AssertionScope())
        {
            withoutValue.Should().Match<Might<int>>(m => m.HasValue == false && m.HasNoValue == true);
            withoutValue.GetValueOr(0).Should().Be(0);
            withoutValue.ToString().Should().Be("Ø");
        }
    }

    [Fact]
    public void The_constructor_should_throw_an_exception_when_the_value_is_null()
    
        =>  FluentActions.Invoking(() => new Might<string>(null!)).Should().Throw<ArgumentNullException>();


    [Fact]
    public void Implicit_conversion_with_a_null_should_create_a_might_without_a_value()
    {
        string nullValue = null;
        Might<string> withoutValue = nullValue!;
        using (new AssertionScope())
        {
            withoutValue.Should().Match<Might<string>>(m => m.HasValue == false && m.HasNoValue == true);
            withoutValue.GetValueOr("Failed").Should().Be("Failed");
            withoutValue.ToString().Should().Be("Ø");
        }
    }
    [Fact]
    public void Implicit_conversion_with_a_none_should_create_a_might_without_a_value()
    {
        Might<int> withoutValue = None.Value;
        using (new AssertionScope())
        {
            withoutValue.Should().Match<Might<int>>(m => m.HasValue == false && m.HasNoValue == true);
            withoutValue.GetValueOr(-1).Should().Be(-1);
            withoutValue.ToString().Should().Be("Ø");
        }
    }
    [Fact]
    public void Implicit_conversion_with_a_value_should_create_a_might_with_a_value()
    {
        Might<string> withValue = "Passed";
        using (new AssertionScope())
        {
            withValue.Should().Match<Might<string>>(m => m.HasValue == true && m.HasNoValue == false);
            withValue.GetValueOr("Failed").Should().Be("Passed");
        }
    }
    [Fact]
    public void Implicit_conversion_with_a_non_failure_should_create_a_successful_flow_with_the_value_assigned()
    {
        Flow<int> successfulFlow = 42;

        successfulFlow.Match(failure => throw new XunitException("Should not be a failure."), success => success).Should().Be(42);
    }

    [Fact]
    public void Match_should_execute_the_on_value_function_when_there_is_a_value()
    {
        Might<int> withValue = 42;

        withValue.Match(onNoValue: () => throw new XunitException("Should not be a no value"), onValue: value => value + 1)
                     .Should().Be(43);
    }

    [Fact]
    public void Match_should_execute_the_onNoValue_function_when_there_is_no_value()
    {
        Might<int> withoutValue = null!;

        withoutValue.Match(onNoValue: () => 42, onValue: _ => throw new XunitException("There should be no value to have this branch executed"))
            .Should().Be(42);
    }


    [Fact]
    public void Match_should_invoke_the_on_no_value_action_when_there_is_no_value()
    {
        var capturedValue = 0;
        
        Might<int> withoutValue = null!;

        withoutValue.Match(NoValueAction, _ => throw new XunitException("There should be no value to have this branch executed"));

        void NoValueAction() => capturedValue = 42;

        capturedValue.Should().Be(42);
    }

    [Fact]
    public void Match_should_invoke_the_on_value_action_when_there_is_a_value()
    {
        var capturedValue = 0;

        Might<int> withValue = 42;

        withValue.Match(() => throw new XunitException("There should be no value to have this branch executed"), ValueAction);
        
        void ValueAction(int value) => capturedValue = value;
        
        capturedValue.Should().Be(42);
    }

    [Fact]
    public void Map_should_invoke_the_on_value_function_when_there_is_a_value()
    {
        Might<int> withValue = 42;

        var mappedMight = withValue.Map(value => value + 1);

        using (new AssertionScope())
        {
            mappedMight.Should().Match<Might<int>>(m => m.HasValue == true && m.HasNoValue == false);
            mappedMight.GetValueOr(0).Should().Be(43);
        }
    }
    [Fact]
    public void Map_should_not_invoke_the_on_value_function_when_there_is_no_value()
    {
        Might<int> withValue = null!;

        var mappedMight = withValue.Map(value => value + 1);

        using (new AssertionScope())
        {
            mappedMight.Should().Match<Might<int>>(m => m.HasValue == false && m.HasNoValue == true);
            mappedMight.GetValueOr(-1).Should().Be(-1);
            mappedMight.ToString().Should().Be("Ø");
        }
    }

    [Fact]
    public void Bind_should_execute_the_might_returning_function_when_there_is_a_value()
    {
        Might<int> withValue = 42;

        var mightBind = withValue.Bind(value => Might<int>.WithValue(value + 1));

        using (new AssertionScope())
        {
            mightBind.Should().Match<Might<int>>(m => m.HasValue == true && m.HasNoValue == false);
            mightBind.GetValueOr(0).Should().Be(43);
        }
    }
    [Fact]
    public void Bind_should_not_execute_the_might_returning_function_when_there_is_no_value()
    {
        Might<int> withValue = null!;

        var mightBind = withValue.Bind<int>(_ => throw new XunitException("Bind should not execute the function without a value"));

        using (new AssertionScope())
        {
            mightBind.Should().Match<Might<int>>(m => m.HasValue == false && m.HasNoValue == true);
            mightBind.GetValueOr(-1).Should().Be(-1);
        }
    }

    [Fact]
    public void ToString_should_return_the_value_when_there_is_a_value()
    {
        Might<int> withValue = 42;
        withValue.ToString().Should().Be("Might(42)");
    }
    [Fact]
    public void ToString_should_return_Ø_when_there_is_no_value()
    {
        Might<int> withoutValue = null!;
        withoutValue.ToString().Should().Be("Ø");
    }
}
