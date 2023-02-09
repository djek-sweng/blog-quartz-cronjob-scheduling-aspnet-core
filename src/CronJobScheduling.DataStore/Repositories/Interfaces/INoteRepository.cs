namespace CronJobScheduling.DataStore.Repositories;

public interface INoteRepository
{
    Task<IReadOnlyList<Note>> GetNotesDescendingAsync(
        int skip = 0,
        CancellationToken cancellationToken = default);

    Task AddNoteAsync(
        Note note,
        CancellationToken cancellationToken = default);

    Task RemoveNotesAsync(
        IReadOnlyList<Note> notes,
        CancellationToken cancellationToken = default);
}
