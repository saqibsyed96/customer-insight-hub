namespace CustomerInsightHub.Domain.Customers;

/// <summary>
/// A customer record. Pure domain object: no database or framework dependencies.
/// </summary>
public class Customer
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
