using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Flow.Core.Areas.Returns;

/// <summary>
/// Represents a base class for different types of failures.
/// </summary>
[JsonDerivedType(typeof(NoFailure), 100)]
[JsonDerivedType(typeof(NetworkFailure), 101)]
[JsonDerivedType(typeof(DatabaseFailure), 102)]
[JsonDerivedType(typeof(FileSystemFailure), 103)]
[JsonDerivedType(typeof(SecurityFailure), 104)]
[JsonDerivedType(typeof(ConnectionFailure), 105)]
[JsonDerivedType(typeof(ValidationFailure), 106)]
[JsonDerivedType(typeof(ConfigurationFailure), 107)]
[JsonDerivedType(typeof(ServiceFailure), 108)]
[JsonDerivedType(typeof(CloudStorageFailure), 109)]
[JsonDerivedType(typeof(ItemNotFoundFailure), 110)]
[JsonDerivedType(typeof(MessagingFailure), 111)]
[JsonDerivedType(typeof(GeneralFailure), 112)]
[JsonDerivedType(typeof(ConstraintFailure), 113)]
[JsonDerivedType(typeof(DomainFailure), 114)]
[JsonDerivedType(typeof(ApplicationFailure), 115)]
[JsonDerivedType(typeof(IOFailure), 116)]
[JsonDerivedType(typeof(HardwareFailure), 117)]
[JsonDerivedType(typeof(SystemFailure), 118)]
[JsonDerivedType(typeof(TaskCancellationFailure), 119)]
[JsonDerivedType(typeof(InternetConnectionFailure), 120)]
[JsonDerivedType(typeof(CacheFailure), 121)]
[JsonDerivedType(typeof(JsonFailure), 122)]
[JsonDerivedType(typeof(GrpcFailure), 123)]
[JsonDerivedType(typeof(ConversionFailure), 124)]

[JsonDerivedType(typeof(UnknownFailure), 199)]

[ProtoContract]
[
    ProtoInclude(100, typeof(NoFailure)), ProtoInclude(101, typeof(NetworkFailure)),
    ProtoInclude(102, typeof(DatabaseFailure)), ProtoInclude(103, typeof(FileSystemFailure)),
    ProtoInclude(104, typeof(SecurityFailure)), ProtoInclude(105, typeof(ConnectionFailure)),
    ProtoInclude(106, typeof(ValidationFailure)), ProtoInclude(107, typeof(ConfigurationFailure)),
    ProtoInclude(108, typeof(ServiceFailure)), ProtoInclude(109, typeof(CloudStorageFailure)),
    ProtoInclude(110, typeof(ItemNotFoundFailure)), ProtoInclude(111, typeof(MessagingFailure)),
    ProtoInclude(112, typeof(GeneralFailure)), ProtoInclude(113, typeof(ConstraintFailure)),
    ProtoInclude(114, typeof(DomainFailure)), ProtoInclude(115, typeof(ApplicationFailure)),
    ProtoInclude(116, typeof(IOFailure)), ProtoInclude(117, typeof(HardwareFailure)),
    ProtoInclude(118, typeof(SystemFailure)), ProtoInclude(119, typeof(TaskCancellationFailure)),
    ProtoInclude(120, typeof(InternetConnectionFailure)), ProtoInclude(121, typeof(CacheFailure)),
    ProtoInclude(122, typeof(JsonFailure)), ProtoInclude(123, typeof(GrpcFailure)),
    ProtoInclude(124, typeof(ConversionFailure)), ProtoInclude(199, typeof(UnknownFailure))

]
public abstract class Failure
{
    /// <summary>
    /// Gets the reason for the failure.
    /// </summary>
    [JsonInclude][ProtoMember(1)] public string Reason { get; }

    /// <summary>
    /// Gets additional details about the failure.
    /// </summary>
    [JsonInclude][ProtoMember(2)] public Dictionary<string, string> Details { get; }

    /// <summary>
    /// Gets the subtype ID of the failure.
    /// </summary>
    [JsonInclude][ProtoMember(3)] public int SubTypeID { get; }

