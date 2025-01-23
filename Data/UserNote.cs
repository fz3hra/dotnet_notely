using System.ComponentModel.DataAnnotations;

namespace dotnet_notely.Data;

public class UserNote
{
    [Key]
    public int Id { get; set; }
    public string UserId { get; set; }
    public int NoteId { get; set; }
}