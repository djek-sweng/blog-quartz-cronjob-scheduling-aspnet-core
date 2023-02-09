namespace CronJobScheduling.DataStore.Repositories;

public interface INoteRepository
{
    Task<IReadOnlyList<Note>> GetNotesAsync(
        int skip = 0,
        int take = int.MaxValue,
        CancellationToken cancellationToken = default);

    Task AddNoteAsync(
        Note note,
        CancellationToken cancellationToken = default);

    Task RemoveNotesAsync(
        IReadOnlyList<Note> notes,
        CancellationToken cancellationToken = default);
}