    /// <summary>
    /// Gets a value indicating whether the operation can be retried.
    /// </summary>
    [JsonInclude][ProtoMember(4)] public bool CanRetry { get; }


    /// <summary>
    /// Gets the date and time (UTC default) when the failure occurred.
    /// </summary>
    [JsonInclude][ProtoMember(5)] public DateTime? OccurredAt { get; }

    /// <summary>
    /// Gets the exception associated with the failure, if any.
    /// This property will purposely not be serialized when using GRPC code first
    /// or the System.Text.Json serializer.
    /// </summary>
    [JsonIgnore] public Exception? Exception { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Failure"/> class.
    /// </summary>
    /// <param name="reason">The reason for the failure.</param>
    /// <param name="details">Additional details about the failure.</param>
    /// <param name="subTypeID">The subtype ID of the failure.</param>
    /// <param name="canRetry">Whether the operation can be retried.</param>
    /// <param name="exception">The exception associated with the failure.</param>
    /// <param name="occurredAt">The date and time when the failure occurred.</param>
    public Failure(string reason, Dictionary<string, string>? details = null, int subTypeID = 0, bool canRetry = false, Exception? exception = null, DateTime? occurredAt = null)
    {

        Reason      = String.IsNullOrWhiteSpace(reason) ? String.Empty : reason;
        Details     = details ?? [];
        SubTypeID   = subTypeID;
        CanRetry    = canRetry;
        Exception   = exception;
        OccurredAt  = occurredAt ?? DateTime.UtcNow;

    }

    /// <summary>
    /// Creates a new instance of the <see cref="NoFailure"/> class.
    /// </summary>
    /// <returns>A new instance of the <see cref="NoFailure"/> class.</returns>
    public static NoFailure CreateNoFailure() => new();

    #region Derived Nested classes

    /// <summary>
    /// Represents a no failure state.
    /// </summary>
    [ProtoContract]
    public sealed class NoFailure : Failure
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoFailure"/> class.
        /// </summary>
        public NoFailure() : base("No Failure") { }
    }
    /*
        * protobuf-net treats empty collections and nulls the same.
        * In order to the get a default empty Dictionary<string,string>, leaving the constructor as optional, I needed to go the private constructor route for protobuf-net.
        * During deserialization protobuf will run the code in the private constructor and then apply any values based on the ProtoMember attribute to the corresponding properties.
        * With regards collections, protobuf-net does not overwrite collections but adds to them, so for the empty dictionary, it will get created with nothing added to it, 
        * for the default option case.
    */


    /// <summary>
    /// Represents a network failure.
    /// </summary>
    [ProtoContract]
    public sealed class NetworkFailure : Failure
    {
        private NetworkFailure() : base("", [], 0, false, null, null) { }

        /// <inheritdoc/>
        [JsonConstructor]
        public NetworkFailure(string reason, Dictionary<string, string>? details = null, int subTypeID = 0, bool canRetry = false, Exception? exception = null, DateTime? occurredAt = null)
            : base(reason, details, subTypeID, canRetry, exception, occurredAt) { }
    }

    /// <summary>
    /// Represents a database failure.
    /// </summary>
    [ProtoContract]
    public sealed class DatabaseFailure : Failure
    {
        private DatabaseFailure() : base("", [], 0, false, null, null) { }

        /// <inheritdoc/>
        [JsonConstructor()]
        public DatabaseFailure(string reason, Dictionary<string, string>? details = null, int subTypeID = 0, bool canRetry = false, Exception? exception = null, DateTime? occurredAt = null)
            : base(reason, details, subTypeID, canRetry, exception, occurredAt) { }
    }

    /// <summary>
    /// Represents a file system failure.
    /// </summary>
    [ProtoContract]
    public sealed class FileSystemFailure : Failure
    {
        private FileSystemFailure() : base("", [], 0, false, null, null) { }

        /// <inheritdoc/>
        [JsonConstructor()]
        public FileSystemFailure(string reason, Dictionary<string, string>? details = null, int subTypeID = 0, bool canRetry = false, Exception? exception = null, DateTime? occurredAt = null)
            : base(reason, details, subTypeID, canRetry, exception, occurredAt) { }
    }

