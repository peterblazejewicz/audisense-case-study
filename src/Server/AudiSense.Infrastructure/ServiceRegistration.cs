using AudiSense.Contracts.Interfaces;
using AudiSense.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AudiSense.Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Register DbContext with InMemory database
        services.AddDbContext<AudiSenseDbContext>(options =>
            options.UseInMemoryDatabase("AudiSenseDb"));

        // Register repositories
        services.AddScoped<IHearingTestRepository, HearingTestRepository>();

        return services;
    }
}
