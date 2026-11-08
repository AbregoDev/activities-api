using Domain.Entities;
using Presentation.Contracts.Activities;

namespace Presentation.Extensions;

public static class ActivityMappingExtensions
{
    public static ActivityResponse ToActivityResponse(this Activity activity) =>
        new(
            activity.Id,
            activity.Name,
            activity.CategoryId,
            activity.Date,
            activity.StartTime,
            activity.EndTime,
            activity.IsCompleted);
}