    /// <summary>
    /// Represents a validation failure.
    /// </summary>
    [ProtoContract]
    public sealed class ValidationFailure : Failure
    {
        private ValidationFailure() : base("", [], 0, false, null, null) { }

        /// <inheritdoc/>
        [JsonConstructor()]
        public ValidationFailure(string reason, Dictionary<string, string>? details = null, int subTypeID = 0, bool canRetry = false, Exception? exception = null, DateTime? occurredAt = null)
            : base(reason, details, subTypeID, canRetry, exception, occurredAt) { }
    }

    /// <summary>
    /// Represents a security failure.
    /// </summary>
    [ProtoContract]
    public sealed class SecurityFailure : Failure
    {
        private SecurityFailure() : base("", [], 0, false, null, null) { }

        /// <inheritdoc/>
        [JsonConstructor()]
        public SecurityFailure(string reason, Dictionary<string, string>? details = null, int subTypeID = 0, bool canRetry = false, Exception? exception = null, DateTime? occurredAt = null)
            : base(reason, details, subTypeID, canRetry, exception, occurredAt) { }
    }

    /// <summary>
    /// Represents a configuration failure.
    /// </summary>
    [ProtoContract]
    public sealed class ConfigurationFailure : Failure
    {
        private ConfigurationFailure() : base("", [], 0, false, null, null) { }

        /// <inheritdoc/>
        [JsonConstructor()]
        public ConfigurationFailure(string reason, Dictionary<string, string>? details = null, int subTypeID = 0, bool canRetry = false, Exception? exception = null, DateTime? occurredAt = null)
            : base(reason, details, subTypeID, canRetry, exception, occurredAt) { }
    }

    /// <summary>
    /// Represents a service failure.
    /// </summary>
    [ProtoContract]
    public sealed class ServiceFailure : Failure
    {
        private ServiceFailure() : base("", [], 0, false, null, null) { }

        /// <inheritdoc/>
        [JsonConstructor()]
        public ServiceFailure(string reason, Dictionary<string, string>? details = null, int subTypeID = 0, bool canRetry = false, Exception? exception = null, DateTime? occurredAt = null)
            : base(reason, details, subTypeID, canRetry, exception, occurredAt) { }
    }

    /// <summary>
    /// Represents a cloud storage failure.
    /// </summary>
    [ProtoContract]
    public sealed class CloudStorageFailure : Failure
    {
        private CloudStorageFailure() : base("", [], 0, false, null, null) { }

        /// <inheritdoc/>
        [JsonConstructor()]
        public CloudStorageFailure(string reason, Dictionary<string, string>? details = null, int subTypeID = 0, bool canRetry = false, Exception? exception = null, DateTime? occurredAt = null)
            : base(reason, details, subTypeID, canRetry, exception, occurredAt) { }
    }

    /// <summary>
    /// Represents an item not found failure.
    /// </summary>
    [ProtoContract]
    public sealed class ItemNotFoundFailure : Failure
    {
        private ItemNotFoundFailure() : base("", [], 0, false, null, null) { }

        /// <inheritdoc/>
        [JsonConstructor()]
        public ItemNotFoundFailure(string reason, Dictionary<string, string>? details = null, int subTypeID = 0, bool canRetry = false, Exception? exception = null, DateTime? occurredAt = null)
            : base(reason, details, subTypeID, canRetry, exception, occurredAt) { }
    }

    /// <summary>
    /// Represents a messaging failure.
    /// </summary>
    [ProtoContract]
    public sealed class MessagingFailure : Failure
    {
        private MessagingFailure() : base("", [], 0, false, null, null) { }

        /// <inheritdoc/>
        [JsonConstructor()]
        public MessagingFailure(string reason, Dictionary<string, string>? details = null, int subTypeID = 0, bool canRetry = false, Exception? exception = null, DateTime? occurredAt = null)
            : base(reason, details, subTypeID, canRetry, exception, occurredAt) { }
    }

    /// <summary>
    /// Represents a general failure.
    /// </summary>
    [ProtoContract]
    public sealed class GeneralFailure : Failure
    {
        private GeneralFailure() : base("", [], 0, false, null, null) { }

