using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dotnet_notely.Data.Configuration;

public class NoteConfiguration: IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Title)
            .IsRequired();

        builder.Property(n => n.Description)
            .IsRequired(false);

        builder.Property(n => n.CreatedAt)
            .IsRequired();
    }
}