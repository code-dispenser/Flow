using Flow.Core.Areas.Returns;
using Flow.Core.Common.Models;
using Flow.Core.Tests.SharedDataAndFixtures.Common.Models;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit.Sdk;

namespace Flow.Core.Tests.Unit.Areas.Returns;

public class FlowTests
{
    [Fact]
    public void The_static_success_method_should_create_a_successful_flow_with_a_value_if_used()
    {
        Flow<int> successFlow = Flow<int>.Success(42);

        using(new AssertionScope())
        {
            successFlow.Should().Match<Flow<int>>(s => s.IsSuccess == true && s.IsFailure == false);
            successFlow.Match(failure => throw new XunitException("Should not be a failure") , success => success).Should().Be(42);
        }
  
    }
    [Fact]
    public void The_static_success_method_using_none_should_create_a_successful_flow_without_specifying_none_value()
    {
        Flow<None> successFlow = Flow<None>.Success();

        using (new AssertionScope())
        {
            successFlow.Should().Match<Flow<None>>(s => s.IsSuccess == true && s.IsFailure == false);
            successFlow.Match(failure => throw new XunitException("Should not be a failure"), success => success).Should().Be(None.Value);
        }

    }
    [Fact]
    public void The_static_fail_method_should_create_a_failed_flow_with_a_type_of_failure()
    {
        Flow<None> failedFlow = Flow<None>.Failed(new Failure.ApplicationFailure("Some failure"));

        using (new AssertionScope())
        {
            failedFlow.Should().Match<Flow<None>>(f => f.IsSuccess == false && f.IsFailure == true);
            failedFlow.Match(failure => failure, success => throw new ArgumentException("Should not be a success"))
                       .Should().BeOfType<Failure.ApplicationFailure>();

        }
    }

    [Fact]
    public void Should_throw_argument_null_exception_creating_a_failed_flow_with_a_null_failure()
        
        => FluentActions.Invoking(() => Flow<None>.Failed(null!)).Should().ThrowExactly<ArgumentNullException>();

    [Fact]
    public void Should_throw_argument_null_exception_creating_a_successful_flow_with_a_null_value()

        => FluentActions.Invoking(() => Flow<string>.Success(null!)).Should().ThrowExactly<ArgumentException>();

    [Fact]
    public void Should_throw_argument_exception_creating_a_successful_flow_with_a_value_of_type_failure()

        => FluentActions.Invoking(() => Flow<Failure>.Success(new Failure.ApplicationFailure("Some failure"))).Should().ThrowExactly<ArgumentException>();

    [Fact]
    public void Implicit_conversion_with_a_failure_should_create_a_failed_flow_with_the_failure_assigned()
    {
        Flow<None> failedFlow = new Failure.ApplicationFailure("Some failure");

        failedFlow.Match(failure => failure, success => throw new XunitException("Should not be a success"))
                   .Should().BeOfType<Failure.ApplicationFailure>();
                   
    }

    [Fact]
    public void Implicit_conversion_with_a_non_failure_should_create_a_successful_flow_with_the_value_assigned()
    {
        Flow<int> successfulFlow = 42;

        successfulFlow.Match(failure => throw new XunitException("Should not be a failure."), success => success).Should().Be(42);
    }

    [Fact]
    public void Implicit_conversion_with_a_null_should_create_a_failed_flow_of_type_conversion_failure()
    {
        Flow<string> implicitFlow = ConvertImplicitly(null!);

        static Flow<string> ConvertImplicitly(string value) => value;

        using(new AssertionScope())
        {
            implicitFlow.Should().Match<Flow<string>>(f => f.IsSuccess == false && f.IsFailure == true);
            implicitFlow.Match(failure => failure, success => throw new XunitException("Should not be a success"))
                           .Should().BeOfType<Failure.ConversionFailure>();
        }
    }

    [Fact]
    public void The_internal_json_constructor_should_set_all_properties_correctly()
    {
        var successfulFlow  = new Flow<decimal>(42.0M, Failure.CreateNoFailure(), true);
        var failedFlow      = new Flow<string>(null, new Failure.ApplicationFailure("Some failure"), false);

        using (new AssertionScope())
        {
            successfulFlow.Should().Match<Flow<decimal>>(s => s.IsSuccess == true && s.IsFailure == false);
            successfulFlow.Match(failure => 0, success => success).Should().Be(42);

            failedFlow.Should().Match<Flow<string>>(f => f.IsSuccess == false && f.IsFailure == true);
            failedFlow.Match(failure => failure, success => throw new ArgumentException("Should not be a success"))
                       .Should().BeOfType<Failure.ApplicationFailure>();
        }

    }

