using System.ComponentModel.DataAnnotations;

namespace dotnet_notely.Data;

public enum NoteShareRole
{
    Viewer,
    Editor
}
public class NoteShare
{
    [Key]
    public int Id { get; set; }
    public string UserId { get; set; }
    public int NoteId { get; set; }
    public NoteShareRole Role { get; set; }
    public Note Note { get; set; }
}