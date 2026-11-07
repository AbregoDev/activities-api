using Domain.Entities;

namespace Application.Activities.GetActivity;

public sealed class GetActivitiesInRangeUseCase(IActivityRepository repository)
{
    public Task<IEnumerable<Activity>> ExecuteAsync(
        DateOnly startDate, DateOnly endDate, CancellationToken ct)
    {
        if (startDate > endDate)
            throw new ArgumentException("Start date must be on or before end date.", nameof(startDate));

        return repository.GetActivitiesInRangeAsync(startDate, endDate, ct);
    }
}
