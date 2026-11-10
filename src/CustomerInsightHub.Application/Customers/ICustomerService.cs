using CustomerInsightHub.Application.Common;

namespace CustomerInsightHub.Application.Customers;

public interface ICustomerService
{
    Task<IReadOnlyList<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult<CustomerDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ServiceResult<CustomerDto>> CreateAsync(SaveCustomerRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult<CustomerDto>> UpdateAsync(Guid id, SaveCustomerRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
