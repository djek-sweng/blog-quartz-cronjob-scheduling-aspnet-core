namespace CronJobScheduling.Jobs.DataStore;

public class CreateNoteJob : CronJobBase<CreateNoteJob>
{
    public override string Description => "Creates one note each time it is executed.";
    public override string Group => CronGroupDefaults.User;
    public override string CronExpression => CronExpressionDefaults.Every5ThSecondFrom0Through59;

    private readonly INoteRepository _noteRepository;

    public CreateNoteJob(INoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }

    protected override async Task InvokeAsync(CancellationToken cancellationToken)
    {
        var note = Note.Create($"Created by '{Name}' at '{DateTime.UtcNow}'.");

        await _noteRepository.AddNoteAsync(note, cancellationToken);
    }
}
