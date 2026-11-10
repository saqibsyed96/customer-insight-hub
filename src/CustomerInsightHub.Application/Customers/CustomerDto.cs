namespace CustomerInsightHub.Application.Customers;

/// <summary>
/// What the API returns to clients. Kept separate from the Domain entity so the
/// API shape can change without touching the database model.
/// </summary>
public record CustomerDto(
    Guid Id,
    string Name,
    string Email,
    string? Phone,
    string? City,
    string? Country,
    DateTime CreatedAtUtc);
