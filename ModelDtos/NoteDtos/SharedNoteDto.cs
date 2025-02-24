using dotnet_notely.Data;

namespace dotnet_notely.ModelDtos.NoteDtos;

public class SharedNoteDto
{
    public int Id { get; set; }
    public List<string>? SharedUsersId { get; set; }
    public NoteShareRole Role { get; set; } = NoteShareRole.Viewer;
}

public class AddNoteShareDto
{
    public string UserId { get; set; }
    public NoteShareRole Role { get; set; } = NoteShareRole.Viewer;
}

// For updating a user's role
public class UpdateShareRoleDto
{
    public NoteShareRole Role { get; set; }
}