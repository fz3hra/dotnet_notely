using AutoMapper;
using dotnet_notely.Contracts;
using dotnet_notely.Data;
using dotnet_notely.ModelDtos.NoteDtos;
using Microsoft.AspNetCore.Identity;

namespace dotnet_notely.Repository;

public class NoteRepository: GenericRepository<Note>, INoteRepository {
    
    private readonly NotelyDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<ApiUser> _userManager;

    public NoteRepository(NotelyDbContext notelyDbContext, IMapper mapper, UserManager<ApiUser> manager) : base(notelyDbContext)
    {
        this._context = notelyDbContext;
        this._mapper = mapper;
        this._userManager = manager;
    }

    /*
     * TODO::
     * create note only if authenticated.
     * if username not found then throw an error
     */
    public async Task<NoteResponseDto> CreateNote(CreateNoteDto note)
    {
        var mapNote =  _mapper.Map<Note>(note);
        // does not need to crate manually, since mapper does it automatically
        var result = await _context.Notes.AddAsync(mapNote);
        await _context.SaveChangesAsync();
        
        
        if (note.SharedUsersId?.Any() == true)
        {
            foreach (var username in note.SharedUsersId)
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user != null)
                {
                    await _context.UserNotes.AddAsync(new UserNote 
                    { 
                        NoteId = mapNote.Id, 
                        UserId = user.Id 
                    });
                }
            }
            await _context.SaveChangesAsync();
        }
        
        return _mapper.Map<NoteResponseDto>(mapNote);
    }
    
    
// TODO
    public Task<NoteResponseDto> GetNote(GetNoteDto note)
    {
        throw new NotImplementedException();
    }
}