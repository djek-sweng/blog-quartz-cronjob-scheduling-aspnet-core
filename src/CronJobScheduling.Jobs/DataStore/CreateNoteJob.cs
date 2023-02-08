namespace CronJobScheduling.Jobs.DataStore;

public class CreateNoteJob : CronJobBase<CreateNoteJob>
{
    public override string Group => CronGroupDefaults.User;
    public override string CronExpression => CronExpressionDefaults.EverySecondFrom0Through59;

    private readonly INoteRepository _noteRepository;

    public CreateNoteJob(INoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }

    protected override async Task ExecuteAsync()
    {
        var note = Note.Create($"Created by {Name} at {DateTime.UtcNow}");
        await _noteRepository.AddNoteAsync(note);
    }
}
