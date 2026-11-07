using Domain.Entities;

namespace Application.Activities;

public interface IActivityRepository
{
    Task<Activity> CreateActivityAsync(Activity activity, CancellationToken ct);

    Task<IEnumerable<Activity>> GetActivitiesAsync(CancellationToken ct);

    Task<IEnumerable<Activity>> GetActivitiesInRangeAsync(
        DateOnly startDate, DateOnly endDate, CancellationToken ct);

    Task<Activity> GetActivityByIdAsync(Guid id, CancellationToken ct);

    Task<Activity> UpdateActivityAsync(Guid id, ActivityDataToUpdate data, CancellationToken ct);

    Task DeleteActivityAsync(Guid id, CancellationToken ct);
}

public sealed record ActivityDataToUpdate(
    string? Name,
    DateOnly? Date,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    Guid? CategoryId,
    bool? IsCompleted);
