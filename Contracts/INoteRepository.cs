using dotnet_notely.Data;
using dotnet_notely.ModelDtos.NoteDtos;

namespace dotnet_notely.Contracts;

public interface INoteRepository: IGenericManager<Note>
{
    // create note
    Task<NoteResponseDto> CreateNote(CreateNoteDto note, HttpContext httpContext);
    //get note
    Task<List<NoteResponseDto>> GetNote(GetNoteDto note, HttpContext httpContext);
    //update note - allow to make changes and allow for shared user to also make changes - then implement live changes
    Task<bool> UpdateNote(UpdateNoteDto note, HttpContext httpContext); 
    // delete note
    Task<bool> DeleteNote(int id, HttpContext httpContext);
    // delete one or multiple note
    
}