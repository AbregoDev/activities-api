using Domain.Entities;

namespace Application.Activities.Createactivity;

public sealed class CreateActivityUseCase(IActivityRepository repository)
{
    public Task<Activity> ExecuteAsync(
        string name,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        CancellationToken ct)
    {
        var activity = new Activity(name, date, startTime, endTime);
        return repository.CreateActivityAsync(activity, ct);
    }
}
