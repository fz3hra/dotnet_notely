using System.Security.Claims;
using dotnet_notely.Contracts;
using dotnet_notely.Data;
using dotnet_notely.ModelDtos.NoteDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_notely.Controllers;

[ApiController]
[Route("api/notes")]
[Authorize]
public class NoteController : ControllerBase
{
    private readonly INoteRepository _noteRepository;

    public NoteController(INoteRepository noteRepository)
    {
        this._noteRepository = noteRepository;
    }

    // create note:
    [HttpPost]
    public async Task<IActionResult> CreateNote(CreateNoteDto note)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))

            {
                return Unauthorized();
            }

            var result = await _noteRepository.CreateNote(note, userId);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error creating note", error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetNote()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))

        {
            return Unauthorized();
        }
        var result = await _noteRepository.GetNote(userId);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpPatch]
    public async Task<IActionResult> UpdateNote(UpdateNoteDto note)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))

        {
            return Unauthorized();
        }

        var result = await _noteRepository.UpdateNote(note, userId);
        if (!result)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteNote(DeleteNoteDto deleteDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))

        {
            return Unauthorized();
        }

        var result = await _noteRepository.DeleteNote(deleteDto.Id, userId);
        if (!result)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpPatch("share")]
    public async Task<IActionResult> ShareNote(SharedNoteDto shareDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))

        {
            return Unauthorized();
        }

        var result = await _noteRepository.ShareNote(shareDto, userId);
        if (!result)
        {
            return NotFound();
        }
        return Ok(result);
    }
    
    [HttpGet("shared")]
    public async Task<IActionResult> GetSharedUsers()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        try 
        {
            var users = await _noteRepository.GetSharedUsers(userId);
            return Ok(users.Select(u => new 
            {
                Id = u.Id,
                Email = u.Email,
                UserName = u.UserName,
                FirstName = u.FirstName,
                LastName = u.LastName
            }));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving shared users", error = ex.Message });
        }
    }
}