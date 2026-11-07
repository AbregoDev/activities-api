using Domain.Entities;

namespace Application.Activities.GetActivity;

public sealed class GetActivityByIdUseCase(IActivityRepository repository)
{
    public Task<Activity> ExecuteAsync(Guid id, CancellationToken ct) =>
        repository.GetActivityByIdAsync(id, ct);
}
