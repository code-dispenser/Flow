using Flow.Core.Areas.Extensions;
using Flow.Core.Areas.Returns;
using Flow.Core.Common.Models;
using Flow.Core.Demos.AppServer.Common.Seeds;
using Flow.Core.Demos.AppServer.Common.Validators;
using Flow.Core.Demos.AppServer.EFCore.Models;
using Flow.Core.Demos.Contracts.Areas.Customers;

namespace Flow.Core.Demos.AppServer.Areas.Customers;

public record class AddCustomerCommand(string CustomerID, string CompanyName, string ContactName, string ContactTitle) : IValidatable
{
    public AddCustomerCommand(CustomerData customerData) : this(customerData.CustomerID, customerData.CompanyName, customerData.ContactName, customerData.ContactTitle) { }
}

public class AddCustomerCommandHandler(IDbContextWrite readWriteDb, IDbExceptionHandler exceptionHandler)
{
    private readonly IDbContextWrite     _readWriteDb     = readWriteDb;
    private readonly IDbExceptionHandler _exceptionHander = exceptionHandler;

    public async Task<Flow<None>> Handle(AddCustomerCommand command)

    => await ValidatorFactory.CreateValidatorFor(command)
            .Then(validator => validator.IsValid(command)) //Has a random one in four chance of succeeding
                .OnSuccessTry(async (_) =>
                {
                    /*
                        * The demo tries to add a customer that has the same primary key as an existing customer causing a constraint violation 
                     */ 
                    _readWriteDb.Customers.Add(new Customer(command.CustomerID, command.CompanyName, command.ContactName, command.ContactTitle));
                    
                    await _readWriteDb.SaveChangesAsync();

                    return None.Value;

                },(exception) => _exceptionHander.Handle<None>(exception));

}
