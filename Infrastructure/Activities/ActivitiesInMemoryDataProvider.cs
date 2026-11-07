using Application.Activities;
using Domain.Entities;

namespace Infrastructure.Activities;

public sealed class ActivitiesInMemoryDataProvider : IActivityRepository
{
    private readonly List<Activity> _activities = [];

    public Task<Activity> CreateActivityAsync(Activity activity, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        _activities.Add(activity);
        return Task.FromResult(activity);
    }

    public Task<IEnumerable<Activity>> GetActivitiesAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        return Task.FromResult<IEnumerable<Activity>>(_activities);
    }

    public Task<Activity> GetActivityByIdAsync(Guid id, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var activity = _activities.FirstOrDefault(a => a.Id == id)
            ?? throw new KeyNotFoundException($"Activity with id {id} not found.");

        return Task.FromResult(activity);
    }

    public Task<IEnumerable<Activity>> GetActivitiesInRangeAsync(
        DateOnly startDate, DateOnly endDate, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        throw new NotImplementedException();
    }

    public Task<Activity> UpdateActivityAsync(
        Guid id,
        ActivityDataToUpdate data,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var activity = _activities.FirstOrDefault(a => a.Id == id)
            ?? throw new KeyNotFoundException($"Activity with id {id} not found.");

        activity.Name = data.Name ?? activity.Name;
        activity.Date = data.Date ?? activity.Date;
        activity.StartTime = data.StartTime ?? activity.StartTime;
        activity.EndTime = data.EndTime ?? activity.EndTime;
        activity.CategoryId = data.CategoryId ?? activity.CategoryId;
        activity.IsCompleted = data.IsCompleted ?? activity.IsCompleted;

        return Task.FromResult(activity);
    }

    public Task DeleteActivityAsync(Guid id, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var activity = _activities.FirstOrDefault(a => a.Id == id)
            ?? throw new KeyNotFoundException($"Activity with id {id} not found.");

        _activities.Remove(activity);
        return Task.CompletedTask;
    }
}
