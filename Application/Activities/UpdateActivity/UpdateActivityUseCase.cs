using Domain.Entities;

namespace Application.Activities.UpdateActivity;

public sealed class UpdateActivityUseCase(IActivityRepository repository)
{
    public Task<Activity> ExecuteAsync(Guid id, ActivityDataToUpdate data)
    {
        return repository.UpdateActivityAsync(id, data);
    }
}
