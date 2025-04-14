using ProtoBuf;
using System.Text.Json.Serialization;

namespace Flow.Core.Common.Models;

/// <summary>
/// The singleton instance of the None type.
/// </summary>
[ProtoContract]
public sealed record None
{
    /// <summary>
    /// Gets the singleton instance of the <see cref="None"/> type.
    /// </summary>
    [ProtoMember(1)]
    public static None Value { get; } = new();

    [JsonConstructor()]
    private None() { }
    /// <summary>
    /// Returns a string representation of the None value, 
    /// which is currently set to the Ø (empty set) symbol.
    /// </summary>
    /// <returns>The string "Ø".</returns>
    public override string ToString() => "Ø";

}
