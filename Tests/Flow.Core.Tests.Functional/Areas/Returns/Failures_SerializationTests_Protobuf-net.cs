using Flow.Core.Areas.Returns;
using Flow.Core.Tests.SharedDataAndFixtures.Common.CustomFailures;
using Flow.Core.Tests.SharedDataAndFixtures.Common.Models;
using Flow.Core.Tests.SharedDataAndFixtures.Common.Utilities;
using FluentAssertions;
using FluentAssertions.Execution;
using ProtoBuf;
using ProtoBuf.Meta;
using System.Reflection;
using Xunit.Sdk;

namespace Flow.Core.Tests.Functional.Areas.Returns;

public class Failure__SerializationTests_Protobuf
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
    [InlineData(nameof(Failure.JsonFailure), "Reason for failure", null, 0, false, null, null)]
    [InlineData(nameof(Failure.JsonFailure), "  ", "Key:Value", 1, true, "Exception message", "2024-05-17T10:00:00Z")]
    [InlineData(nameof(Failure.GrpcFailure), "Reason for failure", null, 0, false, null, null)]
    [InlineData(nameof(Failure.GrpcFailure), "  ", "Key:Value", 1, true, "Exception message", "2024-05-17T10:00:00Z")]
    [InlineData(nameof(Failure.ConversionFailure), "Reason for failure", null, 0, false, null, null)]
    [InlineData(nameof(Failure.ConversionFailure), "  ", "Key:Value", 1, true, "Exception message", "2024-05-17T10:00:00Z")]
    [InlineData(nameof(Failure.UnknownFailure), "Reason for failure", null, 0, false, null, null)]
    [InlineData(nameof(Failure.UnknownFailure), "  ", "Key:Value", 1, true, "Exception message", "2024-05-17T10:00:00Z")]
    public void Should_be_able_to_serialize_and_deserialize_the_failure_using_protobuf_net_serializer(string typeName, string? reason, string? details, int subTypeID, bool canRetry, string? exceptionMessage, string? occurredAtString)
    {
        var failureType              = Type.GetType(typeof(Failure).AssemblyQualifiedName!.Replace("Failure", String.Concat("Failure+", typeName)))!;
        var failureAssertionsMethod  = typeof(Failure__SerializationTests_Protobuf)
                                            .GetMethod(nameof(FailureAssertions), BindingFlags.NonPublic | BindingFlags.Instance)!
                                            .MakeGenericMethod(failureType);

        failureAssertionsMethod.Invoke(this,[reason, details, subTypeID, canRetry, exceptionMessage, occurredAtString]);
    }

    [Fact]
    public void Should_be_able_to_serialize_and_deserialize_the_no_failure_class_using_the_protobuf_net_serializer()
    {
        var noFailure = Failure.CreateNoFailure();

        var deserializedFailure = Serializer.DeepClone(noFailure);

        using (new AssertionScope())
        {
            deserializedFailure.Should().Match<Failure.NoFailure>(f => f.Reason == "No Failure" && f.Details.Count == 0 && f.Exception == null && f.CanRetry == false);
            deserializedFailure.OccurredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
        }
    }

    private void FailureAssertions<T>(string? reason, string? failureDetails, int subTypeID, bool canRetry, string? exceptionMessage, string? occurredAtString) where T : Failure
    {
        Dictionary<string, string>? details = String.IsNullOrWhiteSpace(failureDetails) ? null : new Dictionary<string, string>() { ["Key"]="Value" };
        DateTime? occurredAt = occurredAtString is null ? null : DateTime.Parse(occurredAtString);
        Exception? exception = exceptionMessage is null ? null : new Exception(exceptionMessage);

        var failureType         = typeof(T);
        var failureToSerialize  = FailureUtility.CreateFailure<T>(reason, details, subTypeID, canRetry, exception, occurredAt);
        var deserializedFailure = Serializer.DeepClone<T>(failureToSerialize);

        using (new AssertionScope())
        {
            deserializedFailure.Details.Should().BeEquivalentTo(details ?? []);

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
            * In normal programs just adding the types at startup using the static RuntimeTypeModel.Default is enough.
            * As this in a test class with other tests dependant on the test order to avoid errors its simpler just to create a separate runtime. 
        */

        var customFailureRuntimeSerializer = RuntimeTypeModel.Create();
        customFailureRuntimeSerializer.Add(typeof(Failure), true).AddSubType(200, typeof(IMapFailure));

        
        var failedFlow = Flow<Person>.Failed(new IMapFailure("Connection issue", "outlook.office365.com", "some-user@outlook.com"));

        var deserializedFlow = customFailureRuntimeSerializer.DeepClone<Flow<Person>>(failedFlow);

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
