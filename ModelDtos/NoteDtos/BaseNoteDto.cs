namespace dotnet_notely.ModelDtos.NoteDtos;

public abstract class BaseNoteDto
{
    //
    public string? Title { get; set; }
    public string? Description { get; set; }
    public List<string>? SharedUsersId { get; set; }
}