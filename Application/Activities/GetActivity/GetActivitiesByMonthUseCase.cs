using Domain.Entities;

namespace Application.Activities.GetActivity;

public sealed class GetActivitiesByMonthUseCase(IActivityRepository repository)
{
    public Task<IEnumerable<Activity>> ExecuteAsync(
        int year, int month, CancellationToken ct)
    {
        var startDate = new DateOnly(year, month, 1);
        var endDate = new DateOnly(year, month, DateTime.DaysInMonth(year, month));
        return repository.GetActivitiesInRangeAsync(startDate, endDate, ct);
    }
}
