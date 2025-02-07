using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dotnet_notely.Data.Configuration;

public class UserNoteConfiguration: IEntityTypeConfiguration<NoteShare>
{
    public void Configure(EntityTypeBuilder<NoteShare> builder)
    {
        builder.HasKey(un => un.Id);

        builder.HasIndex(un => new { un.UserId, un.NoteId })
            .IsUnique();
    }
}