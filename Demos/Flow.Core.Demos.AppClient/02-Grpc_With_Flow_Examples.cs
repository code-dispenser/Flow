using Flow.Core.Demos.AppClient.Common.Utilities;
using Flow.Core.Demos.AppClient.Services;
using Flow.Core.Areas.Extensions;
using Flow.Core.Demos.Contracts.Areas.Customers;
using Flow.Core.Areas.Returns;
using Flow.Core.Demos.Contracts.Common.CustomFailures;

namespace Flow.Core.Demos.AppClient;

public class Grpc_With_Flow_Examples(GrpcCustomerService customerService)
{
    private readonly GrpcCustomerService _customerService = customerService;

    public async Task RunCustomerSearchExample()
    {
        var searchResults = await NetworkingUtility
                                    .HasInternetConnection(alwaysOn: false)
                                        .OnSuccess(_ => _customerService.CustomerSearch("Al"))
                                            .OnFailure(failure => Console.Out.WriteLineAsync($"{failure.GetType().Name}: {failure.Reason}"))
                                                .Finally(failure => [], success => success.SearchResults.ToList());

        await Console.Out.WriteLineAsync($"Result count: {searchResults.Count}");
        await Console.Out.WriteLineAsync($"First result: {(searchResults.Count > 0 ? searchResults[0] : "[]")}");
        await Console.Out.WriteLineAsync();
        /*
            * Long live Northwind traders, loved access 2.0 my first database. 
        */
    }

    public async Task RunCustomerAddExample(CustomerData customerData)

        => await NetworkingUtility.HasInternetConnection(alwaysOn: true)
                                    .OnSuccess(_ => _customerService.AddCustomer(customerData))
                                        .OnFailure(failure =>
                                        {
                                            var reason = failure is Failure.ValidationFailure
                                                                 ? $"{failure.Reason}\r\n{failure.Details.Keys.First()} : {failure.Details.First().Value}"
                                                                 : failure.Reason;

                                            Console.Out.WriteLineAsync($"{failure.GetType().Name}: {reason}\r\n");
                                        })
                                        .OnSuccess(_ => Console.Out.WriteLineAsync("Customer added\r\n"));

    public async Task RunRaiseServerGrpcExceptionExample()

        => await NetworkingUtility.HasInternetConnection(alwaysOn: true)
                                            .OnSuccess(_ => _customerService.RaiseGrpcServiceException())
                                                .OnFailure(failure => Console.Out.WriteLineAsync($"{failure.GetType().Name}: {failure.Reason}\r\n"));



    public async Task RunUseCustomFailureExample()
    {
        await NetworkingUtility.HasInternetConnection(alwaysOn: true)
                    .OnSuccess(_ => _customerService.ApproveApplication())
                        .OnFailure(failure =>
                        {
                            var failureMessage = (failure is NotApprovedFailure rejection) 
                                                          ? $"Rejected: {rejection.Reason}, Checked by: {rejection.RejectedBy}" 
                                                          : failure.Reason;

                            Console.Out.WriteLineAsync($"{failure.GetType().Name}: {failureMessage}");
                        });
    }
}
