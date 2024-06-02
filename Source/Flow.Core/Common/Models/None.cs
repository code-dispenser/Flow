using ProtoBuf;

namespace Flow.Core.Common.Models;

[ProtoContract]
public readonly record struct None
{
    [ProtoMember(1)]
    public static None Value { get; } = new None();
    public override string ToString() => "Ø";

}