namespace CustomerInsightHub.Application.Customers;

/// <summary>
/// Contract the Application layer needs. Infrastructure implements it (EF Core today,
/// something else tomorrow) so Application never depends on the database.
/// </summary>
public interface ICustomerRepository
{
    Task<IReadOnlyList<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default);
}
