using dotnet_notely.Contracts;
using dotnet_notely.Data;
using dotnet_notely.ModelDtos.NoteDtos;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_notely.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NoteController: ControllerBase
{
    private readonly INoteRepository _noteRepository;

    public NoteController(INoteRepository noteRepository)
    {
        this._noteRepository = noteRepository;
    }
    
    // create note:
    [HttpPost]
    [Route("createNote")]
    public async Task<IActionResult> CreateNote(CreateNoteDto note)
    {
        try 
        {
            var result = await _noteRepository.CreateNote(note);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error creating note", error = ex.Message });
        }
    }

    [HttpPost]
    [Route("getNotes")]
    public async Task<IActionResult> GetNote(GetNoteDto note)
    {
        var result = await _noteRepository.GetNote(note);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }
}