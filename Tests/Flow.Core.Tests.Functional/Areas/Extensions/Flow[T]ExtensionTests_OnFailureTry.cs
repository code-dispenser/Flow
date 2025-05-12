using Flow.Core.Areas.Extensions;
using Flow.Core.Areas.Returns;
using Flow.Core.Common.Models;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit.Sdk;

namespace Flow.Core.Tests.Functional.Areas.Extensions;

public class Flow_ExtensionTests_OnFailureTry
{
    private static Flow<T> ExceptionHandler<T>(Exception exception)

        => exception switch
        {
            DivideByZeroException => new Failure.ApplicationFailure("Converted to failure"),
            _ => new Failure.UnknownFailure(exception.Message),
        };

    [Fact]
    public void On_failure_try_should_execute_the_function_and_return_an_explicit_flow_instance_of_the_same_type()
    {
        var returnedFlow = Flow<int>.Failed(new Failure.ApplicationFailure("Bad stuff happened"))
                                     .OnFailureTry(failure => Flow<int>.Success(42), ex => ExceptionHandler<int>(ex));//Being explicit but flow has implicit operators so did not need to wrap 42 in a flow

        using (new AssertionScope())
        {
            returnedFlow.Should().Match<Flow<int>>(r => r.IsSuccess == true);
            returnedFlow.Match(failure => -1, success => success).Should().Be(42);

        }
        
    }

    [Fact]
    public void On_failure_try_should_not_execute_the_function_when_the_current_flow_is_a_success()
    {
        var returnedFlow = Flow<int>.Success(42)
                                    .OnFailureTry(failure => Flow<int>.Failed(new Failure.UnknownFailure("Should not be here")), ex => ExceptionHandler<int>(ex));

        using (new AssertionScope())
        {
            returnedFlow.Should().Match<Flow<int>>(r => r.IsSuccess == true);
            returnedFlow.Match(failure => -1, success => success).Should().Be(42);

        }

    }
    [Fact]
    public void On_failure_try_should_execute_the_function_and_use_the_provided_exception_handler_to_return_a_flow_instance()
    {
        var returnedFlow = Flow<int>.Failed(new Failure.ApplicationFailure("Bad stuff happened"))
                                     .OnFailureTry(failure => throw new DivideByZeroException(), ex => ExceptionHandler<int>(ex));

        using (new AssertionScope())
        {
            returnedFlow.Should().Match<Flow<int>>(r => r.IsSuccess == false);
            returnedFlow.Match(failure => failure, success => new Failure.UnknownFailure("Should not be here")).Should().BeOfType<Failure.ApplicationFailure>();

        }

    }

    [Fact]
    public async Task On_failure_try_should_execute_the_async_function_and_return_an_explicit_flow_instance_of_the_same_type()
    {
        var returnedFlow = await Task.FromResult(Flow<int>.Failed(new Failure.ApplicationFailure("Bad stuff happened")))
                                     .OnFailureTry(failure => Task.FromResult(Flow<int>.Success(42)), ex => ExceptionHandler<int>(ex));

        using (new AssertionScope())
        {
            returnedFlow.Should().Match<Flow<int>>(r => r.IsSuccess == true);
            returnedFlow.Match(failure => -1, success => success).Should().Be(42);

        }

    }


    [Fact]
    public async Task On_failure_try_should_not_execute_the_async_function_when_the_current_flow_is_a_success()
    {
        var returnedFlow = await Task.FromResult(Flow<int>.Success(42))
                                    .OnFailureTry(failure =>
                                    {
                                        int x = 1, y = 0;
                                        return Task.FromResult(Flow<int>.Success(x/y));
                                    }
                                    , ex => ExceptionHandler<int>(ex));

        using (new AssertionScope())
        {
            returnedFlow.Should().Match<Flow<int>>(r => r.IsSuccess == true);
            returnedFlow.Match(failure => -1, success => success).Should().Be(42);

        }

    }

    [Fact]
    public async Task On_failure_try_should_execute_the_async_function_and_use_the_provided_exception_handler_to_return_a_flow_instance()
    {
        var returnedFlow = await Task.FromResult(Flow<int>.Failed(new Failure.ApplicationFailure("Bad stuff happened")))
                                     .OnFailureTry(async (failure) => 
                                        {
                                            int x = 1;
                                            int y = 0;
                                            return await Task.FromResult(Flow<int>.Success(x/y));
                                         }, ex => ExceptionHandler<int>(ex));

        using (new AssertionScope())
        {
            returnedFlow.Should().Match<Flow<int>>(r => r.IsSuccess == false);
            returnedFlow.Match(failure => failure, success => new Failure.UnknownFailure("Should not be here")).Should().BeOfType<Failure.ApplicationFailure>();

        }

    }


    [Fact]
    public async Task On_failure_try_should_execute_the_async_non_flow_function_and_return_an_explicit_flow_instance_of_the_same_type()
    {
        var returnedFlow = await Task.FromResult(Flow<int>.Failed(new Failure.ApplicationFailure("Bad stuff happened")))
                                     .OnFailureTry(failure => Task.FromResult(42), ex => ExceptionHandler<int>(ex));

        using (new AssertionScope())
        {
            returnedFlow.Should().Match<Flow<int>>(r => r.IsSuccess == true);
            returnedFlow.Match(failure => -1, success => success).Should().Be(42);

        }

    }


    [Fact]
    public async Task On_failure_try_should_not_execute_the_async_non_flow_function_when_the_current_flow_is_a_success()
    {
        var returnedFlow = await Task.FromResult(Flow<int>.Success(42))
                                    .OnFailureTry(failure =>
                                    {
                                        int x = 1, y = 0;
                                        return Task.FromResult(x/y);
                                    }
                                    , ex => ExceptionHandler<int>(ex));

        using (new AssertionScope())
        {
            returnedFlow.Should().Match<Flow<int>>(r => r.IsSuccess == true);
            returnedFlow.Match(failure => -1, success => success).Should().Be(42);

        }

    }


    [Fact]
    public async Task On_failure_try_should_execute_the_async_non_flow_function_and_use_the_provided_exception_handler_to_return_a_flow_instance()
    {
        var returnedFlow = await Task.FromResult(Flow<int>.Failed(new Failure.ApplicationFailure("Bad stuff happened")))
                                     .OnFailureTry(async (failure) =>
                                     {
                                         int x = 1;
                                         int y = 0;
                                         return await Task.FromResult(x / y);
                                     }, ex => ExceptionHandler<int>(ex));

        using (new AssertionScope())
        {
            returnedFlow.Should().Match<Flow<int>>(r => r.IsSuccess == false);
            returnedFlow.Match(failure => failure, success => new Failure.UnknownFailure("Should not be here")).Should().BeOfType<Failure.ApplicationFailure>();

        }

    }



}
