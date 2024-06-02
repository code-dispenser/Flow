using Flow.Core.Areas.Returns;
using Flow.Core.Tests.SharedDataAndFixtures.Common.CustomFailures;
using Flow.Core.Tests.SharedDataAndFixtures.Common.Models;
using Flow.Core.Tests.SharedDataAndFixtures.Common.Utilities;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Security.AccessControl;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Xunit.Sdk;

namespace Flow.Core.Tests.Functional.Areas.Returns;

public class Failure_SerializationTests_Json
{
    [Theory]
    [InlineData(nameof(Failure.NetworkFailure), "Reason for failure", null, 0, false, null, null)]
    [InlineData(nameof(Failure.NetworkFailure), "  ", "Key:Value", 1, true, "Exception message", "2024-05-17T10:00:00Z")]
    [InlineData(nameof(Failure.DatabaseFailure), "Reason for failure", null, 0, false, null, null)]
    [InlineData(nameof(Failure.DatabaseFailure), "  ", "Key:Value", 1, true, "Exception message", "2024-05-17T10:00:00Z")]
    [InlineData(nameof(Failure.FileSystemFailure), "Reason for failure", null, 0, false, null, null)]
    [InlineData(nameof(Failure.FileSystemFailure), "  ", "Key:Value", 1, true, "Exception message", "2024-05-17T10:00:00Z")]
    [InlineData(nameof(Failure.SecurityFailure), "Reason for failure", null, 0, false, null, null)]
    [InlineData(nameof(Failure.SecurityFailure), "  ", "Key:Value", 1, true, "Exception message", "2024-05-17T10:00:00Z")]
    [InlineData(nameof(Failure.ConnectionFailure), "Reason for failure", null, 0, false, null, null)]
    [InlineData(nameof(Failure.ConnectionFailure), "  ", "Key:Value", 1, true, "Exception message", "2024-05-17T10:00:00Z")]
    [InlineData(nameof(Failure.ValidationFailure), "Reason for failure", null, 0, false, null, null)]
    [InlineData(nameof(Failure.ValidationFailure), "  ", "Key:Value", 1, true, "Exception message", "2024-05-17T10:00:00Z")]
    [InlineData(nameof(Failure.ConfigurationFailure), "Reason for failure", null, 0, false, null, null)]
    [InlineData(nameof(Failure.ConfigurationFailure), "  ", "Key:Value", 1, true, "Exception message", "2024-05-17T10:00:00Z")]
    [InlineData(nameof(Failure.ServiceFailure), "Reason for failure", null, 0, false, null, null)]
    [InlineData(nameof(Failure.ServiceFailure), "  ", "Key:Value", 1, true, "Exception message", "2024-05-17T10:00:00Z")]
    [InlineData(nameof(Failure.CloudStorageFailure), "Reason for failure", null, 0, false, null, null)]
    [InlineData(nameof(Failure.CloudStorageFailure), "  ", "Key:Value", 1, true, "Exception message", "2024-05-17T10:00:00Z")]
    [InlineData(nameof(Failure.ItemNotFoundFailure), "Reason for failure", null, 0, false, null, null)]
    [InlineData(nameof(Failure.ItemNotFoundFailure), "  ", "Key:Value", 1, true, "Exception message", "2024-05-17T10:00:00Z")]
    [InlineData(nameof(Failure.MessagingFailure), "Reason for failure", null, 0, false, null, null)]
    [InlineData(nameof(Failure.MessagingFailure), "  ", "Key:Value", 1, true, "Exception message", "2024-05-17T10:00:00Z")]
    [InlineData(nameof(Failure.GeneralFailure), "Reason for failure", null, 0, false, null, null)]
    [InlineData(nameof(Failure.GeneralFailure), "  ", "Key:Value", 1, true, "Exception message", "2024-05-17T10:00:00Z")]
    [InlineData(nameof(Failure.ConstraintFailure), "Reason for failure", null, 0, false, null, null)]
    [InlineData(nameof(Failure.ConstraintFailure), "  ", "Key:Value", 1, true, "Exception message", "2024-05-17T10:00:00Z")]
    [InlineData(nameof(Failure.DomainFailure), "Reason for failure", null, 0, false, null, null)]
    [InlineData(nameof(Failure.DomainFailure), "  ", "Key:Value", 1, true, "Exception message", "2024-05-17T10:00:00Z")]
    [InlineData(nameof(Failure.ApplicationFailure), "Reason for failure", null, 0, false, null, null)]
    [InlineData(nameof(Failure.ApplicationFailure), "  ", "Key:Value", 1, true, "Exception message", "2024-05-17T10:00:00Z")]
    [InlineData(nameof(Failure.IOFailure), "Reason for failure", null, 0, false, null, null)]
    [InlineData(nameof(Failure.IOFailure), "  ", "Key:Value", 1, true, "Exception message", "2024-05-17T10:00:00Z")]
    [InlineData(nameof(Failure.HardwareFailure), "Reason for failure", null, 0, false, null, null)]
    [InlineData(nameof(Failure.HardwareFailure), "  ", "Key:Value", 1, true, "Exception message", "2024-05-17T10:00:00Z")]
    [InlineData(nameof(Failure.SystemFailure), "Reason for failure", null, 0, false, null, null)]
    [InlineData(nameof(Failure.SystemFailure), "  ", "Key:Value", 1, true, "Exception message", "2024-05-17T10:00:00Z")]
    [InlineData(nameof(Failure.TaskCancellationFailure), "Reason for failure", null, 0, false, null, null)]
    [InlineData(nameof(Failure.TaskCancellationFailure), "  ", "Key:Value", 1, true, "Exception message", "2024-05-17T10:00:00Z")]
    [InlineData(nameof(Failure.InternetConnectionFailure), "Reason for failure", null, 0, false, null, null)]
    [InlineData(nameof(Failure.InternetConnectionFailure), "  ", "Key:Value", 1, true, "Exception message", "2024-05-17T10:00:00Z")]
    [InlineData(nameof(Failure.CacheFailure), "Reason for failure", null, 0, false, null, null)]
    [InlineData(nameof(Failure.CacheFailure), "  ", "Key:Value", 1, true, "Exception message", "2024-05-17T10:00:00Z")]
    [InlineData(nameof(Failure.UnknownFailure), "Reason for failure", null, 0, false, null, null)]
    [InlineData(nameof(Failure.UnknownFailure), "  ", "Key:Value", 1, true, "Exception message", "2024-05-17T10:00:00Z")]
    public void Should_be_able_to_serialize_and_deserialize_all_failure_types_using_the_json_constructor_attribute(string typeName, string? reason, string? details, int subTypeID, bool canRetry, string? exceptionMessage, string? occurredAtString)
    {
        var failureType = Type.GetType(typeof(Failure).AssemblyQualifiedName!.Replace("Failure", String.Concat("Failure+", typeName)))!;
        FailureAssertions(failureType, reason, details, subTypeID, canRetry, exceptionMessage, occurredAtString);
    }

