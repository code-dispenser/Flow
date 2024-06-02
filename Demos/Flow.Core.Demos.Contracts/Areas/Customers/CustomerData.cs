using ProtoBuf;

namespace Flow.Core.Demos.Contracts.Areas.Customers;

[ProtoContract]
public class CustomerData
{
    [ProtoMember(1)] public string CustomerID { get; set; } = default!;
    [ProtoMember(2)] public string CompanyName { get; set; } = default!;
    [ProtoMember(3)] public string ContactName { get; set; } = default!;
    [ProtoMember(4)] public string ContactTitle { get; set; } = default!;

}
