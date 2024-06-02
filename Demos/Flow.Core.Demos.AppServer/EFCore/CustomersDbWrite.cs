using Flow.Core.Demos.AppServer.Common.Seeds;
using Flow.Core.Demos.AppServer.EFCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Flow.Core.Demos.AppServer.EFCore;

public partial class CustomersDbWrite : DbContext, IDbContextWrite
{
    public virtual DbSet<Customer> Customers { get; set; }

    public CustomersDbWrite(DbContextOptions<CustomersDbWrite> options)
    : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new Configurations.CustomerConfiguration());
    }

}

