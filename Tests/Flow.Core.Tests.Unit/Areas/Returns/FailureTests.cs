using Flow.Core.Areas.Returns;
using Flow.Core.Tests.SharedDataAndFixtures.Common.Utilities;
using FluentAssertions;
using FluentAssertions.Execution;


namespace Flow.Core.Tests.Unit.Areas.Returns;

public class FailureTests
{
    [Theory]
    [InlineData(nameof(Failure.NetworkFailure),"Reason for failure", null, 0, false, null, null)]
    [InlineData(nameof(Failure.NetworkFailure),"  ", "Key:Value", 1, true, "Exception message", "2024-05-17T10:00:00Z")]
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
    public void Derived_constructor_should_pass_all_params_to_the_base_constructor_or_use_the_optional_values(string typeName, string? reason, string? details, int subTypeID, bool canRetry, string? exceptionMessage, string? occurredAtString)
    {
        var failureType = Type.GetType(typeof(Failure).AssemblyQualifiedName!.Replace("Failure", String.Concat("Failure+", typeName)))!;
            
        FailureAssertions(failureType, reason, details, subTypeID, canRetry, exceptionMessage, occurredAtString);
    }

    private void FailureAssertions(Type failureType, string? reason, string? failureDetails, int subTypeID, bool canRetry, string? exceptionMessage, string? occurredAtString)
    {
        Dictionary<string, string>? details = String.IsNullOrWhiteSpace(failureDetails) ? null : new Dictionary<string, string>() { ["Key"]="Value" };
        DateTime? occurredAt = occurredAtString is null ? null : DateTime.Parse(occurredAtString);
        Exception? exception = exceptionMessage is null ? null : new Exception(exceptionMessage);

        var failure = (Failure)FailureUtility.CreateFailure(failureType, reason, details, subTypeID, canRetry, exception, occurredAt);

        using (new AssertionScope())
        {
            failure.Details.Should().BeEquivalentTo(details ?? new Dictionary<string, string>());

            if (true == String.IsNullOrWhiteSpace(reason)) failure.Reason.Should().Be(String.Empty);
            if (false == String.IsNullOrWhiteSpace(reason)) failure.Reason?.Should().Be(reason);

            failure.Exception.Should().BeEquivalentTo(exception);
            failure.CanRetry.Should().Be(canRetry);
            failure.SubTypeID.Should().Be(subTypeID);
            failure.OccurredAt.Should().BeCloseTo(occurredAt ?? DateTime.UtcNow, TimeSpan.FromSeconds(10));

            failure.GetType().Should().Be(failureType);
        }
    }


    [Fact]
    public void The_static_create_no_failure_method_on_the_base_failure_class_should_create_a_failure_of_type_no_failure()
    
         => Failure.CreateNoFailure().Should().Match<Failure.NoFailure>(r => r.Reason == "No Failure");

}
