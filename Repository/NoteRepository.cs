using System.Security.Claims;
using AutoMapper;
using dotnet_notely.Configurations;
using dotnet_notely.Contracts;
using dotnet_notely.Data;
using dotnet_notely.ModelDtos.NoteDtos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;

namespace dotnet_notely.Repository;

public class NoteRepository : GenericRepository<Note>, INoteRepository
{

    private readonly NotelyDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<ApiUser> _userManager;
    private readonly IHubContext<NoteHub> _noteHub;

    public NoteRepository(NotelyDbContext notelyDbContext, IMapper mapper, UserManager<ApiUser> manager, IHubContext<NoteHub> noteHub) : base(notelyDbContext)
    {
        this._context = notelyDbContext;
        this._mapper = mapper;
        this._userManager = manager;
        this._noteHub = noteHub;
    }

    public async Task<NoteResponseDto> CreateNote(CreateNoteDto note, string userId)
    {
        var mapNote = _mapper.Map<Note>(note);
        mapNote.CreatedBy = userId;
        var result = await _context.Notes.AddAsync(mapNote);
        await _context.SaveChangesAsync();

        return _mapper.Map<NoteResponseDto>(mapNote);
    }

    // public async Task<List<NoteResponseDto>> GetNote(String userId)
    // {
    //     var notes = await _context.Notes.ToListAsync();
    //     var createdUserNotes = notes.Where(createdUserNote => createdUserNote.CreatedBy == userId).ToList();
    //
    //
    //     if (notes == null)
    //     {
    //         throw new Exception("No notes found");
    //     }
    //
    //     return _mapper.Map<List<NoteResponseDto>>(createdUserNotes);
    // }
    public async Task<List<NoteResponseDto>> GetNote(String userId)
    {
        // Get notes created by the user
        var userNotes = await _context.Notes
            .Where(note => note.CreatedBy == userId)
            .Select(note => new NoteResponseDto
            {
                Id = note.Id,
                Title = note.Title,
                Description = note.Description,
                CreatedDate = note.CreatedAt,
                CreatedBy = note.CreatedBy,
                UserRole = NoteShareRole.Editor  // Creator has full rights
            })
            .ToListAsync();

        // Get notes shared with the user
        var sharedNotes = await _context.NoteShares
            .Where(share => share.UserId == userId)
            .Select(share => new NoteResponseDto
            {
                Id = share.Note.Id,
                Title = share.Note.Title,
                Description = share.Note.Description,
                CreatedDate = share.Note.CreatedAt,
                CreatedBy = share.Note.CreatedBy,
                UserRole = share.Role
            })
            .ToListAsync();

        // Combine both lists
        var allNotes = userNotes.Concat(sharedNotes).ToList();

        // For each note, get the sharing information
        foreach (var note in allNotes)
        {
            var sharedUsers = await _context.NoteShares
                .Where(share => share.NoteId == note.Id)
                .Join(_context.Users,
                    share => share.UserId,
                    user => user.Id,
                    (share, user) => new SharedUserInfo
                    {
                        UserId = user.Id,
                        Email = user.Email,
                        UserName = user.UserName,
                        Role = share.Role
                    })
                .ToListAsync();

            note.SharedWith = sharedUsers;
        }

        return allNotes;
    }

    // add different role for updating a note: editor or viewer only.
    public async Task<bool> UpdateNote(UpdateNoteDto noteDto, String userId)
    {
        var notes = _context.Notes.FirstOrDefault(notes => notes.Id == noteDto.Id);
        if (notes == null)
        {
            throw new Exception("Note not found");
        }
        
        var hasEditorAccess = notes.CreatedBy == userId || 
                              await _context.NoteShares.AnyAsync(ns => 
                                  ns.NoteId == notes.Id && 
                                  ns.UserId == userId && 
                                  ns.Role == NoteShareRole.Editor);

        if (!hasEditorAccess)
        {
            throw new UnauthorizedAccessException("User does not have editor access to this note");
        }

        if (noteDto.Title != null) notes.Title = noteDto.Title;
        if (noteDto.Description != null) notes.Description = noteDto.Description;
        _context.Update(notes);
        await _context.SaveChangesAsync();
        Console.WriteLine($"Broadcasting update for note {noteDto.Id}");
        await _noteHub.Clients.All.SendAsync("ReceiveMessage", noteDto.Id, noteDto);
        return true;
    }

    public async Task<bool> DeleteNote(int id, String userId)
    {
        var notes = _context.Notes.FirstOrDefault((userNote) => userNote.Id == id && userNote.CreatedBy == userId);
        Console.WriteLine($"Note found: {notes?.Id}, CreatedBy: {notes?.CreatedBy}");

        if (notes == null)
        {
            throw new Exception("No notes found");
        }
        _context.Notes.Remove(notes);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ShareNote(SharedNoteDto sharedNoteDto, String userId)
    {
        var notes = _context.Notes.FirstOrDefault((userNote) => userNote.Id == sharedNoteDto.Id && userNote.CreatedBy == userId);
        Console.WriteLine($"Note found: {notes?.Id}, CreatedBy: {notes?.CreatedBy}");

        if (notes == null)
        {
            throw new Exception("No notes found");
        }

        var existingShares = await _context.NoteShares
            .Where(ns => ns.NoteId == notes.Id)
            .ToListAsync();
        _context.NoteShares.RemoveRange(existingShares);

        foreach (var sharedUserId in sharedNoteDto.SharedUsersId)
        {
            var noteShare = new NoteShare
            {
                NoteId = notes.Id,
                UserId = sharedUserId,
                Role = sharedNoteDto.Role
            };
            await _context.NoteShares.AddAsync(noteShare);
        }

        await _context.SaveChangesAsync();
        return true;

    }
    
    public async Task<List<ApiUser>> GetSharedUsers(String userId)
    {
        // Get unique users that the authenticated user has shared notes with
        var sharedUserIds = await _context.Notes
            .Where(note => note.CreatedBy == userId) // Notes created by authenticated user
            .Join(_context.NoteShares,
                note => note.Id,
                share => share.NoteId,
                (note, share) => share.UserId)
            .Distinct()
            .ToListAsync();

        // Get the user details for these IDs
        var sharedUsers = await _userManager.Users
            .Where(user => sharedUserIds.Contains(user.Id))
            .ToListAsync();

        return sharedUsers;
    }
}