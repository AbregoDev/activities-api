using Application.Activities.Createactivity;
using Application.Activities.DeleteActivity;
using Application.Activities.GetActivity;
using Application.Activities.UpdateActivity;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateActivityUseCase>();
        services.AddScoped<GetActivitiesUseCase>();
        services.AddScoped<GetActivitiesInRangeUseCase>();
        services.AddScoped<GetActivityByIdUseCase>();
        services.AddScoped<UpdateActivityUseCase>();
        services.AddScoped<DeleteActivityUseCase>();

        return services;
    }
}