    [Fact]
    public void Should_be_able_to_serialize_and_deserialize_the_no_failure_class_using_the_json_serializer()
    {
        var noFailure = Failure.CreateNoFailure();

        var jsonString = JsonSerializer.Serialize<Failure.NoFailure>(noFailure);

        var deserializedFailure = JsonSerializer.Deserialize<Failure.NoFailure>(jsonString)!;

        using (new AssertionScope())
        {
            deserializedFailure.Should().Match<Failure.NoFailure>(f => f.Reason == "No Failure" && f.Details.Count == 0 && f.Exception == null && f.CanRetry == false);
            deserializedFailure.OccurredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
        }
    }

    private void FailureAssertions(Type failureType, string? reason, string? failureDetails, int subTypeID, bool canRetry, string? exceptionMessage, string? occurredAtString)
    {
        Dictionary<string, string>? details = String.IsNullOrWhiteSpace(failureDetails) ? null : new Dictionary<string, string>() { ["Key"]="Value" };
        DateTime? occurredAt = occurredAtString is null ? null : DateTime.Parse(occurredAtString);
        Exception? exception = exceptionMessage is null ? null : new Exception(exceptionMessage);

        var failureToSerialize  = (Failure)FailureUtility.CreateFailure(failureType, reason, details, subTypeID, canRetry, exception, occurredAt);
        var jsonString          = JsonSerializer.Serialize(failureToSerialize);
        var deserializedFailure = (Failure)JsonSerializer.Deserialize(jsonString,failureType)!;

        using (new AssertionScope())
        {
            deserializedFailure.Details.Should().BeEquivalentTo(details ?? new Dictionary<string, string>());

            if (true == String.IsNullOrWhiteSpace(reason)) deserializedFailure.Reason.Should().Be(String.Empty);
            if (false == String.IsNullOrWhiteSpace(reason)) deserializedFailure.Reason?.Should().Be(reason);

            deserializedFailure.Exception.Should().BeNull();//Should always be null as its not serialized

            deserializedFailure.CanRetry.Should().Be(canRetry);
            deserializedFailure.SubTypeID.Should().Be(subTypeID);
            deserializedFailure.OccurredAt.Should().BeCloseTo(occurredAt ?? DateTime.UtcNow, TimeSpan.FromSeconds(10));

            deserializedFailure.GetType().Should().Be(failureType);
        }
    }

    [Fact]
    public void Should_be_able_to_create_serialize_and_deserialize_an_external_custom_derived_failure()
    {
        /*
            * IMapFailure is a custom failure not created inside flow core/the library package, but inside the shared data test project. 
            * We need to add a type resolver for the IMapFailure derived type.

        */
        var options = new JsonSerializerOptions()
        {
            TypeInfoResolver = new DefaultJsonTypeInfoResolver().WithAddedModifier(typeInfo =>
            {
                if (typeInfo.Type.IsAssignableTo(typeof(Failure)))
                {
                    typeInfo.PolymorphismOptions = new() { DerivedTypes = { new(typeof(IMapFailure), 200) }}; 
                }
            }),
        };


        var failedFlow       = Flow<Person>.Failed(new IMapFailure("Connection issue", "outlook.office365.com", "some-user@outlook.com"));
        var jsonString       = JsonSerializer.Serialize(failedFlow, typeof(Flow<Person>),options);
        var deserializedFlow = JsonSerializer.Deserialize<Flow<Person>>(jsonString,options)!;

        using (new AssertionScope())
        {
            deserializedFlow.Should().Match<Flow<Person>>(r => r.IsFailure == true && r.IsSuccess == false);

            var failure = deserializedFlow.Match(failure => (IMapFailure)failure, success => throw new XunitException("Should not be a success"));

            failure.Should().Match<IMapFailure>(i => i.Reason =="Connection issue" && i.HostName == "outlook.office365.com" && i.UserName == "some-user@outlook.com"
                                                          && i.Details.Count == 0 && i.SubTypeID == 0 && i.Exception == null);

            failure.OccurredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }
    }
}
