using Flow.Core.Areas.Returns;
using ProtoBuf;
using System.Text.Json.Serialization;

namespace Flow.Core.Tests.SharedDataAndFixtures.Common.CustomFailures;


[ProtoContract]
public class IMapFailure : Failure
{
    [JsonInclude][ProtoMember(1)] public string HostName { get; }
    [JsonInclude][ProtoMember(2)] public string UserName { get; }


    private IMapFailure() : base("", [], 0, true, null, DateTime.UtcNow) { }

    [JsonConstructor]
    public IMapFailure(string reason, string hostName, string userName)
        : base(reason, [], 0, true, null, DateTime.UtcNow)
    {
        HostName = hostName;
        UserName = userName;
    }
}