        /// <inheritdoc/>
        [JsonConstructor()]
        public GeneralFailure(string reason, Dictionary<string, string>? details = null, int subTypeID = 0, bool canRetry = false, Exception? exception = null, DateTime? occurredAt = null)
            : base(reason, details, subTypeID, canRetry, exception, occurredAt) { }
    }

    /// <summary>
    /// Represents a constraint failure.
    /// </summary>
    [ProtoContract]
    public sealed class ConstraintFailure : Failure
    {
        private ConstraintFailure() : base("", [], 0, false, null, null) { }

        /// <inheritdoc/>
        [JsonConstructor()]
        public ConstraintFailure(string reason, Dictionary<string, string>? details = null, int subTypeID = 0, bool canRetry = false, Exception? exception = null, DateTime? occurredAt = null)
            : base(reason, details, subTypeID, canRetry, exception, occurredAt) { }
    }

    /// <summary>
    /// Represents a domain failure.
    /// </summary>
    [ProtoContract]
    public sealed class DomainFailure : Failure
    {
        private DomainFailure() : base("", [], 0, false, null, null) { }

        /// <inheritdoc/>
        [JsonConstructor()]
        public DomainFailure(string reason, Dictionary<string, string>? details = null, int subTypeID = 0, bool canRetry = false, Exception? exception = null, DateTime? occurredAt = null)
            : base(reason, details, subTypeID, canRetry, exception, occurredAt) { }
    }

    /// <summary>
    /// Represents an application failure.
    /// </summary>
    [ProtoContract]
    public sealed class ApplicationFailure : Failure
    {
        private ApplicationFailure() : base("", [], 0, false, null, null) { }

        /// <inheritdoc/>
        [JsonConstructor()]
        public ApplicationFailure(string reason, Dictionary<string, string>? details = null, int subTypeID = 0, bool canRetry = false, Exception? exception = null, DateTime? occurredAt = null)
            : base(reason, details, subTypeID, canRetry, exception, occurredAt) { }
    }

    /// <summary>
    /// Represents an I/O failure.
    /// </summary>
    [ProtoContract]
    public sealed class IOFailure : Failure
    {
        private IOFailure() : base("", [], 0, false, null, null) { }

        /// <inheritdoc/>
        [JsonConstructor()]
        public IOFailure(string reason, Dictionary<string, string>? details = null, int subTypeID = 0, bool canRetry = false, Exception? exception = null, DateTime? occurredAt = null)
            : base(reason, details, subTypeID, canRetry, exception, occurredAt) { }
    }

    /// <summary>
    /// Represents a hardware failure.
    /// </summary>
    [ProtoContract]
    public sealed class HardwareFailure : Failure
    {
        private HardwareFailure() : base("", [], 0, false, null, null) { }

        /// <inheritdoc/>
        [JsonConstructor()]
        public HardwareFailure(string reason, Dictionary<string, string>? details = null, int subTypeID = 0, bool canRetry = false, Exception? exception = null, DateTime? occurredAt = null)
            : base(reason, details, subTypeID, canRetry, exception, occurredAt) { }
    }

    /// <summary>
    /// Represents a system failure.
    /// </summary>
    [ProtoContract]
    public sealed class SystemFailure : Failure
    {
        private SystemFailure() : base("", [], 0, false, null, null) { }

        /// <inheritdoc/>
        [JsonConstructor()]
        public SystemFailure(string reason, Dictionary<string, string>? details = null, int subTypeID = 0, bool canRetry = false, Exception? exception = null, DateTime? occurredAt = null)
            : base(reason, details, subTypeID, canRetry, exception, occurredAt) { }
    }

    /// <summary>
    /// Represents a connection failure.
    /// </summary>
    [ProtoContract]
    public sealed class ConnectionFailure : Failure
    {
        private ConnectionFailure() : base("", [], 0, false, null, null) { }

        /// <inheritdoc/>
        [JsonConstructor()]
        public ConnectionFailure(string reason, Dictionary<string, string>? details = null, int subTypeID = 0, bool canRetry = false, Exception? exception = null, DateTime? occurredAt = null)
            : base(reason, details, subTypeID, canRetry, exception, occurredAt) { }
    }

