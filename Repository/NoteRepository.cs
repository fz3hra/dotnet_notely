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

    public async Task<NoteResponseDto> CreateNote(CreateNoteDto note, HttpContext context)
    {
        var userEmail = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        var user = await _userManager.FindByEmailAsync(userEmail);

        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found");
        }

        var mapNote = _mapper.Map<Note>(note);
        mapNote.CreatedBy = user.Id;
        var result = await _context.Notes.AddAsync(mapNote);
        await _context.SaveChangesAsync();

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

    // add different role for updating a note: editor or viewer only.
    public async Task<bool> UpdateNote(UpdateNoteDto noteDto, HttpContext context)
    {
        var userEmail = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        var user = await _userManager.FindByEmailAsync(userEmail);

        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found");
        }

        var notes = _context.Notes.FirstOrDefault(notes => notes.Id == noteDto.Id);
        if (notes == null)
        {
            throw new Exception("Note not found");
        }

        if (noteDto.Title != null) notes.Title = noteDto.Title;
        if (noteDto.Description != null) notes.Description = noteDto.Description;
        _context.Update(notes);
        await _context.SaveChangesAsync();
        Console.WriteLine($"Broadcasting update for note {noteDto.Id}");
        await _noteHub.Clients.All.SendAsync("ReceiveMessage", noteDto.Id, noteDto);
        return true;
    }

    public async Task<bool> DeleteNote(int id, HttpContext httpContext)
    {
        var userId = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        Console.WriteLine($"User ID from token: {userId}");

        if (userId == null)
        {
            throw new UnauthorizedAccessException("User not found");
        }

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

    public async Task<bool> ShareNote(SharedNoteDto sharedNoteDto, HttpContext httpContext)
    {
        var userId = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        Console.WriteLine($"User ID from token: {userId}");

        if (userId == null)
        {
            throw new UnauthorizedAccessException("User not found");
        }

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
                UserId = sharedUserId
            };
            await _context.NoteShares.AddAsync(noteShare);
        }

        await _context.SaveChangesAsync();
        return true;

    }
}