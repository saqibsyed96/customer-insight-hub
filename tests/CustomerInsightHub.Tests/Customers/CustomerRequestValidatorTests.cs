using CustomerInsightHub.Application.Customers;

namespace CustomerInsightHub.Tests.Customers;

public class CustomerRequestValidatorTests
{
    private static SaveCustomerRequest ValidRequest() =>
        new("Sample Cafe", "owner@sample-cafe.example", "+1-555-0000", "Austin", "USA", "https://sample-cafe.example");

    [Fact]
    public void Valid_request_has_no_errors()
    {
        var errors = CustomerRequestValidator.Validate(ValidRequest());

        Assert.Empty(errors);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Name_is_required(string? name)
    {
        var errors = CustomerRequestValidator.Validate(ValidRequest() with { Name = name });

        Assert.Contains(nameof(SaveCustomerRequest.Name), errors.Keys);
    }

    [Fact]
    public void Name_longer_than_limit_is_rejected()
    {
        var name = new string('a', CustomerRequestValidator.NameMaxLength + 1);

        var errors = CustomerRequestValidator.Validate(ValidRequest() with { Name = name });

        Assert.Contains(nameof(SaveCustomerRequest.Name), errors.Keys);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-an-email")]
    [InlineData("missing-domain@")]
    [InlineData("two@@signs.example")]
    [InlineData("Display Name <a@b.example>")]
    [InlineData("nodot@localhost")]
    public void Invalid_email_is_rejected(string? email)
    {
        var errors = CustomerRequestValidator.Validate(ValidRequest() with { Email = email });

        Assert.Contains(nameof(SaveCustomerRequest.Email), errors.Keys);
    }

    [Theory]
    [InlineData("ftp://files.example")]
    [InlineData("just-text")]
    [InlineData("www.no-scheme.example")]
    public void Invalid_website_is_rejected(string website)
    {
        var errors = CustomerRequestValidator.Validate(ValidRequest() with { Website = website });

        Assert.Contains(nameof(SaveCustomerRequest.Website), errors.Keys);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Website_is_optional(string? website)
    {
        var errors = CustomerRequestValidator.Validate(ValidRequest() with { Website = website });

        Assert.Empty(errors);
    }

    [Fact]
    public void Several_problems_are_all_reported_together()
    {
        var errors = CustomerRequestValidator.Validate(new SaveCustomerRequest(null, "bad", null, null, null, "nope"));

        Assert.Equal(3, errors.Count);
    }
}
