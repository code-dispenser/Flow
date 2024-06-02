using ProtoBuf;
using System.Text.Json.Serialization;

namespace Flow.Core.Demos.Contracts.Areas.Customers;

[ProtoContract]
public record class CustomerSearchResult
{
    [ProtoMember(1)] public string CustomerID { get; }
    [ProtoMember(2)] public string CompanyName { get; }
    [ProtoMember(3)] public string ContactName { get; }
    [ProtoMember(4)] public string ContactTitle { get; }

    private CustomerSearchResult() { }

    [JsonConstructor]
    public CustomerSearchResult(string customerID, string companyName, string contactName, string contactTitle)
    {
        CustomerID = customerID;
        CompanyName = companyName;
        ContactName = contactName;
        ContactTitle = contactTitle;
    }
}

