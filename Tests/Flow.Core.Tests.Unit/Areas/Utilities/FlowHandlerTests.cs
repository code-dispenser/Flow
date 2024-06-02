using Flow.Core.Areas.Returns;
using Flow.Core.Areas.Utilities;
using Flow.Core.Common.Models;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit.Sdk;
namespace Flow.Core.Tests.Unit.Areas.Utilities;


public class FlowHandlerTests
{

    private Flow<T> ExceptionHandler<T>(Exception exception)

        => exception switch
        {
            DivideByZeroException => new Failure.ApplicationFailure("Converted to failure"),
            _ => new Failure.UnknownFailure(exception.Message),
        };    
    

    [Fact]
    public void try_to_flow_should_execute_the_function_and_return_a_flow_instance()
    {
       var successFlow = FlowHandler.TryToFlow<int>(() => 
                        {
                            return 10;
                        },exception => throw new XunitException("Should not have thrown an exception"));

        using(new AssertionScope())
        {
            successFlow.Match(failure => 0, success => success).Should().Be(10);
            successFlow.Should().Match<Flow<int>>(r => r.IsSuccess == true);
        }
        
    }

    [Fact]
    public void try_to_flow_should_execute_the_function_and_use_the_provided_exception_handler_returning_a_flow_instance()
    {
        var failureMessage = "Division exceptions";

        var failedFlow = FlowHandler.TryToFlow<int>(() =>
        {
            int x = 10;
            return x / 0;
        }, exception => Flow<int>.Failed(new Failure.ApplicationFailure(failureMessage)));

        using (new AssertionScope())
        {
            failedFlow.Match(failure => failure, _ => throw new XunitException("Should not be a success"))
                        .Should().Match<Failure.ApplicationFailure>(f => f.Reason == failureMessage);

            failedFlow.Should().Match<Flow<int>>(r => r.IsFailure == true);
        }

    }

    [Fact]
    public void try_to_flow_should_execute_the_function_and_use_the_provided_exception_handler_method_returning_a_flow_instance()
    {
        var failureMessage = "Converted to failure";//assigned in class level handler

        var failedFlow = FlowHandler.TryToFlow<int>(() =>
        {
            int x = 10;
            return x / 0;
        }, exception => this.ExceptionHandler<int>(exception));

        using (new AssertionScope())
        {
            failedFlow.Match(failure => failure, _ => throw new XunitException("Should not be a success"))
                        .Should().Match<Failure.ApplicationFailure>(f => f.Reason == failureMessage);

            failedFlow.Should().Match<Flow<int>>(r => r.IsFailure == true);
        }

    }


    [Fact]
    public async Task try_to_flow_should_execute_the_async_function_and_return_a_flow_instance()
    {
        var successFlow = await FlowHandler.TryToFlow<int>(async () =>
        {
            return await Task.FromResult(10);
        }, exception => throw new XunitException("Should not have thrown an exception"));

        using (new AssertionScope())
        {
            successFlow.Match(failure => 0, success => success).Should().Be(10);
            successFlow.Should().Match<Flow<int>>(r => r.IsSuccess == true);
        }

    }

    [Fact]
    public async Task try_to_flow_should_execute_the_async_function_and_use_the_provided_exception_handler_returning_a_flow_instance()
    {
        var failureMessage = "Division exceptions";

        var failedFlow = await FlowHandler.TryToFlow<int>(async () =>
        {
            int x = 10;
            return await Task.FromResult(x / 0);
        }, exception => Flow<int>.Failed(new Failure.ApplicationFailure(failureMessage)));

        using (new AssertionScope())
        {
            failedFlow.Match(failure => failure, _ => throw new XunitException("Should not be a success"))
                        .Should().Match<Failure.ApplicationFailure>(f => f.Reason == failureMessage);

            failedFlow.Should().Match<Flow<int>>(r => r.IsFailure == true);
        }

    }


    [Fact]
    public async Task try_to_flow_should_execute_the_async_function_and_return_an_explicit_flow_instance()
    {
        var successFlow = await FlowHandler.TryToFlow<int>(async () =>
        {
            return await Task.FromResult(10);
        }, exception => throw new XunitException("Should not have thrown an exception"));

        using (new AssertionScope())
        {
            successFlow.Match(failure => 0, success => success).Should().Be(10);
            successFlow.Should().Match<Flow<int>>(r => r.IsSuccess == true);
        }

    }

    [Fact]
    public async Task try_to_flow_should_execute_the_async_function_with_the_explicit_flow_but_use_the_provided_exception_handler_to_return_a_flow_instance()
    {
        var failureMessage = "Division exceptions";

        var failedFlow = await FlowHandler.TryToFlow<int>(async () =>
        {
            int x = 10;
            return await Task.FromResult(Flow<int>.Success(x / 0));
        }, exception => Flow<int>.Failed(new Failure.ApplicationFailure(failureMessage)));

        using (new AssertionScope())
        {
            failedFlow.Match(failure => failure, _ => throw new XunitException("Should not be a success"))
                        .Should().Match<Failure.ApplicationFailure>(f => f.Reason == failureMessage);

            failedFlow.Should().Match<Flow<int>>(r => r.IsFailure == true);
        }

    }


}
