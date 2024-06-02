using Flow.Core.Areas.Extensions;
using Flow.Core.Areas.Returns;
using FluentAssertions;

namespace Flow.Core.Tests.Functional.Areas.Extensions;

public class Flow_ExtensionTests_Finally
{ 
    [Fact]
    public void Finally_should_return_the_success_value_if_the_flow_was_successful()
    {
        var finalValue = Flow<decimal>.Success(10.25M)
                                            .Finally(failure => 0.00M, success => success);

        finalValue.Should().Be(10.25M);
    }
    [Fact]
    public void Finally_should_return_the_desired_failure_value_if_the_flow_was_a_failure()
    {
        var finalValue = Flow<decimal>.Failed(new Failure.ApplicationFailure("Some failure"))
                                            .Finally(f => 1.00M, success => success);

        finalValue.Should().Be(1.00M);
    }

    [Fact]
    public async Task Finally_should_return_the_async_success_value_if_the_flow_was_successful()
    {
        var finalValue = await Task.FromResult(Flow<decimal>.Success(10.25M))
                                            .Finally(failure => 0.00M, success => success);

        finalValue.Should().Be(10.25M);
    }
    [Fact]
    public async Task Finally_should_return_the_desired_async_failure_value_if_the_flow_was_a_failure()
    {
        var finalValue = await Task.FromResult(Flow<decimal>.Failed(new Failure.ApplicationFailure("Some failure")))
                                            .Finally(f => 1.00M, success => success);

        finalValue.Should().Be(1.00M);
    }

}
