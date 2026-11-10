using CustomerInsightHub.Application.Common;
using CustomerInsightHub.Application.Customers;
using CustomerInsightHub.Domain.Customers;
using CustomerInsightHub.Tests.Fakes;

namespace CustomerInsightHub.Tests.Customers;

public class CustomerServiceTests
{
    private static readonly DateTimeOffset Now = new(2026, 6, 1, 10, 0, 0, TimeSpan.Zero);

    private readonly FakeCustomerRepository _repository = new();
    private readonly CustomerService _service;

    public CustomerServiceTests()
    {
        _service = new CustomerService(_repository, new FixedTimeProvider(Now));
    }

    private static SaveCustomerRequest Request(string name = "Sample Cafe", string email = "owner@sample-cafe.example") =>
        new(name, email, null, "Austin", "USA", null);

    private Customer Seed(string name, string email)
    {
        var customer = new Customer { Id = Guid.NewGuid(), Name = name, Email = email, CreatedAtUtc = Now.UtcDateTime };
        _repository.Items.Add(customer);
        return customer;
    }

    // ---------- Create ----------

    [Fact]
    public async Task Create_valid_request_stores_customer_with_clean_data()
    {
        var result = await _service.CreateAsync(Request(name: "  Sample Cafe  ", email: "  Owner@Sample-Cafe.Example "));

        Assert.True(result.IsSuccess);
        var saved = Assert.Single(_repository.Items);
        Assert.Equal("Sample Cafe", saved.Name);
        Assert.Equal("owner@sample-cafe.example", saved.Email); // trimmed and lower-cased
        Assert.Equal(Now.UtcDateTime, saved.CreatedAtUtc);       // comes from the injected clock
        Assert.NotEqual(Guid.Empty, saved.Id);
        Assert.Equal(saved.Id, result.Value!.Id);
    }

    [Fact]
    public async Task Create_blank_optional_fields_are_stored_as_null()
    {
        var request = new SaveCustomerRequest("Sample Cafe", "owner@sample-cafe.example", "  ", "", null, " ");

        await _service.CreateAsync(request);

        var saved = Assert.Single(_repository.Items);
        Assert.Null(saved.Phone);
        Assert.Null(saved.City);
        Assert.Null(saved.Website);
    }

    [Fact]
    public async Task Create_invalid_request_returns_Invalid_and_stores_nothing()
    {
        var result = await _service.CreateAsync(Request(name: "", email: "bad"));

        Assert.Equal(ResultStatus.Invalid, result.Status);
        Assert.Contains(nameof(SaveCustomerRequest.Name), result.Errors.Keys);
        Assert.Contains(nameof(SaveCustomerRequest.Email), result.Errors.Keys);
        Assert.Empty(_repository.Items);
    }

    [Theory]
    [InlineData("taken@example.test")]
    [InlineData("TAKEN@EXAMPLE.TEST")]
    [InlineData("  taken@example.test  ")]
    public async Task Create_duplicate_email_returns_Conflict_ignoring_case_and_spaces(string email)
    {
        Seed("Existing Co", "taken@example.test");

        var result = await _service.CreateAsync(Request(email: email));

        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.Single(_repository.Items);
    }

    // ---------- Get ----------

    [Fact]
    public async Task GetById_existing_returns_customer()
    {
        var existing = Seed("Existing Co", "existing@example.test");

        var result = await _service.GetByIdAsync(existing.Id);

        Assert.True(result.IsSuccess);
        Assert.Equal("Existing Co", result.Value!.Name);
    }

    [Fact]
    public async Task GetById_unknown_returns_NotFound()
    {
        var result = await _service.GetByIdAsync(Guid.NewGuid());

        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    // ---------- Update ----------

    [Fact]
    public async Task Update_changes_the_stored_fields()
    {
        var existing = Seed("Old Name", "old@example.test");

        var result = await _service.UpdateAsync(existing.Id, Request(name: "New Name", email: "new@example.test"));

        Assert.True(result.IsSuccess);
        Assert.Equal("New Name", existing.Name);
        Assert.Equal("new@example.test", existing.Email);
        Assert.Equal(Now.UtcDateTime, existing.CreatedAtUtc); // creation time never changes
    }

    [Fact]
    public async Task Update_keeping_own_email_is_allowed()
    {
        var existing = Seed("Same Co", "same@example.test");

        var result = await _service.UpdateAsync(existing.Id, Request(name: "Same Co Renamed", email: "same@example.test"));

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Update_to_email_used_by_another_customer_returns_Conflict()
    {
        Seed("Other Co", "other@example.test");
        var mine = Seed("My Co", "mine@example.test");

        var result = await _service.UpdateAsync(mine.Id, Request(email: "other@example.test"));

        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.Equal("mine@example.test", mine.Email); // unchanged
    }

    [Fact]
    public async Task Update_unknown_customer_returns_NotFound()
    {
        var result = await _service.UpdateAsync(Guid.NewGuid(), Request());

        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task Update_invalid_request_returns_Invalid_and_keeps_old_data()
    {
        var existing = Seed("Old Name", "old@example.test");

        var result = await _service.UpdateAsync(existing.Id, Request(name: ""));

        Assert.Equal(ResultStatus.Invalid, result.Status);
        Assert.Equal("Old Name", existing.Name);
    }

    // ---------- Delete ----------

    [Fact]
    public async Task Delete_existing_removes_it()
    {
        var existing = Seed("Doomed Co", "doomed@example.test");

        var result = await _service.DeleteAsync(existing.Id);

        Assert.True(result.IsSuccess);
        Assert.Empty(_repository.Items);
    }

    [Fact]
    public async Task Delete_unknown_returns_NotFound()
    {
        var result = await _service.DeleteAsync(Guid.NewGuid());

        Assert.Equal(ResultStatus.NotFound, result.Status);
    }
}
