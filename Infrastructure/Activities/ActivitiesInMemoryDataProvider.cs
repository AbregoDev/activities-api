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
}
