using Flow.Core.Areas.Returns;
using Flow.Core.Common.Models;
using Flow.Core.Demos.AppServer.Areas.Customers;
using Flow.Core.Demos.Contracts.Areas.Customers;
using Flow.Core.Demos.Contracts.Common.CustomFailures;
using Flow.Core.Demos.Contracts.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Flow.Core.Demos.AppServer.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomersController : ControllerBase
{

    [HttpPost()]
    public async Task<Flow<None>> AddCustomer([FromServices] AddCustomerCommandHandler addCustomerHandler, [FromBody] CustomerData customerData)
    
        => await addCustomerHandler.Handle(new AddCustomerCommand(customerData));



    [HttpGet]
    public async Task<Flow<IEnumerable<CustomerSearchResult>>> CustomerSearch([FromServices] CustomerSearchQueryHandler searchHandler, [FromQuery] string companyName)
        => await searchHandler.Handle(new CustomerSearchQuery(companyName));
    
    [HttpGet("RaiseException")]
    public Task<Flow<None>> RaiseServerSideException()
        
        // Just continues the code execution, this is part of the demo

        => throw new SystemException("An unhandled error has occurred");

    [HttpPut("{id}/approve")]
    public Task<Flow<bool>> Approve(int id)

        => Task.FromResult(Flow<bool>.Failed(new NotApprovedFailure("Failed credit checks.", "the Boss")));
}







