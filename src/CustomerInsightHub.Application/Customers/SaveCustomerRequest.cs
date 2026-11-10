namespace CustomerInsightHub.Application.Customers;

/// <summary>
/// The data a client sends to create or update a customer (same shape for both).
/// </summary>
public record SaveCustomerRequest(
    string? Name,
    string? Email,
    string? Phone,
    string? City,
    string? Country,
    string? Website);
