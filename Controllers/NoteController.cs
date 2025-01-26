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
public class NoteController: ControllerBase
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
            var userMail = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userMail))
            {
                return Unauthorized();
            }
            
            var result = await _noteRepository.CreateNote(note, HttpContext);
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
    public async Task<IActionResult> GetNote(GetNoteDto note)
    {
        var result = await _noteRepository.GetNote(note, HttpContext);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }
}