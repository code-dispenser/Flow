using Flow.Core.Areas.Returns;
using Flow.Core.Areas.Utilities;
using Flow.Core.Demos.AppServer.Common.Seeds;
using Flow.Core.Demos.AppServer.EFCore;
using Flow.Core.Demos.AppServer.EFCore.Models;
using Flow.Core.Demos.Contracts.Areas.Customers;
using Microsoft.EntityFrameworkCore;

namespace Flow.Core.Demos.AppServer.Areas.Customers;

public record CustomerSearchQuery(string companyName);

public class CustomerSearchQueryHandler(CustomersDbReadOnly readOnlyDB, IDbExceptionHandler exceptionHandler)
{
    private readonly CustomersDbReadOnly _readOnlyDB        = readOnlyDB;
    private readonly IDbExceptionHandler _exceptionHandler  = exceptionHandler;

    public async Task<Flow<IEnumerable<CustomerSearchResult>>> Handle(CustomerSearchQuery searchCriteria)
        /*
            * Utility helper within the Flow library so you can wrap method content in a try catch that returns a flow
            * Using an injected handler specific to database exceptions for the entire app. You could have specific global handlers for file input, messaging etc
         */ 
        => await FlowHandler.TryToFlow
            (
                async () => await _readOnlyDB.Customers.Where(c => c.CompanyName.Contains(searchCriteria.companyName)).Select(Customer.ProjectToCustomerSearchResult).ToListAsync(),
                exception => _exceptionHandler.Handle<IEnumerable<CustomerSearchResult>>(exception)
            );

}
