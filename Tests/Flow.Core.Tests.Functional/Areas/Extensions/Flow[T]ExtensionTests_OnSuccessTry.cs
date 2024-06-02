﻿using Flow.Core.Areas.Extensions;
using Flow.Core.Areas.Returns;
using Flow.Core.Common.Models;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit.Sdk;

namespace Flow.Core.Tests.Functional.Areas.Extensions;

public class Flow_ExtensionTests_OnSuccessTry
{
    private Flow<T> ExceptionHandler<T>(Exception exception)

    => exception switch
    {
        DivideByZeroException => new Failure.ApplicationFailure("Converted to failure"),
        _ => new Failure.UnknownFailure(exception.Message),
    };

    [Fact]
    public async Task On_success_try_to_flow_should_execute_the_async_function_and_return_an_explicit_flow_instance()
    {
        var successFlow = await Task.FromResult(Flow<None>.Success())
                                        .OnSuccessTry(async (_) =>
                                        {
                                            return await Task.FromResult(Flow<int>.Success(42));//can also use implicit conversion.
                                        }, exception => this.ExceptionHandler<int>(exception));

        using (new AssertionScope())
        {
            successFlow.Match(failure => 0, success => success).Should().Be(42);
            successFlow.Should().Match<Flow<int>>(r => r.IsSuccess == true);
        }
    }

    [Fact]
    public async Task On_success_try_to_flow_should_execute_the_async_function_with_the_explicit_flow_but_use_the_provided_exception_handler_to_return_a_flow_instance()
    {
        var failureMessage = "Converted to failure";//assigned in class level handler

        var failedFlow = await Task.FromResult(Flow<None>.Success())
                                .OnSuccessTry(async (_) =>
                                {
                                    int x = 10;
                                    return await Task.FromResult(Flow<int>.Success(x/0));
                                }, exception => this.ExceptionHandler<int>(exception));

        using (new AssertionScope())
        {
            failedFlow.Match(failure => failure, _ => throw new XunitException("Should not be a success"))
                        .Should().Match<Failure.ApplicationFailure>(f => f.Reason == failureMessage);

            failedFlow.Should().Match<Flow<int>>(r => r.IsFailure == true);
        }

    }

    [Fact]
    public void On_success_try_to_flow_should_execute_the_function_and_return_an_explicit_flow_instance()
    {
        var successFlow =  Flow<None>.Success()
                                     .OnSuccessTry(_ => Flow<int>.Success(42), exception => this.ExceptionHandler<int>(exception));
  
        using (new AssertionScope())
        {
            successFlow.Match(failure => 0, success => success).Should().Be(42);
            successFlow.Should().Match<Flow<int>>(r => r.IsSuccess == true);
        }
    }

    [Fact]
    public void On_success_try_to_flow_should_execute_the_function_with_the_explicit_flow_but_use_the_provided_exception_handler_to_return_a_flow_instance()
    {
        var failureMessage = "Converted to failure";//assigned in class level handler

        var failedFlow = Flow<None>.Success()
                                .OnSuccessTry(_ =>
                                {
                                    int x = 10;
                                    return Flow<int>.Success(x/0);
                                }, exception => this.ExceptionHandler<int>(exception));

        using (new AssertionScope())
        {
            failedFlow.Match(failure => failure, _ => throw new XunitException("Should not be a success"))
                        .Should().Match<Failure.ApplicationFailure>(f => f.Reason == failureMessage);

            failedFlow.Should().Match<Flow<int>>(r => r.IsFailure == true);
        }

    }
}
