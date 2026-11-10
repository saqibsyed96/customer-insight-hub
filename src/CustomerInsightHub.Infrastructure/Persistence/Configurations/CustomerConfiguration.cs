using CustomerInsightHub.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerInsightHub.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Email).IsRequired().HasMaxLength(320);
        builder.Property(c => c.Phone).HasMaxLength(50);
        builder.Property(c => c.City).HasMaxLength(100);
        builder.Property(c => c.Country).HasMaxLength(100);
        builder.Property(c => c.Website).HasMaxLength(200);

        builder.HasIndex(c => c.Email).IsUnique();

        // A few made-up sample rows so the first GET returns something.
        builder.HasData(
            new Customer
            {
                Id = new Guid("11111111-1111-1111-1111-111111111111"),
                Name = "Riverstone Bakery",
                Email = "hello@riverstone-bakery.example",
                Phone = "+1-555-0101",
                City = "Portland",
                Country = "USA",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Customer
            {
                Id = new Guid("22222222-2222-2222-2222-222222222222"),
                Name = "Blue Harbor Logistics",
                Email = "contact@blueharbor-logistics.example",
                Phone = "+44-20-5550-0102",
                City = "Bristol",
                Country = "UK",
                CreatedAtUtc = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc)
            },
            new Customer
            {
                Id = new Guid("33333333-3333-3333-3333-333333333333"),
                Name = "Maple Grove Dental",
                Email = "office@maplegrove-dental.example",
                Phone = "+1-555-0103",
                City = "Toronto",
                Country = "Canada",
                CreatedAtUtc = new DateTime(2026, 1, 3, 0, 0, 0, DateTimeKind.Utc)
            });
    }
}
