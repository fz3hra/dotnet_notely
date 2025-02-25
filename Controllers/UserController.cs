using System.Security.Claims;
using dotnet_notely.Contracts;
using dotnet_notely.ModelDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_notely.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UserController: ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository)
    {
        this._userRepository = userRepository;
    }
    
    [HttpGet("search")]
    public async Task<IActionResult> SearchUsers([FromQuery] string searchTerm)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserId))
        {
            return Unauthorized();
        }

        try
        {
            var users = await _userRepository.SearchUsers(searchTerm, currentUserId);
            return Ok(users.Select(u => new 
            {
                Id = u.Id,
                Email = u.Email,
                UserName = u.UserName,
                FirstName = u.FirstName,
                LastName = u.LastName,
                DisplayName = $"{u.FirstName} {u.LastName}" // Optional convenience field
            }));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error searching users", error = ex.Message });
        }
    }
    
    //
    [HttpGet("profile")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserProfileDto>> GetUserProfile()
    {
        try
        {
            var userProfile = await _userRepository.GetCurrentUserProfile();
            if (userProfile == null)
            {
                return NotFound("User profile not found");
            }

            return Ok(userProfile);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}