    [Fact]
    public async Task Match_should_return_a_failing_task_when_its_a_failed_flow()
    {
        var failedFlow = Flow<int>.Failed(new Failure.ApplicationFailure("Some failure"));

        var result = await failedFlow.Match(onFailure: _ => Task.FromResult(-1), _ => throw new XunitException("Should not be a success"));

        result.Should().Be(-1);
    }
    [Fact]
    public async Task Match_should_return_a_successful_task_when_its_a_successful_flow()
    {
        var successValue = 42;
        var successFlow  = Flow<int>.Success(successValue);

        var result = await successFlow.Match(onFailure:_ => throw new XunitException("Should not be a failure"), success => Task.FromResult(success));

        result.Should().Be(successValue);
    }

    [Fact]
    public void Match_should_invoke_the_success_action_when_its_a_successful_flow()
    {
        var successValue    = 42;
        var successFlow     = Flow<int>.Success(successValue);
        var capturedValue   = 0;

        successFlow.Match(failure => { }, success => capturedValue = success);
        
        capturedValue.Should().Be(successValue);
    }

    [Fact]
    public void Match_should_invoke_the_failed_action_when_its_a_failed_flow()
    {
        var failureMessage      = "Some failure";
        var failedFlow          = Flow<int>.Failed(new Failure.ApplicationFailure(failureMessage));
        string capturedMessage  = String.Empty;

        failedFlow.Match(FailedAction, _ => throw new XunitException("Should not be a success"));

        void FailedAction(Failure failure) => capturedMessage = failure.Reason;

        capturedMessage.Should().Be(failureMessage);
    }

    [Fact]
    public async Task Match_should_invoke_the_failed_async_action_when_its_a_failed_flow()
    {

        var failureMessage      = "Some failure";
        var failedFlow          = Flow<int>.Failed(new Failure.ApplicationFailure(failureMessage));
        string capturedMessage  = String.Empty;

        await failedFlow.Match(FailureAction, _ => throw new XunitException("Should not be a success"));

        async Task FailureAction(Failure failure)
        {
            capturedMessage = failure.Reason;
            await Task.CompletedTask;
        }

        capturedMessage.Should().Be(failureMessage);
    }

    [Fact]
    public async Task Match_should_invoke_the_successful_async_action_when_its_a_successful_flow()
    {
        var successValue   = 42;
        var successFlow    = Flow<int>.Success(successValue);
        int capturedValue  = 0;

        await successFlow.Match(_ => throw new XunitException("Should not be a failure"), SuccessfulAction);

        async Task SuccessfulAction(int successValue)
        {
            capturedValue = successValue;
            await Task.CompletedTask;
        }

        capturedValue.Should().Be(successValue);
    }

    [Fact]
    public void Map_should_transform_the_success_value_and_return_a_successful_flow_with_the_transformed_value_when_its_a_successful_flow()
    {
        var successValue = 42M;
        var successFlow  = Flow<decimal>.Success(successValue);

        var flowResult = successFlow.Map(success => success * 0.25M);

        var outputValue = flowResult.Match(_ => throw new XunitException("Should not be a failure"), success => success);

        using(new AssertionScope())
        {
            flowResult.Should().BeOfType<Flow<decimal>>();
            outputValue.Should().Be(10.5M);
        }
    }
    [Fact]
    public void Map_should_return_the_failed_flow_when_its_a_failed_flow()
    {
        var failureMessage  = "Some failure";
        var failedFlow      = Flow<decimal>.Failed(new Failure.ApplicationFailure(failureMessage));

        var flowResult = failedFlow.Map(success => success * 0.25M);

        var outputValue = flowResult.Match(failure => failure.Reason, _ => throw new XunitException("Should not be a failure"));

        using (new AssertionScope())
        {
            flowResult.Should().BeOfType<Flow<decimal>>();
            outputValue.Should().Be(failureMessage);
        }
    }

    [Fact]
    public void Map_should_transform_the_success_value_and_return_a_successful_flow_when_its_a_successful_flow()
    {
        var successValue = 42M;
        var successFlow = Flow<decimal>.Success(successValue);

        var flowResult = successFlow.Map(failure => throw new XunitException("Should not be a failure"), success => success * 0.25M);

        var outputValue = flowResult.Match(_ => throw new XunitException("Should not be a failure"), success => success);

        using (new AssertionScope())
        {
            flowResult.Should().BeOfType<Flow<decimal>>();
            outputValue.Should().Be(10.5M);
        }
    }

    [Fact]
    public void Map_should_transform_the_failure_and_return_a_failed_flow_when_its_a_failed_flow()
    {
        var failureMessage = "Some failure";
        var failedFlow = Flow<decimal>.Failed(new Failure.ApplicationFailure("App failure"));

        failedFlow.Map(failure => new Failure.CloudStorageFailure(failureMessage), success => success * 0.25M)
                  .Match(failure =>
                        {
                            using (new AssertionScope())
                            {
                                failure.Should().BeOfType<Failure.CloudStorageFailure>();
                                failure.Reason.Should().Be(failureMessage);
                            }

                        }, _ => throw new XunitException("Should not be a success"));
    }

