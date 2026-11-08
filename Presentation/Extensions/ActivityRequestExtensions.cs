using Application.Activities;
using Presentation.Contracts.Activities;

namespace Presentation.Extensions;

public static class ActivityRequestExtensions
{
    public static ActivityDataToUpdate ToActivityDataToUpdate(this UpdateActivityRequest request)
    {
        return new ActivityDataToUpdate(
            request.Name,
            request.Date,
            request.StartTime,
            request.EndTime,
            request.CategoryId,
            request.IsCompleted);
    }
}
