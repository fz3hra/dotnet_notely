namespace dotnet_notely.ModelDtos.NoteDtos;

public class GetNoteDto
{
    /*
     * title, description, list of sharedTo, createdAt
     */
    public DateTime CreatedAt { get; set; }
    public string CreatedByUserId { get; set; }
}