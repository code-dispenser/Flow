using Flow.Core.Demos.AppServer.EFCore.Models;
using Microsoft.EntityFrameworkCore;

namespace Flow.Core.Demos.AppServer.Common.Seeds;

public interface IDbContextWrite
{
    public DbSet<Customer> Customers { get; set; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
