using ProtoBuf;

namespace Flow.Core.Common.Models;

/// <summary>
/// Represents the absence of a value. 
/// </summary>
[ProtoContract]
public readonly record struct None
{
    /// <summary>
    /// The singleton instance of the None type.
    /// </summary>
    [ProtoMember(1)]
    public static None Value { get; } = new None();

    /// <summary>
    /// Returns a string representation of the None value, 
    /// which is currently set to the Ø (empty set) symbol.
    /// </summary>
    /// <returns>The string "Ø".</returns>
    public override string ToString() => "Ø";

}