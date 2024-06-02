
using Flow.Core.Demos.Contracts.Areas.Customers;
using System.Linq.Expressions;

namespace Flow.Core.Demos.AppServer.EFCore.Models;

public record class Customer(string CustomerID, string CompanyName,string ContactName, string ContactTitle)
{ 
    public static Expression<Func<Customer, CustomerSearchResult>> ProjectToCustomerSearchResult

        => (c) => new CustomerSearchResult(c.CustomerID, c.CompanyName, c.ContactName, c.ContactTitle);
}
