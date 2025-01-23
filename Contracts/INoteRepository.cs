using dotnet_notely.Data;
using dotnet_notely.ModelDtos.NoteDtos;

namespace dotnet_notely.Contracts;

public interface INoteRepository: IGenericManager<Note>
{
    // create note
    Task<NoteResponseDto> CreateNote(CreateNoteDto note);
    //get note
    Task<NoteResponseDto> GetNote(GetNoteDto note);
    //update note
    // delete note
}