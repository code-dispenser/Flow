using Flow.Core.Areas.Returns;
using ProtoBuf;
using System.Text.Json.Serialization;

namespace Flow.Core.Demos.Contracts.Common.CustomFailures;


[ProtoContract]
public sealed class NotApprovedFailure : Failure
{
    [JsonInclude][ProtoMember(1)] public string RejectedBy { get; }

    private NotApprovedFailure() : base("", [], 0, false, null, null) { RejectedBy = String.Empty; }

    [JsonConstructor]
    public NotApprovedFailure(string reason, string rejectedBy) : base(reason, [], 0, false, null, null)
    {
        RejectedBy = rejectedBy;
    }
}
