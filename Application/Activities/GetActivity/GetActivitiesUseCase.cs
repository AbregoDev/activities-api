using Domain.Entities;

namespace Application.Activities.GetActivity;

public sealed class GetActivitiesUseCase(IActivityRepository repository)
{
    public Task<IEnumerable<Activity>> ExecuteAsync(CancellationToken ct) =>
        repository.GetActivitiesAsync(ct);
}
