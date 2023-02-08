namespace CronJobScheduling.DataStore.Repositories;

public interface INoteRepository
{
    Task AddNoteAsync(Note note, CancellationToken cancellationToken = default);
}