    [Fact]
    public void Bind_should_return_a_successful_flow_with_the_result_of_the_successful_function_when_its_a_successful_flow()
    {
        var successValue    = 42;
        var successFlow     = Flow<int>.Success(successValue);
        var resultFlow      = successFlow.Bind(success => Flow<string>.Success(success.ToString()));

        resultFlow.Match(failure => throw new XunitException("Should not be a failure"),
            success =>
            {
                success.Should().BeOfType<string>();
                success.Should().Be(successValue.ToString());
            }
        );
    }

    [Fact]
    public void Bind_should_return_a_failed_flow_with_the_failure_value_when_its_a_failed_flow()
    {
        var failureMessage  = "Some failure";
        var failedFlow      = Flow<int>.Failed(new Failure.ApplicationFailure(failureMessage));
        var resultFlow      = failedFlow.Bind(success => Flow<int>.Success(success * 2));

        resultFlow.Match(failure =>
        {
            failure.Should().Match<Failure.ApplicationFailure>(f => f.Reason == failureMessage);

        }, success => throw new XunitException("Should not be a success"));
    }

    [Fact]
    public async Task Bind_should_return_a_successful_flow_task_with_the_result_of_the_successful_function_when_its_a_successful_flow()
    {
        var successValue = 42;
        var successFlow  = Flow<int>.Success(successValue);
        var resultFlow   = await successFlow.Bind(success => Task.FromResult(Flow<string>.Success(success.ToString())));

        resultFlow.Match(failure => throw new XunitException("Should not be a failure"),
            success =>
            {
                success.Should().BeOfType<string>();
                success.Should().Be(successValue.ToString());
            }
        );
    }
    [Fact]
    public async Task Bind_should_return_a_failed_flow_task_with_the_failure_value_when_its_a_failed_flow()
    {
        var failureMessage  = "Some failure";
        var failedFlow      = Flow<int>.Failed(new Failure.ApplicationFailure(failureMessage));
        var resultFlow      = await failedFlow.Bind(success => Task.FromResult(Flow<int>.Success(success * 2)));

        resultFlow.Match(failure =>
        {
            failure.Should().Match<Failure.ApplicationFailure>(f => f.Reason == failureMessage);

        }, success => throw new XunitException("Should not be a success"));
    }















    [Fact]
    public void BindFailure_should_execute_the_failure_function_when_its_a_failed_flow_returning_the_same_flow_type()
    {
        var failureFlow = Flow<int>.Failed(new Failure.ApplicationFailure("Some failure"));
        var resultFlow  = failureFlow.BindFailure(failure => Flow<int>.Success(42));

        using(new AssertionScope())
        {
            resultFlow.Should().BeOfType(failureFlow.GetType());

            resultFlow.Match(failure => throw new XunitException("Should not be a failure"), success => success).Should().Be(42);
        }
        
    }
    [Fact]
    public void BindFailure_should_not_execute_the_failure_function_when_its_a_success_flow_and_should_return_the_current_flow()
    {
        var successFlow = Flow<int>.Success(42);
        var resultFlow = successFlow.BindFailure(failure => Flow<int>.Success(24));

        using (new AssertionScope())
        {
            resultFlow.Should().BeOfType(successFlow.GetType());

            resultFlow.Match(failure => throw new XunitException("Should not be a failure"), success => success).Should().Be(42);
        }

    }


    [Fact]
    public async Task BindFailure_should_execute_the_failure_task_function_when_its_a_failed_flow_returning_the_same_flow_type()
    {
        var failureFlow = Flow<int>.Failed(new Failure.ApplicationFailure("Some failure"));
        var resultFlow  = await failureFlow.BindFailure(failure => Task.FromResult(Flow<int>.Success(42)));

        using (new AssertionScope())
        {
            resultFlow.Should().BeOfType(failureFlow.GetType());

            resultFlow.Match(failure => throw new XunitException("Should not be a failure"), success => success).Should().Be(42);
        }

    }
    [Fact]
    public async Task BindFailure_should_not_execute_the_failure_task_function_when_its_a_success_flow_and_should_return_the_current_flow()
    {
        var successFlow = Flow<int>.Success(42);
        var resultFlow = await successFlow.BindFailure(async failure => await Task.FromResult(Flow<int>.Success(24)));

        using (new AssertionScope())
        {
            resultFlow.Should().BeOfType(successFlow.GetType());

            resultFlow.Match(failure => throw new XunitException("Should not be a failure"), success => success).Should().Be(42);
        }

    }
}
