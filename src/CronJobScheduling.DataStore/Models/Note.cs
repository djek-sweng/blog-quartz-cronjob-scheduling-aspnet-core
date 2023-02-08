namespace CronJobScheduling.DataStore.Models;

public class Note
{
    public Guid Id { get; }
    public string Message { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public const int MessageMaxLength = 800;

    private Note(
        Guid id,
        string message,
        DateTime createdAt)
    {
        Id = id;
        Message = message;
        CreatedAt = createdAt;
    }

    public static Note Create(string message)
    {
        var note = new Note(
            id: Guid.NewGuid(),
            message: message,
            createdAt: DateTime.UtcNow);

        return note;
    }
}
