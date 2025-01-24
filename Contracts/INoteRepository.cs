using dotnet_notely.Data;
using dotnet_notely.ModelDtos.NoteDtos;

namespace dotnet_notely.Contracts;

public interface INoteRepository: IGenericManager<Note>
{
    // create note
    Task<NoteResponseDto> CreateNote(CreateNoteDto note, HttpContext httpContext);
    //get note
    Task<List<NoteResponseDto>> GetNote(GetNoteDto note, HttpContext httpContext);
    //update note
    // delete note
}