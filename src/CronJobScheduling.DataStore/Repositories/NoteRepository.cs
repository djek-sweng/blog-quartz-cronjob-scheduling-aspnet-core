namespace CronJobScheduling.DataStore.Repositories;

public class NoteRepository : INoteRepository
{
    private readonly ApplicationDbContext _ctx;

    public NoteRepository(ApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task AddNoteAsync(Note note, CancellationToken cancellationToken = default)
    {
        _ctx.Notes.Add(note);
        await _ctx.SaveChangesAsync(cancellationToken);
    }
}
