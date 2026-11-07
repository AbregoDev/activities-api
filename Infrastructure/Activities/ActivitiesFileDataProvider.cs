using Application.Activities;
using Domain.Entities;
using System.Text.Json;

namespace Infrastructure.Activities;

public sealed class ActivitiesFileDataProvider : IActivityRepository
{
    const string FILE_NAME = "App_Data/activities-db.json"; 
    private readonly string _filePath = Path.GetFullPath(FILE_NAME);
    private readonly List<Activity> _inMemoryActivities = [];
    private readonly SemaphoreSlim _gate = new(1, 1);

    public ActivitiesFileDataProvider()
    {
        EnsureFileExists();

        using var stream = File.OpenRead(_filePath);
        var activities = JsonSerializer.Deserialize<StoredActivity[]>(stream)
            ?? throw new JsonException("The activities file must contain an array.");
        _inMemoryActivities.AddRange(activities.Select(activity => activity.ToActivity()));
    }

    private void EnsureFileExists()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);

        try
        {
            using var stream = new FileStream(_filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
            stream.Write("[]"u8);
        }
        catch (IOException) when (File.Exists(_filePath))
        {
        }
    }

    public async Task<Activity> CreateActivityAsync(Activity activity, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(activity);
        await _gate.WaitAsync(ct);
        try
        {
            ct.ThrowIfCancellationRequested();
            var storedActivity = Copy(activity);
            var activities = new List<Activity>(_inMemoryActivities) { storedActivity };
            await SaveActivitiesAsync(activities, ct);
            return Copy(storedActivity);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task DeleteActivityAsync(Guid id, CancellationToken ct)
    {
        await _gate.WaitAsync(ct);
        try
        {
            ct.ThrowIfCancellationRequested();
            var activity = FindActivity(id);
            var activities = new List<Activity>(_inMemoryActivities);
            activities.Remove(activity);
            await SaveActivitiesAsync(activities, ct);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<IEnumerable<Activity>> GetActivitiesAsync(CancellationToken ct)
    {
        await _gate.WaitAsync(ct);
        try
        {
            ct.ThrowIfCancellationRequested();
            return _inMemoryActivities.Select(Copy).ToArray();
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<IEnumerable<Activity>> GetActivitiesInRangeAsync(
        DateOnly startDate, DateOnly endDate, CancellationToken ct)
    {
        await _gate.WaitAsync(ct);
        try
        {
            ct.ThrowIfCancellationRequested();
            return _inMemoryActivities
                .Where(activity => activity.Date >= startDate && activity.Date <= endDate)
                .OrderBy(activity => activity.Date)
                .ThenBy(activity => activity.StartTime)
                .ThenBy(activity => activity.Id)
                .Select(Copy)
                .ToArray();
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<Activity> GetActivityByIdAsync(Guid id, CancellationToken ct)
    {
        await _gate.WaitAsync(ct);
        try
        {
            ct.ThrowIfCancellationRequested();
            return Copy(FindActivity(id));
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<Activity> UpdateActivityAsync(
        Guid id,
        ActivityDataToUpdate data,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(data);
        await _gate.WaitAsync(ct);
        try
        {
            ct.ThrowIfCancellationRequested();
            var existingActivity = FindActivity(id);
            var activity = Copy(existingActivity);
            activity.Name = data.Name ?? activity.Name;
            activity.Date = data.Date ?? activity.Date;
            activity.StartTime = data.StartTime ?? activity.StartTime;
            activity.EndTime = data.EndTime ?? activity.EndTime;
            activity.CategoryId = data.CategoryId ?? activity.CategoryId;
            activity.IsCompleted = data.IsCompleted ?? activity.IsCompleted;

            var result = Copy(activity);
            var activities = new List<Activity>(_inMemoryActivities);
            activities[activities.IndexOf(existingActivity)] = activity;
            await SaveActivitiesAsync(activities, ct);
            return result;
        }
        finally
        {
            _gate.Release();
        }
    }

    private Activity FindActivity(Guid id)
    {
        return _inMemoryActivities.FirstOrDefault(activity => activity.Id == id)
            ?? throw new KeyNotFoundException($"Activity with id {id} not found.");
    }

    private async Task SaveActivitiesAsync(List<Activity> activities, CancellationToken ct)
    {
        var temporaryPath = $"{_filePath}.{Guid.NewGuid():N}.tmp";
        try
        {
            await using (var stream = new FileStream(
                temporaryPath, FileMode.CreateNew, FileAccess.Write, FileShare.None,
                bufferSize: 4096, useAsync: true))
            {
                await JsonSerializer.SerializeAsync(
                    stream,
                    activities.Select(StoredActivity.FromActivity).ToArray(),
                    cancellationToken: ct);
                await stream.FlushAsync(ct);
            }

            ct.ThrowIfCancellationRequested();
            File.Move(temporaryPath, _filePath, overwrite: true);
            _inMemoryActivities.Clear();
            _inMemoryActivities.AddRange(activities);
        }
        finally
        {
            File.Delete(temporaryPath);
        }
    }

    private static Activity Copy(Activity activity) => StoredActivity.FromActivity(activity).ToActivity();

    private sealed record StoredActivity(
        Guid Id,
        string Name,
        Guid? CategoryId,
        DateOnly Date,
        TimeOnly StartTime,
        TimeOnly EndTime,
        bool IsCompleted)
    {
        public static StoredActivity FromActivity(Activity activity) => new(
            activity.Id,
            activity.Name,
            activity.CategoryId,
            activity.Date,
            activity.StartTime,
            activity.EndTime,
            activity.IsCompleted);

        public Activity ToActivity()
        {
            return new Activity(Id, Name, Date, StartTime, EndTime, CategoryId, IsCompleted);
        }
    }
}