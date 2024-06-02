using Flow.Core.Areas.Returns;
using Flow.Core.Areas.Extensions;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit.Sdk;

namespace Flow.Core.Tests.Functional.Areas.Extensions;

public class Flow_ExtensionTests_ReturnAs
{
    [Fact]
    public async Task Return_as_should_transform_the_success_value_and_return_a_successful_flow_when_its_a_successful_flow()
    {
        var successValue = 42M;
        var successFlow = await Task.FromResult(Flow<decimal>.Success(successValue))
                                  .ReturnAs(failure => throw new XunitException("Should not be a failure"), success => (int)success + 8);

        using (new AssertionScope())
        {
            successFlow.Match(failure => throw new XunitException("Should not be a failure"), success => success).Should().Be(50);
            successFlow.Should().BeOfType<Flow<int>>();
        }
      
    }

    [Fact]
    public async Task Return_as_task_should_transform_the_failure_and_return_a_failed_flow_when_its_a_failed_flow()
    {
        var failureMessage = "Some failure";
        var failedFlow = await Task.FromResult(Flow<decimal>.Failed(new Failure.ApplicationFailure("App failure")))
                                .ReturnAs(failure => new Failure.CloudStorageFailure(failureMessage), success => success * 0.25M);

        failedFlow.Match(failure =>
                  {
                      using (new AssertionScope())
                      {
                          failure.Should().BeOfType<Failure.CloudStorageFailure>();
                          failure.Reason.Should().Be(failureMessage);
                      }

                  }, _ => throw new XunitException("Should not be a success"));
    }


    [Fact]
    public async Task Return_as_task_without_the_failure_func_should_transform_the_success_value_and_return_a_successful_flow_when_its_a_successful_flow()
    {
        var successValue = 42M;
        var successFlow = await Task.FromResult(Flow<decimal>.Success(successValue))
                                  .ReturnAs(success => (int)success + 8);

        using (new AssertionScope())
        {
            successFlow.Match(failure => throw new XunitException("Should not be a failure"), success => success).Should().Be(50);
            successFlow.Should().BeOfType<Flow<int>>();
        }

    }

    [Fact]
    public async Task Return_as_task_should_just_return_the_failed_flow_when_its_a_failed_flow()
    {
        var failureMessage = "Some failure";
        var failedFlow = await Task.FromResult(Flow<decimal>.Failed(new Failure.ApplicationFailure(failureMessage)))
                                .ReturnAs(success => success * 0.25M);

        failedFlow.Match(failure =>
        {
            using (new AssertionScope())
            {
                failure.Should().BeOfType<Failure.ApplicationFailure>();
                failure.Reason.Should().Be(failureMessage);
            }

        }, _ => throw new XunitException("Should not be a success"));
    }


    [Fact]
    public void Return_as_should_transform_the_failure_and_return_a_failed_flow_when_its_a_failed_flow()
    {
        var failureMessage = "Some failure";
        var failedFlow = Flow<decimal>.Failed(new Failure.ApplicationFailure("App failure"))
                                .ReturnAs(failure => new Failure.CloudStorageFailure(failureMessage), success => success * 0.25M);

        failedFlow.Match(failure =>
        {
            using (new AssertionScope())
            {
                failure.Should().BeOfType<Failure.CloudStorageFailure>();
                failure.Reason.Should().Be(failureMessage);
            }

        }, _ => throw new XunitException("Should not be a success"));
    }






}
