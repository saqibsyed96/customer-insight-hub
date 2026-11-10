using CustomerInsightHub.Application.Customers;
using Microsoft.EntityFrameworkCore;

namespace CustomerInsightHub.Infrastructure.Persistence;

public class CustomerRepository(AppDbContext db) : ICustomerRepository
{
    public async Task<IReadOnlyList<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await db.Customers
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CustomerDto(c.Id, c.Name, c.Email, c.Phone, c.City, c.Country, c.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }
}
