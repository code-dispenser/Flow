using Flow.Core.Areas.Extensions;
using Flow.Core.Areas.Returns;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit.Sdk;

namespace Flow.Core.Tests.Functional.Areas.Extensions;

public class Flow_ExtensionTests_OnSuccess
{
    [Fact]
    public void On_success_should_invoke_the_next_flow_returning_function_if_the_current_flow_is_successful()
   
        =>  Flow<int>.Success(42)
                        .OnSuccess(success => success * 2)
                            .Match(failure => throw new XunitException("Should not be a failure"), success => success)
                                .Should().Be(84);

    [Fact]
    public void On_success_should_return_the_current_flow_instance_if_its_a_failure()

        => Flow<int>.Failed(new Failure.ApplicationFailure("Some failure"))
                        .OnSuccess(success => success * 2)
                            .Match(failure => failure, success => throw new XunitException("Should not be a success"))
                                .Should().Match<Failure.ApplicationFailure>(f => f.Reason == "Some failure");


    [Fact]
    public async Task A_successful_flow_task_should_invoke_the_next_async_flow_returning_function()
    {
        var awaitedFlow = await Task.FromResult(Flow<int>.Success(42))
                                            .OnSuccess(success => Task.FromResult(Flow<int>.Success(success * 2)));

        awaitedFlow.Match(failure => throw new XunitException("Should not be a failure"),success => success)
                        .Should().Be(84);   
    }

    [Fact]
    public async Task A_failed_flow_task_should_return_the_current_async_flow_task_without_invoking_on_success()
    {
        var awaitedFlow = await Task.FromResult(Flow<int>.Failed(new Failure.ApplicationFailure("Some failure")))
                                        .OnSuccess(success => Task.FromResult(Flow<int>.Success(success * 2)));

        awaitedFlow.Match(failure => failure, success => throw new XunitException("Should not be a success"))
                        .Should().Match<Failure.ApplicationFailure>(f => f.Reason == "Some failure");
    }


    [Fact]
    public async Task A_successful_flow_should_invoke_the_next_async_flow_returning_function()
    {
        var awaitedFlow = await Flow<int>.Success(42)
                                            .OnSuccess(success => Task.FromResult(Flow<int>.Success(success * 2)));

        awaitedFlow.Match(failure => throw new XunitException("Should not be a failure"), success => success)
                        .Should().Be(84);
    }

    [Fact]
    public async Task A_failed_flow_should_return_the_current_async_flow_task_without_invoking_on_success()
    {
        var awaitedFlow = await Flow<int>.Failed(new Failure.ApplicationFailure("Some failure"))
                                        .OnSuccess(success => Task.FromResult(Flow<int>.Success(success * 2)));

        awaitedFlow.Match(failure => failure, success => throw new XunitException("Should not be a success"))
                        .Should().Match<Failure.ApplicationFailure>(f => f.Reason == "Some failure");
    }


    [Fact]
    public async Task A_successful_flow_task_should_invoke_the_next_flow_returning_function()
    {
        var awaitedFlow = await Task.FromResult(Flow<int>.Success(42))
                                            .OnSuccess(success => Flow<int>.Success(success * 2));

        awaitedFlow.Match(failure => throw new XunitException("Should not be a failure"), success => success)
                        .Should().Be(84);
    }

    [Fact]
    public async Task A_failed_flow_task_should_return_the_current_flow_task_without_invoking_on_success()
    {
        var awaitedFlow = await Task.FromResult(Flow<int>.Failed(new Failure.ApplicationFailure("Some failure")))
                                        .OnSuccess(success => Flow<int>.Success(success * 2));

        awaitedFlow.Match(failure => failure, success => throw new XunitException("Should not be a success"))
                        .Should().Match<Failure.ApplicationFailure>(f => f.Reason == "Some failure");
    }



    [Fact]
    public async Task A_successful_flow_task_should_invoke_the_on_success_action_and_return_the_current_flow_instance()
    {
        var successValue  = 42;
        var capturedValue = 0;

        var awaitedFlow = await Task.FromResult(Flow<int>.Success(successValue))
                                    .OnSuccess(act_onSuccess: success => capturedValue = success);

        using(new AssertionScope())
        {
            awaitedFlow.Should().Match<Flow<int>>(s => s.IsSuccess == true);
            capturedValue.Should().Be(successValue);
        }
    }

    [Fact]
    public async Task A_failed_flow_task_should_just_return_the_current_flow_instance_without_invoking_the_action()
    {
        var failureMessage = "Some failure";
        var capturedValue = 0;

        var awaitedFlow = await Task.FromResult(Flow<int>.Failed(new Failure.ApplicationFailure(failureMessage)))
                                    .OnSuccess(_ => PerformAction());

        void PerformAction() => capturedValue = 10;

        
        using (new AssertionScope())
        {
            awaitedFlow.Should().Match<Flow<int>>(f => f.IsFailure == true);
            capturedValue.Should().Be(0);
        }
    }

    [Fact]
    public void A_successful_flow_should_invoke_the_on_success_action_and_return_the_current_flow_instance()
    {
        var successValue = 42;
        var capturedValue = 0;

        var successFlow = Flow<int>.Success(successValue) //need to use the statement block so it calls the correct extension to get 100% coverage, result is the same.
                                   .OnSuccess(act_onSuccess: success => {capturedValue = success;});

        using (new AssertionScope())
        {
            successFlow.Should().Match<Flow<int>>(s => s.IsSuccess == true);
            capturedValue.Should().Be(successValue);
        }
    }

    [Fact]
    public void A_failed_flow_should_just_return_the_current_flow_instance_without_invoking_the_action()
    {
        var failureMessage = "Some failure";
        var capturedMessage = String.Empty;

        var failedFlow = Flow<int>.Failed(new Failure.ApplicationFailure(failureMessage))
                                    .OnSuccess(success => throw new XunitException("Should not be a success"));
                                    
        failedFlow.Should().Match<Flow<int>>(f => f.IsFailure == true);
    }


    [Fact]
    public async Task A_successful_flow_should_invoke_the_on_success_async_action_and_return_the_current_flow_instance()
    {
        var successValue = 42;
        var capturedValue = 0;

        var successFlow = await Task.FromResult(Flow<int>.Success(successValue))
                                       .OnSuccess(PerformAction);

        async Task PerformAction(int value)
        {
            capturedValue = value;
            await Task.CompletedTask;
        }

        using (new AssertionScope())
        {
            successFlow.Should().Match<Flow<int>>(s => s.IsSuccess == true);
            capturedValue.Should().Be(successValue);
        }
    }
    [Fact]
    public async Task A_failed_flow_should_just_return_the_current_flow_instances_without_invoking_on_success_async_action()
    {

        var failureMessage = "Some failure";
        var capturedValue = 0;

        var failedFlow = await Task.FromResult(Flow<int>.Failed(new Failure.ApplicationFailure(failureMessage)))
                                    .OnSuccess((_) => PerformAction());

        async Task PerformAction()
        {
            capturedValue = 10;
            await Task.CompletedTask;
        }

        using (new AssertionScope())
        {
            failedFlow.Should().Match<Flow<int>>(f => f.IsFailure == true);
            capturedValue.Should().Be(0);
        }

    }
}
