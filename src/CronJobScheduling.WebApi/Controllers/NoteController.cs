namespace CronJobScheduling.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NoteController : ControllerBase
{
    private readonly INoteRepository _noteRepository;

    public NoteController(INoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }

    [HttpPost]
    public async Task<IActionResult> CreateNoteAsync(CancellationToken cancellationToken)
    {
        var note = Note.Create($"Created by '{GetType().Name}' at '{DateTime.UtcNow}'.");

        await _noteRepository.AddNoteAsync(note, cancellationToken);

        return Ok(note);
    }
}
