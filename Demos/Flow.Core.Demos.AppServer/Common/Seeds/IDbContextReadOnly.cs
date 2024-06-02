using Flow.Core.Demos.AppServer.EFCore.Models;
using Microsoft.EntityFrameworkCore;

namespace Flow.Core.Demos.AppServer.Common.Seeds;

public interface IDbContextReadOnly
{
    public DbSet<Customer> Customers { get; set; }
}
