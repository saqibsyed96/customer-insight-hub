using System.Net.Mail;

namespace CustomerInsightHub.Application.Customers;

/// <summary>
/// Input rules for a customer. Max lengths must stay in sync with CustomerConfiguration
/// (Infrastructure), otherwise the database would reject data this validator accepted.
/// </summary>
public static class CustomerRequestValidator
{
    public const int NameMaxLength = 200;
    public const int EmailMaxLength = 320;
    public const int PhoneMaxLength = 50;
    public const int CityMaxLength = 100;
    public const int CountryMaxLength = 100;
    public const int WebsiteMaxLength = 200;

    public static Dictionary<string, string[]> Validate(SaveCustomerRequest request)
    {
        var errors = new Dictionary<string, List<string>>();

        void Add(string field, string message)
        {
            if (!errors.TryGetValue(field, out var list))
            {
                list = [];
                errors[field] = list;
            }
            list.Add(message);
        }

        var name = request.Name?.Trim();
        if (string.IsNullOrEmpty(name)) Add(nameof(request.Name), "Name is required.");
        else if (name.Length > NameMaxLength) Add(nameof(request.Name), $"Name must be at most {NameMaxLength} characters.");

        var email = request.Email?.Trim();
        if (string.IsNullOrEmpty(email)) Add(nameof(request.Email), "Email is required.");
        else if (email.Length > EmailMaxLength) Add(nameof(request.Email), $"Email must be at most {EmailMaxLength} characters.");
        else if (!IsValidEmail(email)) Add(nameof(request.Email), "Email is not a valid email address.");

        CheckMax(request.Phone, PhoneMaxLength, nameof(request.Phone), Add);
        CheckMax(request.City, CityMaxLength, nameof(request.City), Add);
        CheckMax(request.Country, CountryMaxLength, nameof(request.Country), Add);

        var website = request.Website?.Trim();
        if (!string.IsNullOrEmpty(website))
        {
            if (website.Length > WebsiteMaxLength) Add(nameof(request.Website), $"Website must be at most {WebsiteMaxLength} characters.");
            else if (!IsValidHttpUrl(website)) Add(nameof(request.Website), "Website must be a valid http or https URL.");
        }

        return errors.ToDictionary(e => e.Key, e => e.Value.ToArray());
    }

    private static void CheckMax(string? value, int max, string field, Action<string, string> add)
    {
        if (value is not null && value.Trim().Length > max)
            add(field, $"{field} must be at most {max} characters.");
    }

    // MailAddress accepts "Name <a@b.com>", so we also require the parsed address to equal the input.
    private static bool IsValidEmail(string email) =>
        MailAddress.TryCreate(email, out var parsed) && parsed.Address == email && parsed.Host.Contains('.');

    private static bool IsValidHttpUrl(string value) =>
        Uri.TryCreate(value, UriKind.Absolute, out var uri) &&
        (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
}
