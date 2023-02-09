namespace CronJobScheduling.Jobs.DataStore;

public class DeleteNotesJob : CronJobBase<DeleteNotesJob>
{
    public override string Description => "Deletes all notes except the two latest notes.";
    public override string Group => CronGroupDefaults.User;
    public override string CronExpression => CronExpressionDefaults.EveryMinuteAtSecond0;

    private readonly INoteRepository _noteRepository;

    public DeleteNotesJob(INoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }

    protected override async Task InvokeAsync(CancellationToken cancellationToken)
    {
        var notes = await _noteRepository.GetNotesDescendingAsync(skip: 2, cancellationToken);

        await _noteRepository.RemoveNotesAsync(notes, cancellationToken);
    }
}
