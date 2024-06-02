using ProtoBuf;
using System.Text.Json.Serialization;

namespace Flow.Core.Demos.Contracts.Areas.Customers;

[ProtoContract]
public class CustomerSearch
{
    [ProtoMember(1)] public string CompanyName { get; set; } = null!;

    private CustomerSearch() { }

    public CustomerSearch(string companyName) => CompanyName = companyName;

}

[ProtoContract]
public class CustomerSearchResponse
{
    [ProtoMember(1)] public IEnumerable<CustomerSearchResult> SearchResults { get; set; } = new List<CustomerSearchResult>();

    public CustomerSearchResponse(IEnumerable<CustomerSearchResult> searchResults) => SearchResults = searchResults;

    private CustomerSearchResponse() { }

}

[ProtoContract]
public class AddCustomer
{
    [ProtoMember(1)] public CustomerData CustomerData { get; } = default!;
    
    private AddCustomer() { }

    public AddCustomer(CustomerData customerData) => CustomerData = customerData;
    
}

