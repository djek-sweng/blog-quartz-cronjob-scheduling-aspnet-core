namespace CronJobScheduling.DataStore.Repositories;

public class NoteRepository : INoteRepository
{
    private readonly ApplicationDbContext _ctx;

    public NoteRepository(ApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<IReadOnlyList<Note>> GetNotesDescendingAsync(
        int skip = 0,
        CancellationToken cancellationToken = default)
    {
        return await _ctx.Notes
            .OrderByDescending(n => n.CreatedAt)
            .Skip(skip)
            .ToListAsync(cancellationToken);
    }

    public async Task AddNoteAsync(
        Note note,
        CancellationToken cancellationToken = default)
    {
        _ctx.Notes.Add(note);
        await _ctx.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveNotesAsync(
        IReadOnlyList<Note> notes,
        CancellationToken cancellationToken = default)
    {
        _ctx.Notes.RemoveRange(notes);
        await _ctx.SaveChangesAsync(cancellationToken);
    }
}
