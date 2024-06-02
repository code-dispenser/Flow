using Flow.Core.Areas.Extensions;
using Flow.Core.Areas.Returns;
using FluentAssertions;
using FluentAssertions.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace Flow.Core.Tests.Functional.Areas.Extensions;

public class Flow_ExtensionTests_Then
{
    [Fact]
    public void The_extension_should_allow_a_type_to_execute_a_function_that_can_use_the_type_value_and_return_a_flow()
    {
        var successFlow = (5 + 5).Then(x => Flow<string>.Success(x.ToString()));

        using (new AssertionScope())
        {
            successFlow.Should().Match<Flow<string>>(r => r.IsSuccess == true && r.IsFailure == false);

            successFlow.Match(failure => throw new XunitException("Should not be a failure"), success => success.Should().Be("10"));

        }
    }

    [Fact]
    public async Task The_extension_should_allow_a_type_to_execute_an_async_function_that_can_use_the_type_value_and_return_a_flow()
    {
        var successFlow = await Task.FromResult(5 + 5).Then(x => Task.FromResult(Flow<string>.Success(x.ToString())));

        using (new AssertionScope())
        {
            successFlow.Should().Match<Flow<string>>(r => r.IsSuccess == true && r.IsFailure == false);

            successFlow.Match(failure => throw new XunitException("Should not be a failure"), success => success.Should().Be("10"));

        }
    }
}
