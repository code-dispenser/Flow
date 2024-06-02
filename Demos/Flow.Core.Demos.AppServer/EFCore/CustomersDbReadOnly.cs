using Flow.Core.Demos.AppServer.Common.Seeds;
using Flow.Core.Demos.AppServer.EFCore.Models;
using Microsoft.EntityFrameworkCore;

namespace Flow.Core.Demos.AppServer.EFCore;

public class CustomersDbReadOnly : DbContext, IDbContextReadOnly
{
    public virtual DbSet<Customer> Customers { get; set; }

    public CustomersDbReadOnly(DbContextOptions<CustomersDbReadOnly> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    
        => modelBuilder.ApplyConfiguration(new Configurations.CustomerConfiguration());

    public override int SaveChanges() 
        
        => -1;

}
