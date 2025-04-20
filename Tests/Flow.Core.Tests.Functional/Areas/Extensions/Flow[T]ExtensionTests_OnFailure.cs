using Flow.Core.Areas.Extensions;
using Flow.Core.Areas.Returns;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit.Sdk;

namespace Flow.Core.Tests.Functional.Areas.Extensions;

public class Flow_ExtensionTests_OnFailure
{
    [Fact]
    public void A_failed_flow_should_invoke_the_on_failure_func_and_return_its_flow()

        => Flow<int>.Failed(new Failure.ApplicationFailure("Some failure"))
                        .OnFailure(failure => Flow<int>.Success(42))
                        .Match(failure => throw new XunitException("Should not be a failure as we failed and then ran another method which succeeded"),
                               success => success.Should().Be(42));

    [Fact]
    public void A_successful_flow_should_not_invoke_the_On_failure_func_and_should_only_return_the_current_flow_instance()
    {
        var successFlow = Flow<int>.Success(42)
                         .OnFailure(failure => Flow<int>.Success(84));

        successFlow.Should().BeOfType<Flow<int>>();
        successFlow.Match(failure => throw new XunitException("Should not be a failure"), success => success.Should().Be(42));

    }  

    [Fact]
    public void A_failed_flow_should_invoke_the_on_failure_action_and_return_the_current_flow_instance()
    {

        var failedFlow = Flow<int>.Failed(new Failure.ApplicationFailure("Some failure"))
                                     .OnFailure(failure => failure.Should().BeOfType<Failure.ApplicationFailure>());

        failedFlow.Should().Match<Flow<int>>(f => f.IsFailure == true);

    }

    [Fact]
    public void A_successful_flow_should_just_return_the_current_flow_instance_without_invoking_the_action()
    {
        var successFlow = Flow<int>.Success(42)
                                    .OnFailure(act_onFailure: failure => throw new XunitException("Should not be a failure"));

        successFlow.Should().Match<Flow<int>>(r => r.IsSuccess == true);
    }


    [Fact]
    public void Should_be_able_to_transform_the_failure_value_if_its_a_failed_flow()
    {
        var failure = Flow<int>.Failed(new Failure.ApplicationFailure("Some failure"))
                                        .OnFailure(_ => new Failure.CacheFailure("Cache failure"))
                                        .Finally(failure => failure, success => new Failure.ApplicationFailure("Some failure"));

        failure.Should().BeOfType<Failure.CacheFailure>();

    }
    [Fact]
    public void Should_not_be_able_to_transform_the_failure_value_if_its_a_successful_flow()
    {
        var successFlow = Flow<int>.Success(42)
                                .OnFailure(_ => new Failure.CacheFailure("Cache failure"));

        successFlow.Should().Match<Flow<int>>(r => r.IsSuccess == true);

    }



    [Fact]
    public async Task A_failed_flow_task_should_invoke_the_on_failure_action_and_return_the_current_flow_instance()
    {
        var expectedFailure = new Failure.ApplicationFailure("Some failure");

        var awaitedFlow = await Task.FromResult(Flow<int>.Failed(expectedFailure))
                                        .OnFailure(failure =>
                                        {
                                            failure.Should().Be(expectedFailure);
                                        });

        awaitedFlow.Match(failure => failure, success => throw new XunitException("Should not be a success"))
                        .Should().Be(expectedFailure);
    }

    [Fact]
    public async Task A_successful_flow_task_should_not_invoke_the_on_failure_action_and_should_return_the_current_flow_instance()
    {
        var successValue = 42;

        var awaitedFlow = await Task.FromResult(Flow<int>.Success(successValue))
                                        .OnFailure(_ => successValue = 0);
 

        awaitedFlow.Match(failure => throw new XunitException("Should not be a failure"), success => success)
                        .Should().Be(successValue);
    }

    [Fact]
    public async Task A_success_flow_task_should_not_invoke_the_on_failure_action_and_only_return_the_current_flow_instance()
    {
        var successValue = 42;

        var awaitedFlow = await Task.FromResult(Flow<int>.Success(42))
                                        .OnFailure(act_onFailure: failure =>
                                        {
                                            throw new XunitException("Should not be a failure");
                                        });

        awaitedFlow.Match(failure => throw new XunitException("Should not be a success"), success => success)
                        .Should().Be(successValue);
    }

    [Fact]
    public async Task A_failed_flow_task_should_invoke_the_on_failure_task_and_return_the_current_flow_instance()
    {
        var expectedFailure = new Failure.ApplicationFailure("Some failure");

        var awaitedFlow = await Task.FromResult(Flow<int>.Failed(expectedFailure))
                                        .OnFailure(async failure =>
                                        {
                                            (await Task.FromResult(failure)).Should().Be(expectedFailure);
                                        });

        awaitedFlow.Match(failure => failure, success => throw new XunitException("Should not be a success"))
                        .Should().Be(expectedFailure);
    }

    [Fact]
    public async Task A_failed_flow_task_should_not_invoke_the_on_failure_task_if_its_a_successful_flow()
    {
        var awaitedFlow = await Task.FromResult(Flow<int>.Success(42))
                                        .OnFailure(async failure =>
                                        {
                                            throw new XunitException("Should not be a failure");
                                        });

        awaitedFlow.Should().Match<Flow<int>>(f => f.IsSuccess);
    }

    [Fact]
    public async Task Should_not_be_able_to_transform_the_failure_value_if_its_a_successful_flow_task()
    {
        var awaitedFlow = await Task.FromResult(Flow<int>.Success(42)).OnFailure(f => new Failure.ApplicationFailure("Some Failure"));

        awaitedFlow.Should().Match<Flow<int>>(f => f.IsSuccess);
    }

    [Fact]
    public async Task Should_be_able_to_transform_a_failure_if_its_a_failed_task()
    {

        var awaitedFlow = await Task.FromResult(Flow<int>.Failed(new Failure.ApplicationFailure("Some failure")))
                                           .OnFailure(failure => new Failure.CacheFailure("Cache failure"));

        var awaitedFlowValue = awaitedFlow.Finally(failure => failure, sucess => new Failure.ApplicationFailure("Some failure"));

        using(new AssertionScope())
        {
            awaitedFlow.Should().Match<Flow<int>>(f => f.IsFailure);
            awaitedFlowValue.Should().BeOfType<Failure.CacheFailure>();
        }


    }
}
