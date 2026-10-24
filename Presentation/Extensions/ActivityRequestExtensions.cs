using Application.Activities;
using Presentation.Controllers;

namespace Presentation.Extensions;

public static class ActivityRequestExtensions
{
    public static ActivityDataToUpdate ToActivityDataToUpdate(this ActivityUpdatedDataRequest request)
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
