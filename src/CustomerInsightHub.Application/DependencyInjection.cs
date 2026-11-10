using CustomerInsightHub.Application.Customers;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerInsightHub.Application;

public static class DependencyInjection
{
    /// <summary>Registers the Application layer. Called once from Program.cs.</summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<ICustomerService, CustomerService>();
        return services;
    }
}
