using System.Security.Claims;
using AutoMapper;
using dotnet_notely.Contracts;
using dotnet_notely.Data;
using dotnet_notely.ModelDtos.NoteDtos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;

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

    public async Task<NoteResponseDto> CreateNote(CreateNoteDto note, HttpContext context)
    {
        var userEmail = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        var user = await _userManager.FindByEmailAsync(userEmail);

        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found");
        }
        
        var mapNote =  _mapper.Map<Note>(note);
        mapNote.CreatedBy = user.Id;  
        var result = await _context.Notes.AddAsync(mapNote);
        await _context.SaveChangesAsync();
        
        
        if (note.SharedUsersId?.Any() == true)
        {
            foreach (var username in note.SharedUsersId)
            {
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
    
    public async Task<List<NoteResponseDto>> GetNote(GetNoteDto note, HttpContext httpContext)
    {
        var userEmail = httpContext.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Email || c.Type == JwtRegisteredClaimNames.Email)?.Value;
        
        var user = await _userManager.FindByEmailAsync(userEmail);
    
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found");
        }

        var notes = await _context.Notes.ToListAsync();
        var createdUserNotes = notes.Where(createdUserNote => createdUserNote.CreatedBy == user.Id).ToList();

        
        if (notes == null)
        {
            throw new Exception("No notes found");
        }
        
        return _mapper.Map<List<NoteResponseDto>>(createdUserNotes);
    }
}