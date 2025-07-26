using AudiSense.Application.Services;
using AudiSense.Contracts.Interfaces;

using Microsoft.Extensions.DependencyInjection;

namespace AudiSense.Application;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register application services
        services.AddScoped<IHearingTestService, HearingTestService>();

        return services;
    }
}
