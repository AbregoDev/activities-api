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

    public async Task<Activity> CreateActivityAsync(Activity activity)
    {
        ArgumentNullException.ThrowIfNull(activity);
        await _gate.WaitAsync();
        try
        {
            var storedActivity = Copy(activity);
            var activities = new List<Activity>(_inMemoryActivities) { storedActivity };
            await SaveActivitiesAsync(activities);
            return Copy(storedActivity);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task DeleteActivityAsync(Guid id)
    {
        await _gate.WaitAsync();
        try
        {
            var activity = FindActivity(id);
            var activities = new List<Activity>(_inMemoryActivities);
            activities.Remove(activity);
            await SaveActivitiesAsync(activities);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<IEnumerable<Activity>> GetActivitiesAsync()
    {
        await _gate.WaitAsync();
        try
        {
            return _inMemoryActivities.Select(Copy).ToArray();
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<Activity> GetActivityByIdAsync(Guid id)
    {
        await _gate.WaitAsync();
        try
        {
            return Copy(FindActivity(id));
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<Activity> UpdateActivityAsync(Guid id, ActivityDataToUpdate data)
    {
        ArgumentNullException.ThrowIfNull(data);
        await _gate.WaitAsync();
        try
        {
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
            await SaveActivitiesAsync(activities);
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

    private async Task SaveActivitiesAsync(List<Activity> activities)
    {
        var temporaryPath = $"{_filePath}.{Guid.NewGuid():N}.tmp";
        try
        {
            await using (var stream = new FileStream(
                temporaryPath, FileMode.CreateNew, FileAccess.Write, FileShare.None,
                bufferSize: 4096, useAsync: true))
            {
                await JsonSerializer.SerializeAsync(stream, activities.Select(StoredActivity.FromActivity).ToArray());
                await stream.FlushAsync();
            }

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