using Domain.Entities;

namespace Application.Activities;

public interface IActivityRepository
{
    Task<Activity> CreateActivityAsync(Activity activity);

    Task<IEnumerable<Activity>> GetActivitiesAsync();

    Task<IEnumerable<Activity>> GetActivitiesInRangeAsync(
        DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken);

    Task<Activity> GetActivityByIdAsync(Guid id);

    Task<Activity> UpdateActivityAsync(Guid id, ActivityDataToUpdate data);

    Task DeleteActivityAsync(Guid id);
}

public sealed record ActivityDataToUpdate(
    string? Name,
    DateOnly? Date,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    Guid? CategoryId,
    bool? IsCompleted);
