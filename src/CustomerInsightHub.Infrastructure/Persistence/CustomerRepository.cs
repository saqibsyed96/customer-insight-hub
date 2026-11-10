using CustomerInsightHub.Application.Customers;
using CustomerInsightHub.Domain.Customers;
using Microsoft.EntityFrameworkCore;

namespace CustomerInsightHub.Infrastructure.Persistence;

public class CustomerRepository(AppDbContext db) : ICustomerRepository
{
    public async Task<IReadOnlyList<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await db.Customers
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CustomerDto(c.Id, c.Name, c.Email, c.Phone, c.City, c.Country, c.Website, c.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public Task<bool> EmailExistsAsync(string email, Guid? excludeCustomerId, CancellationToken cancellationToken = default) =>
        // SQL Server's default collation is case-insensitive, so this matches regardless of case.
        db.Customers.AnyAsync(c => c.Email == email && c.Id != excludeCustomerId, cancellationToken);

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        db.Customers.Add(customer);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        // The entity is already tracked from GetByIdAsync, so EF knows what changed.
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        db.Customers.Remove(customer);
        await db.SaveChangesAsync(cancellationToken);
    }
}
