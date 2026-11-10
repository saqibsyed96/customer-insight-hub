using CustomerInsightHub.Domain.Customers;

namespace CustomerInsightHub.Application.Customers;

/// <summary>
/// Contract the Application layer needs. Infrastructure implements it (EF Core today,
/// something else tomorrow) so Application never depends on the database.
/// </summary>
public interface ICustomerRepository
{
    Task<IReadOnlyList<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>True if another customer already uses this email (case-insensitive).</summary>
    Task<bool> EmailExistsAsync(string email, Guid? excludeCustomerId, CancellationToken cancellationToken = default);

    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);

    /// <summary>Saves changes made to a customer previously loaded with GetByIdAsync.</summary>
    Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default);

    Task DeleteAsync(Customer customer, CancellationToken cancellationToken = default);
}
