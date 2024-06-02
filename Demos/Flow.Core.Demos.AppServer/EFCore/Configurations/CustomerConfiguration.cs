using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Flow.Core.Demos.AppServer.EFCore.Models;

namespace Flow.Core.Demos.AppServer.EFCore.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> entity)
    {
        entity.HasKey(e => e.CustomerID);

        entity.Property(e => e.CustomerID)
            .HasColumnType("nchar(5)")
            .HasColumnName("CustomerID");
        entity.Property(e => e.CompanyName)
            .IsRequired()
            .UseCollation("NOCASE")
            .HasColumnType("nvarchar(50)");
        entity.Property(e => e.ContactName)
            .IsRequired()
            .UseCollation("NOCASE")
            .HasColumnType("nvarchar(50)");
        entity.Property(e => e.ContactTitle)
            .IsRequired()
            .UseCollation("NOCASE")
            .HasColumnType("nvarchar(25)");
    }


}
