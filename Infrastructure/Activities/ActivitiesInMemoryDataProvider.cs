using Application.Activities;
using Domain.Entities;

namespace Infrastructure.Activities;

public sealed class ActivitiesInMemoryDataProvider : IActivityRepository
{
    private readonly List<Activity> _activities = [];

    public Task<Activity> CreateActivityAsync(Activity activity)
    {
        _activities.Add(activity);
        return Task.FromResult(activity);
    }

    public Task<IEnumerable<Activity>> GetActivitiesAsync()
    {
        return Task.FromResult<IEnumerable<Activity>>(_activities);
    }

    public Task<Activity> GetActivityByIdAsync(Guid id)
    {
        var activity = _activities.FirstOrDefault(a => a.Id == id)
            ?? throw new KeyNotFoundException($"Activity with id {id} not found.");

        return Task.FromResult(activity);
    }

    public Task<Activity> UpdateActivityAsync(Guid id, ActivityDataToUpdate data)
    {
        var activity = _activities.FirstOrDefault(a => a.Id == id)
            ?? throw new KeyNotFoundException($"Activity with id {id} not found.");

        activity.Name = data.Name ?? activity.Name;
        activity.Date = data.Date ?? activity.Date;
        activity.StartTime = data.StartTime ?? activity.StartTime;
        activity.EndTime = data.EndTime ?? activity.EndTime;

        return Task.FromResult(activity);
    }

    public Task DeleteActivityAsync(Guid id)
    {
        var activity = _activities.FirstOrDefault(a => a.Id == id)
            ?? throw new KeyNotFoundException($"Activity with id {id} not found.");

        _activities.Remove(activity);
        return Task.CompletedTask;
    }
}
