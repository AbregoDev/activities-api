using Domain.Entities;

namespace Application.Activities;

public interface IActivityRepository
{
    Task<Activity> CreateActivityAsync(Activity activity);

    Task<IEnumerable<Activity>> GetActivitiesAsync();

    Task<Activity> GetActivityByIdAsync(Guid id);
}
