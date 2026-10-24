using Domain.Entities;

namespace Application.Activities.GetActivity;

public sealed class GetActivityUseCase(IActivityRepository repository)
{
    public Task<Activity> ExecuteAsync(Guid id) =>
        repository.GetActivityByIdAsync(id);
}
