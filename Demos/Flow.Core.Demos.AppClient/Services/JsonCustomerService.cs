using Flow.Core.Areas.Returns;
using Flow.Core.Common.Models;
using Flow.Core.Demos.AppClient.Common.Extensions;
using Flow.Core.Demos.Contracts.Areas.Customers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Flow.Core.Demos.AppClient.Services;

public class JsonCustomerService
{
    private readonly HttpClient _httpClient;
    public JsonCustomerService(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<Flow<None>> AddCustomer(CustomerData customer)

        => await _httpClient.PostAsJsonAsync("", customer).TryCatchJsonResult<None>();

    public async Task<Flow<IEnumerable<CustomerSearchResult>>> CustomerSearch(string companyName)

        => await _httpClient.GetAsync($"?companyname={companyName}").TryCatchJsonResult<IEnumerable<CustomerSearchResult>>();

    public async Task<Flow<None>> RaiseServerSideException()

        => await _httpClient.GetAsync("raiseexception").TryCatchJsonResult<None>();

    public async Task<Flow<bool>> ApproveApplication(int customerID)

        => await _httpClient.PutAsync($"{customerID}/approve", null).TryCatchJsonResult<bool>();

    private StringContent CreateContent<T>(T dataToSerialize)

        => new StringContent(JsonSerializer.Serialize<T>(dataToSerialize), Encoding.UTF8, "application/json");


}
