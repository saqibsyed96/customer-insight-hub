using CustomerInsightHub.Application.Customers;
using CustomerInsightHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerInsightHub.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// One place where the Infrastructure layer registers itself with the DI container.
    /// The Api project calls this once from Program.cs.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");

        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<ICustomerRepository, CustomerRepository>();

        return services;
    }
}
