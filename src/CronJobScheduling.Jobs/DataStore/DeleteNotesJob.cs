namespace CronJobScheduling.Jobs.DataStore;

public class DeleteNotesJob : CronJobBase<DeleteNotesJob>
{
    public override string Description => "Deletes all notes except the latest 10.";
    public override string Group => CronGroupDefaults.User;
    public override string CronExpression => CronExpressionDefaults.EverySecondFrom0Through59;

    private readonly INoteRepository _noteRepository;

    public DeleteNotesJob(INoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }

    protected override async Task ExecuteAsync()
    {
        var notes = await _noteRepository.GetNotesAsync(skip: 10);

        await _noteRepository.RemoveNotesAsync(notes);
    }
}
