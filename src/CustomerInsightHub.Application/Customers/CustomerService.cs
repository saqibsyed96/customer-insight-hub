using CustomerInsightHub.Application.Common;
using CustomerInsightHub.Domain.Customers;

namespace CustomerInsightHub.Application.Customers;

/// <summary>
/// All customer business rules live here: validation, unique email, timestamps.
/// It only talks to the ICustomerRepository interface, so it can be tested without a database.
/// </summary>
public class CustomerService(ICustomerRepository repository, TimeProvider clock) : ICustomerService
{
    private const string DuplicateEmailMessage = "A customer with this email already exists.";

    public Task<IReadOnlyList<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default) =>
        repository.GetAllAsync(cancellationToken);

    public async Task<ServiceResult<CustomerDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await repository.GetByIdAsync(id, cancellationToken);
        return customer is null
            ? ServiceResult<CustomerDto>.NotFound()
            : ServiceResult<CustomerDto>.Ok(ToDto(customer));
    }

    public async Task<ServiceResult<CustomerDto>> CreateAsync(SaveCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var errors = CustomerRequestValidator.Validate(request);
        if (errors.Count > 0) return ServiceResult<CustomerDto>.Invalid(errors);

        var email = NormalizeEmail(request.Email!);
        if (await repository.EmailExistsAsync(email, excludeCustomerId: null, cancellationToken))
            return ServiceResult<CustomerDto>.Conflict(DuplicateEmailMessage);

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            CreatedAtUtc = clock.GetUtcNow().UtcDateTime
        };
        Apply(customer, request, email);

        await repository.AddAsync(customer, cancellationToken);
        return ServiceResult<CustomerDto>.Ok(ToDto(customer));
    }

    public async Task<ServiceResult<CustomerDto>> UpdateAsync(Guid id, SaveCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var customer = await repository.GetByIdAsync(id, cancellationToken);
        if (customer is null) return ServiceResult<CustomerDto>.NotFound();

        var errors = CustomerRequestValidator.Validate(request);
        if (errors.Count > 0) return ServiceResult<CustomerDto>.Invalid(errors);

        var email = NormalizeEmail(request.Email!);
        if (await repository.EmailExistsAsync(email, excludeCustomerId: id, cancellationToken))
            return ServiceResult<CustomerDto>.Conflict(DuplicateEmailMessage);

        Apply(customer, request, email);

        await repository.UpdateAsync(customer, cancellationToken);
        return ServiceResult<CustomerDto>.Ok(ToDto(customer));
    }

    public async Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await repository.GetByIdAsync(id, cancellationToken);
        if (customer is null) return ServiceResult.NotFound();

        await repository.DeleteAsync(customer, cancellationToken);
        return ServiceResult.Ok();
    }

    // Emails are stored trimmed and lower-cased so "A@x.com" and "a@x.com" are the same customer.
    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    private static string? NullIfBlank(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static void Apply(Customer customer, SaveCustomerRequest request, string normalizedEmail)
    {
        customer.Name = request.Name!.Trim();
        customer.Email = normalizedEmail;
        customer.Phone = NullIfBlank(request.Phone);
        customer.City = NullIfBlank(request.City);
        customer.Country = NullIfBlank(request.Country);
        customer.Website = NullIfBlank(request.Website);
    }

    private static CustomerDto ToDto(Customer c) =>
        new(c.Id, c.Name, c.Email, c.Phone, c.City, c.Country, c.Website, c.CreatedAtUtc);
}