    /// <summary>
    /// Represents a task cancellation failure.
    /// </summary>
    [ProtoContract]
    public sealed class TaskCancellationFailure : Failure
    {
        private TaskCancellationFailure() : base("", [], 0, false, null, null) { }

        /// <inheritdoc/>
        [JsonConstructor()]
        public TaskCancellationFailure(string reason, Dictionary<string, string>? details = null, int subTypeID = 0, bool canRetry = false, Exception? exception = null, DateTime? occurredAt = null)
            : base(reason, details, subTypeID, canRetry, exception, occurredAt) { }
    }

    /// <summary>
    /// Represents an internet connection failure.
    /// </summary>
    [ProtoContract]
    public sealed class InternetConnectionFailure : Failure
    {
        private InternetConnectionFailure() : base("", [], 0, false, null, null) { }

        /// <inheritdoc/>
        [JsonConstructor()]
        public InternetConnectionFailure(string reason, Dictionary<string, string>? details = null, int subTypeID = 0, bool canRetry = false, Exception? exception = null, DateTime? occurredAt = null)
            : base(reason, details, subTypeID, canRetry, exception, occurredAt) { }
    }

    /// <summary>
    /// Represents a cache failure.
    /// </summary>
    [ProtoContract]
    public sealed class CacheFailure : Failure
    {
        private CacheFailure() : base("", [], 0, false, null, null) { }

        /// <inheritdoc/>
        [JsonConstructor()]
        public CacheFailure(string reason, Dictionary<string, string>? details = null, int subTypeID = 0, bool canRetry = false, Exception? exception = null, DateTime? occurredAt = null)
            : base(reason, details, subTypeID, canRetry, exception, occurredAt) { }
    }

    /// <summary>
    /// Represents a Json Serialisation or Deserialisation failure.
    /// </summary>
    [ProtoContract]
    public sealed class JsonFailure : Failure
    {
        private JsonFailure() : base("", [], 0, false, null, null) { }

        /// <inheritdoc/>
        [JsonConstructor()]
        public JsonFailure(string reason, Dictionary<string, string>? details = null, int subTypeID = 0, bool canRetry = false, Exception? exception = null, DateTime? occurredAt = null)
            : base(reason, details, subTypeID, canRetry, exception, occurredAt) { }
    }
    
    /// <summary>
    /// Represents a Grpc Serialisation or Deserialisation failure.
    /// </summary>
    [ProtoContract]
    public sealed class GrpcFailure : Failure
    {
        private GrpcFailure() : base("", [], 0, false, null, null) { }

        /// <inheritdoc/>
        [JsonConstructor()]
        public GrpcFailure(string reason, Dictionary<string, string>? details = null, int subTypeID = 0, bool canRetry = false, Exception? exception = null, DateTime? occurredAt = null)
            : base(reason, details, subTypeID, canRetry, exception, occurredAt) { }
    }

    /// <summary>
    /// Represents some sort of conversion failure.
    /// </summary>
    [ProtoContract]
    public sealed class ConversionFailure : Failure
    {
        private ConversionFailure() : base("", [], 0, false, null, null) { }

        /// <inheritdoc/>
        [JsonConstructor()]
        public ConversionFailure(string reason, Dictionary<string, string>? details = null, int subTypeID = 0, bool canRetry = false, Exception? exception = null, DateTime? occurredAt = null)
            : base(reason, details, subTypeID, canRetry, exception, occurredAt) { }
    }


    /// <summary>
    /// Represents an unknown failure.
    /// </summary>
    [ProtoContract]
    public sealed class UnknownFailure : Failure
    {
        private UnknownFailure() : base("", [], 0, false, null, null) { }

        /// <inheritdoc/>
        [JsonConstructor()]
        public UnknownFailure(string reason, Dictionary<string, string>? details = null, int subTypeID = 0, bool canRetry = false, Exception? exception = null, DateTime? occurredAt = null)
            : base(reason, details, subTypeID, canRetry, exception, occurredAt) { }
    }

    #endregion
}

