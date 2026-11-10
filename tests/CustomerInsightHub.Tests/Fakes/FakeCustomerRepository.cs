using CustomerInsightHub.Application.Customers;
using CustomerInsightHub.Domain.Customers;

namespace CustomerInsightHub.Tests.Fakes;

/// <summary>
/// A tiny in-memory stand-in for the real database repository.
/// It lets us test the business rules in CustomerService without SQL Server.
/// </summary>
public class FakeCustomerRepository : ICustomerRepository
{
    public List<Customer> Items { get; } = [];

    public Task<IReadOnlyList<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<CustomerDto>>(Items
            .OrderBy(c => c.Name)
            .Select(c => new CustomerDto(c.Id, c.Name, c.Email, c.Phone, c.City, c.Country, c.Website, c.CreatedAtUtc))
            .ToList());

    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(Items.FirstOrDefault(c => c.Id == id));

    public Task<bool> EmailExistsAsync(string email, Guid? excludeCustomerId, CancellationToken cancellationToken = default) =>
        Task.FromResult(Items.Any(c =>
            string.Equals(c.Email, email, StringComparison.OrdinalIgnoreCase) && c.Id != excludeCustomerId));

    public Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        Items.Add(customer);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task DeleteAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        Items.Remove(customer);
        return Task.CompletedTask;
    }
}
