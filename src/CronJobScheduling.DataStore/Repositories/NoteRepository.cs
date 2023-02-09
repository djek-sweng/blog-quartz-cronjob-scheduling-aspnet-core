namespace CronJobScheduling.DataStore.Repositories;

public class NoteRepository : INoteRepository
{
    private readonly ApplicationDbContext _ctx;

    public NoteRepository(ApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<IReadOnlyList<Note>> GetNotesAsync(
        int skip = 0,
        int take = int.MaxValue,
        CancellationToken cancellationToken = default)
    {
        return await _ctx.Notes
            .OrderBy(n => n.CreatedAt)
            .Skip(skip)
            .Take(take)
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
