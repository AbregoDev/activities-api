using Application.Activities.Createactivity;
using Application.Activities.GetActivity;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateActivityUseCase>();
        services.AddScoped<GetActivitiesUseCase>();
        services.AddScoped<GetActivityUseCase>();

        return services;
    }
}
