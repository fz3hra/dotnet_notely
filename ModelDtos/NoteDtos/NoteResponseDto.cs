using dotnet_notely.Data;

namespace dotnet_notely.ModelDtos.NoteDtos;

public class NoteResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; }  // Add this to know who created the note
    public List<SharedUserInfo> SharedWith { get; set; } = new();  // Add this for sharing info
    public NoteShareRole? UserRole { get; set; }
}

public class SharedUserInfo
{
    public string UserId { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public NoteShareRole Role { get; set; }
}