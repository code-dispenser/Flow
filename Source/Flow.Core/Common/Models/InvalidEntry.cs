using ProtoBuf;
using System.Text.Json.Serialization;

namespace Flow.Core.Common.Models;

/// <summary>
/// Represents an invalid entry encountered during a validation or processing operation.
/// </summary>
/// <remarks>This structure encapsulates details about an invalid entry, including a failure message, its location, the property
/// involved, a user-friendly display name,  and the cause of the failure.</remarks>
[ProtoContract]
public sealed record InvalidEntry
{

    /// <summary>
    /// Gets the failure message
    /// </summary>
    [ProtoMember(1)]
    public string FailureMessage { get; }

    /// <summary>
    /// Gets the object path to the member failure
    /// </summary>
    [ProtoMember(2)]
    public string Path { get; }

    /// <summary>
    /// Gets the property name of the failing member
    /// </summary>
    [ProtoMember(3)]
    public string PropertyName { get; }

    /// <summary>
    /// Gets the display / label name that the user sees
    /// </summary>
    [ProtoMember(4)]
    public string DisplayName { get; }

    /// <summary>
    /// Gets the cause of the failure which should be validation unless
    /// the failure was due to a configuration or system error
    /// </summary>
    [ProtoMember(5)]
    public string Cause { get; }

    /// <summary>
    /// Initialises a new InvalidEntry.
    /// </summary>
    /// <remarks>This structure encapsulates details about an invalid entry, including a failure message, its location, the property
    /// involved, a user-friendly display name,  and the cause of the failure.</remarks>
    /// <param name="failureMessage"></param>
    /// <param name="path"></param>
    /// <param name="propertyName"></param>
    /// <param name="displayName"></param>
    /// <param name="cause"></param>
    [JsonConstructor]
    public InvalidEntry(string failureMessage, string path = "", string propertyName = "", string displayName = "", string cause = "")
    {
        FailureMessage = failureMessage  ?? "";
        Path            = path           ?? "";
        PropertyName    = propertyName   ?? "";
        DisplayName     = displayName    ?? "";
        Cause           = cause          ?? "";
    }

    private InvalidEntry() { }
}