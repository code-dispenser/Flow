using Flow.Core.Areas.Returns;
using Flow.Core.Tests.SharedDataAndFixtures.Common.Models;
using FluentAssertions;
using FluentAssertions.Execution;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace Flow.Core.Tests.Functional.Areas.Returns;

public class Flow_SerializationTests
{
    [Fact]
    public void Protobuf_net_serializer_should_be_able_to_serialize_and_deserialize_a_failed_flow_correctly()
    {

        var failedFlow          = Flow<Person>.Failed(new Failure.ApplicationFailure("Some failure")); 
        var deserializedFlow    = Serializer.DeepClone<Flow<Person>>(failedFlow);

        using(new AssertionScope())
        {
            deserializedFlow.Should().Match<Flow<Person>>(f => f.IsFailure == true && f.IsSuccess == false);

            deserializedFlow.Match(failure => failure, success => throw new XunitException("Should not be a success"))
                            .Should().Match<Failure.ApplicationFailure>(f => f.Reason == "Some failure" && f.Details.Count == 0);
        }
    }

    [Fact]
    public void Protobuf_net_serializer_should_be_able_to_serialize_and_deserialize_a_successful_flow_correctly()
    {
        Flow<Person> successfulFlow = new Person("John", "Doe", 25);

        var deserializedFlow = Serializer.DeepClone(successfulFlow);

        using (new AssertionScope())
        { 
            deserializedFlow.Should().Match<Flow<Person>>(r => r.IsSuccess == true && r.IsFailure == false);

            deserializedFlow.Match(failure => throw new XunitException("Should not be a failure"), success => success)
                              .Should().Match<Person>(p => p.FirstName == "John" && p.Surname == "Doe"&& p.Age == 25);
        }
    }

    [Fact]
    public void Json_serializer_should_be_able_to_serialize_and_deserialize_a_failed_flow_correctly()
    {
        var failedFlow       = Flow<Person>.Failed(new Failure.ApplicationFailure("Some failure"));
        var jsonString       = JsonSerializer.Serialize(failedFlow);
        var deserializedFlow = JsonSerializer.Deserialize<Flow<Person>>(jsonString)!;

        using (new AssertionScope())
        {
            deserializedFlow.Should().Match<Flow<Person>>(f => f.IsFailure == true && f.IsSuccess == false);

            deserializedFlow.Match(failure => failure, success => throw new XunitException("Should not be a success"))
                            .Should().Match<Failure.ApplicationFailure>(f => f.Reason == "Some failure" && f.Details.Count == 0);
        }
    }

    [Fact]
    public void Json_serializer_should_be_able_to_serialize_and_deserialize_a_successful_flow_correctly()
    {
        Flow<Person> successfulFlow = new Person("John", "Doe", 25);

        var jsonString       = JsonSerializer.Serialize(successfulFlow);
        var deserializedFlow = JsonSerializer.Deserialize<Flow<Person>>(jsonString)!;

        using (new AssertionScope())
        {
            deserializedFlow.Should().Match<Flow<Person>>(r => r.IsSuccess == true && r.IsFailure == false);

            deserializedFlow.Match(failure => throw new XunitException("Should not be a failure"), success => success)
                              .Should().Match<Person>(p => p.FirstName == "John" && p.Surname == "Doe"&& p.Age == 25);
        }
    }
}
