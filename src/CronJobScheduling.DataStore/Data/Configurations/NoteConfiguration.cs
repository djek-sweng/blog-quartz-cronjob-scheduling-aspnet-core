namespace CronJobScheduling.DataStore.Data.Configurations;

public class NoteConfiguration : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.ToTable("Notes");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Message)
            .HasMaxLength(Note.MessageMaxLength)
            .IsRequired();

        builder.Property(n => n.CreatedAt)
            .IsRequired();
    }
}
