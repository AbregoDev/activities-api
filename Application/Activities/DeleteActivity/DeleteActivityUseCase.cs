using Domain.Entities;

namespace Application.Activities.DeleteActivity;

public sealed class DeleteActivityUseCase(IActivityRepository repository)
{
    public Task ExecuteAsync(Guid id, CancellationToken ct) =>
        repository.DeleteActivityAsync(id, ct);
}
