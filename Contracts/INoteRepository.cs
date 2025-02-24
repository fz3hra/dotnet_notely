using dotnet_notely.Data;
using dotnet_notely.ModelDtos.NoteDtos;

namespace dotnet_notely.Contracts;

public interface INoteRepository : IGenericManager<Note>
{
    // create note
    Task<NoteResponseDto> CreateNote(CreateNoteDto note, String userId);
    //get note
    Task<List<NoteResponseDto>> GetNote(String userId);
    //update note - allow to make changes and allow for shared user to also make changes - then implement live changes
    Task<bool> UpdateNote(UpdateNoteDto note, String userId);
    // delete note
    Task<bool> DeleteNote(int id, String userId);
    // delete one or multiple note
    
    Task<bool> AddNoteShare(int noteId, string sharedUserId, NoteShareRole role, string ownerId);
    Task<bool> RemoveNoteShare(int noteId, string sharedUserId, string ownerId);
    Task<bool> UpdateNoteShareRole(int noteId, string sharedUserId, NoteShareRole role, string ownerId);

    // Task<bool> ShareNote(SharedNoteDto sharedNoteDto, String userId);

    Task<List<ApiUser>> GetSharedUsers(String userId);

}